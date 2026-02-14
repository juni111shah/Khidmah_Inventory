import { Component, ElementRef, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { Subject, Subscription } from 'rxjs';
import { debounceTime, distinctUntilChanged } from 'rxjs/operators';
import jsPDF from 'jspdf';
import { Router } from '@angular/router';
import { HeaderService } from '../../../core/services/header.service';
import { AiOrchestratorService } from '../../../core/services/ai-orchestrator.service';
import { SpeechService } from '../../../core/services/speech.service';
import { VoiceService } from '../../../core/services/voice.service';
import { SignalRService } from '../../../core/services/signalr.service';
import { ApiConfigService } from '../../../core/services/api-config.service';
import { OnboardingService } from '../../../core/services/onboarding.service';
import { AppHelpService } from '../../../core/services/app-help.service';

type ChatRole = 'user' | 'assistant';
type DownloadMethod = 'GET' | 'POST' | 'CLIENT_CSV' | 'CLIENT_PDF';
interface QuickOption {
  label: string;
  command: string;
}
interface DownloadAction {
  method: DownloadMethod;
  url?: string;
  body?: unknown;
  fileName?: string;
}
interface ChatMessage {
  role: ChatRole;
  text: string;
  ts: Date;
  downloadAction?: DownloadAction;
}

@Component({
  selector: 'app-chat-assistant',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './chat-assistant.component.html',
  styleUrls: ['./chat-assistant.component.scss']
})
export class ChatAssistantComponent implements OnInit, OnDestroy {
  @ViewChild('messagesBox') private messagesBox?: ElementRef<HTMLDivElement>;

  messages: ChatMessage[] = [];
  input = '';
  loading = false;
  listening = false;
  starterOptions: QuickOption[] = [
    { label: 'Hello', command: 'hello' },
    { label: 'Hi', command: 'hi' },
    { label: 'Start App Tour', command: 'start app tour' },
    { label: 'Start Page Tour', command: 'start page tour' },
    { label: 'Create Sales Order', command: 'create sales order' },
    { label: 'Create Purchase Order', command: 'create purchase order' },
    { label: 'Stock Query', command: 'stock query' },
    { label: 'Generate Invoice', command: 'generate invoice' },
    { label: 'Sales Report', command: 'sales report this month' },
    { label: 'Create Product', command: 'create product' },
    { label: 'Update Supplier', command: 'update supplier' }
  ];
  contextualOptions: QuickOption[] = [];
  readonly slashHints = '/help  /sales  /purchase  /stock  /invoice  /report  /app-tour  /page-tour  /download  /clear';
  private voiceSub?: Subscription;
  private voiceDispatchSub?: Subscription;
  private stockSub?: Subscription;
  private lastDownloadAction?: DownloadAction;
  private voiceTranscript$ = new Subject<string>();
  private lastVoiceSentText = '';
  private lastVoiceSentAt = 0;
  private queuedVoiceText: string | null = null;
  private micWasActiveBeforeAssistantSpeech = false;

  get showTaskControls(): boolean {
    const lastAssistant = [...this.messages].reverse().find(m => m.role === 'assistant');
    if (!lastAssistant?.text) return false;
    const text = lastAssistant.text.toLowerCase();
    return text.includes('say done') ||
      text.includes('say yes or no') ||
      text.includes('did you mean') ||
      text.includes('what is the') ||
      text.includes('which product') ||
      text.includes('from which warehouse') ||
      text.includes('please provide') ||
      text.includes('optional, say next to skip');
  }

  constructor(
    private header: HeaderService,
    private orchestrator: AiOrchestratorService,
    private speech: SpeechService,
    private voice: VoiceService,
    private signalR: SignalRService,
    private apiConfig: ApiConfigService,
    private http: HttpClient,
    private router: Router,
    private onboarding: OnboardingService,
    private appHelp: AppHelpService
  ) {}

  ngOnInit(): void {
    this.header.setHeaderInfo({
      title: 'AI Copilot Assistant',
      description: 'Create orders, update inventory and prices using chat or voice'
    });
    this.contextualOptions = this.getMainOptions();
    this.pushAssistant("Hello! I'm your Khidmah Copilot. Tell me what you want to do: Sales Order, Purchase Order, Inventory Update, Price Update, Stock Query, reports, invoices, or CRUD for Products/Customers/Suppliers.");
    this.voiceSub = this.voice.commands$.subscribe((cmd) => {
      if (!this.listening) return;
      if (!cmd.rawTranscript?.trim()) return;
      this.voiceTranscript$.next(cmd.rawTranscript.trim());
    });
    this.voiceDispatchSub = this.voiceTranscript$
      .pipe(debounceTime(420), distinctUntilChanged())
      .subscribe((text) => {
        if (!this.listening || !text) return;
        if (this.loading) {
          this.queuedVoiceText = text;
          return;
        }
        const now = Date.now();
        const normalized = text.toLowerCase();
        if (normalized === this.lastVoiceSentText && (now - this.lastVoiceSentAt) < 1600) {
          return;
        }
        this.lastVoiceSentText = normalized;
        this.lastVoiceSentAt = now;
        this.input = text;
        this.send(false);
      });
    this.stockSub = this.signalR.getStockChanged().subscribe(() => {
      this.pushAssistant('Notice: stock changed externally. I will re-validate before confirmation.');
    });
  }

  ngOnDestroy(): void {
    this.voiceSub?.unsubscribe();
    this.voiceDispatchSub?.unsubscribe();
    this.stockSub?.unsubscribe();
    this.voice.stop();
  }

  send(confirmed = false): void {
    const text = this.normalizeInput(this.input);
    if (!text || this.loading) return;
    if (this.handleLocalCommand(text)) {
      this.input = '';
      return;
    }
    this.pushUser(text);
    this.loading = true;
    this.input = '';
    this.orchestrator.send(text, confirmed).subscribe({
      next: ({ reply, raw }) => {
        this.loading = false;
        const downloadAction = this.extractDownloadAction(reply, raw?.result);
        this.pushAssistant(reply, downloadAction);
        if (downloadAction) this.lastDownloadAction = downloadAction;
        if (downloadAction && this.isDownloadCommand(text)) {
          this.openDownload(downloadAction);
        }
        this.speakAssistantReply(reply);
      },
      error: () => {
        this.loading = false;
        const err = 'Assistant failed. Please try again.';
        this.pushAssistant(err);
        this.speakAssistantReply(err);
      }
    });
  }

  confirmDone(): void {
    this.input = 'done';
    this.send(true);
  }

  cancelTask(): void {
    this.input = 'cancel';
    this.send(false);
  }

  repeat(): void {
    this.input = 'repeat';
    this.send(false);
  }

  next(): void {
    this.input = 'next';
    this.send(false);
  }

  toggleMic(): void {
    if (this.listening) {
      this.voice.stop();
      this.listening = false;
      return;
    }
    const started = this.voice.startListening({ continuous: true, interimResults: false, lang: 'en-US' });
    this.listening = started;
  }

  sendPreset(command: string): void {
    if (this.loading) return;
    this.input = command;
    this.send(false);
  }

  getDownloadLabel(action: DownloadAction): string {
    const method = (action.method || 'GET').toUpperCase();
    const name = (action.fileName || '').toLowerCase();
    if (method === 'CLIENT_CSV' || name.endsWith('.csv')) return 'Download CSV';
    if (method === 'CLIENT_PDF' || name.endsWith('.pdf')) return 'Download PDF';
    return 'Download File';
  }

  sendHelp(): void {
    this.sendPreset('help');
  }

  showMainOptions(): void {
    this.contextualOptions = this.getMainOptions();
    this.pushAssistant('Here are common options: Sales Order, Purchase Order, Inventory Update, Price Update, Stock Query, Product/Customer/Supplier CRUD, reports, and invoices.');
  }

  clearChat(): void {
    if (this.loading) return;
    this.messages = [];
    this.lastDownloadAction = undefined;
    this.contextualOptions = this.getMainOptions();
    this.pushAssistant("Chat cleared. Hello again! Choose an option below or type /help.");
  }

  downloadLast(): void {
    if (!this.lastDownloadAction) {
      this.pushAssistant('No generated document found yet. Ask me to generate an invoice/report first.');
      return;
    }
    this.openDownload(this.lastDownloadAction);
  }

  private pushUser(text: string): void {
    this.messages.push({ role: 'user', text, ts: new Date() });
    this.scrollToBottom();
  }

  private pushAssistant(text: string, downloadAction?: DownloadAction): void {
    this.messages.push({ role: 'assistant', text, ts: new Date(), downloadAction });
    this.contextualOptions = this.buildContextualOptions(text);
    this.scrollToBottom();
  }

  openDownload(action: DownloadAction): void {
    const method = (action.method || 'GET').toUpperCase();
    if (method === 'CLIENT_CSV') {
      this.downloadClientCsv(action.body, action.fileName || 'export.csv');
      return;
    }
    if (method === 'CLIENT_PDF') {
      this.downloadClientPdf(action.body, action.fileName || 'export.pdf');
      return;
    }

    if (!action.url) {
      this.pushAssistant('Download failed: missing file URL.');
      return;
    }

    const absolute = this.toAbsoluteDownloadUrl(action.url);
    const request$ = method === 'POST'
      ? this.http.post(absolute, this.parseDownloadBody(action.body), { observe: 'response', responseType: 'blob' })
      : this.http.get(absolute, { observe: 'response', responseType: 'blob' });

    request$.subscribe({
      next: (res) => {
        const contentType = (res.headers.get('content-type') || '').toLowerCase();
        if (!contentType.includes('application/pdf')) {
          this.pushAssistant('Download failed: server did not return a PDF. Please check your permissions and try again.');
          return;
        }

        const blob = res.body;
        if (!blob) {
          this.pushAssistant('Download failed: empty file response.');
          return;
        }

        const blobUrl = URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = blobUrl;
        a.download = action.fileName || this.deriveFileName(action.url || '');
        a.click();
        URL.revokeObjectURL(blobUrl);
      },
      error: () => {
        this.pushAssistant('Download failed. Please make sure you are logged in and have document permissions.');
      }
    });
  }

  private extractDownloadAction(reply: string, result: unknown): DownloadAction | undefined {
    const resultObj = (result as {
      downloadUrl?: string;
      downloadAction?: { method?: string; url?: string; body?: unknown; fileName?: string };
      pdfEndpoint?: string;
      pdfRequest?: unknown;
    } | null);

    const action = resultObj?.downloadAction;
    if (action?.method?.toUpperCase() === 'CLIENT_CSV') {
      return {
        method: 'CLIENT_CSV',
        body: action.body,
        fileName: action.fileName || 'export.csv'
      };
    }
    if (action?.method?.toUpperCase() === 'CLIENT_PDF') {
      return {
        method: 'CLIENT_PDF',
        body: action.body,
        fileName: action.fileName || 'export.pdf'
      };
    }

    if (action?.url) {
      return {
        method: (action.method?.toUpperCase() as DownloadMethod) || 'GET',
        url: this.toAbsoluteDownloadUrl(action.url),
        body: action.body,
        fileName: action.fileName
      };
    }

    const resultDownload = resultObj?.downloadUrl;
    if (resultDownload) return { method: 'GET', url: this.toAbsoluteDownloadUrl(resultDownload) };

    if (resultObj?.pdfEndpoint) {
      return {
        method: 'POST',
        url: this.toAbsoluteDownloadUrl(resultObj.pdfEndpoint),
        body: resultObj.pdfRequest
      };
    }

    const match = (reply || '').match(/(\/api\/documents\/[^\s]+)/i);
    if (match?.[1]) return { method: 'GET', url: this.toAbsoluteDownloadUrl(match[1]) };
    return undefined;
  }

  private downloadClientCsv(content: unknown, fileName: string): void {
    const text = typeof content === 'string' ? content : '';
    if (!text) {
      this.pushAssistant('Download failed: CSV content is empty.');
      return;
    }

    const blob = new Blob([text], { type: 'text/csv;charset=utf-8;' });
    const blobUrl = URL.createObjectURL(blob);
    const a = document.createElement('a');
    a.href = blobUrl;
    a.download = fileName || 'export.csv';
    a.click();
    URL.revokeObjectURL(blobUrl);
  }

  private downloadClientPdf(content: unknown, fileName: string): void {
    const payload = typeof content === 'string' ? this.parseDownloadBody(content) : content;
    const data = payload as { title?: string; headers?: string[]; rows?: string[][] } | undefined;
    if (!data || !Array.isArray(data.headers) || !Array.isArray(data.rows)) {
      this.pushAssistant('Download failed: invalid PDF payload.');
      return;
    }

    const pdf = new jsPDF('landscape');
    const pageWidth = pdf.internal.pageSize.getWidth();
    const pageHeight = pdf.internal.pageSize.getHeight();
    const margin = 10;
    let y = margin;

    pdf.setFontSize(14);
    pdf.text(data.title || 'Export', margin, y);
    y += 8;
    pdf.setFontSize(8);
    pdf.text(`Generated: ${new Date().toLocaleString()}`, margin, y);
    y += 8;

    const columns = data.headers.length || 1;
    const colWidth = (pageWidth - (margin * 2)) / columns;

    pdf.setFont('helvetica', 'bold');
    data.headers.forEach((header, idx) => {
      pdf.text(String(header ?? ''), margin + (idx * colWidth), y);
    });
    y += 5;
    pdf.line(margin, y, pageWidth - margin, y);
    y += 5;
    pdf.setFont('helvetica', 'normal');

    data.rows.forEach((row) => {
      if (y > pageHeight - margin) {
        pdf.addPage();
        y = margin;
      }
      row.forEach((cell, idx) => {
        const value = String(cell ?? '');
        const truncated = value.length > 26 ? `${value.slice(0, 23)}...` : value;
        pdf.text(truncated, margin + (idx * colWidth), y);
      });
      y += 5;
    });

    pdf.save(fileName || 'export.pdf');
  }

  private toAbsoluteDownloadUrl(pathOrUrl: string): string {
    if (!pathOrUrl) return '';
    if (pathOrUrl.startsWith('http://') || pathOrUrl.startsWith('https://')) return pathOrUrl;
    const base = this.apiConfig.getBaseUrl().replace(/\/+$/, '');
    const path = pathOrUrl.startsWith('/') ? pathOrUrl : `/${pathOrUrl}`;
    return `${base}${path}`;
  }

  private deriveFileName(url: string): string {
    const normalized = (url || '').toLowerCase();
    if (normalized.includes('/invoice/')) return 'invoice.pdf';
    if (normalized.includes('/purchase-order/')) return 'purchase-order.pdf';
    if (normalized.includes('/reports/sales/pdf')) return 'sales-report.pdf';
    if (normalized.includes('/reports/purchase/pdf')) return 'purchase-report.pdf';
    if (normalized.includes('/reports/inventory/pdf')) return 'inventory-report.pdf';
    return 'document.pdf';
  }

  private isDownloadCommand(text: string): boolean {
    const normalized = (text || '').trim().toLowerCase();
    return normalized === 'download' || normalized === 'open file' || normalized === 'open link' || normalized === '/download';
  }

  private parseDownloadBody(body: unknown): unknown {
    if (typeof body !== 'string') return body;
    try {
      return JSON.parse(body);
    } catch {
      return {};
    }
  }

  private normalizeInput(value: string): string {
    const normalized = (value || '').trim();
    if (!normalized) return '';

    const shortcutMap: Record<string, string> = {
      '/help': 'help',
      '/menu': 'help',
      '/sales': 'sales order',
      '/purchase': 'purchase order',
      '/inventory': 'inventory update',
      '/price': 'price update',
      '/stock': 'stock query',
      '/invoice': 'generate invoice',
      '/report': 'report',
      '/app-tour': 'start app tour',
      '/page-tour': 'start page tour',
      '/products': 'list products',
      '/customers': 'list customers',
      '/salesdetails': 'sales order details',
      '/exportproducts': 'export products',
      '/exportcustomers': 'export customers',
      '/exportsales': 'export sales orders',
      '/download': 'download'
    };

    return shortcutMap[normalized.toLowerCase()] || normalized;
  }

  private handleLocalCommand(text: string): boolean {
    const normalized = text.toLowerCase();
    if (normalized === '/clear' || normalized === 'clear chat') {
      this.clearChat();
      return true;
    }

    if ((normalized === 'download last' || normalized === 'last download') && this.lastDownloadAction) {
      this.openDownload(this.lastDownloadAction);
      return true;
    }

    if (normalized === '/app-tour' || normalized === 'app tour' || normalized === 'start app tour') {
      this.pushUser(text);
      this.onboarding.startTour();
      this.pushAssistant('Started app tour. Follow the highlighted steps.');
      return true;
    }

    if (normalized === '/page-tour' || normalized === 'page tour' || normalized === 'start page tour') {
      this.pushUser(text);
      this.onboarding.startPageTour();
      this.pushAssistant('Started page tour for the current screen.');
      return true;
    }

    // Only trigger local help on explicit help/tour/control questions.
    // All operational commands (sales report, create order, stock query, etc.)
    // must go to backend intent execution.
    if (this.isExplicitHelpQuery(normalized) && !this.isAwaitingTransactionalInput()) {
      const help = this.appHelp.resolveHelpQuestion(normalized, this.router.url);
      if (help.handled) {
        this.pushUser(text);
        this.pushAssistant(help.reply);
        return true;
      }
    }

    return false;
  }

  private flushQueuedVoice(): void {
    if (this.loading || !this.queuedVoiceText?.trim()) {
      return;
    }
    const queued = this.queuedVoiceText.trim();
    this.queuedVoiceText = null;
    this.input = queued;
    this.send(false);
  }

  private speakAssistantReply(text: string): void {
    if (!text?.trim()) {
      this.flushQueuedVoice();
      return;
    }

    this.micWasActiveBeforeAssistantSpeech = this.listening;
    if (this.micWasActiveBeforeAssistantSpeech) {
      this.voice.stop();
      this.listening = false;
    }

    this.speech.speakAsync(text, { interrupt: true }).finally(() => {
      if (this.micWasActiveBeforeAssistantSpeech) {
        const resumed = this.voice.startListening({ continuous: true, interimResults: false, lang: 'en-US' });
        this.listening = resumed;
      }
      this.micWasActiveBeforeAssistantSpeech = false;
      this.flushQueuedVoice();
    });
  }

  private isExplicitHelpQuery(text: string): boolean {
    return text === '/help' ||
      text.includes('help') ||
      text.includes('how to') ||
      text.includes('where is') ||
      text.includes('what does') ||
      text.includes('guide') ||
      text.includes('tour') ||
      text.includes('button') ||
      text.includes('control') ||
      text.includes('screen') ||
      text.includes('page');
  }

  private isAwaitingTransactionalInput(): boolean {
    const lastAssistant = [...this.messages].reverse().find(m => m.role === 'assistant');
    if (!lastAssistant?.text) return false;
    const t = lastAssistant.text.toLowerCase();
    return t.includes('did you mean') ||
      t.includes('say yes or no') ||
      t.includes('who is the customer') ||
      t.includes('which product') ||
      t.includes('what quantity') ||
      t.includes('from which warehouse') ||
      t.includes('please provide') ||
      t.includes('say done to confirm') ||
      t.includes('say done or cancel');
  }

  private getMainOptions(): QuickOption[] {
    return [
      { label: 'App Tour', command: 'start app tour' },
      { label: 'Page Tour', command: 'start page tour' },
      { label: 'Sales Order', command: 'sales order' },
      { label: 'Purchase Order', command: 'purchase order' },
      { label: 'Stock Query', command: 'stock query' },
      { label: 'Price Update', command: 'price update' },
      { label: 'Create Product', command: 'create product' },
      { label: 'Create Customer', command: 'create customer' },
      { label: 'Create Supplier', command: 'create supplier' },
      { label: 'Generate Invoice', command: 'generate invoice' },
      { label: 'Sales Report', command: 'sales report' },
      { label: 'Product Details', command: 'product details' },
      { label: 'Export Products', command: 'export products' }
    ];
  }

  private buildContextualOptions(lastAssistantText: string): QuickOption[] {
    const text = (lastAssistantText || '').toLowerCase();
    if (text.includes('confirm')) {
      return [
        { label: 'Done', command: 'done' },
        { label: 'Cancel', command: 'cancel' },
        { label: 'Repeat', command: 'repeat' }
      ];
    }

    if (text.includes('provide from date') || text.includes('provide to date') || text.includes('as of date')) {
      return this.getDateRangeOptions();
    }

    if (text.includes('which report do you want') || text.includes('i can generate sales, purchase, inventory, profit') || text.includes('for reports, i will ask from date and to date')) {
      return [
        { label: 'Sales Report', command: 'sales report' },
        { label: 'Purchase Report', command: 'purchase report' },
        { label: 'Inventory Report', command: 'inventory report' },
        { label: 'Profit & Loss', command: 'profit and loss report' },
        { label: 'Balance Sheet', command: 'balance sheet report' }
      ];
    }

    if (text.includes('download') || text.includes('invoice') || text.includes('pdf') || text.includes('report')) {
      return [
        { label: 'Download', command: 'download' },
        { label: 'Generate Invoice', command: 'generate invoice' },
        { label: 'Sales Report', command: 'sales report this month' },
        { label: 'Purchase Report', command: 'purchase report this month' }
      ];
    }

    if (text.includes('customer')) {
      return [
        { label: 'Create Customer', command: 'create customer' },
        { label: 'Update Customer', command: 'update customer' },
        { label: 'Delete Customer', command: 'delete customer' },
        { label: 'List Customers', command: 'list customers' },
        { label: 'Customer Details', command: 'customer details' },
        { label: 'Export Customers', command: 'export customers' }
      ];
    }

    if (text.includes('supplier')) {
      return [
        { label: 'Create Supplier', command: 'create supplier' },
        { label: 'Update Supplier', command: 'update supplier' },
        { label: 'Delete Supplier', command: 'delete supplier' },
        { label: 'List Suppliers', command: 'list suppliers' }
      ];
    }

    if (text.includes('product')) {
      return [
        { label: 'Create Product', command: 'create product' },
        { label: 'Update Product', command: 'update product' },
        { label: 'Delete Product', command: 'delete product' },
        { label: 'List Products', command: 'list products' },
        { label: 'Product Details', command: 'product details' },
        { label: 'Export Products', command: 'export products' }
      ];
    }

    if (text.includes('stock') || text.includes('warehouse')) {
      return [
        { label: 'Stock Query', command: 'stock query' },
        { label: 'Inventory Update', command: 'inventory update' },
        { label: 'Sales Order', command: 'sales order' }
      ];
    }

    return this.getMainOptions();
  }

  private getDateRangeOptions(): QuickOption[] {
    const today = new Date();
    const thisWeekStart = this.startOfWeek(today);
    const thisMonthStart = new Date(today.getFullYear(), today.getMonth(), 1);
    const lastMonthStart = new Date(today.getFullYear(), today.getMonth() - 1, 1);
    const lastMonthEnd = new Date(today.getFullYear(), today.getMonth(), 0);

    return [
      { label: 'Today', command: `${this.formatDate(today)} to ${this.formatDate(today)}` },
      { label: 'This Week', command: `${this.formatDate(thisWeekStart)} to ${this.formatDate(today)}` },
      { label: 'This Month', command: `${this.formatDate(thisMonthStart)} to ${this.formatDate(today)}` },
      { label: 'Last Month', command: `${this.formatDate(lastMonthStart)} to ${this.formatDate(lastMonthEnd)}` },
      { label: 'Yesterday', command: `${this.formatDate(new Date(today.getFullYear(), today.getMonth(), today.getDate() - 1))} to ${this.formatDate(new Date(today.getFullYear(), today.getMonth(), today.getDate() - 1))}` }
    ];
  }

  private startOfWeek(date: Date): Date {
    const d = new Date(date.getFullYear(), date.getMonth(), date.getDate());
    const day = d.getDay();
    const diff = day === 0 ? -6 : 1 - day; // Monday-based week
    d.setDate(d.getDate() + diff);
    return d;
  }

  private formatDate(date: Date): string {
    const yyyy = date.getFullYear();
    const mm = `${date.getMonth() + 1}`.padStart(2, '0');
    const dd = `${date.getDate()}`.padStart(2, '0');
    return `${yyyy}-${mm}-${dd}`;
  }

  private scrollToBottom(): void {
    setTimeout(() => {
      const box = this.messagesBox?.nativeElement;
      if (!box) return;
      box.scrollTop = box.scrollHeight;
    });
  }
}
