using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Khidmah_Inventory.Application.Common.Interfaces;
using Khidmah_Inventory.Domain.Entities;
using Khidmah_Inventory.Application.Features.Reports.Models;

namespace Khidmah_Inventory.Infrastructure.Services;

public class DocumentService : IDocumentService
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public DocumentService(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
        QuestPDF.Settings.License = LicenseType.Community;
    }

    public byte[] GenerateInvoicePdf(SalesOrder salesOrder)
    {
        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(10));

                page.Header()
                    .Column(column =>
                    {
                        column.Item().Row(row =>
                        {
                            row.RelativeItem().Column(col =>
                            {
                                col.Item().Text("INVOICE").FontSize(24).Bold();
                                col.Item().Text($"Invoice #: {salesOrder.OrderNumber}");
                                col.Item().Text($"Date: {salesOrder.OrderDate:dd MMM yyyy}");
                            });

                            row.RelativeItem().AlignRight().Column(col =>
                            {
                                col.Item().Text("Bill To:").Bold();
                                col.Item().Text(salesOrder.Customer.Name);
                                if (!string.IsNullOrEmpty(salesOrder.Customer.Address))
                                    col.Item().Text(salesOrder.Customer.Address);
                                if (!string.IsNullOrEmpty(salesOrder.Customer.PhoneNumber))
                                    col.Item().Text($"Phone: {salesOrder.Customer.PhoneNumber}");
                                if (!string.IsNullOrEmpty(salesOrder.Customer.Email))
                                    col.Item().Text($"Email: {salesOrder.Customer.Email}");
                            });
                        });

                        column.Item().PaddingTop(10).LineHorizontal(1);
                    });

                page.Content()
                    .PaddingVertical(1, Unit.Centimetre)
                    .Column(column =>
                    {
                        column.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(3);
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(2);
                            });

                            table.Header(header =>
                            {
                                header.Cell().Element(CellStyle).Text("Item").Bold();
                                header.Cell().Element(CellStyle).AlignRight().Text("Quantity").Bold();
                                header.Cell().Element(CellStyle).AlignRight().Text("Unit Price").Bold();
                                header.Cell().Element(CellStyle).AlignRight().Text("Discount").Bold();
                                header.Cell().Element(CellStyle).AlignRight().Text("Total").Bold();

                                static IContainer CellStyle(IContainer container)
                                {
                                    return container
                                        .BorderBottom(1)
                                        .PaddingVertical(5)
                                        .BorderColor(Colors.Grey.Lighten2);
                                }
                            });

                            foreach (var item in salesOrder.Items)
                            {
                                table.Cell().Element(CellStyle).Text($"{item.Product.Name} ({item.Product.SKU})");
                                table.Cell().Element(CellStyle).AlignRight().Text(item.Quantity.ToString("N2"));
                                table.Cell().Element(CellStyle).AlignRight().Text(item.UnitPrice.ToString("C"));
                                table.Cell().Element(CellStyle).AlignRight().Text(item.DiscountAmount.ToString("C"));
                                table.Cell().Element(CellStyle).AlignRight().Text(item.LineTotal.ToString("C"));
                            }

                            static IContainer CellStyle(IContainer container)
                            {
                                return container
                                    .BorderBottom(1)
                                    .BorderColor(Colors.Grey.Lighten2)
                                    .PaddingVertical(5);
                            }
                        });

                        column.Item().AlignRight().PaddingTop(10).Column(col =>
                        {
                            col.Item().Row(row =>
                            {
                                row.ConstantItem(100).Text("Subtotal:");
                                row.ConstantItem(100).AlignRight().Text(salesOrder.SubTotal.ToString("C"));
                            });
                            col.Item().Row(row =>
                            {
                                row.ConstantItem(100).Text("Discount:");
                                row.ConstantItem(100).AlignRight().Text(salesOrder.DiscountAmount.ToString("C"));
                            });
                            col.Item().Row(row =>
                            {
                                row.ConstantItem(100).Text("Tax:");
                                row.ConstantItem(100).AlignRight().Text(salesOrder.TaxAmount.ToString("C"));
                            });
                            col.Item().PaddingTop(5).Row(row =>
                            {
                                row.ConstantItem(100).Text("Total:").Bold().FontSize(12);
                                row.ConstantItem(100).AlignRight().Text(salesOrder.TotalAmount.ToString("C")).Bold().FontSize(12);
                            });
                        });
                    });

                page.Footer()
                    .AlignCenter()
                    .Text(x =>
                    {
                        x.Span("Thank you for your business!");
                        x.AlignCenter();
                    });
            });
        });

        return document.GeneratePdf();
    }

    public byte[] GeneratePurchaseOrderPdf(PurchaseOrder purchaseOrder)
    {
        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(10));

                page.Header()
                    .Column(column =>
                    {
                        column.Item().Row(row =>
                        {
                            row.RelativeItem().Column(col =>
                            {
                                col.Item().Text("PURCHASE ORDER").FontSize(24).Bold();
                                col.Item().Text($"PO #: {purchaseOrder.OrderNumber}");
                                col.Item().Text($"Date: {purchaseOrder.OrderDate:dd MMM yyyy}");
                                col.Item().Text($"Status: {purchaseOrder.Status}");
                            });

                            row.RelativeItem().AlignRight().Column(col =>
                            {
                                col.Item().Text("Supplier:").Bold();
                                col.Item().Text(purchaseOrder.Supplier.Name);
                                if (!string.IsNullOrEmpty(purchaseOrder.Supplier.Address))
                                    col.Item().Text(purchaseOrder.Supplier.Address);
                                if (!string.IsNullOrEmpty(purchaseOrder.Supplier.PhoneNumber))
                                    col.Item().Text($"Phone: {purchaseOrder.Supplier.PhoneNumber}");
                                if (!string.IsNullOrEmpty(purchaseOrder.Supplier.Email))
                                    col.Item().Text($"Email: {purchaseOrder.Supplier.Email}");
                            });
                        });

                        column.Item().PaddingTop(10).LineHorizontal(1);
                    });

                page.Content()
                    .PaddingVertical(1, Unit.Centimetre)
                    .Column(column =>
                    {
                        column.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(3);
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(2);
                            });

                            table.Header(header =>
                            {
                                header.Cell().Element(CellStyle).Text("Item").Bold();
                                header.Cell().Element(CellStyle).AlignRight().Text("Quantity").Bold();
                                header.Cell().Element(CellStyle).AlignRight().Text("Unit Price").Bold();
                                header.Cell().Element(CellStyle).AlignRight().Text("Discount").Bold();
                                header.Cell().Element(CellStyle).AlignRight().Text("Total").Bold();
                            });

                            foreach (var item in purchaseOrder.Items)
                            {
                                table.Cell().Element(CellStyle).Text($"{item.Product.Name} ({item.Product.SKU})");
                                table.Cell().Element(CellStyle).AlignRight().Text(item.Quantity.ToString("N2"));
                                table.Cell().Element(CellStyle).AlignRight().Text(item.UnitPrice.ToString("C"));
                                table.Cell().Element(CellStyle).AlignRight().Text(item.DiscountAmount.ToString("C"));
                                table.Cell().Element(CellStyle).AlignRight().Text(item.LineTotal.ToString("C"));
                            }

                            static IContainer CellStyle(IContainer container)
                            {
                                return container
                                    .BorderBottom(1)
                                    .BorderColor(Colors.Grey.Lighten2)
                                    .PaddingVertical(5);
                            }
                        });

                        column.Item().AlignRight().PaddingTop(10).Column(col =>
                        {
                            col.Item().Row(row =>
                            {
                                row.ConstantItem(100).Text("Subtotal:");
                                row.ConstantItem(100).AlignRight().Text(purchaseOrder.SubTotal.ToString("C"));
                            });
                            col.Item().Row(row =>
                            {
                                row.ConstantItem(100).Text("Discount:");
                                row.ConstantItem(100).AlignRight().Text(purchaseOrder.DiscountAmount.ToString("C"));
                            });
                            col.Item().Row(row =>
                            {
                                row.ConstantItem(100).Text("Tax:");
                                row.ConstantItem(100).AlignRight().Text(purchaseOrder.TaxAmount.ToString("C"));
                            });
                            col.Item().PaddingTop(5).Row(row =>
                            {
                                row.ConstantItem(100).Text("Total:").Bold().FontSize(12);
                                row.ConstantItem(100).AlignRight().Text(purchaseOrder.TotalAmount.ToString("C")).Bold().FontSize(12);
                            });
                        });
                    });
            });
        });

        return document.GeneratePdf();
    }

    public byte[] GenerateDeliveryNotePdf(SalesOrder salesOrder)
    {
        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(10));

                page.Header()
                    .Column(column =>
                    {
                        column.Item().Row(row =>
                        {
                            row.RelativeItem().Column(col =>
                            {
                                col.Item().Text("DELIVERY NOTE").FontSize(24).Bold();
                                col.Item().Text($"DN #: {salesOrder.OrderNumber}");
                                col.Item().Text($"Date: {salesOrder.OrderDate:dd MMM yyyy}");
                            });

                            row.RelativeItem().AlignRight().Column(col =>
                            {
                                col.Item().Text("Deliver To:").Bold();
                                col.Item().Text(salesOrder.Customer.Name);
                                if (!string.IsNullOrEmpty(salesOrder.Customer.Address))
                                    col.Item().Text(salesOrder.Customer.Address);
                            });
                        });

                        column.Item().PaddingTop(10).LineHorizontal(1);
                    });

                page.Content()
                    .PaddingVertical(1, Unit.Centimetre)
                    .Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(3);
                            columns.RelativeColumn(2);
                            columns.RelativeColumn(2);
                        });

                        table.Header(header =>
                        {
                            header.Cell().Element(CellStyle).Text("Item").Bold();
                            header.Cell().Element(CellStyle).AlignRight().Text("Quantity").Bold();
                            header.Cell().Element(CellStyle).AlignRight().Text("Unit").Bold();
                        });

                        foreach (var item in salesOrder.Items)
                        {
                            table.Cell().Element(CellStyle).Text($"{item.Product.Name} ({item.Product.SKU})");
                            table.Cell().Element(CellStyle).AlignRight().Text(item.Quantity.ToString("N2"));
                            table.Cell().Element(CellStyle).AlignRight().Text(item.Product.UnitOfMeasure.Name);
                        }

                        static IContainer CellStyle(IContainer container)
                        {
                            return container
                                .BorderBottom(1)
                                .BorderColor(Colors.Grey.Lighten2)
                                .PaddingVertical(5);
                        }
                    });
            });
        });

        return document.GeneratePdf();
    }

    public byte[] GenerateBarcodeLabel(string barcode, string productName, string? sku = null)
    {
        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(4, 2, Unit.Inch);
                page.Margin(0.2f, Unit.Inch);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(8));

                page.Content()
                    .Column(column =>
                    {
                        column.Item().Text(productName).Bold().FontSize(10);
                        if (!string.IsNullOrEmpty(sku))
                            column.Item().Text($"SKU: {sku}").FontSize(8);
                        column.Item().PaddingTop(5).Text(barcode).FontSize(12).FontFamily("Courier");
                        column.Item().Text(barcode).AlignCenter().FontSize(8);
                    });
            });
        });

        return document.GeneratePdf();
    }

    public byte[] GenerateSalesReportPdf(SalesReportDto report)
    {
        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(10));

                page.Header()
                    .Column(column =>
                    {
                        column.Item().Row(row =>
                        {
                            row.RelativeItem().Column(col =>
                            {
                                col.Item().Text("SALES REPORT").FontSize(24).Bold();
                                col.Item().Text($"Period: {report.FromDate:dd MMM yyyy} - {report.ToDate:dd MMM yyyy}");
                                col.Item().Text($"Generated on: {DateTime.UtcNow:dd MMM yyyy HH:mm}");
                            });
                        });

                        column.Item().PaddingTop(10).LineHorizontal(1);
                    });

                page.Content()
                    .PaddingVertical(1, Unit.Centimetre)
                    .Column(column =>
                    {
                        // Summary section
                        column.Item().Column(summaryCol =>
                        {
                            summaryCol.Item().Text("Summary").FontSize(14).Bold();
                            summaryCol.Item().PaddingTop(5).Row(row =>
                            {
                                row.ConstantItem(150).Text("Total Sales:");
                                row.ConstantItem(150).AlignRight().Text(report.TotalSales.ToString("C")).Bold();
                            });
                            summaryCol.Item().Row(row =>
                            {
                                row.ConstantItem(150).Text("Total Cost:");
                                row.ConstantItem(150).AlignRight().Text(report.TotalCost.ToString("C"));
                            });
                            summaryCol.Item().Row(row =>
                            {
                                row.ConstantItem(150).Text("Total Profit:");
                                row.ConstantItem(150).AlignRight().Text(report.TotalProfit.ToString("C"));
                            });
                            summaryCol.Item().Row(row =>
                            {
                                row.ConstantItem(150).Text("Profit Margin:");
                                row.ConstantItem(150).AlignRight().Text($"{report.ProfitMargin:F2}%");
                            });
                            summaryCol.Item().Row(row =>
                            {
                                row.ConstantItem(150).Text("Total Orders:");
                                row.ConstantItem(150).AlignRight().Text(report.TotalOrders.ToString());
                            });
                        });

                        column.Item().PaddingTop(20).Text("Sales Details").FontSize(14).Bold();

                        // Table with sales details
                        column.Item().PaddingTop(10).Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(2); // Date
                                columns.RelativeColumn(2); // Order Number
                                columns.RelativeColumn(3); // Customer
                                columns.RelativeColumn(2); // Amount
                                columns.RelativeColumn(2); // Cost
                                columns.RelativeColumn(2); // Profit
                            });

                            table.Header(header =>
                            {
                                header.Cell().Element(CellStyle).Text("Date").Bold();
                                header.Cell().Element(CellStyle).Text("Order #").Bold();
                                header.Cell().Element(CellStyle).Text("Customer").Bold();
                                header.Cell().Element(CellStyle).AlignRight().Text("Amount").Bold();
                                header.Cell().Element(CellStyle).AlignRight().Text("Cost").Bold();
                                header.Cell().Element(CellStyle).AlignRight().Text("Profit").Bold();

                                static IContainer CellStyle(IContainer container)
                                {
                                    return container
                                        .BorderBottom(1)
                                        .PaddingVertical(5)
                                        .BorderColor(Colors.Grey.Lighten2);
                                }
                            });

                            foreach (var item in report.Items)
                            {
                                table.Cell().Element(CellStyle).Text(item.Date.ToString("dd/MM/yyyy"));
                                table.Cell().Element(CellStyle).Text(item.OrderNumber);
                                table.Cell().Element(CellStyle).Text(item.CustomerName);
                                table.Cell().Element(CellStyle).AlignRight().Text(item.Amount.ToString("C"));
                                table.Cell().Element(CellStyle).AlignRight().Text(item.Cost.ToString("C"));
                                table.Cell().Element(CellStyle).AlignRight().Text(item.Profit.ToString("C"));
                            }

                            static IContainer CellStyle(IContainer container)
                            {
                                return container
                                    .BorderBottom(1)
                                    .BorderColor(Colors.Grey.Lighten2)
                                    .PaddingVertical(5);
                            }
                        });
                    });

                page.Footer()
                    .AlignCenter()
                    .Text(x =>
                    {
                        x.Span("Generated by Khidmah Inventory System");
                        x.AlignCenter();
                    });
            });
        });

        return document.GeneratePdf();
    }

    public byte[] GenerateInventoryReportPdf(InventoryReportDto report)
    {
        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(10));

                page.Header()
                    .Column(column =>
                    {
                        column.Item().Row(row =>
                        {
                            row.RelativeItem().Column(col =>
                            {
                                col.Item().Text("INVENTORY REPORT").FontSize(24).Bold();
                                col.Item().Text($"Generated on: {DateTime.UtcNow:dd MMM yyyy HH:mm}");
                            });
                        });

                        column.Item().PaddingTop(10).LineHorizontal(1);
                    });

                page.Content()
                    .PaddingVertical(1, Unit.Centimetre)
                    .Column(column =>
                    {
                        // Summary section
                        column.Item().Column(summaryCol =>
                        {
                            summaryCol.Item().Text("Summary").FontSize(14).Bold();
                            summaryCol.Item().PaddingTop(5).Row(row =>
                            {
                                row.ConstantItem(150).Text("Total Products:");
                                row.ConstantItem(150).AlignRight().Text(report.TotalProducts.ToString());
                            });
                            summaryCol.Item().Row(row =>
                            {
                                row.ConstantItem(150).Text("Total Stock Value:");
                                row.ConstantItem(150).AlignRight().Text(report.TotalStockValue.ToString("C"));
                            });
                            summaryCol.Item().Row(row =>
                            {
                                row.ConstantItem(150).Text("Low Stock Items:");
                                row.ConstantItem(150).AlignRight().Text(report.LowStockItems.ToString());
                            });
                            summaryCol.Item().Row(row =>
                            {
                                row.ConstantItem(150).Text("Out of Stock:");
                                row.ConstantItem(150).AlignRight().Text(report.OutOfStockItems.ToString());
                            });
                        });

                        column.Item().PaddingTop(20).Text("Inventory Details").FontSize(14).Bold();

                        // Table with inventory details
                        column.Item().PaddingTop(10).Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(2); // SKU
                                columns.RelativeColumn(3); // Product Name
                                columns.RelativeColumn(2); // Warehouse
                                columns.RelativeColumn(2); // Quantity
                                columns.RelativeColumn(2); // Average Cost
                                columns.RelativeColumn(2); // Stock Value
                            });

                            table.Header(header =>
                            {
                                header.Cell().Element(CellStyle).Text("SKU").Bold();
                                header.Cell().Element(CellStyle).Text("Product").Bold();
                                header.Cell().Element(CellStyle).Text("Warehouse").Bold();
                                header.Cell().Element(CellStyle).AlignRight().Text("Quantity").Bold();
                                header.Cell().Element(CellStyle).AlignRight().Text("Avg Cost").Bold();
                                header.Cell().Element(CellStyle).AlignRight().Text("Stock Value").Bold();

                                static IContainer CellStyle(IContainer container)
                                {
                                    return container
                                        .BorderBottom(1)
                                        .PaddingVertical(5)
                                        .BorderColor(Colors.Grey.Lighten2);
                                }
                            });

                            foreach (var item in report.Items)
                            {
                                table.Cell().Element(CellStyle).Text(item.ProductSKU);
                                table.Cell().Element(CellStyle).Text(item.ProductName);
                                table.Cell().Element(CellStyle).Text(item.WarehouseName);
                                table.Cell().Element(CellStyle).AlignRight().Text(item.Quantity.ToString("N2"));
                                table.Cell().Element(CellStyle).AlignRight().Text(item.AverageCost.ToString("C"));
                                table.Cell().Element(CellStyle).AlignRight().Text(item.StockValue.ToString("C"));
                            }

                            static IContainer CellStyle(IContainer container)
                            {
                                return container
                                    .BorderBottom(1)
                                    .BorderColor(Colors.Grey.Lighten2)
                                    .PaddingVertical(5);
                            }
                        });
                    });

                page.Footer()
                    .AlignCenter()
                    .Text(x =>
                    {
                        x.Span("Generated by Khidmah Inventory System");
                        x.AlignCenter();
                    });
            });
        });

        return document.GeneratePdf();
    }

    public byte[] GeneratePurchaseReportPdf(PurchaseReportDto report)
    {
        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(10));

                page.Header()
                    .Column(column =>
                    {
                        column.Item().Row(row =>
                        {
                            row.RelativeItem().Column(col =>
                            {
                                col.Item().Text("PURCHASE REPORT").FontSize(24).Bold();
                                col.Item().Text($"Period: {report.FromDate:dd MMM yyyy} - {report.ToDate:dd MMM yyyy}");
                                col.Item().Text($"Generated on: {DateTime.UtcNow:dd MMM yyyy HH:mm}");
                            });
                        });

                        column.Item().PaddingTop(10).LineHorizontal(1);
                    });

                page.Content()
                    .PaddingVertical(1, Unit.Centimetre)
                    .Column(column =>
                    {
                        // Summary section
                        column.Item().Column(summaryCol =>
                        {
                            summaryCol.Item().Text("Summary").FontSize(14).Bold();
                            summaryCol.Item().PaddingTop(5).Row(row =>
                            {
                                row.ConstantItem(150).Text("Total Purchases:");
                                row.ConstantItem(150).AlignRight().Text(report.TotalPurchases.ToString("C")).Bold();
                            });
                            summaryCol.Item().Row(row =>
                            {
                                row.ConstantItem(150).Text("Total Orders:");
                                row.ConstantItem(150).AlignRight().Text(report.TotalOrders.ToString());
                            });
                        });

                        column.Item().PaddingTop(20).Text("Purchase Details").FontSize(14).Bold();

                        // Table with purchase details
                        column.Item().PaddingTop(10).Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(2); // Date
                                columns.RelativeColumn(2); // Order Number
                                columns.RelativeColumn(3); // Supplier
                                columns.RelativeColumn(2); // Amount
                            });

                            table.Header(header =>
                            {
                                header.Cell().Element(CellStyle).Text("Date").Bold();
                                header.Cell().Element(CellStyle).Text("Order #").Bold();
                                header.Cell().Element(CellStyle).Text("Supplier").Bold();
                                header.Cell().Element(CellStyle).AlignRight().Text("Amount").Bold();

                                static IContainer CellStyle(IContainer container)
                                {
                                    return container
                                        .BorderBottom(1)
                                        .PaddingVertical(5)
                                        .BorderColor(Colors.Grey.Lighten2);
                                }
                            });

                            foreach (var item in report.Items)
                            {
                                table.Cell().Element(CellStyle).Text(item.Date.ToString("dd/MM/yyyy"));
                                table.Cell().Element(CellStyle).Text(item.OrderNumber);
                                table.Cell().Element(CellStyle).Text(item.SupplierName);
                                table.Cell().Element(CellStyle).AlignRight().Text(item.Amount.ToString("C"));
                            }

                            static IContainer CellStyle(IContainer container)
                            {
                                return container
                                    .BorderBottom(1)
                                    .BorderColor(Colors.Grey.Lighten2)
                                    .PaddingVertical(5);
                            }
                        });
                    });

                page.Footer()
                    .AlignCenter()
                    .Text(x =>
                    {
                        x.Span("Generated by Khidmah Inventory System");
                        x.AlignCenter();
                    });
            });
        });

        return document.GeneratePdf();
    }
}

