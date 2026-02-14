import { Component, OnDestroy, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { Subscription } from 'rxjs';
import { VoiceService } from '../../../core/services/voice.service';
import { SpeechService } from '../../../core/services/speech.service';
import { HandsFreeApiService } from '../../../core/services/hands-free-api.service';
import { WarehouseApiService } from '../../../core/services/warehouse-api.service';
import { SignalRService } from '../../../core/services/signalr.service';
import { OfflineService } from '../../../core/services/offline.service';
import { SyncService } from '../../../core/services/sync.service';
import {
  HandsFreeTaskDto,
  HandsFreeTasksResponse,
  VoiceCommandResult
} from '../../../core/models/hands-free.model';
import { ApiResponse } from '../../../core/models/api-response.model';
import { CameraBarcodeScannerComponent } from '../camera-barcode-scanner/camera-barcode-scanner.component';
import { HeaderService } from '../../../core/services/header.service';

@Component({
  selector: 'app-hands-free-picking',
  standalone: true,
  imports: [CommonModule, FormsModule, CameraBarcodeScannerComponent],
  templateUrl: './hands-free-picking.component.html',
  styleUrls: ['./hands-free-picking.component.scss']
})
export class HandsFreePickingComponent implements OnInit, OnDestroy {
  step: 'select' | 'picking' | 'done' = 'select';
  warehouses: { id: string; name: string }[] = [];
  selectedWarehouseId = '';
  loading = false;
  session: HandsFreeTasksResponse['session'] | null = null;
  tasks: HandsFreeTaskDto[] = [];
  currentIndex = 0;
  currentTask: HandsFreeTaskDto | null = null;
  pickedQuantity: number | null = null;
  showCamera = false;
  listening = false;
  errorMessage = '';
  statusText = '';
  private voiceSub?: Subscription;
  private signalRSub?: Subscription;

  constructor(
    private voice: VoiceService,
    public speech: SpeechService,
    private handsFreeApi: HandsFreeApiService,
    private warehouseApi: WarehouseApiService,
    private signalR: SignalRService,
    private offline: OfflineService,
    private sync: SyncService,
    private router: Router,
    private header: HeaderService
  ) {}

  ngOnInit(): void {
    this.header.setHeaderInfo({ title: 'Hands-free picking', description: 'Voice and camera warehouse mode' });
    this.loadWarehouses();
  }

  ngOnDestroy(): void {
    this.voice.stop();
    this.speech.cancel();
    this.voiceSub?.unsubscribe();
    this.signalRSub?.unsubscribe();
  }

  loadWarehouses(): void {
    this.loading = true;
    this.warehouseApi.getWarehouses({ isActive: true }).subscribe({
      next: (res: ApiResponse<{ items: { id: string; name: string }[] }>) => {
        this.loading = false;
        if (res.success && res.data?.items) {
          this.warehouses = res.data.items.map((i: { id: string; name: string }) => ({ id: i.id, name: i.name }));
        }
      },
      error: () => { this.loading = false; }
    });
  }

  startSession(): void {
    if (!this.selectedWarehouseId) {
      this.speech.sayError('Select a warehouse first');
      return;
    }
    this.loading = true;
    this.errorMessage = '';
    this.handsFreeApi.getTasks(this.selectedWarehouseId).subscribe({
      next: (res: ApiResponse<HandsFreeTasksResponse>) => {
        this.loading = false;
        if (!res.success || !res.data) {
          this.speech.sayError('Could not load tasks');
          return;
        }
        this.session = res.data.session;
        this.tasks = res.data.tasks || [];
        this.currentIndex = 0;
        this.pickedQuantity = null;
        this.step = 'picking';
        this.setCurrentTask();
        this.signalRSub?.unsubscribe();
        this.signalRSub = this.signalR.getHandsFreeTaskPushed().subscribe(ev => {
          if (this.step !== 'picking' || !ev?.payload) return;
          const task = ev.payload as unknown as HandsFreeTaskDto;
          if (task?.productId && task?.warehouseId === this.selectedWarehouseId) {
            this.tasks.unshift(task);
            this.speech.speak('New urgent task', { interrupt: true });
          }
        });
        if (this.currentTask) {
          this.speech.sayGoToLocation(this.currentTask.location);
          this.speech.speak(`Pick ${this.currentTask.quantity} ${this.currentTask.productName}`);
          this.startVoice();
        } else {
          this.step = 'done';
          this.speech.speak('No tasks');
        }
      },
      error: () => {
        this.loading = false;
        this.speech.sayError('Failed to load tasks');
      }
    });
  }

  private setCurrentTask(): void {
    this.currentTask = this.tasks[this.currentIndex] ?? null;
    this.pickedQuantity = this.currentTask ? this.currentTask.quantity : null;
    this.statusText = this.currentTask
      ? `Task ${this.currentIndex + 1} of ${this.tasks.length}`
      : 'All done';
  }

  private startVoice(): void {
    this.listening = this.voice.startListening({ continuous: true });
    this.voiceSub?.unsubscribe();
    this.voiceSub = this.voice.commands$.subscribe(cmd => this.onVoiceCommand(cmd));
  }

  private onVoiceCommand(cmd: VoiceCommandResult): void {
    if (cmd.confidence < 0.35 && cmd.command !== 'quantity') return;
    switch (cmd.command) {
      case 'start_picking':
        if (this.step === 'select') this.startSession();
        break;
      case 'next':
        this.nextTask();
        break;
      case 'confirm':
        if (this.showCamera) return;
        this.tryCompleteWithScanned(null);
        break;
      case 'cancel':
        this.pauseSession();
        break;
      case 'repeat':
        this.repeatInstruction();
        break;
      case 'scan':
        this.showCamera = true;
        this.speech.sayScanBarcode();
        break;
      case 'quantity':
        if (cmd.numericValue != null && this.currentTask) {
          this.pickedQuantity = cmd.numericValue;
          this.speech.speak(`Quantity set to ${cmd.numericValue}`);
        }
        break;
      case 'done':
        this.exitSession();
        break;
      default:
        break;
    }
  }

  nextTask(): void {
    this.showCamera = false;
    this.currentIndex++;
    this.setCurrentTask();
    if (this.currentTask) {
      this.speech.sayGoToLocation(this.currentTask.location);
      this.speech.speak(`Pick ${this.currentTask.quantity} ${this.currentTask.productName}`);
    } else {
      this.step = 'done';
      this.speech.speak('All tasks complete');
    }
  }

  private repeatInstruction(): void {
    if (this.currentTask) {
      this.speech.sayGoToLocation(this.currentTask.location);
      this.speech.speak(`Pick ${this.pickedQuantity ?? this.currentTask.quantity} ${this.currentTask.productName}`);
    }
  }

  private pauseSession(): void {
    this.voice.stop();
    this.listening = false;
    this.speech.speak('Paused');
  }

  private tryCompleteWithScanned(barcode: string | null): void {
    if (!this.currentTask) return;
    const qty = this.pickedQuantity ?? this.currentTask.quantity;
    const code = barcode ?? (this.currentTask.barcode || this.currentTask.sku);
    if (!code?.trim()) {
      this.speech.sayError('Scan barcode first');
      return;
    }
    const request = {
      taskId: this.currentTask.taskId,
      productId: this.currentTask.productId,
      warehouseId: this.currentTask.warehouseId,
      barcodeScanned: code,
      quantityPicked: qty
    };
    if (!this.offline.isOnline()) {
      this.sync.queueForSync('handsFreeComplete', 'complete', request).then(() => {
        this.speech.speak('Queued for sync');
        this.showCamera = false;
        this.nextTask();
      }).catch(() => this.speech.sayError('Could not queue'));
      return;
    }
    this.loading = true;
    this.errorMessage = '';
    this.handsFreeApi.completeTask(request).subscribe({
      next: (res: ApiResponse<unknown>) => {
        this.loading = false;
        if (res.success) {
          this.speech.sayConfirmed();
          this.showCamera = false;
          this.nextTask();
        } else {
          const msg = (res.errors && res.errors.length) ? res.errors.join(' ') : 'Completion failed';
          this.errorMessage = msg;
          this.speech.sayError(msg);
        }
      },
      error: (err) => {
        this.loading = false;
        const msg = err?.error?.message || err?.error?.errors?.join(' ') || 'Completion failed';
        this.errorMessage = msg;
        this.speech.sayError(msg);
      }
    });
  }

  onBarcodeScanned(code: string): void {
    this.errorMessage = '';
    this.tryCompleteWithScanned(code);
  }

  onCameraError(msg: string): void {
    this.errorMessage = msg;
    this.speech.sayError(msg);
  }

  closeCamera(): void {
    this.showCamera = false;
  }

  exitSession(): void {
    this.voice.stop();
    this.listening = false;
    this.step = 'select';
    this.session = null;
    this.tasks = [];
    this.currentTask = null;
  }

  back(): void {
    this.router.navigate(['/inventory/stock-levels']);
  }
}
