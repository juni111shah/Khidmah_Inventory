# Hands-Free Warehouse Mode

Optional voice + camera mode for workers. Integrates with existing inventory, warehouse, stock transactions, and order flows. Uses free technologies (Web Speech API, speechSynthesis, ZXing).

## Delivered

### Part 1 – Frontend voice (VoiceService)
- **VoiceService** (`core/services/voice.service.ts`): Web Speech API.
- **Capabilities**: start/stop listening, parse command, confidence.
- **Commands**: start picking, next, confirm, cancel, repeat, scan, quantity N, done.
- **Numeric recognition**: "quantity five", "five", digits.

### Part 2 – Speech output (SpeechService)
- **SpeechService** (`core/services/speech.service.ts`): speechSynthesis.
- Speaks: "Go to shelf X", "Pick N items", "Scan barcode", "Confirmed", errors.

### Part 3 – Camera / barcode
- **@zxing/library**: camera barcode scan in `CameraBarcodeScannerComponent`.
- On scan: validate with backend `GET api/warehouse/handsfree/validate-barcode?code=...`.

### Part 4 – Task flow API
- **GET api/warehouse/handsfree/tasks?warehouseId=&maxTasks=50**: returns session + tasks (product, location, quantity) from stock levels.
- **POST api/warehouse/handsfree/complete**: body `taskId, productId, warehouseId, barcodeScanned, quantityPicked`; validates and creates StockOut transaction.
- **Permissions**: Inventory:StockLevel:List (tasks), Inventory:StockTransaction:Create (complete).

### Part 5 – Voice-to-action mapping
- In **HandsFreePickingComponent**: confirm → complete, repeat → replay instruction, cancel → pause, scan → show camera, quantity N → set qty, done → exit.

### Part 6 – Error prevention
- Before complete: validate barcode (product match) and quantity (stock check). On mismatch: speak error and show message.

### Part 7 – Real time (SignalR)
- **HandsFreeTaskPushed** event on OperationsHub; client subscribes and prepends urgent tasks when in picking. Backend can call `IOperationsBroadcast.BroadcastAsync("HandsFreeTaskPushed", companyId, ...)` to push tasks.

### Part 8 – UI
- **Full-screen** hands-free page: big text, minimal buttons.
- Shows: current item, location, qty, status; microphone indicator; Scan / Next / Done.
- Route: `/inventory/hands-free`. Nav: Inventory → Hands-free picking.

### Part 9 – Offline
- Complete requests queued via **SyncService** when offline (`handsFreeComplete` / `complete`).
- Sync when back online. SyncService processes queue and POSTs to `/api/warehouse/handsfree/complete`. Ensure HttpClient base URL targets your API when using sync.

### Part 10 – Supervisor view
- **Hands-free supervisor** page: placeholder for who is picking, progress, speed, errors.
- Route: `/inventory/hands-free/supervisor`. Backend can add `GET api/warehouse/handsfree/sessions` for live data.

### Part 11 – AI placeholders
- **HandsFreeAiService**: expandIntent, suggestTaskOrder, getFatigueLevel (stubs for voice intent, route optimization, fatigue detection).

### Part 12 – Security
- Hands-free and validate-barcode require **Inventory** permissions (StockLevel:List, StockTransaction:Create as above).

## Usage

1. Ensure roles have **Inventory:StockLevel:List** and **Inventory:StockTransaction:Create**.
2. Open **Inventory → Hands-free picking**.
3. Select warehouse, start session (or say "Start picking" if voice is running).
4. Follow spoken instructions; say "Scan" to open camera, scan barcode to confirm pick (or say "Confirm" if task has barcode/SKU).
5. Say "Next", "Repeat", "Quantity N", "Done", or "Cancel" as needed.

## Backend

- **HandsFreeController**: tasks, complete, validate-barcode.
- **GetHandsFreeTasksQueryHandler**: builds tasks from stock levels (product, warehouse location, qty).
- **CompleteHandsFreeTaskCommandHandler**: validates barcode/product and quantity, then creates StockOut via CreateStockTransactionCommand.

## Pushing urgent tasks (optional)

From any backend service inject `IOperationsBroadcast` and call:

```csharp
await _broadcast.BroadcastAsync(
  "HandsFreeTaskPushed",
  companyId,
  null,
  "HandsFreeTask",
  new { taskId, productId, productName, barcode, sku, location, warehouseId, quantity, sequence },
  cancellationToken);
```

Clients in hands-free picking for that warehouse will receive the task and hear "New urgent task".
