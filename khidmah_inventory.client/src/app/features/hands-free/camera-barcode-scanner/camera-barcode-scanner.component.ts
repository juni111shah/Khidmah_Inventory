import { Component, OnDestroy, OnInit, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { BrowserMultiFormatReader } from '@zxing/library';

@Component({
  selector: 'app-camera-barcode-scanner',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './camera-barcode-scanner.component.html',
  styleUrls: ['./camera-barcode-scanner.component.scss']
})
export class CameraBarcodeScannerComponent implements OnInit, OnDestroy {
  @Output() barcodeScanned = new EventEmitter<string>();
  @Output() error = new EventEmitter<string>();

  videoElement: HTMLVideoElement | null = null;
  isScanning = false;
  hasPermission = false;
  private reader: BrowserMultiFormatReader | null = null;
  private stream: MediaStream | null = null;

  ngOnInit(): void {
    this.reader = new BrowserMultiFormatReader();
    if (this.reader) {
      setTimeout(() => this.start(), 100);
    }
  }

  ngOnDestroy(): void {
    this.stop();
  }

  async start(): Promise<void> {
    if (!this.reader) return;
    try {
      const devices = await this.reader.listVideoInputDevices();
      if (devices.length === 0) {
        this.error.emit('No camera found');
        return;
      }
      const video = document.getElementById('hands-free-barcode-video') as HTMLVideoElement;
      if (!video) {
        this.error.emit('Video element not found');
        return;
      }
      this.videoElement = video;
      this.stream = await navigator.mediaDevices.getUserMedia({ video: { facingMode: 'environment' } });
      video.srcObject = this.stream;
      await video.play();
      this.hasPermission = true;
      this.isScanning = true;
      this.decodeLoop();
    } catch (e: any) {
      this.error.emit(e?.message || 'Camera access failed');
    }
  }

  stop(): void {
    this.isScanning = false;
    if (this.stream) {
      this.stream.getTracks().forEach(t => t.stop());
      this.stream = null;
    }
    if (this.videoElement) {
      this.videoElement.srcObject = null;
      this.videoElement = null;
    }
  }

  private decodeLoop(): void {
    if (!this.isScanning || !this.reader || !this.videoElement) return;
    this.reader.decodeFromVideoElement(this.videoElement)
      .then((result) => {
        if (this.isScanning && result?.getText()) {
          this.barcodeScanned.emit(result.getText());
        }
        if (this.isScanning) this.decodeLoop();
      })
      .catch(() => {
        if (this.isScanning) setTimeout(() => this.decodeLoop(), 300);
      });
  }
}
