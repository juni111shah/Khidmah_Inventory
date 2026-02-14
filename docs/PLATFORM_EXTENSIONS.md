# Platform extension guide

The platform layer is designed so new integration types, report types, and webhook events can be added without changing core code.

## Adding a new integration type

1. **Backend**: In `GetIntegrationsListQueryHandler` (Application/Features/Platform/Integrations), add a new entry to the `AllTypes` list:
   - `Type`: unique key (e.g. `"Slack"`)
   - `DisplayName`, `Description`: for the UI
2. **Frontend**: No change required; the Integration Center loads types from the API and renders cards with enable/disable.

## Adding a new webhook event

1. **Domain**: Use a consistent event name (e.g. `StockLow`, `ApprovalDone`). Document it in the webhook UI and API.
2. **Dispatch**: From the relevant MediatR handler (e.g. when stock falls below threshold), inject `IWebhookDispatchService` and call:
   ```csharp
   await _webhookDispatch.DispatchAsync(companyId, "StockLow", new { productId, currentLevel, threshold }, cancellationToken);
   ```
3. **Frontend**: Add the new event name to `WEBHOOK_EVENTS` in `platform.model.ts` and to the webhook form placeholder so users can subscribe.

## Adding a new scheduled report type

1. **Backend**: In the scheduled report runner (e.g. background service that evaluates `NextRunAt`), add a branch for the new `ReportType` that runs the corresponding report logic and sends the result (e.g. by email).
2. **Frontend**: Add the type to `REPORT_TYPES` in `platform.model.ts` and to the scheduled report form dropdown.

## Adding a new API permission for API keys

1. **Backend**: Document the permission string (e.g. `Reports:Read`). API key permissions are stored as a comma-separated list; the API key middleware adds each as a `Permission` claim so existing permission checks apply.
2. **Frontend**: When creating/editing API keys, you can add checkboxes or a multi-select for the new permission.

## Public API (API key) access

External systems authenticate with the `X-Api-Key` (or `Api-Key`) header. The same controllers and routes used by the Angular app are used; permission checks are satisfied by the permissions attached to the API key. To expose a new resource:

- Add a controller/action as usual, protected by `[Authorize]` and the appropriate permission (e.g. `Products:Read`).
- Ensure the permission is one that can be assigned to an API key (comma-separated in the key’s `Permissions` field).
- No separate “public API” surface is required; the API key middleware sets the user/company context so existing multi-tenant and RBAC logic apply.

## Dependencies

- **Webhook engine**: `IWebhookDispatchService` (Application), implemented in API with an in-memory channel and `WebhookDeliveryBackgroundService`.
- **API key auth**: `ApiKeyAuthenticationMiddleware` runs after JWT; when `X-Api-Key` is present and valid, it sets `ClaimsPrincipal` with `CompanyId` and permission claims.
- **Usage logging**: `ApiKeyUsageMiddleware` logs each request (when authenticated by API key) to `ApiKey` aggregates and `ApiKeyUsageLog` for the dashboard.
