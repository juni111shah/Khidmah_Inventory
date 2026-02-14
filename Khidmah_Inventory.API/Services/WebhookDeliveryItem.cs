namespace Khidmah_Inventory.API.Services;

public record WebhookDeliveryItem(Guid WebhookId, Guid CompanyId, string EventName, string PayloadJson);
