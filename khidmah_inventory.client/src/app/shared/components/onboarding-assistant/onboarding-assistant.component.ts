import { CommonModule } from '@angular/common';
import { Component, OnDestroy, OnInit } from '@angular/core';
import { Subscription } from 'rxjs';
import { OnboardingService } from '../../../core/services/onboarding.service';

@Component({
  selector: 'app-onboarding-assistant',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="onboarding-help-fab" *ngIf="!tourVisible">
      <button
        type="button"
        class="btn btn-primary btn-sm rounded-pill"
        data-help-skip
        (click)="startTour()"
        title="Start app tour">
        <i class="bi bi-signpost-split me-1"></i>
        App Tour
      </button>

      <button
        type="button"
        class="btn btn-outline-primary btn-sm rounded-pill"
        data-help-skip
        (click)="startPageTour()"
        title="Start page tour">
        <i class="bi bi-map me-1"></i>
        Page Tour
      </button>

      <button
        type="button"
        class="btn btn-outline-secondary btn-sm rounded-pill"
        data-help-skip
        [class.active]="helpMode"
        (click)="toggleHelpMode()"
        [title]="helpMode ? 'Disable help mode' : 'Enable help mode'">
        <i class="bi" [class.bi-info-circle-fill]="helpMode" [class.bi-info-circle]="!helpMode"></i>
        <span class="ms-1">Help</span>
      </button>
    </div>

    <div class="onboarding-tour-overlay" *ngIf="tourVisible">
      <div class="onboarding-tour-backdrop" data-help-skip (click)="skipTour()"></div>
      <div
        class="onboarding-tour-card"
        data-help-skip
        [class.mobile-tour-card]="isMobileLayout"
        [style.left.px]="isMobileLayout ? null : cardX">
        <div class="d-flex justify-content-between align-items-start gap-2">
          <div>
            <div class="small text-muted">Step {{ currentIndex + 1 }} / {{ totalSteps }}</div>
            <h5 class="mb-1">{{ currentTitle }}</h5>
          </div>
          <div class="d-flex align-items-center gap-2">
            <div class="btn-group btn-group-sm tour-align-toggle" role="group" aria-label="Tour card position">
              <button type="button" class="btn btn-outline-secondary" [class.active]="cardAlign === 'auto'" data-help-skip (click)="setCardAlign('auto')" title="Auto position">A</button>
              <button type="button" class="btn btn-outline-secondary" [class.active]="cardAlign === 'left'" data-help-skip (click)="setCardAlign('left')" title="Pin left">L</button>
              <button type="button" class="btn btn-outline-secondary" [class.active]="cardAlign === 'center'" data-help-skip (click)="setCardAlign('center')" title="Pin center">C</button>
              <button type="button" class="btn btn-outline-secondary" [class.active]="cardAlign === 'right'" data-help-skip (click)="setCardAlign('right')" title="Pin right">R</button>
            </div>
            <button type="button" class="btn-close" data-help-skip (click)="skipTour()" aria-label="Close"></button>
          </div>
        </div>
        <p class="mb-3">{{ currentDescription }}</p>
        <div class="d-flex justify-content-between gap-2 flex-wrap">
          <button class="btn btn-outline-secondary btn-sm" data-help-skip (click)="prevStep()">Back</button>
          <div class="d-flex gap-2">
            <button
              *ngIf="hasStepRoute"
              class="btn btn-outline-primary btn-sm"
              data-help-skip
              (click)="openPageOnly()">
              Open Page Tour
            </button>
            <button class="btn btn-primary btn-sm" data-help-skip (click)="onPrimaryAction()">
              {{ primaryActionLabel }}
            </button>
          </div>
        </div>
      </div>
    </div>

    <div
      *ngIf="helpMode && helpInfo"
      class="onboarding-help-bubble"
      data-help-skip
      [style.left.px]="helpInfo.x"
      [style.top.px]="helpInfo.y">
      <div class="d-flex justify-content-between align-items-start gap-2">
        <strong class="small">{{ helpInfo.title }}</strong>
        <button type="button" class="btn-close btn-close-sm" data-help-skip aria-label="Close" (click)="closeHelp()"></button>
      </div>
      <div class="small mt-1">{{ helpInfo.description }}</div>
    </div>
  `,
  styles: [`
    .onboarding-help-fab {
      position: fixed;
      right: 1rem;
      bottom: 1rem;
      z-index: 20010;
      display: flex;
      gap: 0.5rem;
      align-items: center;
    }

    .onboarding-tour-overlay {
      position: fixed;
      inset: 0;
      z-index: 20000;
    }

    .onboarding-tour-backdrop {
      position: absolute;
      inset: 0;
      background: rgba(17, 24, 39, 0.42);
      backdrop-filter: blur(1px);
    }

    .onboarding-tour-card {
      position: fixed;
      top: 0.625rem;
      left: 1rem;
      width: min(340px, calc(100vw - 1rem));
      background: var(--surface-color);
      color: var(--text-color);
      border: 1px solid var(--border-color);
      border-radius: 12px;
      padding: 0.75rem;
      box-shadow: var(--elevation-3);
      z-index: 20020;
      max-height: calc(100vh - 1.25rem);
      overflow: auto;
    }

    @media (max-width: 767.98px) {
      .onboarding-tour-card {
        top: 0.75rem;
        left: 0.75rem;
        right: 0.75rem;
        width: auto;
        max-height: calc(100vh - 1.5rem);
      }
    }

    .onboarding-tour-card p {
      white-space: pre-line;
      font-size: 0.9rem;
      line-height: 1.35;
      margin-bottom: 0.75rem !important;
    }

    .onboarding-tour-card h5 {
      font-size: 1.03rem;
      line-height: 1.2;
    }

    .onboarding-help-bubble {
      position: fixed;
      z-index: 20030;
      transform: translateX(-50%);
      max-width: min(360px, calc(100vw - 1rem));
      background: var(--surface-color);
      border: 1px solid var(--border-color);
      border-radius: 12px;
      box-shadow: var(--elevation-2);
      padding: 0.75rem;
    }

    .btn-close-sm {
      width: 0.7rem;
      height: 0.7rem;
      padding: 0.15rem;
    }

    .tour-align-toggle .btn {
      min-width: 1.65rem;
      padding: 0.1rem 0.35rem;
      line-height: 1;
      font-size: 0.72rem;
    }
  `]
})
export class OnboardingAssistantComponent implements OnInit, OnDestroy {
  tourVisible = false;
  helpMode = false;
  totalSteps = 0;
  currentIndex = 0;
  currentTitle = 'Welcome';
  currentDescription = 'This tour will guide you through the main app flow.';
  currentRoute?: string;
  isLastStep = false;
  cardX = 24;
  cardY = 24;
  isMobileLayout = false;
  cardAlign: 'auto' | 'left' | 'center' | 'right' = 'auto';
  helpInfo: { title: string; description: string; x: number; y: number } | null = null;

  private readonly subscriptions = new Subscription();

  constructor(private readonly onboardingService: OnboardingService) {}

  get hasStepRoute(): boolean {
    return !!this.currentRoute;
  }

  get primaryActionLabel(): string {
    return this.isLastStep ? 'Finish' : 'Next';
  }

  ngOnInit(): void {
    this.totalSteps = this.onboardingService.getTotalSteps();

    this.subscriptions.add(
      this.onboardingService.tourVisible$.subscribe(visible => {
        this.tourVisible = visible;
        if (visible) {
          this.repositionCard();
        }
      })
    );

    this.subscriptions.add(
      this.onboardingService.currentStep$.subscribe(step => {
        if (!step) {
          return;
        }
        this.currentTitle = step.title;
        this.currentDescription = step.description;
        this.currentRoute = step.route;
        this.repositionCard(step.selector);
      })
    );

    this.subscriptions.add(
      this.onboardingService.currentStepIndex$.subscribe(index => {
        this.currentIndex = index;
        this.totalSteps = Math.max(this.totalSteps, this.onboardingService.getTotalSteps());
        this.isLastStep = this.totalSteps > 0 && index >= this.totalSteps - 1;
      })
    );

    this.subscriptions.add(
      this.onboardingService.helpMode$.subscribe(enabled => {
        this.helpMode = enabled;
      })
    );

    this.subscriptions.add(
      this.onboardingService.activeHelp$.subscribe(helpInfo => {
        this.helpInfo = helpInfo;
      })
    );
  }

  ngOnDestroy(): void {
    this.subscriptions.unsubscribe();
  }

  startTour(): void {
    this.onboardingService.startTour();
    this.totalSteps = this.onboardingService.getTotalSteps();
  }

  startPageTour(): void {
    this.onboardingService.startPageTour();
    this.totalSteps = this.onboardingService.getTotalSteps();
  }

  skipTour(): void {
    this.onboardingService.endTour(true);
  }

  nextStep(): void {
    this.onboardingService.nextStep();
  }

  onPrimaryAction(): void {
    this.nextStep();
  }

  openPageOnly(): void {
    if (!this.hasStepRoute) {
      return;
    }
    this.onboardingService.openCurrentStepRoute(true);
  }

  prevStep(): void {
    this.onboardingService.previousStep();
  }

  toggleHelpMode(): void {
    this.onboardingService.toggleHelpMode();
  }

  closeHelp(): void {
    this.onboardingService.closeActiveHelp();
  }

  setCardAlign(align: 'auto' | 'left' | 'center' | 'right'): void {
    this.cardAlign = align;
    this.repositionCard();
  }

  private repositionCard(selector?: string): void {
    window.setTimeout(() => {
      this.isMobileLayout = window.innerWidth < 768;
      if (this.isMobileLayout) {
        return;
      }

      const cardElement = document.querySelector('.onboarding-tour-card') as HTMLElement | null;
      const cardWidth = Math.min(cardElement?.offsetWidth || 340, window.innerWidth - 16);
      const maxLeft = Math.max(12, window.innerWidth - cardWidth - 12);

      if (this.cardAlign !== 'auto') {
        if (this.cardAlign === 'left') {
          this.cardX = 12;
          return;
        }
        if (this.cardAlign === 'right') {
          this.cardX = maxLeft;
          return;
        }
        this.cardX = Math.max(12, Math.min((window.innerWidth - cardWidth) / 2, maxLeft));
        return;
      }

      const target = selector ? this.findBestTarget(selector) : null;
      if (!target) {
        this.cardX = Math.max(12, Math.min((window.innerWidth - cardWidth) / 2, maxLeft));
        return;
      }

      const rect = target.getBoundingClientRect();
      const targetCenter = rect.left + (rect.width / 2);
      const desiredLeft = targetCenter - (cardWidth / 2);
      let left = Math.max(12, Math.min(desiredLeft, maxLeft));

      // Keep the card on top row, but avoid covering top targets (header/user menu).
      const cardTop = 10;
      const cardHeight = cardElement?.offsetHeight || 220;
      const targetIsNearTop = rect.top < 180;
      const verticalOverlap = (cardTop + cardHeight) > rect.top;
      const horizontalOverlap = this.rangesOverlap(left, left + cardWidth, rect.left, rect.right);

      if (targetIsNearTop && verticalOverlap && horizontalOverlap) {
        const placeLeft = Math.max(12, rect.left - cardWidth - 14);
        const placeRight = Math.min(maxLeft, rect.right + 14);

        const leftSpace = rect.left - 14;
        const rightSpace = window.innerWidth - rect.right - 14;
        left = rightSpace >= leftSpace ? placeRight : placeLeft;
        left = Math.max(12, Math.min(left, maxLeft));
      }

      this.cardX = left;
    }, 40);
  }

  private rangesOverlap(aStart: number, aEnd: number, bStart: number, bEnd: number): boolean {
    return aStart < bEnd && bStart < aEnd;
  }

  private findBestTarget(selector: string): HTMLElement | null {
    const candidates = Array.from(document.querySelectorAll<HTMLElement>(selector));
    if (candidates.length === 0) {
      return null;
    }

    const visible = candidates.filter(el => this.isElementVisible(el));
    if (visible.length === 0) {
      return candidates[0];
    }

    const nearViewport = visible.find(el => {
      const rect = el.getBoundingClientRect();
      return rect.bottom > 0 && rect.top < window.innerHeight;
    });

    return nearViewport || visible[0];
  }

  private isElementVisible(element: HTMLElement): boolean {
    const style = window.getComputedStyle(element);
    if (style.display === 'none' || style.visibility === 'hidden' || style.opacity === '0') {
      return false;
    }
    const rect = element.getBoundingClientRect();
    return rect.width > 0 && rect.height > 0;
  }
}

