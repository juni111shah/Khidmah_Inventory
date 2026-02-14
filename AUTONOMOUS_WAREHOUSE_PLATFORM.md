# Autonomous Warehouse Platform

Khidmah evolves into an **Autonomous Warehouse platform**: plan, assign, and optimize operations with minimal human input. Supports human workers, future robots, and AI-driven decisions. Built with **Clean Architecture**, **CQRS**, **multi-tenant**, and **Result&lt;ApiResponse&gt;** patterns.

---

## Delivered

### Part 1 – Digital Warehouse Map
- **Entities**: `WarehouseMap`, `MapZone`, `MapAisle`, `MapRack`, `MapBin` (each bin has **x, y** for routing).
- **API**: `api/warehouse-map` CRUD (maps, zones, aisles, racks, bins).
- **Tree**: GET `api/warehouse-map/{id}` returns full map tree (zones → aisles → racks → bins with coordinates).

### Part 2 – Task Orchestration Engine
- **WorkTask** entity: Type (Pick, Putaway, Count, Transfer), Priority, Status, AssignedToId/AssignedToType (Human/Robot), Location (MapBinId, LocationCode), Product, Quantity, SourceOrderId.
- **ITaskPlanner** (Application) + **TaskPlannerService** (Infrastructure):
  - **PlanFromOrdersAsync**: breaks sales/purchase orders into tasks.
  - **PrioritizeAsync**: orders by priority and created date.
  - **AssignToAgentsAsync**: assigns pending tasks to nearest free workers (by current workload).

### Part 3 – Route Optimization
- **IRouteOptimizer** + **RouteOptimizerService**: nearest-neighbor / shortest path.
- **Input**: current position (x,y or StartMapBinId), list of task ids.
- **Output**: optimal sequence of task ids + estimated total distance.
- **API**: POST `api/warehouse/routes?warehouseId=...` with body `RouteOptimizerRequest`.

### Part 4 – Autonomous Assignment (foundation)
- Assignment logic in **TaskPlannerService.AssignToAgentsAsync** (assigns to users by workload).
- SignalR can be used to push assignments; existing **OperationsHub** and company groups are in place for real-time events.

### Part 5 – Worker / Robot Abstraction
- **IOperationAgent** (Application): `AgentId`, `Type` (Human/Robot), `DisplayName`, `IsAvailable`, `CurrentWarehouseId`, `CurrentPosition (x,y)`.
- Human workers map to users; robots can be implemented later via the same interface and API.

### Part 6 – Live Operations Board (backend ready)
- Work tasks and assignment data available via GET `api/warehouse/tasks`.
- Frontend placeholder at **/autonomous/live-ops**. Real-time board can consume SignalR for task/position updates when events are broadcast.

### Part 7 – AI Copilot (Natural Language Ops)
- **IIntentParserService** (rule-based) + **IntentParserService** (Infrastructure): parses intents such as:
  - "Create purchase order for 50 laptops from Ali Traders"
  - "How much profit today?"
  - "Which items will run out this week?"
  - "Move stock from A to B"
  - "Show slow products"
- **API**: POST `api/copilot/execute` with `{ "input": "...", "confirmed": false }`. Returns action, confirmation message, or result. No external paid AI.

### Part 8 – Safe Execution Layer
- **ExecuteCopilotCommand**: if intent `RequiresConfirmation` and `confirmed` is false, returns `ConfirmationMessage` without executing.
- **AI decision logging**: can be added by persisting intent + user + timestamp in a dedicated table or activity log when executing confirmed actions.

### Part 9 – Predictive Task Creation (extensible)
- **ITaskPlanner** and **WorkTask** support creation from orders. Auto-creation of replenishment/count tasks from sales velocity, threshold, and pending deliveries can be implemented in a background job or new command that calls `PlanFromOrdersAsync` or creates tasks directly.

### Part 10 – Performance Scoring (extensible)
- **WorkTask** has `AssignedAt`, `StartedAt`, `CompletedAt`; can derive pick speed, task time. Accuracy/distance can be added via extra fields or separate metrics table.

### Part 11 – Robot-Ready Endpoints
- **IOperationAgent** and **WorkTask.AssignedToId/AssignedToType** support robots.
- **API**: POST `api/warehouse/tasks/{taskId}/complete` (used by any agent). Dedicated **robot position** and **robot task completion** endpoints can be added (e.g. POST `api/warehouse/robot/position`, POST `api/warehouse/robot/complete`) that accept robot id and update position/task status.

### Part 12 – Frontend
- **Routes**: `/autonomous`, `/autonomous/routes`, `/autonomous/live-ops`, `/copilot`.
- **Components**: Autonomous dashboard, Copilot (input + execute), Routes placeholder, Live Ops placeholder.
- **Nav**: “Autonomous Warehouse” (Dashboard, Routes, Live Ops) and “AI Copilot” in sidebar.

### Part 13 – Security
- **Copilot**: `api/copilot/execute` uses **Dashboard:Read** (CopilotPermissions). Intent execution can check specific permissions (e.g. PurchaseOrders:Create for create PO) before performing the action.
- **Warehouse map & autonomous**: Use **Warehouses** permissions (List, Read, Create, Update, Delete). Task complete uses **Inventory:StockTransaction:Create**.

---

## API Summary

| Area | Method | Endpoint | Description |
|------|--------|----------|-------------|
| Warehouse map | GET | api/warehouse-map | List maps (optional warehouseId, isActive) |
| Warehouse map | GET | api/warehouse-map/{id} | Map tree (zones → aisles → racks → bins with x,y) |
| Warehouse map | POST/PUT/DELETE | api/warehouse-map, .../zones, .../aisles, .../racks, .../bins | CRUD |
| Autonomous | GET | api/warehouse/tasks | List work tasks (warehouseId, assignedToId, status, type) |
| Autonomous | POST | api/warehouse/plan | Plan tasks from orders (warehouseId + body: salesOrderIds, purchaseOrderIds) |
| Autonomous | POST | api/warehouse/assign | Assign tasks to agents (warehouseId + body: taskIds) |
| Autonomous | POST | api/warehouse/routes | Optimized route (warehouseId + body: currentX/Y or startMapBinId, taskIds) |
| Autonomous | POST | api/warehouse/tasks/{taskId}/complete | Complete task (body: notes) |
| Copilot | POST | api/copilot/execute | Execute natural language (body: input, confirmed) |

---

## Suggested Next Steps
1. **SignalR**: Broadcast task assigned / task completed / agent position from backend so Live Ops and clients update in real time.
2. **Robot API**: Add POST `api/warehouse/robot/position` and POST `api/warehouse/robot/complete` and link to **WorkTask** and **IOperationAgent**.
3. **Copilot execution**: Wire each intent (CreatePurchaseOrder, GetProfitToday, etc.) to real commands/queries and enforce permissions per action.
4. **AI decision log**: Persist intent, user, company, timestamp, and outcome for audit.
5. **Predictive tasks**: Background job that creates replenishment/count tasks from thresholds and sales velocity.
6. **Performance metrics**: Store or compute pick speed, accuracy, distance, task time from **WorkTask** and optional agent position events.
