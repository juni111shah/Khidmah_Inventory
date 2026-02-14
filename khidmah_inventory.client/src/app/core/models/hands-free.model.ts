/** Voice command types for hands-free warehouse */
export type HandsFreeVoiceCommand =
  | 'start_picking'
  | 'next'
  | 'confirm'
  | 'cancel'
  | 'repeat'
  | 'scan'
  | 'quantity'
  | 'done'
  | 'unknown';

export interface VoiceCommandResult {
  command: HandsFreeVoiceCommand;
  /** For "quantity five" this is 5 */
  numericValue?: number;
  rawTranscript: string;
  confidence: number;
}

export interface HandsFreeTaskDto {
  taskId: string;
  productId: string;
  productName: string;
  barcode: string | null;
  sku: string;
  location: string;
  warehouseId: string;
  quantity: number;
  sequence: number;
}

export interface HandsFreeSessionDto {
  sessionId: string;
  startedAt: string;
  warehouseId: string;
  warehouseName: string;
}

export interface HandsFreeTasksResponse {
  session: HandsFreeSessionDto;
  tasks: HandsFreeTaskDto[];
  currentIndex: number;
}

export interface CompleteHandsFreeTaskRequest {
  taskId: string;
  productId: string;
  warehouseId: string;
  barcodeScanned: string;
  quantityPicked: number;
}

export interface HandsFreeSupervisorWorkerDto {
  userId: string;
  userName: string;
  sessionId: string;
  startedAt: string;
  lastActivityAt?: string;
  warehouseId?: string;
  warehouseName?: string;
  totalTasks: number;
  completedTasks: number;
  currentTaskIndex: number;
  errors: number;
}
