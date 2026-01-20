using Khidmah_Inventory.Domain.Entities;

namespace Khidmah_Inventory.Application.Common.Interfaces;

public interface IDocumentService
{
    byte[] GenerateInvoicePdf(SalesOrder salesOrder);
    byte[] GeneratePurchaseOrderPdf(PurchaseOrder purchaseOrder);
    byte[] GenerateDeliveryNotePdf(SalesOrder salesOrder);
    byte[] GenerateBarcodeLabel(string barcode, string productName, string? sku = null);
}

