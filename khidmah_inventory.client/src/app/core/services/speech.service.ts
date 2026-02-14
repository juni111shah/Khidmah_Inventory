import { Injectable } from '@angular/core';

/**
 * Speech output using browser speechSynthesis (text-to-speech).
 * Used to speak instructions to the worker in hands-free mode.
 */
@Injectable({
  providedIn: 'root'
})
export class SpeechService {
  private synth: SpeechSynthesis;
  private _enabled = true;
  private _rate = 0.95;
  private _pitch = 1;
  private _volume = 1;

  constructor() {
    this.synth = typeof window !== 'undefined' ? window.speechSynthesis : (null as unknown as SpeechSynthesis);
  }

  isSupported(): boolean {
    return typeof this.synth?.speak === 'function';
  }

  set enabled(value: boolean) {
    this._enabled = value;
  }

  get enabled(): boolean {
    return this._enabled;
  }

  set rate(value: number) {
    this._rate = Math.max(0.5, Math.min(2, value));
  }

  set pitch(value: number) {
    this._pitch = Math.max(0.5, Math.min(2, value));
  }

  set volume(value: number) {
    this._volume = Math.max(0, Math.min(1, value));
  }

  /**
   * Speak a phrase. Queues if something is already speaking.
   */
  speak(text: string, options?: { interrupt?: boolean }): void {
    if (!this._enabled || !text?.trim() || !this.isSupported()) return;
    if (options?.interrupt) {
      this.synth.cancel();
    }
    const u = new SpeechSynthesisUtterance(text.trim());
    u.rate = this._rate;
    u.pitch = this._pitch;
    u.volume = this._volume;
    u.lang = 'en-US';
    this.synth.speak(u);
  }

  /**
   * Speak and return a Promise that resolves when done (or when interrupted).
   */
  speakAsync(text: string, options?: { interrupt?: boolean }): Promise<void> {
    return new Promise((resolve) => {
      if (!this._enabled || !text?.trim() || !this.isSupported()) {
        resolve();
        return;
      }
      if (options?.interrupt) {
        this.synth.cancel();
      }
      const u = new SpeechSynthesisUtterance(text.trim());
      u.rate = this._rate;
      u.pitch = this._pitch;
      u.volume = this._volume;
      u.lang = 'en-US';
      u.onend = () => resolve();
      u.onerror = () => resolve();
      this.synth.speak(u);
    });
  }

  cancel(): void {
    if (this.isSupported()) {
      this.synth.cancel();
    }
  }

  /** Convenience: speak go-to location instruction */
  sayGoToLocation(location: string): void {
    this.speak(`Go to ${location}`, { interrupt: true });
  }

  /** Convenience: speak pick quantity instruction */
  sayPickQuantity(quantity: number, itemName?: string): void {
    const item = itemName ? ` ${itemName}` : '';
    this.speak(`Pick ${quantity}${item}`, { interrupt: true });
  }

  /** Convenience: speak scan barcode instruction */
  sayScanBarcode(): void {
    this.speak('Scan barcode', { interrupt: true });
  }

  sayConfirmed(): void {
    this.speak('Confirmed', { interrupt: true });
  }

  sayError(message: string): void {
    this.speak(message, { interrupt: true });
  }
}
