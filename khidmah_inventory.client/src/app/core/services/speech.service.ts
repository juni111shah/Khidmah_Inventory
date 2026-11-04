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
  private _rate = 1;
  private _pitch = 1;
  private _volume = 1;
  private preferredVoice: SpeechSynthesisVoice | null = null;

  constructor() {
    this.synth = typeof window !== 'undefined' ? window.speechSynthesis : (null as unknown as SpeechSynthesis);
    if (this.isSupported()) {
      this.pickPreferredVoice();
      if (typeof this.synth.onvoiceschanged !== 'undefined') {
        this.synth.onvoiceschanged = () => this.pickPreferredVoice();
      }
    }
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
    const chunks = this.chunkText(text);
    for (const chunk of chunks) {
      const u = this.buildUtterance(chunk);
      this.synth.speak(u);
    }
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
      const chunks = this.chunkText(text);
      if (chunks.length === 0) {
        resolve();
        return;
      }

      let index = 0;
      const speakNext = () => {
        if (index >= chunks.length) {
          resolve();
          return;
        }
        const u = this.buildUtterance(chunks[index]);
        index += 1;
        u.onend = () => speakNext();
        u.onerror = () => speakNext();
        this.synth.speak(u);
      };
      speakNext();
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

  private buildUtterance(text: string): SpeechSynthesisUtterance {
    const u = new SpeechSynthesisUtterance(text.trim());
    u.rate = this._rate;
    u.pitch = this._pitch;
    u.volume = this._volume;
    u.lang = 'en-US';
    if (this.preferredVoice) {
      u.voice = this.preferredVoice;
      u.lang = this.preferredVoice.lang || u.lang;
    }
    return u;
  }

  private pickPreferredVoice(): void {
    const voices = this.synth?.getVoices?.() ?? [];
    if (!voices.length) return;

    const english = voices.filter(v => (v.lang || '').toLowerCase().startsWith('en'));
    const pool = english.length ? english : voices;
    this.preferredVoice =
      pool.find(v => /neural|natural/i.test(v.name)) ||
      pool.find(v => /google|microsoft/i.test(v.name)) ||
      pool[0] ||
      null;
  }

  private chunkText(text: string): string[] {
    const normalized = (text || '').replace(/\s+/g, ' ').trim();
    if (!normalized) return [];
    if (normalized.length <= 220) return [normalized];

    const parts = normalized.split(/(?<=[.!?])\s+/);
    const chunks: string[] = [];
    let current = '';

    for (const part of parts) {
      if (!part) continue;
      const next = current ? `${current} ${part}` : part;
      if (next.length > 220 && current) {
        chunks.push(current);
        current = part;
      } else {
        current = next;
      }
    }
    if (current) chunks.push(current);
    return chunks;
  }
}
