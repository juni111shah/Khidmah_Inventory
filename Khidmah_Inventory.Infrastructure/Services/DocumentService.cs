using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Khidmah_Inventory.Application.Common.Interfaces;
using Khidmah_Inventory.Domain.Entities;

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
}

