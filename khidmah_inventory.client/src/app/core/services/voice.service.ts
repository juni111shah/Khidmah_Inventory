import { Injectable } from '@angular/core';
import { Subject, Observable } from 'rxjs';
import { VoiceCommandResult, HandsFreeVoiceCommand } from '../models/hands-free.model';

// Web Speech API types (not in all TS lib.dom versions)
declare global {
  interface Window {
    SpeechRecognition: new () => ISpeechRecognition;
    webkitSpeechRecognition: new () => ISpeechRecognition;
  }
  interface SpeechRecognitionEvent extends Event {
    resultIndex: number;
    results: SpeechRecognitionResultList;
  }
  interface SpeechRecognitionErrorEvent extends Event {
    error: string;
  }
  interface ISpeechRecognition extends EventTarget {
    continuous: boolean;
    interimResults: boolean;
    lang: string;
    maxAlternatives: number;
    start(): void;
    stop(): void;
    abort(): void;
    onresult: ((event: SpeechRecognitionEvent) => void) | null;
    onerror: ((event: SpeechRecognitionErrorEvent) => void) | null;
    onend: (() => void) | null;
  }
}

/**
 * Voice service using browser Web Speech API (SpeechRecognition).
 * Supports hands-free warehouse commands and numeric recognition.
 */
@Injectable({
  providedIn: 'root'
})
export class VoiceService {
  private recognition: InstanceType<Window['SpeechRecognition']> | null = null;
  private commandSubject = new Subject<VoiceCommandResult>();
  private _isListening = false;
  private restartTimer: ReturnType<typeof setTimeout> | null = null;

  readonly commands$: Observable<VoiceCommandResult> = this.commandSubject.asObservable();

  private static getSpeechRecognition(): (new () => ISpeechRecognition) | null {
    if (typeof window === 'undefined') return null;
    return window.SpeechRecognition || window.webkitSpeechRecognition || null;
  }

  isSupported(): boolean {
    return VoiceService.getSpeechRecognition() !== null;
  }

  get isListening(): boolean {
    return this._isListening;
  }

  /**
   * Start listening for voice commands. Emits parsed commands via commands$.
   */
  startListening(options?: { continuous?: boolean; interimResults?: boolean; lang?: string }): boolean {
    const SR = VoiceService.getSpeechRecognition();
    if (!SR) {
      console.warn('VoiceService: Speech recognition not supported');
      return false;
    }
    if (this._isListening) {
      this.stop();
    }
    this.recognition = new SR();
    this.recognition.continuous = options?.continuous ?? true;
    this.recognition.interimResults = options?.interimResults ?? false;
    this.recognition.lang = options?.lang ?? 'en-US';
    this.recognition.maxAlternatives = 3;

    this.recognition.onresult = (event: SpeechRecognitionEvent) => {
      const result = event.results[event.resultIndex];
      if (!result?.isFinal) {
        return;
      }
      const transcript = result[0].transcript?.trim() ?? '';
      const confidence = result[0].confidence ?? 0;
      const parsed = this.parseCommand(transcript, confidence);
      this.commandSubject.next(parsed);
    };

    this.recognition.onerror = (event: SpeechRecognitionErrorEvent) => {
      if (event.error !== 'aborted' && event.error !== 'no-speech') {
        console.warn('VoiceService recognition error:', event.error);
      }
    };

    this.recognition.onend = () => {
      if (this._isListening && this.recognition) {
        if (this.restartTimer) {
          clearTimeout(this.restartTimer);
        }
        this.restartTimer = setTimeout(() => {
          if (!this._isListening || !this.recognition) return;
          try {
            this.recognition.start();
          } catch {
            this._isListening = false;
          }
        }, 180);
      }
    };

    try {
      this.recognition.start();
      this._isListening = true;
      return true;
    } catch (e) {
      console.warn('VoiceService start failed:', e);
      return false;
    }
  }

  stop(): void {
    this._isListening = false;
    if (this.restartTimer) {
      clearTimeout(this.restartTimer);
      this.restartTimer = null;
    }
    if (this.recognition) {
      try {
        this.recognition.stop();
        this.recognition.abort();
      } catch {}
      this.recognition = null;
    }
  }

  /**
   * Parse transcript into a command and optional numeric value.
   */
  parseCommand(transcript: string, confidence: number): VoiceCommandResult {
    const lower = transcript.toLowerCase().trim();
    const words = lower.split(/\s+/);

    // "quantity five" / "quantity 5" / "five" (standalone number for quantity)
    const quantityMatch = lower.match(/quantity\s+(?:(\d+)|(one|two|three|four|five|six|seven|eight|nine|ten))/);
    if (quantityMatch) {
      const num = quantityMatch[1] ? parseInt(quantityMatch[1], 10) : this.wordToNumber(quantityMatch[2] ?? '');
      if (!isNaN(num)) {
        return { command: 'quantity', numericValue: num, rawTranscript: transcript, confidence };
      }
    }

    // Standalone number (e.g. "five" as quantity)
    const numWord = this.wordToNumber(lower);
    if (numWord >= 0 && numWord <= 99) {
      return { command: 'quantity', numericValue: numWord, rawTranscript: transcript, confidence };
    }
    const digitMatch = lower.match(/^(\d+)$/);
    if (digitMatch) {
      return { command: 'quantity', numericValue: parseInt(digitMatch[1], 10), rawTranscript: transcript, confidence };
    }

    // Command phrases (ordered by specificity)
    const commandPhrases: [string, HandsFreeVoiceCommand][] = [
      ['start picking', 'start_picking'],
      ['start pick', 'start_picking'],
      ['next', 'next'],
      ['next item', 'next'],
      ['confirm', 'confirm'],
      ['confirmed', 'confirm'],
      ['cancel', 'cancel'],
      ['repeat', 'repeat'],
      ['say again', 'repeat'],
      ['scan', 'scan'],
      ['done', 'done'],
      ['finish', 'done'],
      ['complete', 'done']
    ];

    for (const [phrase, cmd] of commandPhrases) {
      if (lower.includes(phrase) || lower === phrase) {
        return { command: cmd, rawTranscript: transcript, confidence };
      }
    }

    return { command: 'unknown', rawTranscript: transcript, confidence };
  }

  private wordToNumber(word: string): number {
    const map: Record<string, number> = {
      zero: 0, one: 1, two: 2, three: 3, four: 4, five: 5, six: 6, seven: 7, eight: 8, nine: 9, ten: 10,
      eleven: 11, twelve: 12, thirteen: 13, fourteen: 14, fifteen: 15, sixteen: 16, seventeen: 17, eighteen: 18, nineteen: 19, twenty: 20
    };
    return map[word] ?? -1;
  }
}
