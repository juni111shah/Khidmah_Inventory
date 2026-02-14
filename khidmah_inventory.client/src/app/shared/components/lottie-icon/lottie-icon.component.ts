import {
  Component,
  Input,
  OnChanges,
  SimpleChanges,
  ChangeDetectionStrategy,
  OnInit,
  ChangeDetectorRef,
  HostBinding,
} from '@angular/core';
import { CommonModule } from '@angular/common';
import { LottieComponent, AnimationOptions } from 'ngx-lottie';
import { LottieIconsService } from '../../../core/services/lottie-icons.service';
import { IconComponent } from '../icon/icon.component';

/**
 * Renders a free Lottie animation (LottieFiles-style) for sidebar/nav icons.
 * Falls back to app-icon (Bootstrap Icons) if no Lottie path is found or on error.
 */
@Component({
  selector: 'app-lottie-icon',
  standalone: true,
  imports: [CommonModule, LottieComponent, IconComponent],
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    <ng-container *ngIf="lottiePath; else fallback">
      <ng-lottie
        [options]="lottieOptions"
        [width]="sizePx + 'px'"
        [height]="sizePx + 'px'"
        [styles]="containerStyles"
        (error)="onLottieError()"
      />
    </ng-container>
    <ng-template #fallback>
      <app-icon [name]="name || 'circle'" size="sm" [customClass]="customClass" />
    </ng-template>
  `,
})
export class LottieIconComponent implements OnInit, OnChanges {
  @Input() name: string = 'circle';
  @Input() customClass: string = '';
  /** Size in pixels for the Lottie container (nav: 24, submenu: 20). */
  @Input() sizePx: number = 24;

  lottiePath: string | null = null;
  lottieOptions: AnimationOptions | null = null;
  containerStyles: Partial<CSSStyleDeclaration> = {
    margin: '0',
    display: 'flex',
    alignItems: 'center',
    justifyContent: 'center',
  };

  @HostBinding('class') get hostClasses(): string {
    return 'lottie-icon-host ' + (this.customClass || '').trim();
  }

  constructor(
    private lottieIcons: LottieIconsService,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.updateLottie();
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['name'] || changes['sizePx']) {
      this.updateLottie();
      this.cdr.markForCheck();
    }
  }

  private updateLottie(): void {
    this.lottiePath = this.lottieIcons.getLottiePath(this.name);
    if (this.lottiePath) {
      this.lottieOptions = {
        path: this.lottiePath,
        loop: true,
        autoplay: true,
        rendererSettings: {
          preserveAspectRatio: 'xMidYMid meet',
          progressiveLoad: true,
        },
      };
    } else {
      this.lottieOptions = null;
    }
  }

  onLottieError(): void {
    this.lottiePath = null;
    this.lottieOptions = null;
    this.cdr.markForCheck();
  }
}
