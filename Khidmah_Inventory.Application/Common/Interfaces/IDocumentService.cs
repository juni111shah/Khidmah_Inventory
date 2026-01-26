using Khidmah_Inventory.Domain.Entities;
using Khidmah_Inventory.Application.Features.Reports.Models;

namespace Khidmah_Inventory.Application.Common.Interfaces;

public interface IDocumentService
{
    byte[] GenerateInvoicePdf(SalesOrder salesOrder);
    byte[] GeneratePurchaseOrderPdf(PurchaseOrder purchaseOrder);
    byte[] GenerateDeliveryNotePdf(SalesOrder salesOrder);
    byte[] GenerateBarcodeLabel(string barcode, string productName, string? sku = null);
    byte[] GenerateSalesReportPdf(SalesReportDto report);
    byte[] GenerateInventoryReportPdf(InventoryReportDto report);
    byte[] GeneratePurchaseReportPdf(PurchaseReportDto report);
}

