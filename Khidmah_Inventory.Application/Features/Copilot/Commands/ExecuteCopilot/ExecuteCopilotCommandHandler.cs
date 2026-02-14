using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Text.Json;
using Khidmah_Inventory.Application.Common.Interfaces;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Common.Services;
using Khidmah_Inventory.Application.Features.Copilot.Models;
using Khidmah_Inventory.Application.Features.Products.Commands.CreateProduct;
using Khidmah_Inventory.Application.Features.PurchaseOrders.Commands.CreatePurchaseOrder;
using Khidmah_Inventory.Application.Features.SalesOrders.Commands.CreateSalesOrder;
using Khidmah_Inventory.Application.Features.Inventory.Commands.CreateStockTransaction;
using Khidmah_Inventory.Application.Features.Products.Commands.UpdateProduct;
using Khidmah_Inventory.Application.Features.Products.Commands.DeleteProduct;
using Khidmah_Inventory.Application.Features.Customers.Commands.CreateCustomer;
using Khidmah_Inventory.Application.Features.Customers.Commands.UpdateCustomer;
using Khidmah_Inventory.Application.Features.Suppliers.Commands.CreateSupplier;
using Khidmah_Inventory.Application.Features.Suppliers.Commands.UpdateSupplier;
using Khidmah_Inventory.Application.Features.Suppliers.Commands.DeleteSupplier;
using Khidmah_Inventory.Application.Features.Warehouses.Commands.CreateWarehouse;
using Khidmah_Inventory.Application.Features.Warehouses.Commands.UpdateWarehouse;
using Khidmah_Inventory.Application.Features.Warehouses.Commands.DeleteWarehouse;
using Khidmah_Inventory.Application.Features.Documents.Commands.GenerateInvoicePdf;
using Khidmah_Inventory.Application.Features.Documents.Commands.GeneratePurchaseOrderPdf;
using Khidmah_Inventory.Application.Features.Reports.Queries.GetSalesReport;
using Khidmah_Inventory.Application.Features.Reports.Queries.GetPurchaseReport;
using Khidmah_Inventory.Application.Features.Reports.Queries.GetInventoryReport;
using Khidmah_Inventory.Application.Features.Finance.Statements.Queries.GetProfitLoss;
using Khidmah_Inventory.Application.Features.Finance.Statements.Queries.GetBalanceSheet;
using Khidmah_Inventory.Domain.Entities;

namespace Khidmah_Inventory.Application.Features.Copilot.Commands.ExecuteCopilot;

public class ExecuteCopilotCommandHandler : IRequestHandler<ExecuteCopilotCommand, Result<CopilotExecuteResult>>
{
    private readonly IIntentParserService _intentParser;
    private readonly ICurrentUserService _currentUser;
    private readonly IApplicationDbContext _context;
    private readonly IMediator _mediator;

    public ExecuteCopilotCommandHandler(
        IIntentParserService intentParser,
        ICurrentUserService currentUser,
        IApplicationDbContext context,
        IMediator mediator)
    {
        _intentParser = intentParser;
        _currentUser = currentUser;
        _context = context;
        _mediator = mediator;
    }

    public async Task<Result<CopilotExecuteResult>> Handle(ExecuteCopilotCommand request, CancellationToken cancellationToken)
    {
        var companyId = _currentUser.CompanyId;
        if (!companyId.HasValue)
            return Result<CopilotExecuteResult>.Failure("Company context is required.");

        var state = request.SessionState ?? new CopilotConversationState();
        var input = (request.Input ?? string.Empty).Trim();
        var inputLower = input.ToLowerInvariant();

        if (inputLower == "cancel")
        {
            return Result<CopilotExecuteResult>.Success(new CopilotExecuteResult
            {
                Success = true,
                Cancelled = true,
                Reply = "Task cancelled.",
                SessionState = new CopilotConversationState { SessionId = state.SessionId }
            });
        }

        if (inputLower == "repeat")
        {
            var repeated = state.LastQuestion ?? state.LastAssistantMessage ?? "Please tell me what you want to do.";
            return Result<CopilotExecuteResult>.Success(new CopilotExecuteResult
            {
                Success = true,
                Reply = repeated,
                NextQuestion = state.LastQuestion,
                SessionState = state
            });
        }

        if (ContainsAny(inputLower, "download", "open file", "open link"))
        {
            var wantsPdf = ContainsAny(inputLower, "pdf");
            var wantsCsv = ContainsAny(inputLower, "csv");

            if (wantsPdf && TryGetField(state, "altDownloadMethod", out var altMethodPdf) && string.Equals(altMethodPdf, "CLIENT_PDF", StringComparison.OrdinalIgnoreCase))
            {
                var body = state.Fields.GetValueOrDefault("altDownloadBody");
                if (!string.IsNullOrWhiteSpace(body))
                {
                    var docName = state.Fields.GetValueOrDefault("altDocumentName") ?? "export.pdf";
                    return Result<CopilotExecuteResult>.Success(new CopilotExecuteResult
                    {
                        Success = true,
                        Reply = $"Download {docName}.",
                        Result = new
                        {
                            downloadAction = new
                            {
                                method = "CLIENT_PDF",
                                body,
                                fileName = docName
                            }
                        },
                        SessionState = state
                    });
                }
            }

            if (wantsCsv && TryGetField(state, "altDownloadMethod", out var altMethodCsv) && string.Equals(altMethodCsv, "CLIENT_CSV", StringComparison.OrdinalIgnoreCase))
            {
                var body = state.Fields.GetValueOrDefault("altDownloadBody");
                if (!string.IsNullOrWhiteSpace(body))
                {
                    var docName = state.Fields.GetValueOrDefault("altDocumentName") ?? "export.csv";
                    return Result<CopilotExecuteResult>.Success(new CopilotExecuteResult
                    {
                        Success = true,
                        Reply = $"Download {docName}.",
                        Result = new
                        {
                            downloadAction = new
                            {
                                method = "CLIENT_CSV",
                                body,
                                fileName = docName
                            }
                        },
                        SessionState = state
                    });
                }
            }

            if (TryGetField(state, "lastDownloadUrl", out var lastDownloadUrl))
            {
                var docName = state.Fields.GetValueOrDefault("lastDocumentName") ?? "document";
                return Result<CopilotExecuteResult>.Success(new CopilotExecuteResult
                {
                    Success = true,
                    Reply = $"Download {docName}: {lastDownloadUrl}",
                    Result = new { downloadUrl = lastDownloadUrl, documentName = docName },
                    SessionState = state
                });
            }
            if (TryGetField(state, "lastDownloadPath", out var lastDownloadPath))
            {
                var docName = state.Fields.GetValueOrDefault("lastDocumentName") ?? "document";
                var method = state.Fields.GetValueOrDefault("lastDownloadMethod") ?? "GET";
                var body = state.Fields.GetValueOrDefault("lastDownloadBody");
                return Result<CopilotExecuteResult>.Success(new CopilotExecuteResult
                {
                    Success = true,
                    Reply = $"Download {docName}: {method} {lastDownloadPath}",
                    Result = new
                    {
                        downloadAction = new
                        {
                            method,
                            url = lastDownloadPath,
                            body = string.IsNullOrWhiteSpace(body) ? null : body,
                            fileName = docName
                        }
                    },
                    SessionState = state
                });
            }
            var lastMethod = state.Fields.GetValueOrDefault("lastDownloadMethod");
            if (string.Equals(lastMethod, "CLIENT_CSV", StringComparison.OrdinalIgnoreCase))
            {
                var body = state.Fields.GetValueOrDefault("lastDownloadBody");
                if (!string.IsNullOrWhiteSpace(body))
                {
                    var docName = state.Fields.GetValueOrDefault("lastDocumentName") ?? "export.csv";
                    return Result<CopilotExecuteResult>.Success(new CopilotExecuteResult
                    {
                        Success = true,
                        Reply = $"Download {docName}.",
                        Result = new
                        {
                            downloadAction = new
                            {
                                method = "CLIENT_CSV",
                                body,
                                fileName = docName
                            }
                        },
                        SessionState = state
                    });
                }
            }
            if (string.Equals(lastMethod, "CLIENT_PDF", StringComparison.OrdinalIgnoreCase))
            {
                var body = state.Fields.GetValueOrDefault("lastDownloadBody");
                if (!string.IsNullOrWhiteSpace(body))
                {
                    var docName = state.Fields.GetValueOrDefault("lastDocumentName") ?? "export.pdf";
                    return Result<CopilotExecuteResult>.Success(new CopilotExecuteResult
                    {
                        Success = true,
                        Reply = $"Download {docName}.",
                        Result = new
                        {
                            downloadAction = new
                            {
                                method = "CLIENT_PDF",
                                body,
                                fileName = docName
                            }
                        },
                        SessionState = state
                    });
                }
            }
        }

        var hadPendingCorrection = HasPendingCorrection(state);
        var pendingResult = HandlePendingCorrection(state, inputLower);
        if (pendingResult != null)
            return pendingResult;
        if (hadPendingCorrection && IsAffirmativeInput(inputLower))
        {
            // Treat correction confirmation as system input; proceed with current task.
            input = string.Empty;
            inputLower = string.Empty;
        }

        var hadPendingIntent = HasPendingIntentSuggestion(state);
        var pendingIntentResult = HandlePendingIntentSuggestion(state, inputLower);
        if (pendingIntentResult != null)
            return pendingIntentResult;
        if (hadPendingIntent && IsAffirmativeInput(inputLower))
        {
            // Intent confirmation accepted; continue with selected task.
            input = string.Empty;
            inputLower = string.Empty;
        }

        var parsedIntent = await _intentParser.ParseAsync(input, cancellationToken);
        if (string.Equals(parsedIntent.Action, "Greeting", StringComparison.OrdinalIgnoreCase))
        {
            return Result<CopilotExecuteResult>.Success(new CopilotExecuteResult
            {
                Success = true,
                Reply = "Hello! I can help step-by-step. You can ask in simple words, like 'make sales order', 'check stock', 'create customer', or 'sales report'.",
                SessionState = state
            });
        }

        if (string.Equals(parsedIntent.Action, "Thanks", StringComparison.OrdinalIgnoreCase))
        {
            return Result<CopilotExecuteResult>.Success(new CopilotExecuteResult
            {
                Success = true,
                Reply = "You're welcome. Tell me the next task when you're ready.",
                SessionState = state
            });
        }

        if (string.Equals(parsedIntent.Action, "Help", StringComparison.OrdinalIgnoreCase))
            return HelpMenu(state);

        if (string.Equals(parsedIntent.Action, "ReportHelp", StringComparison.OrdinalIgnoreCase))
            return Ask(state, "I can generate Sales, Purchase, Inventory, Profit & Loss, and Balance Sheet reports. Tell me the report type, then I will ask From Date and To Date step-by-step.");

        if (string.Equals(parsedIntent.Action, "ReportMenu", StringComparison.OrdinalIgnoreCase))
            return Ask(state, "Which report do you want: Sales Report, Purchase Report, Inventory Report, Profit & Loss Report, or Balance Sheet?");

        if (string.Equals(parsedIntent.Action, "CustomerList", StringComparison.OrdinalIgnoreCase))
            return await ReplyWithEntityList(state, "Customers", _context.Customers.AsNoTracking().Where(x => x.CompanyId == companyId.Value && !x.IsDeleted).Select(x => x.Name), cancellationToken);

        if (string.Equals(parsedIntent.Action, "SupplierList", StringComparison.OrdinalIgnoreCase))
            return await ReplyWithEntityList(state, "Suppliers", _context.Suppliers.AsNoTracking().Where(x => x.CompanyId == companyId.Value && !x.IsDeleted).Select(x => x.Name), cancellationToken);

        if (string.Equals(parsedIntent.Action, "ProductList", StringComparison.OrdinalIgnoreCase))
            return await ReplyWithEntityList(state, "Products", _context.Products.AsNoTracking().Where(x => x.CompanyId == companyId.Value && !x.IsDeleted).Select(x => x.Name), cancellationToken);

        if (string.Equals(parsedIntent.Action, "WarehouseList", StringComparison.OrdinalIgnoreCase))
            return await ReplyWithEntityList(state, "Warehouses", _context.Warehouses.AsNoTracking().Where(x => x.CompanyId == companyId.Value && !x.IsDeleted).Select(x => x.Name), cancellationToken);

        if (string.Equals(parsedIntent.Action, "ExportProducts", StringComparison.OrdinalIgnoreCase))
            return await ExecuteExportProducts(companyId.Value, input, state, cancellationToken);

        if (string.Equals(parsedIntent.Action, "ExportCustomers", StringComparison.OrdinalIgnoreCase))
            return await ExecuteExportCustomers(companyId.Value, input, state, cancellationToken);

        if (string.Equals(parsedIntent.Action, "ProductCrudHelp", StringComparison.OrdinalIgnoreCase))
        {
            return Result<CopilotExecuteResult>.Success(new CopilotExecuteResult
            {
                Success = true,
                Reply = "For products, say one of: Create Product, Update Product, Delete Product, or List Products.",
                SessionState = state
            });
        }

        if (string.Equals(parsedIntent.Action, "CustomerCrudHelp", StringComparison.OrdinalIgnoreCase))
        {
            return Result<CopilotExecuteResult>.Success(new CopilotExecuteResult
            {
                Success = true,
                Reply = "For customers, say one of: Create Customer, Update Customer, Delete Customer, or List Customers.",
                SessionState = state
            });
        }

        if (string.Equals(parsedIntent.Action, "SupplierCrudHelp", StringComparison.OrdinalIgnoreCase))
        {
            return Result<CopilotExecuteResult>.Success(new CopilotExecuteResult
            {
                Success = true,
                Reply = "For suppliers, say one of: Create Supplier, Update Supplier, Delete Supplier, or List Suppliers.",
                SessionState = state
            });
        }

        if (string.Equals(parsedIntent.Action, "WarehouseCrudHelp", StringComparison.OrdinalIgnoreCase))
        {
            return Result<CopilotExecuteResult>.Success(new CopilotExecuteResult
            {
                Success = true,
                Reply = "For warehouses, say one of: Create Warehouse, Update Warehouse, Delete Warehouse, or List Warehouses.",
                SessionState = state
            });
        }

        if (string.Equals(parsedIntent.Action, "GetProfitToday", StringComparison.OrdinalIgnoreCase))
            return await ExecuteProfitToday(companyId.Value, parsedIntent, cancellationToken);

        if (string.Equals(parsedIntent.Action, "GetItemsRunningOut", StringComparison.OrdinalIgnoreCase))
            return await ExecuteItemsRunningOut(companyId.Value, parsedIntent, cancellationToken);

        if (string.Equals(parsedIntent.Action, "GetSlowProducts", StringComparison.OrdinalIgnoreCase))
            return await ExecuteSlowProducts(companyId.Value, parsedIntent, cancellationToken);

        var requestedTask = MapActionToTask(parsedIntent.Action);
        var inputForTask = input;

        // Allow explicit task switching/starting at any time.
        if (!string.IsNullOrWhiteSpace(requestedTask))
        {
            if (!string.Equals(state.CurrentTask, requestedTask, StringComparison.OrdinalIgnoreCase))
            {
                state.CurrentTask = requestedTask;
                state.Fields.Clear();
                state.StepIndex = 0;
                state.AwaitingConfirmation = false;
                state.LastQuestion = null;
                state.LastAssistantMessage = null;
            }

            foreach (var kv in parsedIntent.Parameters)
            {
                state.Fields[kv.Key] = kv.Value?.ToString();
            }

            // If user only said task intent ("sales order", "purchase order"), ask first question and don't treat it as field value.
            if (parsedIntent.Parameters.Count == 0)
            {
                inputForTask = string.Empty;
            }
        }

        if (string.IsNullOrWhiteSpace(state.CurrentTask) && string.IsNullOrWhiteSpace(requestedTask) && !string.IsNullOrWhiteSpace(input))
        {
            if (TryInferTaskFromInput(input, out var inferredTask, out var inferredLabel))
            {
                state.Fields["pendingIntentTask"] = inferredTask;
                state.Fields["pendingIntentLabel"] = inferredLabel;
                return Ask(state, $"Did you mean {inferredLabel}? Please reply Yes or No.");
            }
        }

        if (string.IsNullOrWhiteSpace(state.CurrentTask))
        {
            return HelpMenu(state);
        }

        var doneCommand = inputLower == "done" || request.Confirmed ||
            (state.AwaitingConfirmation && IsAffirmativeInput(inputLower));
        var skipOptional = inputLower == "next";
        var routeResult = state.CurrentTask switch
        {
            "SalesOrder" => await HandleSalesOrder(companyId.Value, state, inputForTask, doneCommand, skipOptional, cancellationToken),
            "PurchaseOrder" => await HandlePurchaseOrder(companyId.Value, state, inputForTask, doneCommand, skipOptional, cancellationToken),
            "InventoryUpdate" => await HandleInventoryUpdate(companyId.Value, state, inputForTask, doneCommand, skipOptional, cancellationToken),
            "PriceUpdate" => await HandlePriceUpdate(companyId.Value, state, inputForTask, doneCommand, skipOptional, cancellationToken),
            "StockQuery" => await HandleStockQuery(companyId.Value, state, inputForTask, doneCommand, skipOptional, cancellationToken),
            "CustomerCreate" => await HandleCustomerCreate(companyId.Value, state, inputForTask, doneCommand, cancellationToken),
            "CustomerUpdate" => await HandleCustomerUpdate(companyId.Value, state, inputForTask, doneCommand, cancellationToken),
            "CustomerDelete" => await HandleCustomerDelete(companyId.Value, state, inputForTask, doneCommand, cancellationToken),
            "SupplierCreate" => await HandleSupplierCreate(companyId.Value, state, inputForTask, doneCommand, cancellationToken),
            "SupplierUpdate" => await HandleSupplierUpdate(companyId.Value, state, inputForTask, doneCommand, cancellationToken),
            "SupplierDelete" => await HandleSupplierDelete(companyId.Value, state, inputForTask, doneCommand, cancellationToken),
            "WarehouseCreate" => await HandleWarehouseCreate(companyId.Value, state, inputForTask, doneCommand, cancellationToken),
            "WarehouseUpdate" => await HandleWarehouseUpdate(companyId.Value, state, inputForTask, doneCommand, cancellationToken),
            "WarehouseDelete" => await HandleWarehouseDelete(companyId.Value, state, inputForTask, doneCommand, cancellationToken),
            "ProductCreate" => await HandleProductCreate(companyId.Value, state, inputForTask, doneCommand, cancellationToken),
            "ProductUpdate" => await HandleProductUpdate(companyId.Value, state, inputForTask, doneCommand, cancellationToken),
            "ProductDelete" => await HandleProductDelete(companyId.Value, state, inputForTask, doneCommand, cancellationToken),
            "ProductDetails" => await ExecuteProductDetails(companyId.Value, inputForTask, state, cancellationToken),
            "CustomerDetails" => await ExecuteCustomerDetails(companyId.Value, inputForTask, state, cancellationToken),
            "SalesOrderDetails" => await ExecuteSalesOrderDetails(companyId.Value, inputForTask, state, cancellationToken),
            "InvoiceDocument" => await HandleGenerateInvoiceDocument(companyId.Value, state, inputForTask, doneCommand, cancellationToken),
            "PurchaseOrderDocument" => await HandleGeneratePurchaseOrderDocument(companyId.Value, state, inputForTask, doneCommand, cancellationToken),
            "SalesReport" => await HandleSalesReport(companyId.Value, state, inputForTask, doneCommand, cancellationToken),
            "PurchaseReport" => await HandlePurchaseReport(companyId.Value, state, inputForTask, doneCommand, cancellationToken),
            "InventoryReport" => await HandleInventoryReport(companyId.Value, state, inputForTask, doneCommand, cancellationToken),
            "ProfitLossReport" => await HandleProfitLossReport(companyId.Value, state, inputForTask, doneCommand, cancellationToken),
            "BalanceSheetReport" => await HandleBalanceSheetReport(companyId.Value, state, inputForTask, doneCommand, cancellationToken),
            "SalesOrdersExport" => await HandleExportSalesOrders(companyId.Value, state, inputForTask, doneCommand, cancellationToken),
            _ => Result<CopilotExecuteResult>.Success(new CopilotExecuteResult
            {
                Success = true,
                Reply = "Please specify a supported task.",
                SessionState = state
            })
        };
        return routeResult;
    }

    private static string? MapActionToTask(string? action) => action switch
    {
        "CreateSalesOrder" => "SalesOrder",
        "CreatePurchaseOrder" => "PurchaseOrder",
        "InventoryUpdate" => "InventoryUpdate",
        "PriceUpdate" => "PriceUpdate",
        "StockQuery" => "StockQuery",
        "CreateCustomer" => "CustomerCreate",
        "UpdateCustomer" => "CustomerUpdate",
        "DeleteCustomer" => "CustomerDelete",
        "CreateSupplier" => "SupplierCreate",
        "UpdateSupplier" => "SupplierUpdate",
        "DeleteSupplier" => "SupplierDelete",
        "CreateWarehouse" => "WarehouseCreate",
        "UpdateWarehouse" => "WarehouseUpdate",
        "DeleteWarehouse" => "WarehouseDelete",
        "CreateProduct" => "ProductCreate",
        "UpdateProduct" => "ProductUpdate",
        "DeleteProduct" => "ProductDelete",
        "ProductDetails" => "ProductDetails",
        "CustomerDetails" => "CustomerDetails",
        "SalesOrderDetails" => "SalesOrderDetails",
        "GenerateInvoice" => "InvoiceDocument",
        "GeneratePurchaseOrderPdf" => "PurchaseOrderDocument",
        "SalesReport" => "SalesReport",
        "PurchaseReport" => "PurchaseReport",
        "InventoryReport" => "InventoryReport",
        "ProfitLossReport" => "ProfitLossReport",
        "BalanceSheetReport" => "BalanceSheetReport",
        "ExportSalesOrders" => "SalesOrdersExport",
        _ => null
    };

    private async Task<Result<CopilotExecuteResult>> HandleSalesOrder(
        Guid companyId,
        CopilotConversationState state,
        string input,
        bool confirmed,
        bool skipOptional,
        CancellationToken cancellationToken)
    {
        // Required: Customer -> Product -> Quantity -> Warehouse ; Optional: Payment Terms
        if (!TryGetField(state, "customerName", out var customerName))
        {
            if (IsListRequest(input, "customer"))
            {
                var customers = await _context.Customers.AsNoTracking()
                    .Where(x => x.CompanyId == companyId && !x.IsDeleted)
                    .OrderBy(x => x.Name)
                    .Take(10)
                    .Select(x => x.Name)
                    .ToListAsync(cancellationToken);
                return Ask(state, $"Customers: {string.Join(", ", customers)}. Who is the customer?");
            }
            if (!string.IsNullOrWhiteSpace(input)) state.Fields["customerName"] = input;
            if (!TryGetField(state, "customerName", out customerName))
                return Ask(state, "Who is the customer?");
        }
        var customer = await _context.Customers.AsNoTracking()
            .Where(x => x.CompanyId == companyId && !x.IsDeleted && x.Name.Contains(customerName))
            .OrderByDescending(x => x.Name == customerName)
            .ThenBy(x => x.Name.Length)
            .FirstOrDefaultAsync(cancellationToken);
        if (customer == null)
        {
            return await AskWithSuggestion(
                state,
                customerName,
                "customerName",
                "customer name",
                _context.Customers.AsNoTracking().Where(x => x.CompanyId == companyId && !x.IsDeleted).Select(x => x.Name),
                $"Customer '{customerName}' not found. Please provide a valid customer name.",
                cancellationToken);
        }

        if (!TryGetField(state, "productName", out var productName))
        {
            if (IsListRequest(input, "product", "item"))
            {
                var products = await _context.Products.AsNoTracking()
                    .Where(x => x.CompanyId == companyId && !x.IsDeleted)
                    .OrderBy(x => x.Name)
                    .Take(10)
                    .Select(x => x.Name)
                    .ToListAsync(cancellationToken);
                return Ask(state, $"Products: {string.Join(", ", products)}. Which product?");
            }
            if (!string.IsNullOrWhiteSpace(input) && !string.Equals(input, customerName, StringComparison.OrdinalIgnoreCase))
                state.Fields["productName"] = input;
            if (!TryGetField(state, "productName", out productName))
                return Ask(state, "Which product?");
        }
        var product = await _context.Products.AsNoTracking()
            .Where(x => x.CompanyId == companyId && !x.IsDeleted && x.Name.Contains(productName))
            .OrderByDescending(x => x.Name == productName)
            .ThenBy(x => x.Name.Length)
            .FirstOrDefaultAsync(cancellationToken);
        if (product == null)
        {
            return await AskWithSuggestion(
                state,
                productName,
                "productName",
                "product name",
                _context.Products.AsNoTracking().Where(x => x.CompanyId == companyId && !x.IsDeleted).Select(x => x.Name),
                $"Product '{productName}' not found. Please provide a valid product name.",
                cancellationToken);
        }

        if (!TryGetField(state, "quantity", out var quantityText) || !decimal.TryParse(quantityText, out var quantity) || quantity <= 0)
        {
            if (TryParseDecimal(input, out var parsedQty) && parsedQty > 0) state.Fields["quantity"] = parsedQty.ToString();
            if (TryGetField(state, "quantity", out quantityText) && decimal.TryParse(quantityText, out quantity) && quantity > 0)
            {
                // parsed from current input
            }
            else
            {
            state.Fields.Remove("quantity");
            return Ask(state, "What quantity?");
            }
        }

        if (!TryGetField(state, "warehouseName", out var warehouseName))
        {
            if (IsListRequest(input, "warehouse"))
            {
                var warehouses = await _context.Warehouses.AsNoTracking()
                    .Where(x => x.CompanyId == companyId && !x.IsDeleted)
                    .OrderBy(x => x.Name)
                    .Take(10)
                    .Select(x => x.Name)
                    .ToListAsync(cancellationToken);
                return Ask(state, $"Warehouses: {string.Join(", ", warehouses)}. From which warehouse?");
            }
            if (!string.IsNullOrWhiteSpace(input) && !TryParseDecimal(input, out _))
                state.Fields["warehouseName"] = input;
            if (!TryGetField(state, "warehouseName", out warehouseName))
                return Ask(state, "From which warehouse?");
        }
        var warehouse = await _context.Warehouses.AsNoTracking()
            .Where(x => x.CompanyId == companyId && !x.IsDeleted && x.Name.Contains(warehouseName))
            .OrderByDescending(x => x.Name == warehouseName)
            .ThenBy(x => x.Name.Length)
            .FirstOrDefaultAsync(cancellationToken);
        if (warehouse == null)
        {
            return await AskWithSuggestion(
                state,
                warehouseName,
                "warehouseName",
                "warehouse name",
                _context.Warehouses.AsNoTracking().Where(x => x.CompanyId == companyId && !x.IsDeleted).Select(x => x.Name),
                $"Warehouse '{warehouseName}' not found. Please provide a valid warehouse name.",
                cancellationToken);
        }

        var available = (await _context.StockLevels.AsNoTracking()
            .Where(s => s.CompanyId == companyId && !s.IsDeleted && s.WarehouseId == warehouse.Id && s.ProductId == product.Id)
            .Select(s => (decimal?)(s.Quantity - (s.ReservedQuantity ?? 0m)))
            .SumAsync(cancellationToken)) ?? 0m;
        if (available < quantity)
        {
            return Ask(state, $"Insufficient stock for {product.Name}. Available: {available}. Enter a lower quantity.");
        }

        if (!state.Fields.ContainsKey("paymentTerms"))
        {
            if (skipOptional) state.Fields["paymentTerms"] = string.Empty;
            else return Ask(state, "Payment terms? (optional, say Next to skip)");
        }

        if (!confirmed)
        {
            var summary = $"Confirm Sales Order: customer {customer.Name}, product {product.Name}, qty {quantity}, warehouse {warehouse.Name}. Say Done to confirm or Cancel.";
            state.AwaitingConfirmation = true;
            return Result<CopilotExecuteResult>.Success(new CopilotExecuteResult
            {
                Success = true,
                Action = "CreateSalesOrder",
                ConfirmationMessage = summary,
                Reply = summary,
                NextQuestion = "Say Done to confirm or Cancel.",
                SessionState = state
            });
        }

        if (!_currentUser.HasPermission("SalesOrders:Create"))
            return Result<CopilotExecuteResult>.Failure("You do not have permission: SalesOrders:Create");

        var create = new CreateSalesOrderCommand
        {
            CustomerId = customer.Id,
            Notes = "Created by AI chat assistant",
            TermsAndConditions = state.Fields.GetValueOrDefault("paymentTerms"),
            Items = new List<CreateSalesOrderItemDto>
            {
                new() { ProductId = product.Id, Quantity = quantity, UnitPrice = product.SalePrice }
            }
        };

        var result = await _mediator.Send(create, cancellationToken);
        if (!result.Succeeded || result.Data == null)
            return Result<CopilotExecuteResult>.Failure(result.Errors);

        var reset = new CopilotConversationState { SessionId = state.SessionId };
        return Result<CopilotExecuteResult>.Success(new CopilotExecuteResult
        {
            Success = true,
            Action = "CreateSalesOrder",
            Completed = true,
            Reply = $"Sales order {result.Data.OrderNumber} created successfully.",
            Result = new { orderId = result.Data.Id, orderNumber = result.Data.OrderNumber },
            SessionState = reset
        });
    }

    private async Task<Result<CopilotExecuteResult>> HandlePurchaseOrder(
        Guid companyId,
        CopilotConversationState state,
        string input,
        bool confirmed,
        bool skipOptional,
        CancellationToken cancellationToken)
    {
        if (!TryGetField(state, "supplierName", out var supplierName))
        {
            if (IsListRequest(input, "supplier", "vendor"))
            {
                var suppliers = await _context.Suppliers.AsNoTracking()
                    .Where(x => x.CompanyId == companyId && !x.IsDeleted)
                    .OrderBy(x => x.Name)
                    .Take(10)
                    .Select(x => x.Name)
                    .ToListAsync(cancellationToken);
                return Ask(state, $"Suppliers: {string.Join(", ", suppliers)}. Which supplier?");
            }
            if (!string.IsNullOrWhiteSpace(input)) state.Fields["supplierName"] = input;
            if (!TryGetField(state, "supplierName", out supplierName))
                return Ask(state, "Which supplier?");
        }
        var supplier = await _context.Suppliers.AsNoTracking().FirstOrDefaultAsync(x => x.CompanyId == companyId && !x.IsDeleted && x.Name.Contains(supplierName), cancellationToken);
        if (supplier == null)
        {
            return await AskWithSuggestion(
                state,
                supplierName,
                "supplierName",
                "supplier name",
                _context.Suppliers.AsNoTracking().Where(x => x.CompanyId == companyId && !x.IsDeleted).Select(x => x.Name),
                $"Supplier '{supplierName}' not found. Please provide a valid supplier.",
                cancellationToken);
        }

        if (!TryGetField(state, "productName", out var productName))
        {
            if (IsListRequest(input, "product", "item"))
            {
                var products = await _context.Products.AsNoTracking()
                    .Where(x => x.CompanyId == companyId && !x.IsDeleted)
                    .OrderBy(x => x.Name)
                    .Take(10)
                    .Select(x => x.Name)
                    .ToListAsync(cancellationToken);
                return Ask(state, $"Products: {string.Join(", ", products)}. Which product?");
            }
            if (!string.IsNullOrWhiteSpace(input) && !string.Equals(input, supplierName, StringComparison.OrdinalIgnoreCase)) state.Fields["productName"] = input;
            if (!TryGetField(state, "productName", out productName))
                return Ask(state, "Which product?");
        }
        var product = await _context.Products.AsNoTracking().FirstOrDefaultAsync(x => x.CompanyId == companyId && !x.IsDeleted && x.Name.Contains(productName), cancellationToken);
        if (product == null)
        {
            return await AskWithSuggestion(
                state,
                productName,
                "productName",
                "product name",
                _context.Products.AsNoTracking().Where(x => x.CompanyId == companyId && !x.IsDeleted).Select(x => x.Name),
                $"Product '{productName}' not found. Please provide a valid product.",
                cancellationToken);
        }

        if (!TryGetField(state, "quantity", out var quantityText) || !decimal.TryParse(quantityText, out var quantity) || quantity <= 0)
        {
            if (TryParseDecimal(input, out var q) && q > 0) state.Fields["quantity"] = q.ToString();
            if (!TryGetField(state, "quantity", out quantityText) || !decimal.TryParse(quantityText, out quantity) || quantity <= 0)
                return Ask(state, "What quantity?");
        }

        if (!state.Fields.ContainsKey("expectedDeliveryDate"))
        {
            if (skipOptional) state.Fields["expectedDeliveryDate"] = string.Empty;
            else return Ask(state, "Expected delivery date? (optional, say Next to skip)");
        }

        if (!confirmed)
        {
            var summary = $"Confirm Purchase Order: supplier {supplier.Name}, product {product.Name}, qty {quantity}. Say Done to confirm or Cancel.";
            state.AwaitingConfirmation = true;
            return Result<CopilotExecuteResult>.Success(new CopilotExecuteResult
            {
                Success = true,
                Action = "CreatePurchaseOrder",
                ConfirmationMessage = summary,
                Reply = summary,
                NextQuestion = "Say Done to confirm or Cancel.",
                SessionState = state
            });
        }

        if (!_currentUser.HasPermission("PurchaseOrders:Create"))
            return Result<CopilotExecuteResult>.Failure("You do not have permission: PurchaseOrders:Create");

        DateTime? expected = null;
        if (DateTime.TryParse(state.Fields.GetValueOrDefault("expectedDeliveryDate"), out var parsedDate))
            expected = parsedDate;

        var create = new CreatePurchaseOrderCommand
        {
            SupplierId = supplier.Id,
            ExpectedDeliveryDate = expected,
            Notes = "Created by AI chat assistant",
            Items = new List<CreatePurchaseOrderItemDto>
            {
                new() { ProductId = product.Id, Quantity = quantity, UnitPrice = product.PurchasePrice }
            }
        };

        var result = await _mediator.Send(create, cancellationToken);
        if (!result.Succeeded || result.Data == null)
            return Result<CopilotExecuteResult>.Failure(result.Errors);

        var reset = new CopilotConversationState { SessionId = state.SessionId };
        return Result<CopilotExecuteResult>.Success(new CopilotExecuteResult
        {
            Success = true,
            Action = "CreatePurchaseOrder",
            Completed = true,
            Reply = $"Purchase order {result.Data.OrderNumber} created successfully.",
            Result = new { orderId = result.Data.Id, orderNumber = result.Data.OrderNumber },
            SessionState = reset
        });
    }

    private async Task<Result<CopilotExecuteResult>> HandleInventoryUpdate(
        Guid companyId,
        CopilotConversationState state,
        string input,
        bool confirmed,
        bool skipOptional,
        CancellationToken cancellationToken)
    {
        if (!TryGetField(state, "productName", out var productName))
        {
            if (!string.IsNullOrWhiteSpace(input)) state.Fields["productName"] = input;
            if (!TryGetField(state, "productName", out productName))
                return Ask(state, "Which product?");
        }
        var product = await _context.Products.AsNoTracking().FirstOrDefaultAsync(x => x.CompanyId == companyId && !x.IsDeleted && x.Name.Contains(productName), cancellationToken);
        if (product == null)
        {
            return await AskWithSuggestion(
                state,
                productName,
                "productName",
                "product name",
                _context.Products.AsNoTracking().Where(x => x.CompanyId == companyId && !x.IsDeleted).Select(x => x.Name),
                $"Product '{productName}' not found. Please provide a valid product.",
                cancellationToken);
        }

        if (!TryGetField(state, "warehouseName", out var warehouseName))
        {
            if (!string.IsNullOrWhiteSpace(input) && !string.Equals(input, productName, StringComparison.OrdinalIgnoreCase)) state.Fields["warehouseName"] = input;
            if (!TryGetField(state, "warehouseName", out warehouseName))
                return Ask(state, "Which warehouse?");
        }
        var warehouse = await _context.Warehouses.AsNoTracking().FirstOrDefaultAsync(x => x.CompanyId == companyId && !x.IsDeleted && x.Name.Contains(warehouseName), cancellationToken);
        if (warehouse == null)
        {
            return await AskWithSuggestion(
                state,
                warehouseName,
                "warehouseName",
                "warehouse name",
                _context.Warehouses.AsNoTracking().Where(x => x.CompanyId == companyId && !x.IsDeleted).Select(x => x.Name),
                $"Warehouse '{warehouseName}' not found. Please provide a valid warehouse.",
                cancellationToken);
        }

        if (!TryGetField(state, "quantity", out var quantityText) || !decimal.TryParse(quantityText, out var quantity))
        {
            if (TryParseDecimal(input, out var q)) state.Fields["quantity"] = q.ToString();
            if (!TryGetField(state, "quantity", out quantityText) || !decimal.TryParse(quantityText, out quantity))
                return Ask(state, "What quantity adjustment? (use negative for reduction)");
        }

        if (!TryGetField(state, "reason", out var reason))
        {
            if (!string.IsNullOrWhiteSpace(input) && !TryParseDecimal(input, out _)) state.Fields["reason"] = input;
            if (!TryGetField(state, "reason", out reason))
                return Ask(state, "Reason for inventory update?");
        }

        if (!confirmed)
        {
            var summary = $"Confirm Inventory Update: {product.Name}, warehouse {warehouse.Name}, quantity {quantity}, reason '{reason}'. Say Done to confirm.";
            state.AwaitingConfirmation = true;
            return Result<CopilotExecuteResult>.Success(new CopilotExecuteResult
            {
                Success = true,
                Action = "InventoryUpdate",
                ConfirmationMessage = summary,
                Reply = summary,
                SessionState = state
            });
        }

        if (!_currentUser.HasPermission("Inventory:StockTransaction:Create"))
            return Result<CopilotExecuteResult>.Failure("You do not have permission: Inventory:StockTransaction:Create");

        var tx = new CreateStockTransactionCommand
        {
            ProductId = product.Id,
            WarehouseId = warehouse.Id,
            TransactionType = "Adjustment",
            Quantity = quantity,
            Notes = reason,
            ReferenceType = "Copilot"
        };
        var result = await _mediator.Send(tx, cancellationToken);
        if (!result.Succeeded || result.Data == null)
            return Result<CopilotExecuteResult>.Failure(result.Errors);

        var reset = new CopilotConversationState { SessionId = state.SessionId };
        return Result<CopilotExecuteResult>.Success(new CopilotExecuteResult
        {
            Success = true,
            Action = "InventoryUpdate",
            Completed = true,
            Reply = "Inventory update completed successfully.",
            Result = new { transactionId = result.Data.Id, balanceAfter = result.Data.BalanceAfter },
            SessionState = reset
        });
    }

    private async Task<Result<CopilotExecuteResult>> HandlePriceUpdate(
        Guid companyId,
        CopilotConversationState state,
        string input,
        bool confirmed,
        bool skipOptional,
        CancellationToken cancellationToken)
    {
        if (!TryGetField(state, "priceType", out var priceType))
        {
            if (ContainsAny(input, "purchase", "cost"))
                state.Fields["priceType"] = "purchase";
            else if (ContainsAny(input, "sale", "selling"))
                state.Fields["priceType"] = "sale";
            else
                state.Fields["priceType"] = "sale";
        }
        priceType = state.Fields.GetValueOrDefault("priceType") ?? "sale";

        if (!TryGetField(state, "productName", out var productName))
        {
            if (!string.IsNullOrWhiteSpace(input)) state.Fields["productName"] = input;
            if (!TryGetField(state, "productName", out productName))
                return Ask(state, "Which product to update price?");
        }
        var product = await _context.Products.AsNoTracking().FirstOrDefaultAsync(x => x.CompanyId == companyId && !x.IsDeleted && x.Name.Contains(productName), cancellationToken);
        if (product == null)
        {
            return await AskWithSuggestion(
                state,
                productName,
                "productName",
                "product name",
                _context.Products.AsNoTracking().Where(x => x.CompanyId == companyId && !x.IsDeleted).Select(x => x.Name),
                $"Product '{productName}' not found. Please provide a valid product.",
                cancellationToken);
        }

        if (!TryGetField(state, "newPrice", out var priceText) || !decimal.TryParse(priceText, out var newPrice) || newPrice < 0)
        {
            if (TryParseDecimal(input, out var p) && p >= 0) state.Fields["newPrice"] = p.ToString();
            if (!TryGetField(state, "newPrice", out priceText) || !decimal.TryParse(priceText, out newPrice) || newPrice < 0)
                return Ask(state, $"What is the new {priceType} price?");
        }

        if (!state.Fields.ContainsKey("effectiveDate"))
        {
            if (skipOptional) state.Fields["effectiveDate"] = string.Empty;
            else return Ask(state, "Effective date? (optional, say Next to skip)");
        }

        if (!confirmed)
        {
            var summary = $"Confirm Price Update: {product.Name} new {priceType} price {newPrice}. Say Done to confirm.";
            state.AwaitingConfirmation = true;
            return Result<CopilotExecuteResult>.Success(new CopilotExecuteResult
            {
                Success = true,
                Action = "PriceUpdate",
                ConfirmationMessage = summary,
                Reply = summary,
                SessionState = state
            });
        }

        if (!_currentUser.HasPermission("Products:Update"))
            return Result<CopilotExecuteResult>.Failure("You do not have permission: Products:Update");

        var update = new UpdateProductCommand
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            SKU = product.SKU,
            Barcode = product.Barcode,
            CategoryId = product.CategoryId,
            BrandId = product.BrandId,
            UnitOfMeasureId = product.UnitOfMeasureId,
            PurchasePrice = string.Equals(priceType, "purchase", StringComparison.OrdinalIgnoreCase) ? newPrice : product.PurchasePrice,
            SalePrice = string.Equals(priceType, "purchase", StringComparison.OrdinalIgnoreCase) ? product.SalePrice : newPrice,
            CostPrice = product.CostPrice,
            MinStockLevel = product.MinStockLevel,
            MaxStockLevel = product.MaxStockLevel,
            ReorderPoint = product.ReorderPoint,
            TrackQuantity = product.TrackQuantity,
            TrackBatch = product.TrackBatch,
            TrackExpiry = product.TrackExpiry,
            ImageUrl = product.ImageUrl,
            Weight = product.Weight,
            WeightUnit = product.WeightUnit,
            Length = product.Length,
            Width = product.Width,
            Height = product.Height,
            DimensionsUnit = product.DimensionsUnit
        };

        var result = await _mediator.Send(update, cancellationToken);
        if (!result.Succeeded || result.Data == null)
            return Result<CopilotExecuteResult>.Failure(result.Errors);

        var reset = new CopilotConversationState { SessionId = state.SessionId };
        return Result<CopilotExecuteResult>.Success(new CopilotExecuteResult
        {
            Success = true,
            Action = "PriceUpdate",
            Completed = true,
            Reply = $"Price updated for {result.Data.Name}.",
            Result = new { productId = result.Data.Id, salePrice = result.Data.SalePrice, purchasePrice = result.Data.PurchasePrice },
            SessionState = reset
        });
    }

    private async Task<Result<CopilotExecuteResult>> HandleStockQuery(
        Guid companyId,
        CopilotConversationState state,
        string input,
        bool confirmed,
        bool skipOptional,
        CancellationToken cancellationToken)
    {
        try
        {
            if (!_currentUser.HasPermission("Inventory:StockLevel:List"))
                return Result<CopilotExecuteResult>.Failure("You do not have permission: Inventory:StockLevel:List");

            if (!TryGetField(state, "productName", out var productName))
            {
                if (IsListRequest(input, "product", "item"))
                {
                    var products = await _context.Products.AsNoTracking()
                        .Where(x => x.CompanyId == companyId && !x.IsDeleted)
                        .OrderBy(x => x.Name)
                        .Take(10)
                        .Select(x => x.Name)
                        .ToListAsync(cancellationToken);
                    return Ask(state, $"Products: {string.Join(", ", products)}. Which product do you want stock for?");
                }
                if (!string.IsNullOrWhiteSpace(input)) state.Fields["productName"] = input;
                if (!TryGetField(state, "productName", out productName))
                    return Ask(state, "Which product do you want stock for?");
            }

            var product = await _context.Products.AsNoTracking()
                .FirstOrDefaultAsync(x => x.CompanyId == companyId && !x.IsDeleted && x.Name.Contains(productName), cancellationToken);
            if (product == null)
            {
                return await AskWithSuggestion(
                    state,
                    productName,
                    "productName",
                    "product name",
                    _context.Products.AsNoTracking().Where(x => x.CompanyId == companyId && !x.IsDeleted).Select(x => x.Name),
                    $"Product '{productName}' not found. Please provide a valid product.",
                    cancellationToken);
            }

            var rawLevels = await _context.StockLevels.AsNoTracking()
                .Where(x => x.CompanyId == companyId && !x.IsDeleted && x.ProductId == product.Id)
                .Select(x => new
                {
                    x.WarehouseId,
                    x.Quantity,
                    Available = x.AvailableQuantity
                })
                .ToListAsync(cancellationToken);

            var warehouseIds = rawLevels.Select(x => x.WarehouseId).Distinct().ToList();
            var warehouseNames = await _context.Warehouses.AsNoTracking()
                .Where(w => w.CompanyId == companyId && !w.IsDeleted && warehouseIds.Contains(w.Id))
                .ToDictionaryAsync(w => w.Id, w => w.Name, cancellationToken);

            var levels = rawLevels
                .Select(x => new
                {
                    warehouseId = x.WarehouseId,
                    warehouse = warehouseNames.TryGetValue(x.WarehouseId, out var name) ? name : "Unknown warehouse",
                    quantity = x.Quantity,
                    available = x.Available
                })
                .OrderByDescending(x => x.available)
                .ToList();

            var total = levels.Sum(x => x.available);
            var reset = new CopilotConversationState { SessionId = state.SessionId };
            return Result<CopilotExecuteResult>.Success(new CopilotExecuteResult
            {
                Success = true,
                Action = "StockQuery",
                Completed = true,
                Reply = $"Stock for {product.Name}: total available {total}.",
                Result = levels,
                SessionState = reset
            });
        }
        catch (Exception ex)
        {
            return Result<CopilotExecuteResult>.Failure($"Failed to fetch stock details. {ex.Message}");
        }
    }

    private async Task<Result<CopilotExecuteResult>> HandleCustomerCreate(
        Guid companyId,
        CopilotConversationState state,
        string input,
        bool confirmed,
        CancellationToken cancellationToken)
    {
        var isNext = string.Equals(input, "next", StringComparison.OrdinalIgnoreCase);

        if (!TryGetField(state, "customerName", out var customerName))
        {
            if (!string.IsNullOrWhiteSpace(input)) state.Fields["customerName"] = input;
            if (!TryGetField(state, "customerName", out customerName))
                return Ask(state, "What is the customer name?");
        }

        const string codePrompt = "Customer code? (optional, say Next to skip)";
        if (!state.Fields.ContainsKey("customerCode"))
        {
            if (string.Equals(state.LastQuestion, codePrompt, StringComparison.OrdinalIgnoreCase))
            {
                if (isNext) state.Fields["customerCode"] = string.Empty;
                else if (!string.IsNullOrWhiteSpace(input)) state.Fields["customerCode"] = input;
            }
            if (!state.Fields.ContainsKey("customerCode")) return Ask(state, codePrompt);
        }

        const string contactPrompt = "Contact person? (optional, say Next to skip)";
        if (!state.Fields.ContainsKey("customerContact"))
        {
            if (string.Equals(state.LastQuestion, contactPrompt, StringComparison.OrdinalIgnoreCase))
            {
                if (isNext) state.Fields["customerContact"] = string.Empty;
                else if (!string.IsNullOrWhiteSpace(input)) state.Fields["customerContact"] = input;
            }
            if (!state.Fields.ContainsKey("customerContact")) return Ask(state, contactPrompt);
        }

        const string phonePrompt = "Phone number? (optional, say Next to skip)";
        if (!state.Fields.ContainsKey("customerPhone"))
        {
            if (string.Equals(state.LastQuestion, phonePrompt, StringComparison.OrdinalIgnoreCase))
            {
                if (isNext) state.Fields["customerPhone"] = string.Empty;
                else if (!string.IsNullOrWhiteSpace(input)) state.Fields["customerPhone"] = input;
            }
            if (!state.Fields.ContainsKey("customerPhone")) return Ask(state, phonePrompt);
        }

        const string emailPrompt = "Email? (optional, say Next to skip)";
        if (!state.Fields.ContainsKey("customerEmail"))
        {
            if (string.Equals(state.LastQuestion, emailPrompt, StringComparison.OrdinalIgnoreCase))
            {
                if (isNext) state.Fields["customerEmail"] = string.Empty;
                else if (!string.IsNullOrWhiteSpace(input)) state.Fields["customerEmail"] = input;
            }
            if (!state.Fields.ContainsKey("customerEmail")) return Ask(state, emailPrompt);
        }

        const string addressPrompt = "Address? (optional, say Next to skip)";
        if (!state.Fields.ContainsKey("customerAddress"))
        {
            if (string.Equals(state.LastQuestion, addressPrompt, StringComparison.OrdinalIgnoreCase))
            {
                if (isNext) state.Fields["customerAddress"] = string.Empty;
                else if (!string.IsNullOrWhiteSpace(input)) state.Fields["customerAddress"] = input;
            }
            if (!state.Fields.ContainsKey("customerAddress")) return Ask(state, addressPrompt);
        }

        const string paymentPrompt = "Payment terms? (optional, say Next to skip)";
        if (!state.Fields.ContainsKey("customerPaymentTerms"))
        {
            if (string.Equals(state.LastQuestion, paymentPrompt, StringComparison.OrdinalIgnoreCase))
            {
                if (isNext) state.Fields["customerPaymentTerms"] = string.Empty;
                else if (!string.IsNullOrWhiteSpace(input)) state.Fields["customerPaymentTerms"] = input;
            }
            if (!state.Fields.ContainsKey("customerPaymentTerms")) return Ask(state, paymentPrompt);
        }

        const string creditPrompt = "Credit limit? (optional, say Next to skip)";
        if (!state.Fields.ContainsKey("customerCreditLimit"))
        {
            if (string.Equals(state.LastQuestion, creditPrompt, StringComparison.OrdinalIgnoreCase))
            {
                if (isNext) state.Fields["customerCreditLimit"] = string.Empty;
                else if (!string.IsNullOrWhiteSpace(input)) state.Fields["customerCreditLimit"] = input;
            }
            if (!state.Fields.ContainsKey("customerCreditLimit")) return Ask(state, creditPrompt);
        }

        var code = state.Fields.GetValueOrDefault("customerCode")?.Trim();
        var contact = state.Fields.GetValueOrDefault("customerContact")?.Trim();
        var phone = state.Fields.GetValueOrDefault("customerPhone")?.Trim();
        var email = state.Fields.GetValueOrDefault("customerEmail")?.Trim();
        var address = state.Fields.GetValueOrDefault("customerAddress")?.Trim();
        var paymentTerms = state.Fields.GetValueOrDefault("customerPaymentTerms")?.Trim();
        decimal? creditLimit = null;
        var creditRaw = state.Fields.GetValueOrDefault("customerCreditLimit");
        if (!string.IsNullOrWhiteSpace(creditRaw))
        {
            if (!TryParseDecimal(creditRaw, out var parsedCredit) || parsedCredit < 0)
            {
                state.Fields.Remove("customerCreditLimit");
                return Ask(state, "Credit limit must be a valid non-negative number, or say Next to skip.");
            }
            creditLimit = parsedCredit;
        }

        if (!confirmed)
        {
            var summary = $"Confirm Create Customer: {customerName}, code {code ?? "-"}, contact {contact ?? "-"}, phone {phone ?? "-"}, email {email ?? "-"}, address {address ?? "-"}, payment terms {paymentTerms ?? "-"}, credit limit {creditLimit?.ToString() ?? "-"}. Say Done to confirm or Cancel.";
            state.AwaitingConfirmation = true;
            return Ask(state, summary);
        }

        if (!_currentUser.HasPermission("Customers:Create"))
            return Result<CopilotExecuteResult>.Failure("You do not have permission: Customers:Create");

        var result = await _mediator.Send(new CreateCustomerCommand
        {
            Name = customerName,
            Code = string.IsNullOrWhiteSpace(code) ? null : code,
            ContactPerson = string.IsNullOrWhiteSpace(contact) ? null : contact,
            PhoneNumber = string.IsNullOrWhiteSpace(phone) ? null : phone,
            Email = string.IsNullOrWhiteSpace(email) ? null : email,
            Address = string.IsNullOrWhiteSpace(address) ? null : address,
            PaymentTerms = string.IsNullOrWhiteSpace(paymentTerms) ? null : paymentTerms,
            CreditLimit = creditLimit
        }, cancellationToken);

        if (!result.Succeeded || result.Data == null)
            return Result<CopilotExecuteResult>.Failure(result.Errors);

        var reset = new CopilotConversationState { SessionId = state.SessionId };
        return Result<CopilotExecuteResult>.Success(new CopilotExecuteResult
        {
            Success = true,
            Action = "CreateCustomer",
            Completed = true,
            Reply = $"Customer {result.Data.Name} created successfully.",
            Result = new { customerId = result.Data.Id, customerName = result.Data.Name },
            SessionState = reset
        });
    }

    private async Task<Result<CopilotExecuteResult>> HandleCustomerUpdate(
        Guid companyId,
        CopilotConversationState state,
        string input,
        bool confirmed,
        CancellationToken cancellationToken)
    {
        var isNext = string.Equals(input, "next", StringComparison.OrdinalIgnoreCase);

        if (!TryGetField(state, "customerName", out var customerName))
        {
            if (!string.IsNullOrWhiteSpace(input)) state.Fields["customerName"] = input;
            if (!TryGetField(state, "customerName", out customerName))
                return Ask(state, "Which customer do you want to update?");
        }

        var customer = await _context.Customers.AsNoTracking()
            .FirstOrDefaultAsync(x => x.CompanyId == companyId && !x.IsDeleted && x.Name.Contains(customerName), cancellationToken);
        if (customer == null)
        {
            return await AskWithSuggestion(
                state,
                customerName,
                "customerName",
                "customer name",
                _context.Customers.AsNoTracking().Where(x => x.CompanyId == companyId && !x.IsDeleted).Select(x => x.Name),
                $"Customer '{customerName}' not found. Please provide a valid customer name.",
                cancellationToken);
        }

        if (!TryGetField(state, "newCustomerName", out var newCustomerName))
        {
            if (!string.IsNullOrWhiteSpace(input) && !string.Equals(input, customerName, StringComparison.OrdinalIgnoreCase))
                state.Fields["newCustomerName"] = input;
            if (!TryGetField(state, "newCustomerName", out newCustomerName))
                return Ask(state, "What is the new customer name?");
        }

        const string codePrompt = "Customer code? (optional, say Next to keep current)";
        if (!state.Fields.ContainsKey("updateCustomerCode"))
        {
            if (string.Equals(state.LastQuestion, codePrompt, StringComparison.OrdinalIgnoreCase))
            {
                if (isNext) state.Fields["updateCustomerCode"] = customer.Code ?? string.Empty;
                else if (!string.IsNullOrWhiteSpace(input)) state.Fields["updateCustomerCode"] = input;
            }
            if (!state.Fields.ContainsKey("updateCustomerCode")) return Ask(state, codePrompt);
        }

        const string contactPrompt = "Contact person? (optional, say Next to keep current)";
        if (!state.Fields.ContainsKey("updateCustomerContact"))
        {
            if (string.Equals(state.LastQuestion, contactPrompt, StringComparison.OrdinalIgnoreCase))
            {
                if (isNext) state.Fields["updateCustomerContact"] = customer.ContactPerson ?? string.Empty;
                else if (!string.IsNullOrWhiteSpace(input)) state.Fields["updateCustomerContact"] = input;
            }
            if (!state.Fields.ContainsKey("updateCustomerContact")) return Ask(state, contactPrompt);
        }

        const string phonePrompt = "Phone number? (optional, say Next to keep current)";
        if (!state.Fields.ContainsKey("updateCustomerPhone"))
        {
            if (string.Equals(state.LastQuestion, phonePrompt, StringComparison.OrdinalIgnoreCase))
            {
                if (isNext) state.Fields["updateCustomerPhone"] = customer.PhoneNumber ?? string.Empty;
                else if (!string.IsNullOrWhiteSpace(input)) state.Fields["updateCustomerPhone"] = input;
            }
            if (!state.Fields.ContainsKey("updateCustomerPhone")) return Ask(state, phonePrompt);
        }

        const string emailPrompt = "Email? (optional, say Next to keep current)";
        if (!state.Fields.ContainsKey("updateCustomerEmail"))
        {
            if (string.Equals(state.LastQuestion, emailPrompt, StringComparison.OrdinalIgnoreCase))
            {
                if (isNext) state.Fields["updateCustomerEmail"] = customer.Email ?? string.Empty;
                else if (!string.IsNullOrWhiteSpace(input)) state.Fields["updateCustomerEmail"] = input;
            }
            if (!state.Fields.ContainsKey("updateCustomerEmail")) return Ask(state, emailPrompt);
        }

        const string addressPrompt = "Address? (optional, say Next to keep current)";
        if (!state.Fields.ContainsKey("updateCustomerAddress"))
        {
            if (string.Equals(state.LastQuestion, addressPrompt, StringComparison.OrdinalIgnoreCase))
            {
                if (isNext) state.Fields["updateCustomerAddress"] = customer.Address ?? string.Empty;
                else if (!string.IsNullOrWhiteSpace(input)) state.Fields["updateCustomerAddress"] = input;
            }
            if (!state.Fields.ContainsKey("updateCustomerAddress")) return Ask(state, addressPrompt);
        }

        const string paymentPrompt = "Payment terms? (optional, say Next to keep current)";
        if (!state.Fields.ContainsKey("updateCustomerPaymentTerms"))
        {
            if (string.Equals(state.LastQuestion, paymentPrompt, StringComparison.OrdinalIgnoreCase))
            {
                if (isNext) state.Fields["updateCustomerPaymentTerms"] = customer.PaymentTerms ?? string.Empty;
                else if (!string.IsNullOrWhiteSpace(input)) state.Fields["updateCustomerPaymentTerms"] = input;
            }
            if (!state.Fields.ContainsKey("updateCustomerPaymentTerms")) return Ask(state, paymentPrompt);
        }

        const string creditPrompt = "Credit limit? (optional, say Next to keep current)";
        if (!state.Fields.ContainsKey("updateCustomerCreditLimit"))
        {
            if (string.Equals(state.LastQuestion, creditPrompt, StringComparison.OrdinalIgnoreCase))
            {
                if (isNext) state.Fields["updateCustomerCreditLimit"] = customer.CreditLimit?.ToString() ?? string.Empty;
                else if (!string.IsNullOrWhiteSpace(input)) state.Fields["updateCustomerCreditLimit"] = input;
            }
            if (!state.Fields.ContainsKey("updateCustomerCreditLimit")) return Ask(state, creditPrompt);
        }

        var code = state.Fields.GetValueOrDefault("updateCustomerCode")?.Trim();
        var contact = state.Fields.GetValueOrDefault("updateCustomerContact")?.Trim();
        var phone = state.Fields.GetValueOrDefault("updateCustomerPhone")?.Trim();
        var email = state.Fields.GetValueOrDefault("updateCustomerEmail")?.Trim();
        var address = state.Fields.GetValueOrDefault("updateCustomerAddress")?.Trim();
        var paymentTerms = state.Fields.GetValueOrDefault("updateCustomerPaymentTerms")?.Trim();
        decimal? creditLimit = null;
        var creditRaw = state.Fields.GetValueOrDefault("updateCustomerCreditLimit");
        if (!string.IsNullOrWhiteSpace(creditRaw))
        {
            if (!TryParseDecimal(creditRaw, out var parsedCredit) || parsedCredit < 0)
            {
                state.Fields.Remove("updateCustomerCreditLimit");
                return Ask(state, "Credit limit must be a valid non-negative number, or say Next to keep current.");
            }
            creditLimit = parsedCredit;
        }

        if (!confirmed)
        {
            state.AwaitingConfirmation = true;
            return Ask(state, $"Confirm Update Customer: {customer.Name} -> {newCustomerName}, code {code ?? "-"}, contact {contact ?? "-"}, phone {phone ?? "-"}, email {email ?? "-"}, address {address ?? "-"}, payment terms {paymentTerms ?? "-"}, credit limit {creditLimit?.ToString() ?? "-"}. Say Done to confirm or Cancel.");
        }

        if (!_currentUser.HasPermission("Customers:Update"))
            return Result<CopilotExecuteResult>.Failure("You do not have permission: Customers:Update");

        var update = await _mediator.Send(new UpdateCustomerCommand
        {
            Id = customer.Id,
            Name = newCustomerName,
            Code = string.IsNullOrWhiteSpace(code) ? null : code,
            ContactPerson = string.IsNullOrWhiteSpace(contact) ? null : contact,
            Email = string.IsNullOrWhiteSpace(email) ? null : email,
            PhoneNumber = string.IsNullOrWhiteSpace(phone) ? null : phone,
            Address = string.IsNullOrWhiteSpace(address) ? null : address,
            City = customer.City,
            State = customer.State,
            Country = customer.Country,
            PostalCode = customer.PostalCode,
            TaxId = customer.TaxId,
            PaymentTerms = string.IsNullOrWhiteSpace(paymentTerms) ? null : paymentTerms,
            CreditLimit = creditLimit
        }, cancellationToken);

        if (!update.Succeeded || update.Data == null)
            return Result<CopilotExecuteResult>.Failure(update.Errors);

        var reset = new CopilotConversationState { SessionId = state.SessionId };
        return Result<CopilotExecuteResult>.Success(new CopilotExecuteResult
        {
            Success = true,
            Action = "UpdateCustomer",
            Completed = true,
            Reply = $"Customer updated successfully to {update.Data.Name}.",
            Result = new { customerId = update.Data.Id, customerName = update.Data.Name },
            SessionState = reset
        });
    }

    private async Task<Result<CopilotExecuteResult>> HandleCustomerDelete(
        Guid companyId,
        CopilotConversationState state,
        string input,
        bool confirmed,
        CancellationToken cancellationToken)
    {
        if (!TryGetField(state, "customerName", out var customerName))
        {
            if (!string.IsNullOrWhiteSpace(input)) state.Fields["customerName"] = input;
            if (!TryGetField(state, "customerName", out customerName))
                return Ask(state, "Which customer do you want to delete?");
        }

        var customer = await _context.Customers
            .FirstOrDefaultAsync(x => x.CompanyId == companyId && !x.IsDeleted && x.Name.Contains(customerName), cancellationToken);
        if (customer == null)
        {
            return await AskWithSuggestion(
                state,
                customerName,
                "customerName",
                "customer name",
                _context.Customers.AsNoTracking().Where(x => x.CompanyId == companyId && !x.IsDeleted).Select(x => x.Name),
                $"Customer '{customerName}' not found. Please provide a valid customer name.",
                cancellationToken);
        }

        var hasOrders = await _context.SalesOrders.AsNoTracking()
            .AnyAsync(x => x.CompanyId == companyId && !x.IsDeleted && x.CustomerId == customer.Id, cancellationToken);
        if (hasOrders)
            return Result<CopilotExecuteResult>.Failure("Cannot delete customer with sales orders. Please deactivate instead.");

        if (!confirmed)
        {
            state.AwaitingConfirmation = true;
            return Ask(state, $"Confirm Delete Customer: {customer.Name}. Say Done to confirm or Cancel.");
        }

        if (!_currentUser.HasPermission("Customers:Delete"))
            return Result<CopilotExecuteResult>.Failure("You do not have permission: Customers:Delete");

        customer.MarkAsDeleted(_currentUser.UserId);
        await _context.SaveChangesAsync(cancellationToken);

        var reset = new CopilotConversationState { SessionId = state.SessionId };
        return Result<CopilotExecuteResult>.Success(new CopilotExecuteResult
        {
            Success = true,
            Action = "DeleteCustomer",
            Completed = true,
            Reply = $"Customer {customer.Name} deleted successfully.",
            SessionState = reset
        });
    }

    private async Task<Result<CopilotExecuteResult>> HandleSupplierCreate(
        Guid companyId,
        CopilotConversationState state,
        string input,
        bool confirmed,
        CancellationToken cancellationToken)
    {
        var isNext = string.Equals(input, "next", StringComparison.OrdinalIgnoreCase);

        if (!TryGetField(state, "supplierName", out var supplierName))
        {
            if (!string.IsNullOrWhiteSpace(input)) state.Fields["supplierName"] = input;
            if (!TryGetField(state, "supplierName", out supplierName))
                return Ask(state, "What is the supplier name?");
        }

        const string codePrompt = "Supplier code? (optional, say Next to skip)";
        if (!state.Fields.ContainsKey("supplierCode"))
        {
            if (string.Equals(state.LastQuestion, codePrompt, StringComparison.OrdinalIgnoreCase))
            {
                if (isNext) state.Fields["supplierCode"] = string.Empty;
                else if (!string.IsNullOrWhiteSpace(input)) state.Fields["supplierCode"] = input;
            }
            if (!state.Fields.ContainsKey("supplierCode")) return Ask(state, codePrompt);
        }

        const string contactPrompt = "Contact person? (optional, say Next to skip)";
        if (!state.Fields.ContainsKey("supplierContact"))
        {
            if (string.Equals(state.LastQuestion, contactPrompt, StringComparison.OrdinalIgnoreCase))
            {
                if (isNext) state.Fields["supplierContact"] = string.Empty;
                else if (!string.IsNullOrWhiteSpace(input)) state.Fields["supplierContact"] = input;
            }
            if (!state.Fields.ContainsKey("supplierContact")) return Ask(state, contactPrompt);
        }

        const string phonePrompt = "Phone number? (optional, say Next to skip)";
        if (!state.Fields.ContainsKey("supplierPhone"))
        {
            if (string.Equals(state.LastQuestion, phonePrompt, StringComparison.OrdinalIgnoreCase))
            {
                if (isNext) state.Fields["supplierPhone"] = string.Empty;
                else if (!string.IsNullOrWhiteSpace(input)) state.Fields["supplierPhone"] = input;
            }
            if (!state.Fields.ContainsKey("supplierPhone")) return Ask(state, phonePrompt);
        }

        const string emailPrompt = "Email? (optional, say Next to skip)";
        if (!state.Fields.ContainsKey("supplierEmail"))
        {
            if (string.Equals(state.LastQuestion, emailPrompt, StringComparison.OrdinalIgnoreCase))
            {
                if (isNext) state.Fields["supplierEmail"] = string.Empty;
                else if (!string.IsNullOrWhiteSpace(input)) state.Fields["supplierEmail"] = input;
            }
            if (!state.Fields.ContainsKey("supplierEmail")) return Ask(state, emailPrompt);
        }

        const string addressPrompt = "Address? (optional, say Next to skip)";
        if (!state.Fields.ContainsKey("supplierAddress"))
        {
            if (string.Equals(state.LastQuestion, addressPrompt, StringComparison.OrdinalIgnoreCase))
            {
                if (isNext) state.Fields["supplierAddress"] = string.Empty;
                else if (!string.IsNullOrWhiteSpace(input)) state.Fields["supplierAddress"] = input;
            }
            if (!state.Fields.ContainsKey("supplierAddress")) return Ask(state, addressPrompt);
        }

        const string paymentPrompt = "Payment terms? (optional, say Next to skip)";
        if (!state.Fields.ContainsKey("supplierPaymentTerms"))
        {
            if (string.Equals(state.LastQuestion, paymentPrompt, StringComparison.OrdinalIgnoreCase))
            {
                if (isNext) state.Fields["supplierPaymentTerms"] = string.Empty;
                else if (!string.IsNullOrWhiteSpace(input)) state.Fields["supplierPaymentTerms"] = input;
            }
            if (!state.Fields.ContainsKey("supplierPaymentTerms")) return Ask(state, paymentPrompt);
        }

        const string creditPrompt = "Credit limit? (optional, say Next to skip)";
        if (!state.Fields.ContainsKey("supplierCreditLimit"))
        {
            if (string.Equals(state.LastQuestion, creditPrompt, StringComparison.OrdinalIgnoreCase))
            {
                if (isNext) state.Fields["supplierCreditLimit"] = string.Empty;
                else if (!string.IsNullOrWhiteSpace(input)) state.Fields["supplierCreditLimit"] = input;
            }
            if (!state.Fields.ContainsKey("supplierCreditLimit")) return Ask(state, creditPrompt);
        }

        var code = state.Fields.GetValueOrDefault("supplierCode")?.Trim();
        var contact = state.Fields.GetValueOrDefault("supplierContact")?.Trim();
        var phone = state.Fields.GetValueOrDefault("supplierPhone")?.Trim();
        var email = state.Fields.GetValueOrDefault("supplierEmail")?.Trim();
        var address = state.Fields.GetValueOrDefault("supplierAddress")?.Trim();
        var paymentTerms = state.Fields.GetValueOrDefault("supplierPaymentTerms")?.Trim();
        decimal? creditLimit = null;
        var creditRaw = state.Fields.GetValueOrDefault("supplierCreditLimit");
        if (!string.IsNullOrWhiteSpace(creditRaw))
        {
            if (!TryParseDecimal(creditRaw, out var parsedCredit) || parsedCredit < 0)
            {
                state.Fields.Remove("supplierCreditLimit");
                return Ask(state, "Credit limit must be a valid non-negative number, or say Next to skip.");
            }
            creditLimit = parsedCredit;
        }

        if (!confirmed)
        {
            state.AwaitingConfirmation = true;
            return Ask(state, $"Confirm Create Supplier: {supplierName}, code {code ?? "-"}, contact {contact ?? "-"}, phone {phone ?? "-"}, email {email ?? "-"}, address {address ?? "-"}, payment terms {paymentTerms ?? "-"}, credit limit {creditLimit?.ToString() ?? "-"}. Say Done to confirm or Cancel.");
        }

        if (!_currentUser.HasPermission("Suppliers:Create"))
            return Result<CopilotExecuteResult>.Failure("You do not have permission: Suppliers:Create");

        var result = await _mediator.Send(new CreateSupplierCommand
        {
            Name = supplierName,
            Code = string.IsNullOrWhiteSpace(code) ? null : code,
            ContactPerson = string.IsNullOrWhiteSpace(contact) ? null : contact,
            PhoneNumber = string.IsNullOrWhiteSpace(phone) ? null : phone,
            Email = string.IsNullOrWhiteSpace(email) ? null : email,
            Address = string.IsNullOrWhiteSpace(address) ? null : address,
            PaymentTerms = string.IsNullOrWhiteSpace(paymentTerms) ? null : paymentTerms,
            CreditLimit = creditLimit
        }, cancellationToken);
        if (!result.Succeeded || result.Data == null)
            return Result<CopilotExecuteResult>.Failure(result.Errors);

        var reset = new CopilotConversationState { SessionId = state.SessionId };
        return Result<CopilotExecuteResult>.Success(new CopilotExecuteResult
        {
            Success = true,
            Action = "CreateSupplier",
            Completed = true,
            Reply = $"Supplier {result.Data.Name} created successfully.",
            Result = new { supplierId = result.Data.Id, supplierName = result.Data.Name },
            SessionState = reset
        });
    }

    private async Task<Result<CopilotExecuteResult>> HandleSupplierUpdate(
        Guid companyId,
        CopilotConversationState state,
        string input,
        bool confirmed,
        CancellationToken cancellationToken)
    {
        var isNext = string.Equals(input, "next", StringComparison.OrdinalIgnoreCase);

        if (!TryGetField(state, "supplierName", out var supplierName))
        {
            if (!string.IsNullOrWhiteSpace(input)) state.Fields["supplierName"] = input;
            if (!TryGetField(state, "supplierName", out supplierName))
                return Ask(state, "Which supplier do you want to update?");
        }

        var supplier = await _context.Suppliers.AsNoTracking()
            .FirstOrDefaultAsync(x => x.CompanyId == companyId && !x.IsDeleted && x.Name.Contains(supplierName), cancellationToken);
        if (supplier == null)
        {
            return await AskWithSuggestion(
                state,
                supplierName,
                "supplierName",
                "supplier name",
                _context.Suppliers.AsNoTracking().Where(x => x.CompanyId == companyId && !x.IsDeleted).Select(x => x.Name),
                $"Supplier '{supplierName}' not found. Please provide a valid supplier.",
                cancellationToken);
        }

        if (!TryGetField(state, "newSupplierName", out var newSupplierName))
        {
            if (!string.IsNullOrWhiteSpace(input) && !string.Equals(input, supplierName, StringComparison.OrdinalIgnoreCase))
                state.Fields["newSupplierName"] = input;
            if (!TryGetField(state, "newSupplierName", out newSupplierName))
                return Ask(state, "What is the new supplier name?");
        }

        const string codePrompt = "Supplier code? (optional, say Next to keep current)";
        if (!state.Fields.ContainsKey("updateSupplierCode"))
        {
            if (string.Equals(state.LastQuestion, codePrompt, StringComparison.OrdinalIgnoreCase))
            {
                if (isNext) state.Fields["updateSupplierCode"] = supplier.Code ?? string.Empty;
                else if (!string.IsNullOrWhiteSpace(input)) state.Fields["updateSupplierCode"] = input;
            }
            if (!state.Fields.ContainsKey("updateSupplierCode")) return Ask(state, codePrompt);
        }

        const string contactPrompt = "Contact person? (optional, say Next to keep current)";
        if (!state.Fields.ContainsKey("updateSupplierContact"))
        {
            if (string.Equals(state.LastQuestion, contactPrompt, StringComparison.OrdinalIgnoreCase))
            {
                if (isNext) state.Fields["updateSupplierContact"] = supplier.ContactPerson ?? string.Empty;
                else if (!string.IsNullOrWhiteSpace(input)) state.Fields["updateSupplierContact"] = input;
            }
            if (!state.Fields.ContainsKey("updateSupplierContact")) return Ask(state, contactPrompt);
        }

        const string phonePrompt = "Phone number? (optional, say Next to keep current)";
        if (!state.Fields.ContainsKey("updateSupplierPhone"))
        {
            if (string.Equals(state.LastQuestion, phonePrompt, StringComparison.OrdinalIgnoreCase))
            {
                if (isNext) state.Fields["updateSupplierPhone"] = supplier.PhoneNumber ?? string.Empty;
                else if (!string.IsNullOrWhiteSpace(input)) state.Fields["updateSupplierPhone"] = input;
            }
            if (!state.Fields.ContainsKey("updateSupplierPhone")) return Ask(state, phonePrompt);
        }

        const string emailPrompt = "Email? (optional, say Next to keep current)";
        if (!state.Fields.ContainsKey("updateSupplierEmail"))
        {
            if (string.Equals(state.LastQuestion, emailPrompt, StringComparison.OrdinalIgnoreCase))
            {
                if (isNext) state.Fields["updateSupplierEmail"] = supplier.Email ?? string.Empty;
                else if (!string.IsNullOrWhiteSpace(input)) state.Fields["updateSupplierEmail"] = input;
            }
            if (!state.Fields.ContainsKey("updateSupplierEmail")) return Ask(state, emailPrompt);
        }

        const string addressPrompt = "Address? (optional, say Next to keep current)";
        if (!state.Fields.ContainsKey("updateSupplierAddress"))
        {
            if (string.Equals(state.LastQuestion, addressPrompt, StringComparison.OrdinalIgnoreCase))
            {
                if (isNext) state.Fields["updateSupplierAddress"] = supplier.Address ?? string.Empty;
                else if (!string.IsNullOrWhiteSpace(input)) state.Fields["updateSupplierAddress"] = input;
            }
            if (!state.Fields.ContainsKey("updateSupplierAddress")) return Ask(state, addressPrompt);
        }

        const string paymentPrompt = "Payment terms? (optional, say Next to keep current)";
        if (!state.Fields.ContainsKey("updateSupplierPaymentTerms"))
        {
            if (string.Equals(state.LastQuestion, paymentPrompt, StringComparison.OrdinalIgnoreCase))
            {
                if (isNext) state.Fields["updateSupplierPaymentTerms"] = supplier.PaymentTerms ?? string.Empty;
                else if (!string.IsNullOrWhiteSpace(input)) state.Fields["updateSupplierPaymentTerms"] = input;
            }
            if (!state.Fields.ContainsKey("updateSupplierPaymentTerms")) return Ask(state, paymentPrompt);
        }

        const string creditPrompt = "Credit limit? (optional, say Next to keep current)";
        if (!state.Fields.ContainsKey("updateSupplierCreditLimit"))
        {
            if (string.Equals(state.LastQuestion, creditPrompt, StringComparison.OrdinalIgnoreCase))
            {
                if (isNext) state.Fields["updateSupplierCreditLimit"] = supplier.CreditLimit?.ToString() ?? string.Empty;
                else if (!string.IsNullOrWhiteSpace(input)) state.Fields["updateSupplierCreditLimit"] = input;
            }
            if (!state.Fields.ContainsKey("updateSupplierCreditLimit")) return Ask(state, creditPrompt);
        }

        var code = state.Fields.GetValueOrDefault("updateSupplierCode")?.Trim();
        var contact = state.Fields.GetValueOrDefault("updateSupplierContact")?.Trim();
        var phone = state.Fields.GetValueOrDefault("updateSupplierPhone")?.Trim();
        var email = state.Fields.GetValueOrDefault("updateSupplierEmail")?.Trim();
        var address = state.Fields.GetValueOrDefault("updateSupplierAddress")?.Trim();
        var paymentTerms = state.Fields.GetValueOrDefault("updateSupplierPaymentTerms")?.Trim();
        decimal? creditLimit = null;
        var creditRaw = state.Fields.GetValueOrDefault("updateSupplierCreditLimit");
        if (!string.IsNullOrWhiteSpace(creditRaw))
        {
            if (!TryParseDecimal(creditRaw, out var parsedCredit) || parsedCredit < 0)
            {
                state.Fields.Remove("updateSupplierCreditLimit");
                return Ask(state, "Credit limit must be a valid non-negative number, or say Next to keep current.");
            }
            creditLimit = parsedCredit;
        }

        if (!confirmed)
        {
            state.AwaitingConfirmation = true;
            return Ask(state, $"Confirm Update Supplier: {supplier.Name} -> {newSupplierName}, code {code ?? "-"}, contact {contact ?? "-"}, phone {phone ?? "-"}, email {email ?? "-"}, address {address ?? "-"}, payment terms {paymentTerms ?? "-"}, credit limit {creditLimit?.ToString() ?? "-"}. Say Done to confirm or Cancel.");
        }

        if (!_currentUser.HasPermission("Suppliers:Update"))
            return Result<CopilotExecuteResult>.Failure("You do not have permission: Suppliers:Update");

        var update = await _mediator.Send(new UpdateSupplierCommand
        {
            Id = supplier.Id,
            Name = newSupplierName,
            Code = string.IsNullOrWhiteSpace(code) ? null : code,
            ContactPerson = string.IsNullOrWhiteSpace(contact) ? null : contact,
            Email = string.IsNullOrWhiteSpace(email) ? null : email,
            PhoneNumber = string.IsNullOrWhiteSpace(phone) ? null : phone,
            Address = string.IsNullOrWhiteSpace(address) ? null : address,
            City = supplier.City,
            State = supplier.State,
            Country = supplier.Country,
            PostalCode = supplier.PostalCode,
            TaxId = supplier.TaxId,
            PaymentTerms = string.IsNullOrWhiteSpace(paymentTerms) ? null : paymentTerms,
            CreditLimit = creditLimit
        }, cancellationToken);

        if (!update.Succeeded || update.Data == null)
            return Result<CopilotExecuteResult>.Failure(update.Errors);

        var reset = new CopilotConversationState { SessionId = state.SessionId };
        return Result<CopilotExecuteResult>.Success(new CopilotExecuteResult
        {
            Success = true,
            Action = "UpdateSupplier",
            Completed = true,
            Reply = $"Supplier updated successfully to {update.Data.Name}.",
            Result = new { supplierId = update.Data.Id, supplierName = update.Data.Name },
            SessionState = reset
        });
    }

    private async Task<Result<CopilotExecuteResult>> HandleSupplierDelete(
        Guid companyId,
        CopilotConversationState state,
        string input,
        bool confirmed,
        CancellationToken cancellationToken)
    {
        if (!TryGetField(state, "supplierName", out var supplierName))
        {
            if (!string.IsNullOrWhiteSpace(input)) state.Fields["supplierName"] = input;
            if (!TryGetField(state, "supplierName", out supplierName))
                return Ask(state, "Which supplier do you want to delete?");
        }

        var supplier = await _context.Suppliers.AsNoTracking()
            .FirstOrDefaultAsync(x => x.CompanyId == companyId && !x.IsDeleted && x.Name.Contains(supplierName), cancellationToken);
        if (supplier == null)
        {
            return await AskWithSuggestion(
                state,
                supplierName,
                "supplierName",
                "supplier name",
                _context.Suppliers.AsNoTracking().Where(x => x.CompanyId == companyId && !x.IsDeleted).Select(x => x.Name),
                $"Supplier '{supplierName}' not found. Please provide a valid supplier.",
                cancellationToken);
        }

        if (!confirmed)
        {
            state.AwaitingConfirmation = true;
            return Ask(state, $"Confirm Delete Supplier: {supplier.Name}. Say Done to confirm or Cancel.");
        }

        if (!_currentUser.HasPermission("Suppliers:Delete"))
            return Result<CopilotExecuteResult>.Failure("You do not have permission: Suppliers:Delete");

        var result = await _mediator.Send(new DeleteSupplierCommand { Id = supplier.Id }, cancellationToken);
        if (!result.Succeeded)
            return Result<CopilotExecuteResult>.Failure(result.Errors);

        var reset = new CopilotConversationState { SessionId = state.SessionId };
        return Result<CopilotExecuteResult>.Success(new CopilotExecuteResult
        {
            Success = true,
            Action = "DeleteSupplier",
            Completed = true,
            Reply = $"Supplier {supplier.Name} deleted successfully.",
            SessionState = reset
        });
    }

    private async Task<Result<CopilotExecuteResult>> HandleWarehouseCreate(
        Guid companyId,
        CopilotConversationState state,
        string input,
        bool confirmed,
        CancellationToken cancellationToken)
    {
        var isNext = string.Equals(input, "next", StringComparison.OrdinalIgnoreCase);

        if (!TryGetField(state, "warehouseName", out var warehouseName))
        {
            if (!string.IsNullOrWhiteSpace(input)) state.Fields["warehouseName"] = input;
            if (!TryGetField(state, "warehouseName", out warehouseName))
                return Ask(state, "What is the warehouse name?");
        }

        const string codePrompt = "Warehouse code? (optional, say Next to skip)";
        if (!state.Fields.ContainsKey("warehouseCode"))
        {
            if (string.Equals(state.LastQuestion, codePrompt, StringComparison.OrdinalIgnoreCase))
            {
                if (isNext) state.Fields["warehouseCode"] = string.Empty;
                else if (!string.IsNullOrWhiteSpace(input)) state.Fields["warehouseCode"] = input;
            }
            if (!state.Fields.ContainsKey("warehouseCode")) return Ask(state, codePrompt);
        }

        const string addressPrompt = "Warehouse address? (optional, say Next to skip)";
        if (!state.Fields.ContainsKey("warehouseAddress"))
        {
            if (string.Equals(state.LastQuestion, addressPrompt, StringComparison.OrdinalIgnoreCase))
            {
                if (isNext) state.Fields["warehouseAddress"] = string.Empty;
                else if (!string.IsNullOrWhiteSpace(input)) state.Fields["warehouseAddress"] = input;
            }
            if (!state.Fields.ContainsKey("warehouseAddress")) return Ask(state, addressPrompt);
        }

        const string cityPrompt = "Warehouse city? (optional, say Next to skip)";
        if (!state.Fields.ContainsKey("warehouseCity"))
        {
            if (string.Equals(state.LastQuestion, cityPrompt, StringComparison.OrdinalIgnoreCase))
            {
                if (isNext) state.Fields["warehouseCity"] = string.Empty;
                else if (!string.IsNullOrWhiteSpace(input)) state.Fields["warehouseCity"] = input;
            }
            if (!state.Fields.ContainsKey("warehouseCity")) return Ask(state, cityPrompt);
        }

        const string defaultPrompt = "Set as default warehouse? (optional, say Yes, No, or Next to skip)";
        if (!state.Fields.ContainsKey("warehouseIsDefault"))
        {
            if (string.Equals(state.LastQuestion, defaultPrompt, StringComparison.OrdinalIgnoreCase))
            {
                if (isNext) state.Fields["warehouseIsDefault"] = string.Empty;
                else if (IsAffirmativeInput(input)) state.Fields["warehouseIsDefault"] = "true";
                else if (IsNegativeInput(input)) state.Fields["warehouseIsDefault"] = "false";
                else if (!string.IsNullOrWhiteSpace(input))
                    return Ask(state, "Please answer Yes or No, or say Next to skip.");
            }
            if (!state.Fields.ContainsKey("warehouseIsDefault")) return Ask(state, defaultPrompt);
        }

        var code = state.Fields.GetValueOrDefault("warehouseCode")?.Trim();
        var address = state.Fields.GetValueOrDefault("warehouseAddress")?.Trim();
        var city = state.Fields.GetValueOrDefault("warehouseCity")?.Trim();
        var isDefaultRaw = state.Fields.GetValueOrDefault("warehouseIsDefault");
        var isDefault = string.Equals(isDefaultRaw, "true", StringComparison.OrdinalIgnoreCase);

        if (!confirmed)
        {
            state.AwaitingConfirmation = true;
            return Ask(state, $"Confirm Create Warehouse: {warehouseName}, code {code ?? "-"}, address {address ?? "-"}, city {city ?? "-"}, default {(string.IsNullOrWhiteSpace(isDefaultRaw) ? "-" : (isDefault ? "Yes" : "No"))}. Say Done to confirm or Cancel.");
        }

        if (!_currentUser.HasPermission("Warehouses:Create"))
            return Result<CopilotExecuteResult>.Failure("You do not have permission: Warehouses:Create");

        var result = await _mediator.Send(new CreateWarehouseCommand
        {
            Name = warehouseName,
            Code = string.IsNullOrWhiteSpace(code) ? null : code,
            Address = string.IsNullOrWhiteSpace(address) ? null : address,
            City = string.IsNullOrWhiteSpace(city) ? null : city,
            IsDefault = isDefault
        }, cancellationToken);

        if (!result.Succeeded || result.Data == null)
            return Result<CopilotExecuteResult>.Failure(result.Errors);

        var reset = new CopilotConversationState { SessionId = state.SessionId };
        return Result<CopilotExecuteResult>.Success(new CopilotExecuteResult
        {
            Success = true,
            Action = "CreateWarehouse",
            Completed = true,
            Reply = $"Warehouse {result.Data.Name} created successfully.",
            Result = new { warehouseId = result.Data.Id, warehouseName = result.Data.Name },
            SessionState = reset
        });
    }

    private async Task<Result<CopilotExecuteResult>> HandleWarehouseUpdate(
        Guid companyId,
        CopilotConversationState state,
        string input,
        bool confirmed,
        CancellationToken cancellationToken)
    {
        var isNext = string.Equals(input, "next", StringComparison.OrdinalIgnoreCase);

        if (!TryGetField(state, "warehouseName", out var warehouseName))
        {
            if (!string.IsNullOrWhiteSpace(input)) state.Fields["warehouseName"] = input;
            if (!TryGetField(state, "warehouseName", out warehouseName))
                return Ask(state, "Which warehouse do you want to update?");
        }

        var warehouse = await _context.Warehouses.AsNoTracking()
            .FirstOrDefaultAsync(x => x.CompanyId == companyId && !x.IsDeleted && x.Name.Contains(warehouseName), cancellationToken);
        if (warehouse == null)
        {
            return await AskWithSuggestion(
                state,
                warehouseName,
                "warehouseName",
                "warehouse name",
                _context.Warehouses.AsNoTracking().Where(x => x.CompanyId == companyId && !x.IsDeleted).Select(x => x.Name),
                $"Warehouse '{warehouseName}' not found. Please provide a valid warehouse name.",
                cancellationToken);
        }

        if (!TryGetField(state, "newWarehouseName", out var newWarehouseName))
        {
            if (!string.IsNullOrWhiteSpace(input) && !string.Equals(input, warehouseName, StringComparison.OrdinalIgnoreCase))
                state.Fields["newWarehouseName"] = input;
            if (!TryGetField(state, "newWarehouseName", out newWarehouseName))
                return Ask(state, "What is the new warehouse name?");
        }

        const string codePrompt = "New warehouse code? (optional, say Next to keep current)";
        if (!state.Fields.ContainsKey("newWarehouseCode"))
        {
            if (string.Equals(state.LastQuestion, codePrompt, StringComparison.OrdinalIgnoreCase))
            {
                if (isNext) state.Fields["newWarehouseCode"] = string.Empty;
                else if (!string.IsNullOrWhiteSpace(input)) state.Fields["newWarehouseCode"] = input;
            }
            if (!state.Fields.ContainsKey("newWarehouseCode")) return Ask(state, codePrompt);
        }

        const string addressPrompt = "New warehouse address? (optional, say Next to keep current)";
        if (!state.Fields.ContainsKey("newWarehouseAddress"))
        {
            if (string.Equals(state.LastQuestion, addressPrompt, StringComparison.OrdinalIgnoreCase))
            {
                if (isNext) state.Fields["newWarehouseAddress"] = string.Empty;
                else if (!string.IsNullOrWhiteSpace(input)) state.Fields["newWarehouseAddress"] = input;
            }
            if (!state.Fields.ContainsKey("newWarehouseAddress")) return Ask(state, addressPrompt);
        }

        const string cityPrompt = "New warehouse city? (optional, say Next to keep current)";
        if (!state.Fields.ContainsKey("newWarehouseCity"))
        {
            if (string.Equals(state.LastQuestion, cityPrompt, StringComparison.OrdinalIgnoreCase))
            {
                if (isNext) state.Fields["newWarehouseCity"] = string.Empty;
                else if (!string.IsNullOrWhiteSpace(input)) state.Fields["newWarehouseCity"] = input;
            }
            if (!state.Fields.ContainsKey("newWarehouseCity")) return Ask(state, cityPrompt);
        }

        const string defaultPrompt = "Change default warehouse? (optional, say Yes, No, or Next to keep current)";
        if (!state.Fields.ContainsKey("newWarehouseIsDefault"))
        {
            if (string.Equals(state.LastQuestion, defaultPrompt, StringComparison.OrdinalIgnoreCase))
            {
                if (isNext) state.Fields["newWarehouseIsDefault"] = string.Empty;
                else if (IsAffirmativeInput(input)) state.Fields["newWarehouseIsDefault"] = "true";
                else if (IsNegativeInput(input)) state.Fields["newWarehouseIsDefault"] = "false";
                else if (!string.IsNullOrWhiteSpace(input))
                    return Ask(state, "Please answer Yes or No, or say Next to keep current.");
            }
            if (!state.Fields.ContainsKey("newWarehouseIsDefault")) return Ask(state, defaultPrompt);
        }

        var code = state.Fields.GetValueOrDefault("newWarehouseCode");
        var address = state.Fields.GetValueOrDefault("newWarehouseAddress");
        var city = state.Fields.GetValueOrDefault("newWarehouseCity");
        var defaultRaw = state.Fields.GetValueOrDefault("newWarehouseIsDefault");

        var effectiveCode = string.IsNullOrWhiteSpace(code) ? warehouse.Code : code;
        var effectiveAddress = string.IsNullOrWhiteSpace(address) ? warehouse.Address : address;
        var effectiveCity = string.IsNullOrWhiteSpace(city) ? warehouse.City : city;
        var effectiveDefault = string.IsNullOrWhiteSpace(defaultRaw)
            ? warehouse.IsDefault
            : string.Equals(defaultRaw, "true", StringComparison.OrdinalIgnoreCase);

        if (!confirmed)
        {
            state.AwaitingConfirmation = true;
            return Ask(state, $"Confirm Update Warehouse: {warehouse.Name} -> {newWarehouseName}, code {effectiveCode ?? "-"}, address {effectiveAddress ?? "-"}, city {effectiveCity ?? "-"}, default {(effectiveDefault ? "Yes" : "No")}. Say Done to confirm or Cancel.");
        }

        if (!_currentUser.HasPermission("Warehouses:Update"))
            return Result<CopilotExecuteResult>.Failure("You do not have permission: Warehouses:Update");

        var result = await _mediator.Send(new UpdateWarehouseCommand
        {
            Id = warehouse.Id,
            Name = newWarehouseName,
            Code = string.IsNullOrWhiteSpace(effectiveCode) ? null : effectiveCode,
            Description = warehouse.Description,
            Address = string.IsNullOrWhiteSpace(effectiveAddress) ? null : effectiveAddress,
            City = string.IsNullOrWhiteSpace(effectiveCity) ? null : effectiveCity,
            State = warehouse.State,
            Country = warehouse.Country,
            PostalCode = warehouse.PostalCode,
            PhoneNumber = warehouse.PhoneNumber,
            Email = warehouse.Email,
            IsDefault = effectiveDefault
        }, cancellationToken);

        if (!result.Succeeded || result.Data == null)
            return Result<CopilotExecuteResult>.Failure(result.Errors);

        var reset = new CopilotConversationState { SessionId = state.SessionId };
        return Result<CopilotExecuteResult>.Success(new CopilotExecuteResult
        {
            Success = true,
            Action = "UpdateWarehouse",
            Completed = true,
            Reply = $"Warehouse updated successfully to {result.Data.Name}.",
            Result = new { warehouseId = result.Data.Id, warehouseName = result.Data.Name },
            SessionState = reset
        });
    }

    private async Task<Result<CopilotExecuteResult>> HandleWarehouseDelete(
        Guid companyId,
        CopilotConversationState state,
        string input,
        bool confirmed,
        CancellationToken cancellationToken)
    {
        if (!TryGetField(state, "warehouseName", out var warehouseName))
        {
            if (!string.IsNullOrWhiteSpace(input)) state.Fields["warehouseName"] = input;
            if (!TryGetField(state, "warehouseName", out warehouseName))
                return Ask(state, "Which warehouse do you want to delete?");
        }

        var warehouse = await _context.Warehouses.AsNoTracking()
            .FirstOrDefaultAsync(x => x.CompanyId == companyId && !x.IsDeleted && x.Name.Contains(warehouseName), cancellationToken);
        if (warehouse == null)
        {
            return await AskWithSuggestion(
                state,
                warehouseName,
                "warehouseName",
                "warehouse name",
                _context.Warehouses.AsNoTracking().Where(x => x.CompanyId == companyId && !x.IsDeleted).Select(x => x.Name),
                $"Warehouse '{warehouseName}' not found. Please provide a valid warehouse name.",
                cancellationToken);
        }

        if (!confirmed)
        {
            state.AwaitingConfirmation = true;
            return Ask(state, $"Confirm Delete Warehouse: {warehouse.Name}. Say Done to confirm or Cancel.");
        }

        if (!_currentUser.HasPermission("Warehouses:Delete"))
            return Result<CopilotExecuteResult>.Failure("You do not have permission: Warehouses:Delete");

        var result = await _mediator.Send(new DeleteWarehouseCommand { Id = warehouse.Id }, cancellationToken);
        if (!result.Succeeded)
            return Result<CopilotExecuteResult>.Failure(result.Errors);

        var reset = new CopilotConversationState { SessionId = state.SessionId };
        return Result<CopilotExecuteResult>.Success(new CopilotExecuteResult
        {
            Success = true,
            Action = "DeleteWarehouse",
            Completed = true,
            Reply = $"Warehouse {warehouse.Name} deleted successfully.",
            SessionState = reset
        });
    }

    private async Task<Result<CopilotExecuteResult>> HandleProductCreate(
        Guid companyId,
        CopilotConversationState state,
        string input,
        bool confirmed,
        CancellationToken cancellationToken)
    {
        var isNext = string.Equals(input, "next", StringComparison.OrdinalIgnoreCase);

        if (!TryGetField(state, "productName", out var productName))
        {
            if (!string.IsNullOrWhiteSpace(input)) state.Fields["productName"] = input;
            if (!TryGetField(state, "productName", out productName))
                return Ask(state, "What is the product name?");
        }

        if (!string.IsNullOrWhiteSpace(input))
        {
            if (!state.Fields.ContainsKey("sku") && !string.Equals(state.LastQuestion, "What is the product name?", StringComparison.OrdinalIgnoreCase))
            {
                state.Fields["sku"] = input;
            }
            else if (state.Fields.ContainsKey("sku") && !state.Fields.ContainsKey("unitName") && !TryParseDecimal(input, out _))
            {
                state.Fields["unitName"] = input;
            }
            else if (state.Fields.ContainsKey("unitName") && !state.Fields.ContainsKey("purchasePrice") && TryParseDecimal(input, out var parsedPurchase))
            {
                state.Fields["purchasePrice"] = parsedPurchase.ToString();
            }
            else if (state.Fields.ContainsKey("purchasePrice") && !state.Fields.ContainsKey("salePrice") && TryParseDecimal(input, out var parsedSale))
            {
                state.Fields["salePrice"] = parsedSale.ToString();
            }
            else if (state.Fields.ContainsKey("salePrice") && !state.Fields.ContainsKey("categoryName") && !isNext && !TryParseDecimal(input, out _)
                && !string.Equals(state.LastQuestion, "What is the sale price?", StringComparison.OrdinalIgnoreCase))
            {
                state.Fields["categoryName"] = input;
            }
            else if (state.Fields.ContainsKey("categoryName") && !state.Fields.ContainsKey("brandName") && !isNext && !TryParseDecimal(input, out _)
                && !string.Equals(state.LastQuestion, "Category? (optional, say Next to skip)", StringComparison.OrdinalIgnoreCase))
            {
                state.Fields["brandName"] = input;
            }
        }

        if (!TryGetField(state, "sku", out var sku))
            return Ask(state, "What is the SKU?");

        if (!TryGetField(state, "unitName", out var unitName))
            return Ask(state, "What is the unit of measure? (e.g., Piece, Box)");

        if (!TryGetField(state, "purchasePrice", out var purchasePriceText) || !decimal.TryParse(purchasePriceText, out var purchasePrice))
            return Ask(state, "What is the purchase price?");

        if (!TryGetField(state, "salePrice", out var salePriceText) || !decimal.TryParse(salePriceText, out var salePrice))
            return Ask(state, "What is the sale price?");

        var normalizedUnitName = NormalizeUnitInput(unitName);
        var unit = await _context.UnitOfMeasures.AsNoTracking()
            .FirstOrDefaultAsync(x => x.CompanyId == companyId && !x.IsDeleted &&
                (x.Name.Contains(unitName) || x.Code.Contains(unitName) ||
                 x.Name.Contains(normalizedUnitName) || x.Code.Contains(normalizedUnitName)), cancellationToken);
        if (unit == null)
        {
            return await AskWithSuggestion(
                state,
                unitName,
                "unitName",
                "unit name",
                _context.UnitOfMeasures.AsNoTracking().Where(x => x.CompanyId == companyId && !x.IsDeleted).Select(x => x.Name),
                $"Unit '{unitName}' not found. Please provide a valid unit name/code.",
                cancellationToken);
        }

        if (!state.Fields.ContainsKey("categoryName"))
        {
            if (isNext && string.Equals(state.LastQuestion, "Category? (optional, say Next to skip)", StringComparison.OrdinalIgnoreCase))
                state.Fields["categoryName"] = string.Empty;
            else
                return Ask(state, "Category? (optional, say Next to skip)");
        }

        Guid? categoryId = null;
        string categoryLabel = "-";
        if (TryGetField(state, "categoryName", out var categoryName))
        {
            var category = await _context.Categories.AsNoTracking()
                .FirstOrDefaultAsync(x => x.CompanyId == companyId && !x.IsDeleted && x.Name.Contains(categoryName), cancellationToken);
            if (category == null)
            {
                return await AskWithSuggestion(
                    state,
                    categoryName,
                    "categoryName",
                    "category name",
                    _context.Categories.AsNoTracking().Where(x => x.CompanyId == companyId && !x.IsDeleted).Select(x => x.Name),
                    $"Category '{categoryName}' not found. Please provide a valid category or say Next to skip.",
                    cancellationToken);
            }

            categoryId = category.Id;
            categoryLabel = category.Name;
        }

        if (!state.Fields.ContainsKey("brandName"))
        {
            if (isNext && string.Equals(state.LastQuestion, "Brand? (optional, say Next to skip)", StringComparison.OrdinalIgnoreCase))
                state.Fields["brandName"] = string.Empty;
            else
                return Ask(state, "Brand? (optional, say Next to skip)");
        }

        Guid? brandId = null;
        string brandLabel = "-";
        if (TryGetField(state, "brandName", out var brandName))
        {
            var brand = await _context.Brands.AsNoTracking()
                .FirstOrDefaultAsync(x => x.CompanyId == companyId && !x.IsDeleted && x.Name.Contains(brandName), cancellationToken);
            if (brand == null)
            {
                return await AskWithSuggestion(
                    state,
                    brandName,
                    "brandName",
                    "brand name",
                    _context.Brands.AsNoTracking().Where(x => x.CompanyId == companyId && !x.IsDeleted).Select(x => x.Name),
                    $"Brand '{brandName}' not found. Please provide a valid brand or say Next to skip.",
                    cancellationToken);
            }

            brandId = brand.Id;
            brandLabel = brand.Name;
        }

        if (!confirmed)
        {
            state.AwaitingConfirmation = true;
            return Ask(state, $"Confirm Create Product: {productName}, SKU {sku}, purchase {purchasePrice}, sale {salePrice}, unit {unit.Name}, category {categoryLabel}, brand {brandLabel}. Say Done to confirm or Cancel.");
        }

        if (!_currentUser.HasPermission("Products:Create"))
            return Result<CopilotExecuteResult>.Failure("You do not have permission: Products:Create");

        var result = await _mediator.Send(new CreateProductCommand
        {
            Name = productName,
            SKU = sku,
            UnitOfMeasureId = unit.Id,
            CategoryId = categoryId,
            BrandId = brandId,
            PurchasePrice = purchasePrice,
            SalePrice = salePrice
        }, cancellationToken);

        if (!result.Succeeded || result.Data == null)
            return Result<CopilotExecuteResult>.Failure(result.Errors);

        var reset = new CopilotConversationState { SessionId = state.SessionId };
        return Result<CopilotExecuteResult>.Success(new CopilotExecuteResult
        {
            Success = true,
            Action = "CreateProduct",
            Completed = true,
            Reply = $"Product {result.Data.Name} created successfully.",
            Result = new { productId = result.Data.Id, productName = result.Data.Name },
            SessionState = reset
        });
    }

    private async Task<Result<CopilotExecuteResult>> HandleProductUpdate(
        Guid companyId,
        CopilotConversationState state,
        string input,
        bool confirmed,
        CancellationToken cancellationToken)
    {
        if (!TryGetField(state, "productName", out var productName))
        {
            if (!string.IsNullOrWhiteSpace(input)) state.Fields["productName"] = input;
            if (!TryGetField(state, "productName", out productName))
                return Ask(state, "Which product do you want to update?");
        }

        var product = await _context.Products.AsNoTracking()
            .FirstOrDefaultAsync(x => x.CompanyId == companyId && !x.IsDeleted && x.Name.Contains(productName), cancellationToken);
        if (product == null)
        {
            return await AskWithSuggestion(
                state,
                productName,
                "productName",
                "product name",
                _context.Products.AsNoTracking().Where(x => x.CompanyId == companyId && !x.IsDeleted).Select(x => x.Name),
                $"Product '{productName}' not found. Please provide a valid product.",
                cancellationToken);
        }

        if (!TryGetField(state, "newProductName", out var newProductName))
        {
            if (!string.IsNullOrWhiteSpace(input) && !string.Equals(input, productName, StringComparison.OrdinalIgnoreCase))
                state.Fields["newProductName"] = input;
            if (!TryGetField(state, "newProductName", out newProductName))
                return Ask(state, "What is the new product name?");
        }

        if (!confirmed)
        {
            state.AwaitingConfirmation = true;
            return Ask(state, $"Confirm Update Product: {product.Name} -> {newProductName}. Say Done to confirm or Cancel.");
        }

        if (!_currentUser.HasPermission("Products:Update"))
            return Result<CopilotExecuteResult>.Failure("You do not have permission: Products:Update");

        var result = await _mediator.Send(new UpdateProductCommand
        {
            Id = product.Id,
            Name = newProductName,
            Description = product.Description,
            SKU = product.SKU,
            Barcode = product.Barcode,
            CategoryId = product.CategoryId,
            BrandId = product.BrandId,
            UnitOfMeasureId = product.UnitOfMeasureId,
            PurchasePrice = product.PurchasePrice,
            SalePrice = product.SalePrice,
            CostPrice = product.CostPrice,
            MinStockLevel = product.MinStockLevel,
            MaxStockLevel = product.MaxStockLevel,
            ReorderPoint = product.ReorderPoint,
            TrackQuantity = product.TrackQuantity,
            TrackBatch = product.TrackBatch,
            TrackExpiry = product.TrackExpiry,
            ImageUrl = product.ImageUrl,
            Weight = product.Weight,
            WeightUnit = product.WeightUnit,
            Length = product.Length,
            Width = product.Width,
            Height = product.Height,
            DimensionsUnit = product.DimensionsUnit
        }, cancellationToken);

        if (!result.Succeeded || result.Data == null)
            return Result<CopilotExecuteResult>.Failure(result.Errors);

        var reset = new CopilotConversationState { SessionId = state.SessionId };
        return Result<CopilotExecuteResult>.Success(new CopilotExecuteResult
        {
            Success = true,
            Action = "UpdateProduct",
            Completed = true,
            Reply = $"Product updated successfully to {result.Data.Name}.",
            Result = new { productId = result.Data.Id, productName = result.Data.Name },
            SessionState = reset
        });
    }

    private async Task<Result<CopilotExecuteResult>> HandleProductDelete(
        Guid companyId,
        CopilotConversationState state,
        string input,
        bool confirmed,
        CancellationToken cancellationToken)
    {
        if (!TryGetField(state, "productName", out var productName))
        {
            if (!string.IsNullOrWhiteSpace(input)) state.Fields["productName"] = input;
            if (!TryGetField(state, "productName", out productName))
                return Ask(state, "Which product do you want to delete?");
        }

        var product = await _context.Products.AsNoTracking()
            .FirstOrDefaultAsync(x => x.CompanyId == companyId && !x.IsDeleted && x.Name.Contains(productName), cancellationToken);
        if (product == null)
        {
            return await AskWithSuggestion(
                state,
                productName,
                "productName",
                "product name",
                _context.Products.AsNoTracking().Where(x => x.CompanyId == companyId && !x.IsDeleted).Select(x => x.Name),
                $"Product '{productName}' not found. Please provide a valid product.",
                cancellationToken);
        }

        if (!confirmed)
        {
            state.AwaitingConfirmation = true;
            return Ask(state, $"Confirm Delete Product: {product.Name}. Say Done to confirm or Cancel.");
        }

        if (!_currentUser.HasPermission("Products:Delete"))
            return Result<CopilotExecuteResult>.Failure("You do not have permission: Products:Delete");

        var result = await _mediator.Send(new DeleteProductCommand { Id = product.Id }, cancellationToken);
        if (!result.Succeeded)
            return Result<CopilotExecuteResult>.Failure(result.Errors);

        var reset = new CopilotConversationState { SessionId = state.SessionId };
        return Result<CopilotExecuteResult>.Success(new CopilotExecuteResult
        {
            Success = true,
            Action = "DeleteProduct",
            Completed = true,
            Reply = $"Product {product.Name} deleted successfully.",
            SessionState = reset
        });
    }

    private async Task<Result<CopilotExecuteResult>> HandleGenerateInvoiceDocument(
        Guid companyId,
        CopilotConversationState state,
        string input,
        bool confirmed,
        CancellationToken cancellationToken)
    {
        SalesOrder? salesOrder = null;
        if (ContainsAny(input, "last sale", "latest sale", "last sales order", "latest sales order", "previous sale"))
        {
            salesOrder = await _context.SalesOrders.AsNoTracking()
                .Where(x => x.CompanyId == companyId && !x.IsDeleted)
                .OrderByDescending(x => x.OrderDate)
                .ThenByDescending(x => x.CreatedAt)
                .FirstOrDefaultAsync(cancellationToken);

            if (salesOrder == null)
                return Result<CopilotExecuteResult>.Failure("No sales orders found to generate invoice.");

            state.Fields["salesOrderRef"] = salesOrder.OrderNumber;
        }

        if (!TryGetField(state, "salesOrderRef", out var orderRef))
        {
            if (!string.IsNullOrWhiteSpace(input)) state.Fields["salesOrderRef"] = input;
            if (!TryGetField(state, "salesOrderRef", out orderRef))
                return Ask(state, "Which sales order number should I generate invoice for?");
        }

        if (salesOrder == null)
        {
            salesOrder = await _context.SalesOrders.AsNoTracking()
                .FirstOrDefaultAsync(x => x.CompanyId == companyId && !x.IsDeleted && x.OrderNumber.Contains(orderRef), cancellationToken);
        }
        if (salesOrder == null)
        {
            state.Fields.Remove("salesOrderRef");
            return Ask(state, $"Sales order '{orderRef}' not found. Please provide a valid order number.");
        }

        if (!confirmed)
        {
            state.AwaitingConfirmation = true;
            return Ask(state, $"Confirm Generate Invoice for sales order {salesOrder.OrderNumber}. Say Done to confirm or Cancel.");
        }

        var result = await _mediator.Send(new GenerateInvoicePdfCommand { SalesOrderId = salesOrder.Id }, cancellationToken);
        if (!result.Succeeded || result.Data == null)
            return Result<CopilotExecuteResult>.Failure(result.Errors);

        var reset = new CopilotConversationState { SessionId = state.SessionId };
        reset.Fields["lastDownloadUrl"] = $"/api/documents/invoice/{salesOrder.Id}";
        reset.Fields["lastDocumentName"] = $"Invoice {salesOrder.OrderNumber}";
        return Result<CopilotExecuteResult>.Success(new CopilotExecuteResult
        {
            Success = true,
            Action = "GenerateInvoice",
            Completed = true,
            Reply = $"Invoice generated for order {salesOrder.OrderNumber}. Download: /api/documents/invoice/{salesOrder.Id}",
            Result = new
            {
                salesOrderId = salesOrder.Id,
                salesOrderNumber = salesOrder.OrderNumber,
                bytes = result.Data.Length,
                downloadUrl = $"/api/documents/invoice/{salesOrder.Id}"
            },
            SessionState = reset
        });
    }

    private async Task<Result<CopilotExecuteResult>> HandleGeneratePurchaseOrderDocument(
        Guid companyId,
        CopilotConversationState state,
        string input,
        bool confirmed,
        CancellationToken cancellationToken)
    {
        if (!TryGetField(state, "purchaseOrderRef", out var orderRef))
        {
            if (!string.IsNullOrWhiteSpace(input)) state.Fields["purchaseOrderRef"] = input;
            if (!TryGetField(state, "purchaseOrderRef", out orderRef))
                return Ask(state, "Which purchase order number should I generate PDF for?");
        }

        var purchaseOrder = await _context.PurchaseOrders.AsNoTracking()
            .FirstOrDefaultAsync(x => x.CompanyId == companyId && !x.IsDeleted && x.OrderNumber.Contains(orderRef), cancellationToken);
        if (purchaseOrder == null)
        {
            state.Fields.Remove("purchaseOrderRef");
            return Ask(state, $"Purchase order '{orderRef}' not found. Please provide a valid order number.");
        }

        if (!confirmed)
        {
            state.AwaitingConfirmation = true;
            return Ask(state, $"Confirm Generate Purchase Order PDF for {purchaseOrder.OrderNumber}. Say Done to confirm or Cancel.");
        }

        var result = await _mediator.Send(new GeneratePurchaseOrderPdfCommand { PurchaseOrderId = purchaseOrder.Id }, cancellationToken);
        if (!result.Succeeded || result.Data == null)
            return Result<CopilotExecuteResult>.Failure(result.Errors);

        var reset = new CopilotConversationState { SessionId = state.SessionId };
        reset.Fields["lastDownloadUrl"] = $"/api/documents/purchase-order/{purchaseOrder.Id}";
        reset.Fields["lastDocumentName"] = $"Purchase Order {purchaseOrder.OrderNumber}";
        return Result<CopilotExecuteResult>.Success(new CopilotExecuteResult
        {
            Success = true,
            Action = "GeneratePurchaseOrderPdf",
            Completed = true,
            Reply = $"Purchase order PDF generated for {purchaseOrder.OrderNumber}. Download: /api/documents/purchase-order/{purchaseOrder.Id}",
            Result = new
            {
                purchaseOrderId = purchaseOrder.Id,
                purchaseOrderNumber = purchaseOrder.OrderNumber,
                bytes = result.Data.Length,
                downloadUrl = $"/api/documents/purchase-order/{purchaseOrder.Id}"
            },
            SessionState = reset
        });
    }

    private async Task<Result<CopilotExecuteResult>> HandleSalesReport(
        Guid companyId,
        CopilotConversationState state,
        string input,
        bool confirmed,
        CancellationToken cancellationToken)
    {
        if (TryCaptureDateRangeFromInput(state, input))
        {
            // Date range captured from a single user sentence.
        }

        if (!TryGetField(state, "reportFromDate", out var fromText))
        {
            state.AwaitingConfirmation = false;
            return Ask(state, "Please provide From Date for Sales Report (example: 2026-02-01).");
        }

        if (!TryGetField(state, "reportToDate", out var toText))
        {
            if (!TryGetField(state, "reportToDate", out toText))
            {
                state.AwaitingConfirmation = false;
                return Ask(state, "Please provide To Date for Sales Report (example: 2026-02-29).");
            }
        }

        if (!TryParseIsoDate(fromText, out var fromDate))
        {
            state.Fields.Remove("reportFromDate");
            state.AwaitingConfirmation = false;
            return Ask(state, "I could not read From Date. Please enter it as YYYY-MM-DD.");
        }

        if (!TryParseIsoDate(toText, out var toDate))
        {
            state.Fields.Remove("reportToDate");
            state.AwaitingConfirmation = false;
            return Ask(state, "I could not read To Date. Please enter it as YYYY-MM-DD.");
        }

        if (toDate < fromDate)
        {
            state.Fields.Remove("reportToDate");
            state.AwaitingConfirmation = false;
            return Ask(state, "To Date cannot be earlier than From Date. Please provide a valid To Date.");
        }

        if (!confirmed)
        {
            state.AwaitingConfirmation = true;
            return Ask(state, $"Confirm Sales Report from {fromDate:yyyy-MM-dd} to {toDate:yyyy-MM-dd}. Say Done to confirm or Cancel.");
        }

        state.AwaitingConfirmation = false;
        return await ExecuteSalesReportSummary(companyId, fromDate, toDate, state, cancellationToken);
    }

    private async Task<Result<CopilotExecuteResult>> HandlePurchaseReport(
        Guid companyId,
        CopilotConversationState state,
        string input,
        bool confirmed,
        CancellationToken cancellationToken)
    {
        if (TryCaptureDateRangeFromInput(state, input))
        {
            // Date range captured from a single user sentence.
        }

        if (!TryGetField(state, "reportFromDate", out var fromText))
        {
            state.AwaitingConfirmation = false;
            return Ask(state, "Please provide From Date for Purchase Report (example: 2026-02-01).");
        }

        if (!TryGetField(state, "reportToDate", out var toText))
        {
            if (!TryGetField(state, "reportToDate", out toText))
            {
                state.AwaitingConfirmation = false;
                return Ask(state, "Please provide To Date for Purchase Report (example: 2026-02-29).");
            }
        }

        if (!TryParseIsoDate(fromText, out var fromDate))
        {
            state.Fields.Remove("reportFromDate");
            state.AwaitingConfirmation = false;
            return Ask(state, "I could not read From Date. Please enter it as YYYY-MM-DD.");
        }

        if (!TryParseIsoDate(toText, out var toDate))
        {
            state.Fields.Remove("reportToDate");
            state.AwaitingConfirmation = false;
            return Ask(state, "I could not read To Date. Please enter it as YYYY-MM-DD.");
        }

        if (toDate < fromDate)
        {
            state.Fields.Remove("reportToDate");
            state.AwaitingConfirmation = false;
            return Ask(state, "To Date cannot be earlier than From Date. Please provide a valid To Date.");
        }

        if (!confirmed)
        {
            state.AwaitingConfirmation = true;
            return Ask(state, $"Confirm Purchase Report from {fromDate:yyyy-MM-dd} to {toDate:yyyy-MM-dd}. Say Done to confirm or Cancel.");
        }

        state.AwaitingConfirmation = false;
        return await ExecutePurchaseReportSummary(companyId, fromDate, toDate, state, cancellationToken);
    }

    private async Task<Result<CopilotExecuteResult>> HandleInventoryReport(
        Guid companyId,
        CopilotConversationState state,
        string input,
        bool confirmed,
        CancellationToken cancellationToken)
    {
        if (TryCaptureDateRangeFromInput(state, input))
        {
            // Keep dates for user-friendly context and file naming.
        }

        if (!TryGetField(state, "reportFromDate", out var fromText))
        {
            state.AwaitingConfirmation = false;
            return Ask(state, "Please provide From Date for Inventory Report (example: 2026-02-01).");
        }

        if (!TryGetField(state, "reportToDate", out var toText))
        {
            if (!TryGetField(state, "reportToDate", out toText))
            {
                state.AwaitingConfirmation = false;
                return Ask(state, "Please provide To Date for Inventory Report (example: 2026-02-29).");
            }
        }

        if (!TryParseIsoDate(fromText, out var fromDate))
        {
            state.Fields.Remove("reportFromDate");
            state.AwaitingConfirmation = false;
            return Ask(state, "I could not read From Date. Please enter it as YYYY-MM-DD.");
        }

        if (!TryParseIsoDate(toText, out var toDate))
        {
            state.Fields.Remove("reportToDate");
            state.AwaitingConfirmation = false;
            return Ask(state, "I could not read To Date. Please enter it as YYYY-MM-DD.");
        }

        if (toDate < fromDate)
        {
            state.Fields.Remove("reportToDate");
            state.AwaitingConfirmation = false;
            return Ask(state, "To Date cannot be earlier than From Date. Please provide a valid To Date.");
        }

        if (!confirmed)
        {
            state.AwaitingConfirmation = true;
            return Ask(state, $"Confirm Inventory Report from {fromDate:yyyy-MM-dd} to {toDate:yyyy-MM-dd}. Say Done to confirm or Cancel.");
        }

        state.AwaitingConfirmation = false;
        return await ExecuteInventoryReportSummary(companyId, fromDate, toDate, state, cancellationToken);
    }

    private async Task<Result<CopilotExecuteResult>> HandleProfitLossReport(
        Guid companyId,
        CopilotConversationState state,
        string input,
        bool confirmed,
        CancellationToken cancellationToken)
    {
        if (TryCaptureDateRangeFromInput(state, input))
        {
            // Date range captured from a single user sentence.
        }

        if (!TryGetField(state, "reportFromDate", out var fromText))
        {
            state.AwaitingConfirmation = false;
            return Ask(state, "Please provide From Date for Profit & Loss Report (example: 2026-02-01).");
        }

        if (!TryGetField(state, "reportToDate", out var toText))
        {
            if (!TryGetField(state, "reportToDate", out toText))
            {
                state.AwaitingConfirmation = false;
                return Ask(state, "Please provide To Date for Profit & Loss Report (example: 2026-02-29).");
            }
        }

        if (!TryParseIsoDate(fromText, out var fromDate))
        {
            state.Fields.Remove("reportFromDate");
            state.AwaitingConfirmation = false;
            return Ask(state, "I could not read From Date. Please enter it as YYYY-MM-DD.");
        }

        if (!TryParseIsoDate(toText, out var toDate))
        {
            state.Fields.Remove("reportToDate");
            state.AwaitingConfirmation = false;
            return Ask(state, "I could not read To Date. Please enter it as YYYY-MM-DD.");
        }

        if (toDate < fromDate)
        {
            state.Fields.Remove("reportToDate");
            state.AwaitingConfirmation = false;
            return Ask(state, "To Date cannot be earlier than From Date. Please provide a valid To Date.");
        }

        if (!confirmed)
        {
            state.AwaitingConfirmation = true;
            return Ask(state, $"Confirm Profit & Loss Report from {fromDate:yyyy-MM-dd} to {toDate:yyyy-MM-dd}. Say Done to confirm or Cancel.");
        }

        state.AwaitingConfirmation = false;
        return await ExecuteProfitLossSummary(companyId, fromDate, toDate, state, cancellationToken);
    }

    private async Task<Result<CopilotExecuteResult>> HandleBalanceSheetReport(
        Guid companyId,
        CopilotConversationState state,
        string input,
        bool confirmed,
        CancellationToken cancellationToken)
    {
        if (!TryGetField(state, "asOfDate", out var asOfText))
        {
            if (TryExtractSingleDate(input, out var asOfInputDate))
                state.Fields["asOfDate"] = asOfInputDate.ToString("yyyy-MM-dd");
            else if (ContainsAny(input, "today"))
                state.Fields["asOfDate"] = DateTime.UtcNow.Date.ToString("yyyy-MM-dd");
            else if (ContainsAny(input, "yesterday"))
                state.Fields["asOfDate"] = DateTime.UtcNow.Date.AddDays(-1).ToString("yyyy-MM-dd");
        }

        if (!TryGetField(state, "asOfDate", out asOfText) || !TryParseIsoDate(asOfText, out var asOfDate))
        {
            state.Fields.Remove("asOfDate");
            state.AwaitingConfirmation = false;
            return Ask(state, "Please provide As Of Date for Balance Sheet (example: 2026-02-29).");
        }

        if (!confirmed)
        {
            state.AwaitingConfirmation = true;
            return Ask(state, $"Confirm Balance Sheet as of {asOfDate:yyyy-MM-dd}. Say Done to confirm or Cancel.");
        }

        state.AwaitingConfirmation = false;
        return await ExecuteBalanceSheetSummary(companyId, asOfDate, state, cancellationToken);
    }

    private async Task<Result<CopilotExecuteResult>> ExecuteSalesReportSummary(Guid companyId, DateTime fromDate, DateTime toDate, CopilotConversationState state, CancellationToken cancellationToken)
    {
        var requestBody = new { fromDate, toDate, customerId = (Guid?)null };
        var result = await _mediator.Send(new GetSalesReportQuery
        {
            FromDate = fromDate,
            ToDate = toDate
        }, cancellationToken);
        if (!result.Succeeded || result.Data == null)
            return Result<CopilotExecuteResult>.Failure(result.Errors);

        var reset = new CopilotConversationState { SessionId = state.SessionId };
        reset.Fields["lastDownloadMethod"] = "POST";
        reset.Fields["lastDownloadPath"] = "/api/reports/sales/pdf";
        reset.Fields["lastDownloadBody"] = JsonSerializer.Serialize(requestBody);
        reset.Fields["lastDocumentName"] = $"Sales Report {fromDate:yyyy-MM-dd}_{toDate:yyyy-MM-dd}.pdf";

        return Result<CopilotExecuteResult>.Success(new CopilotExecuteResult
        {
            Success = true,
            Action = "SalesReport",
            Completed = true,
            Reply = $"Sales report ({fromDate:yyyy-MM-dd} to {toDate:yyyy-MM-dd}): orders {result.Data.TotalOrders}, sales {result.Data.TotalSales}, profit {result.Data.TotalProfit}. PDF: POST /api/reports/sales/pdf",
            Result = new
            {
                summary = result.Data,
                pdfEndpoint = "/api/reports/sales/pdf",
                pdfRequest = requestBody
            },
            SessionState = reset
        });
    }

    private async Task<Result<CopilotExecuteResult>> ExecutePurchaseReportSummary(Guid companyId, DateTime fromDate, DateTime toDate, CopilotConversationState state, CancellationToken cancellationToken)
    {
        var requestBody = new { fromDate, toDate, supplierId = (Guid?)null };
        var result = await _mediator.Send(new GetPurchaseReportQuery
        {
            FromDate = fromDate,
            ToDate = toDate
        }, cancellationToken);
        if (!result.Succeeded || result.Data == null)
            return Result<CopilotExecuteResult>.Failure(result.Errors);

        var reset = new CopilotConversationState { SessionId = state.SessionId };
        reset.Fields["lastDownloadMethod"] = "POST";
        reset.Fields["lastDownloadPath"] = "/api/reports/purchase/pdf";
        reset.Fields["lastDownloadBody"] = JsonSerializer.Serialize(requestBody);
        reset.Fields["lastDocumentName"] = $"Purchase Report {fromDate:yyyy-MM-dd}_{toDate:yyyy-MM-dd}.pdf";

        return Result<CopilotExecuteResult>.Success(new CopilotExecuteResult
        {
            Success = true,
            Action = "PurchaseReport",
            Completed = true,
            Reply = $"Purchase report ({fromDate:yyyy-MM-dd} to {toDate:yyyy-MM-dd}): orders {result.Data.TotalOrders}, purchases {result.Data.TotalPurchases}. PDF: POST /api/reports/purchase/pdf",
            Result = new
            {
                summary = result.Data,
                pdfEndpoint = "/api/reports/purchase/pdf",
                pdfRequest = requestBody
            },
            SessionState = reset
        });
    }

    private async Task<Result<CopilotExecuteResult>> ExecuteInventoryReportSummary(Guid companyId, DateTime fromDate, DateTime toDate, CopilotConversationState state, CancellationToken cancellationToken)
    {
        var requestBody = new { warehouseId = (Guid?)null, categoryId = (Guid?)null, lowStockOnly = (bool?)null };
        var result = await _mediator.Send(new GetInventoryReportQuery(), cancellationToken);
        if (!result.Succeeded || result.Data == null)
            return Result<CopilotExecuteResult>.Failure(result.Errors);

        var reset = new CopilotConversationState { SessionId = state.SessionId };
        reset.Fields["lastDownloadMethod"] = "POST";
        reset.Fields["lastDownloadPath"] = "/api/reports/inventory/pdf";
        reset.Fields["lastDownloadBody"] = JsonSerializer.Serialize(requestBody);
        reset.Fields["lastDocumentName"] = $"Inventory Report {fromDate:yyyy-MM-dd}_{toDate:yyyy-MM-dd}.pdf";

        return Result<CopilotExecuteResult>.Success(new CopilotExecuteResult
        {
            Success = true,
            Action = "InventoryReport",
            Completed = true,
            Reply = $"Inventory report ({fromDate:yyyy-MM-dd} to {toDate:yyyy-MM-dd}): products {result.Data.TotalProducts}, low stock {result.Data.LowStockItems}, out of stock {result.Data.OutOfStockItems}, stock value {result.Data.TotalStockValue}. PDF: POST /api/reports/inventory/pdf",
            Result = new
            {
                summary = result.Data,
                pdfEndpoint = "/api/reports/inventory/pdf",
                pdfRequest = requestBody
            },
            SessionState = reset
        });
    }

    private async Task<Result<CopilotExecuteResult>> ExecuteProfitLossSummary(Guid companyId, DateTime fromDate, DateTime toDate, CopilotConversationState state, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetProfitLossQuery
        {
            FromDate = fromDate,
            ToDate = toDate
        }, cancellationToken);
        if (!result.Succeeded || result.Data == null)
            return Result<CopilotExecuteResult>.Failure(result.Errors);

        var reset = new CopilotConversationState { SessionId = state.SessionId };
        return Result<CopilotExecuteResult>.Success(new CopilotExecuteResult
        {
            Success = true,
            Action = "ProfitLossReport",
            Completed = true,
            Reply = $"Profit & Loss ({fromDate:yyyy-MM-dd} to {toDate:yyyy-MM-dd}): revenue {result.Data.Revenue}, expenses {result.Data.Expenses}, net profit {result.Data.NetProfit}.",
            Result = result.Data,
            SessionState = reset
        });
    }

    private async Task<Result<CopilotExecuteResult>> ExecuteBalanceSheetSummary(Guid companyId, DateTime asOf, CopilotConversationState state, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetBalanceSheetQuery
        {
            AsOfDate = asOf
        }, cancellationToken);
        if (!result.Succeeded || result.Data == null)
            return Result<CopilotExecuteResult>.Failure(result.Errors);

        var reset = new CopilotConversationState { SessionId = state.SessionId };
        return Result<CopilotExecuteResult>.Success(new CopilotExecuteResult
        {
            Success = true,
            Action = "BalanceSheetReport",
            Completed = true,
            Reply = $"Balance sheet (as of {asOf:yyyy-MM-dd}): assets {result.Data.TotalAssets}, liabilities {result.Data.TotalLiabilities}, equity {result.Data.TotalEquity}.",
            Result = result.Data,
            SessionState = reset
        });
    }

    private async Task<Result<CopilotExecuteResult>> ExecuteProductDetails(Guid companyId, string input, CopilotConversationState state, CancellationToken cancellationToken)
    {
        var searchText = TryGetField(state, "productName", out var savedProductName)
            ? savedProductName
            : ExtractSearchText(input, "product details", "details product", "details of product", "product info", "get product details", "show product details");

        if (string.IsNullOrWhiteSpace(searchText) && !string.IsNullOrWhiteSpace(input))
            searchText = input.Trim();

        if (string.IsNullOrWhiteSpace(searchText))
            return Ask(state, "Which product do you want details for?");

        state.Fields["productName"] = searchText;

        var product = await _context.Products.AsNoTracking()
            .Where(x => x.CompanyId == companyId && !x.IsDeleted && x.Name.Contains(searchText))
            .OrderByDescending(x => x.Name == searchText)
            .ThenBy(x => x.Name.Length)
            .Select(x => new
            {
                x.Id,
                x.Name,
                x.SKU,
                x.Barcode,
                x.PurchasePrice,
                x.SalePrice,
                x.IsActive
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (product == null)
            return Ask(state, $"Product '{searchText}' not found. Please provide a valid product name.");

        var stock = await _context.StockLevels.AsNoTracking()
            .Where(x => x.CompanyId == companyId && x.ProductId == product.Id)
            .Select(x => x.Quantity - (x.ReservedQuantity ?? 0m))
            .DefaultIfEmpty(0m)
            .SumAsync(cancellationToken);

        return Result<CopilotExecuteResult>.Success(new CopilotExecuteResult
        {
            Success = true,
            Action = "ProductDetails",
            Reply = $"Product: {product.Name} | SKU: {product.SKU ?? "-"} | Barcode: {product.Barcode ?? "-"} | Purchase: {product.PurchasePrice} | Sale: {product.SalePrice} | Available Stock: {stock} | Status: {(product.IsActive ? "Active" : "Inactive")}.",
            Result = new
            {
                product.Name,
                product.SKU,
                product.Barcode,
                product.PurchasePrice,
                product.SalePrice,
                availableStock = stock,
                product.IsActive
            },
            SessionState = new CopilotConversationState { SessionId = state.SessionId }
        });
    }

    private async Task<Result<CopilotExecuteResult>> ExecuteCustomerDetails(Guid companyId, string input, CopilotConversationState state, CancellationToken cancellationToken)
    {
        var searchText = TryGetField(state, "customerName", out var savedCustomerName)
            ? savedCustomerName
            : ExtractSearchText(input, "customer details", "details customer", "details of customer", "customer info", "get customer details", "show customer details");

        if (string.IsNullOrWhiteSpace(searchText) && !string.IsNullOrWhiteSpace(input))
            searchText = input.Trim();

        if (string.IsNullOrWhiteSpace(searchText))
            return Ask(state, "Which customer do you want details for?");

        state.Fields["customerName"] = searchText;

        var customer = await _context.Customers.AsNoTracking()
            .Where(x => x.CompanyId == companyId && !x.IsDeleted && x.Name.Contains(searchText))
            .OrderByDescending(x => x.Name == searchText)
            .ThenBy(x => x.Name.Length)
            .Select(x => new
            {
                x.Id,
                x.Name,
                x.Code,
                x.PhoneNumber,
                x.Email,
                x.ContactPerson,
                x.Address,
                x.CreditLimit,
                x.PaymentTerms
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (customer == null)
            return Ask(state, $"Customer '{searchText}' not found. Please provide a valid customer name.");

        var totalSales = await _context.SalesOrders.AsNoTracking()
            .Where(x => x.CompanyId == companyId && !x.IsDeleted && x.CustomerId == customer.Id)
            .Select(x => x.TotalAmount)
            .DefaultIfEmpty(0m)
            .SumAsync(cancellationToken);

        return Result<CopilotExecuteResult>.Success(new CopilotExecuteResult
        {
            Success = true,
            Action = "CustomerDetails",
            Reply = $"Customer: {customer.Name} | Code: {customer.Code ?? "-"} | Contact: {customer.ContactPerson ?? "-"} | Phone: {customer.PhoneNumber ?? "-"} | Email: {customer.Email ?? "-"} | Credit Limit: {customer.CreditLimit} | Payment Terms: {customer.PaymentTerms ?? "-"} | Lifetime Sales: {totalSales}.",
            Result = new
            {
                customer.Name,
                customer.Code,
                customer.ContactPerson,
                customer.PhoneNumber,
                customer.Email,
                customer.Address,
                customer.CreditLimit,
                customer.PaymentTerms,
                lifetimeSales = totalSales
            },
            SessionState = new CopilotConversationState { SessionId = state.SessionId }
        });
    }

    private async Task<Result<CopilotExecuteResult>> ExecuteSalesOrderDetails(Guid companyId, string input, CopilotConversationState state, CancellationToken cancellationToken)
    {
        var searchText = TryGetField(state, "salesOrderRef", out var savedOrderRef)
            ? savedOrderRef
            : ExtractSearchText(input, "sales details", "sale details", "sales order details", "order details", "details of sales order");

        if (string.IsNullOrWhiteSpace(searchText) && !string.IsNullOrWhiteSpace(input))
            searchText = input.Trim();

        if (string.IsNullOrWhiteSpace(searchText))
            return Ask(state, "Which sales order number do you want details for?");

        state.Fields["salesOrderRef"] = searchText;

        var order = await _context.SalesOrders.AsNoTracking()
            .Include(x => x.Customer)
            .Include(x => x.Items).ThenInclude(i => i.Product)
            .Where(x => x.CompanyId == companyId && !x.IsDeleted && x.OrderNumber.Contains(searchText))
            .OrderByDescending(x => x.OrderDate)
            .FirstOrDefaultAsync(cancellationToken);

        if (order == null)
            return Ask(state, $"Sales order '{searchText}' not found. Please provide a valid order number.");

        var lines = order.Items
            .Select(i => $"{i.Product?.Name ?? "Item"} x{i.Quantity} @ {i.UnitPrice}")
            .Take(6)
            .ToList();
        var itemText = lines.Count == 0 ? "No items." : string.Join("; ", lines);

        return Result<CopilotExecuteResult>.Success(new CopilotExecuteResult
        {
            Success = true,
            Action = "SalesOrderDetails",
            Reply = $"Sales Order {order.OrderNumber}: customer {order.Customer?.Name ?? "-"}, date {order.OrderDate:yyyy-MM-dd}, total {order.TotalAmount}, status {order.Status}. Items: {itemText}",
            Result = new
            {
                order.Id,
                order.OrderNumber,
                customer = order.Customer?.Name,
                order.OrderDate,
                order.TotalAmount,
                order.Status,
                items = order.Items.Select(i => new
                {
                    product = i.Product?.Name,
                    i.Quantity,
                    i.UnitPrice,
                    i.LineTotal
                }).ToList()
            },
            SessionState = new CopilotConversationState { SessionId = state.SessionId }
        });
    }

    private async Task<Result<CopilotExecuteResult>> ExecuteExportProducts(Guid companyId, string input, CopilotConversationState state, CancellationToken cancellationToken)
    {
        var rows = await _context.Products.AsNoTracking()
            .Where(x => x.CompanyId == companyId && !x.IsDeleted)
            .OrderBy(x => x.Name)
            .Take(1000)
            .Select(x => new
            {
                x.Name,
                x.SKU,
                x.Barcode,
                x.PurchasePrice,
                x.SalePrice,
                x.IsActive
            })
            .ToListAsync(cancellationToken);

        var csv = BuildCsv(
            "Name,SKU,Barcode,PurchasePrice,SalePrice,Status",
            rows.Select(x => $"{CsvEscape(x.Name)},{CsvEscape(x.SKU)},{CsvEscape(x.Barcode)},{x.PurchasePrice},{x.SalePrice},{(x.IsActive ? "Active" : "Inactive")}"));

        var totalRows = rows.Count;
        if (ContainsAny(input, "pdf"))
        {
            var pdfRows = rows
                .Select(x => new[] { x.Name ?? "-", x.SKU ?? "-", x.Barcode ?? "-", x.PurchasePrice.ToString("0.##"), x.SalePrice.ToString("0.##"), x.IsActive ? "Active" : "Inactive" })
                .ToList();
            return BuildClientPdfResponse(
                "ProductListExportPdf",
                $"Products PDF ready ({totalRows} rows). Click Download PDF or say 'download'.",
                "Products List",
                new[] { "Name", "SKU", "Barcode", "Purchase Price", "Sale Price", "Status" },
                pdfRows,
                $"products-{DateTime.UtcNow:yyyyMMdd}.pdf",
                state);
        }

        var csvFile = $"products-{DateTime.UtcNow:yyyyMMdd}.csv";
        var pdfFile = $"products-{DateTime.UtcNow:yyyyMMdd}.pdf";
        var pdfPayload = JsonSerializer.Serialize(new
        {
            title = "Products List",
            headers = new[] { "Name", "SKU", "Barcode", "Purchase Price", "Sale Price", "Status" },
            rows = rows.Select(x => new[] { x.Name ?? "-", x.SKU ?? "-", x.Barcode ?? "-", x.PurchasePrice.ToString("0.##"), x.SalePrice.ToString("0.##"), x.IsActive ? "Active" : "Inactive" }).ToList()
        });
        return BuildCsvExportResponse(
            "ProductListExport",
            $"Products list ready ({totalRows} rows). Click Download CSV or say 'download'. You can also say 'download pdf'.",
            csv,
            csvFile,
            state,
            "CLIENT_PDF",
            pdfPayload,
            pdfFile);
    }

    private async Task<Result<CopilotExecuteResult>> ExecuteExportCustomers(Guid companyId, string input, CopilotConversationState state, CancellationToken cancellationToken)
    {
        var rows = await _context.Customers.AsNoTracking()
            .Where(x => x.CompanyId == companyId && !x.IsDeleted)
            .OrderBy(x => x.Name)
            .Take(1000)
            .Select(x => new
            {
                x.Name,
                x.Code,
                x.ContactPerson,
                x.PhoneNumber,
                x.Email,
                x.CreditLimit,
                x.PaymentTerms
            })
            .ToListAsync(cancellationToken);

        var csv = BuildCsv(
            "Name,Code,ContactPerson,Phone,Email,CreditLimit,PaymentTerms",
            rows.Select(x => $"{CsvEscape(x.Name)},{CsvEscape(x.Code)},{CsvEscape(x.ContactPerson)},{CsvEscape(x.PhoneNumber)},{CsvEscape(x.Email)},{x.CreditLimit},{CsvEscape(x.PaymentTerms)}"));

        var totalRows = rows.Count;
        if (ContainsAny(input, "pdf"))
        {
            var pdfRows = rows
                .Select(x => new[] { x.Name ?? "-", x.Code ?? "-", x.ContactPerson ?? "-", x.PhoneNumber ?? "-", x.Email ?? "-", x.CreditLimit?.ToString("0.##") ?? "0", x.PaymentTerms ?? "-" })
                .ToList();
            return BuildClientPdfResponse(
                "CustomerListExportPdf",
                $"Customers PDF ready ({totalRows} rows). Click Download PDF or say 'download'.",
                "Customers List",
                new[] { "Name", "Code", "Contact", "Phone", "Email", "Credit Limit", "Payment Terms" },
                pdfRows,
                $"customers-{DateTime.UtcNow:yyyyMMdd}.pdf",
                state);
        }

        var csvFile = $"customers-{DateTime.UtcNow:yyyyMMdd}.csv";
        var pdfFile = $"customers-{DateTime.UtcNow:yyyyMMdd}.pdf";
        var pdfPayload = JsonSerializer.Serialize(new
        {
            title = "Customers List",
            headers = new[] { "Name", "Code", "Contact", "Phone", "Email", "Credit Limit", "Payment Terms" },
            rows = rows.Select(x => new[] { x.Name ?? "-", x.Code ?? "-", x.ContactPerson ?? "-", x.PhoneNumber ?? "-", x.Email ?? "-", x.CreditLimit?.ToString("0.##") ?? "0", x.PaymentTerms ?? "-" }).ToList()
        });
        return BuildCsvExportResponse(
            "CustomerListExport",
            $"Customers list ready ({totalRows} rows). Click Download CSV or say 'download'. You can also say 'download pdf'.",
            csv,
            csvFile,
            state,
            "CLIENT_PDF",
            pdfPayload,
            pdfFile);
    }

    private async Task<Result<CopilotExecuteResult>> HandleExportSalesOrders(
        Guid companyId,
        CopilotConversationState state,
        string input,
        bool confirmed,
        CancellationToken cancellationToken)
    {
        if (!TryGetField(state, "exportFromDate", out _) && TryExtractSingleDate(input, out var singleFrom))
            state.Fields["exportFromDate"] = singleFrom.ToString("yyyy-MM-dd");

        if (TryExtractDateRange(input, out var rangeFrom, out var rangeTo))
        {
            state.Fields["exportFromDate"] = rangeFrom.ToString("yyyy-MM-dd");
            state.Fields["exportToDate"] = rangeTo.ToString("yyyy-MM-dd");
        }

        if (!TryGetField(state, "exportFromDate", out var fromText))
        {
            state.AwaitingConfirmation = false;
            return Ask(state, "Please provide From Date for Sales export (example: 2026-02-01).");
        }

        if (!TryGetField(state, "exportToDate", out var toText))
        {
            state.AwaitingConfirmation = false;
            return Ask(state, "Please provide To Date for Sales export (example: 2026-02-29).");
        }

        if (!TryParseIsoDate(fromText, out var fromDate))
        {
            state.Fields.Remove("exportFromDate");
            return Ask(state, "I could not read From Date. Please enter it as YYYY-MM-DD.");
        }

        if (!TryParseIsoDate(toText, out var toDate))
        {
            state.Fields.Remove("exportToDate");
            return Ask(state, "I could not read To Date. Please enter it as YYYY-MM-DD.");
        }

        if (toDate < fromDate)
        {
            state.Fields.Remove("exportToDate");
            return Ask(state, "To Date cannot be earlier than From Date. Please provide a valid To Date.");
        }

        if (!confirmed)
        {
            state.AwaitingConfirmation = true;
            return Ask(state, $"Confirm Sales export from {fromDate:yyyy-MM-dd} to {toDate:yyyy-MM-dd}. Say Done to confirm or Cancel.");
        }

        state.AwaitingConfirmation = false;
        return await ExecuteExportSalesOrders(companyId, state, fromDate, toDate, cancellationToken);
    }

    private async Task<Result<CopilotExecuteResult>> ExecuteExportSalesOrders(
        Guid companyId,
        CopilotConversationState state,
        DateTime fromDate,
        DateTime toDate,
        CancellationToken cancellationToken)
    {
        var rows = await _context.SalesOrders.AsNoTracking()
            .Include(x => x.Customer)
            .Where(x => x.CompanyId == companyId && !x.IsDeleted && x.OrderDate >= fromDate && x.OrderDate <= toDate)
            .OrderByDescending(x => x.OrderDate)
            .Take(1000)
            .Select(x => new
            {
                x.OrderNumber,
                x.OrderDate,
                CustomerName = x.Customer != null ? x.Customer.Name : string.Empty,
                x.TotalAmount,
                x.Status
            })
            .ToListAsync(cancellationToken);

        var csv = BuildCsv(
            "OrderNumber,OrderDate,Customer,TotalAmount,Status",
            rows.Select(x => $"{CsvEscape(x.OrderNumber)},{x.OrderDate:yyyy-MM-dd},{CsvEscape(x.CustomerName)},{x.TotalAmount},{CsvEscape(x.Status.ToString())}"));

        var totalRows = rows.Count;
        return BuildCsvExportResponse("SalesOrdersExport", $"Sales orders export ready ({totalRows} rows) for {fromDate:yyyy-MM-dd} to {toDate:yyyy-MM-dd}. Click Download CSV or say 'download'.", csv, $"sales-orders-{fromDate:yyyyMMdd}-{toDate:yyyyMMdd}.csv", state);
    }

    private static async Task<Result<CopilotExecuteResult>> ReplyWithEntityList(
        CopilotConversationState state,
        string label,
        IQueryable<string> query,
        CancellationToken cancellationToken)
    {
        var items = await query.OrderBy(x => x).Take(20).ToListAsync(cancellationToken);
        var message = items.Count == 0
            ? $"{label}: no records found."
            : $"{label}: {string.Join(", ", items)}";
        return Result<CopilotExecuteResult>.Success(new CopilotExecuteResult
        {
            Success = true,
            Reply = message,
            SessionState = state
        });
    }

    private static Result<CopilotExecuteResult>? HandlePendingCorrection(CopilotConversationState state, string inputLower)
    {
        if (!TryGetField(state, "pendingField", out var pendingField) || !TryGetField(state, "pendingValue", out var pendingValue))
            return null;

        if (IsAffirmativeInput(inputLower))
        {
            state.Fields[pendingField] = pendingValue;
            state.Fields.Remove("pendingField");
            state.Fields.Remove("pendingValue");
            state.Fields.Remove("pendingLabel");
            return null;
        }

        if (IsNegativeInput(inputLower))
        {
            state.Fields.Remove(pendingField);
            state.Fields.Remove("pendingField");
            state.Fields.Remove("pendingValue");
            var pendingLabel = state.Fields.GetValueOrDefault("pendingLabel") ?? pendingField;
            state.Fields.Remove("pendingLabel");
            return Ask(state, $"Okay, please provide the correct {pendingLabel}.");
        }

        return Ask(state, $"Did you mean '{pendingValue}'? Please reply Yes or No.");
    }

    private static bool HasPendingCorrection(CopilotConversationState state)
    {
        return TryGetField(state, "pendingField", out _) && TryGetField(state, "pendingValue", out _);
    }

    private static Result<CopilotExecuteResult>? HandlePendingIntentSuggestion(CopilotConversationState state, string inputLower)
    {
        if (!TryGetField(state, "pendingIntentTask", out var pendingTask) || !TryGetField(state, "pendingIntentLabel", out var pendingLabel))
            return null;

        if (IsAffirmativeInput(inputLower))
        {
            state.CurrentTask = pendingTask;
            state.StepIndex = 0;
            state.AwaitingConfirmation = false;
            state.LastQuestion = null;
            state.LastAssistantMessage = null;
            state.Fields.Clear();
            return null;
        }

        if (IsNegativeInput(inputLower))
        {
            state.Fields.Remove("pendingIntentTask");
            state.Fields.Remove("pendingIntentLabel");
            return HelpMenu(state);
        }

        return Ask(state, $"Did you mean {pendingLabel}? Please reply Yes or No.");
    }

    private static bool HasPendingIntentSuggestion(CopilotConversationState state)
    {
        return TryGetField(state, "pendingIntentTask", out _) && TryGetField(state, "pendingIntentLabel", out _);
    }

    private static bool TryInferTaskFromInput(string input, out string task, out string label)
    {
        task = string.Empty;
        label = string.Empty;
        if (string.IsNullOrWhiteSpace(input)) return false;

        var normalizedInput = NormalizeForMatch(input);
        var candidates = new List<(string task, string label, string[] aliases)>
        {
            ("SalesOrder", "Sales Order", new[] { "sales order", "sell order", "create sales order" }),
            ("PurchaseOrder", "Purchase Order", new[] { "purchase order", "buy order", "create purchase order" }),
            ("InventoryUpdate", "Inventory Update", new[] { "inventory update", "stock update", "adjust inventory" }),
            ("PriceUpdate", "Price Update", new[] { "price update", "update price", "change price" }),
            ("StockQuery", "Stock Query", new[] { "stock query", "stock", "check stock", "stock status" }),
            ("ProductCreate", "Create Product", new[] { "create product", "add product", "new product" }),
            ("ProductUpdate", "Update Product", new[] { "update product", "edit product" }),
            ("ProductDelete", "Delete Product", new[] { "delete product", "remove product" }),
            ("CustomerCreate", "Create Customer", new[] { "create customer", "add customer", "new customer" }),
            ("CustomerUpdate", "Update Customer", new[] { "update customer", "edit customer" }),
            ("CustomerDelete", "Delete Customer", new[] { "delete customer", "remove customer" }),
            ("SupplierCreate", "Create Supplier", new[] { "create supplier", "add supplier", "new supplier" }),
            ("SupplierUpdate", "Update Supplier", new[] { "update supplier", "edit supplier" }),
            ("SupplierDelete", "Delete Supplier", new[] { "delete supplier", "remove supplier" }),
            ("WarehouseCreate", "Create Warehouse", new[] { "create warehouse", "add warehouse", "new warehouse", "create wharehouse" }),
            ("WarehouseUpdate", "Update Warehouse", new[] { "update warehouse", "edit warehouse", "update wharehouse" }),
            ("WarehouseDelete", "Delete Warehouse", new[] { "delete warehouse", "remove warehouse", "delete wharehouse" })
        };

        double bestScore = 0;
        var bestTask = string.Empty;
        var bestLabel = string.Empty;
        foreach (var candidate in candidates)
        {
            foreach (var alias in candidate.aliases)
            {
                var normalizedAlias = NormalizeForMatch(alias);
                if (normalizedAlias.Contains(normalizedInput, StringComparison.OrdinalIgnoreCase) ||
                    normalizedInput.Contains(normalizedAlias, StringComparison.OrdinalIgnoreCase))
                {
                    task = candidate.task;
                    label = candidate.label;
                    return true;
                }

                var score = SimilarityScore(normalizedInput, normalizedAlias);
                if (score > bestScore)
                {
                    bestScore = score;
                    bestTask = candidate.task;
                    bestLabel = candidate.label;
                }
            }
        }

        if (bestScore >= 0.62 && !string.IsNullOrWhiteSpace(bestTask))
        {
            task = bestTask;
            label = bestLabel;
            return true;
        }

        return false;
    }

    private static Result<CopilotExecuteResult> HelpMenu(CopilotConversationState state)
    {
        return Result<CopilotExecuteResult>.Success(new CopilotExecuteResult
        {
            Success = true,
            Reply = "I can guide you step-by-step in simple language. Try: Sales Order, Purchase Order, Inventory Update, Price Update, Stock Query, Create/Update/Delete Product, Customer, Supplier, Warehouse, Generate Invoice, or Reports. For reports, I will ask From Date and To Date.",
            SessionState = state
        });
    }

    private async Task<Result<CopilotExecuteResult>> AskWithSuggestion(
        CopilotConversationState state,
        string enteredValue,
        string fieldKey,
        string fieldLabel,
        IQueryable<string> namesQuery,
        string fallbackQuestion,
        CancellationToken cancellationToken)
    {
        state.Fields.Remove(fieldKey);
        var suggestion = await FindBestMatchAsync(enteredValue, namesQuery, cancellationToken);
        if (!string.IsNullOrWhiteSpace(suggestion))
        {
            state.Fields["pendingField"] = fieldKey;
            state.Fields["pendingValue"] = suggestion;
            state.Fields["pendingLabel"] = fieldLabel;
            return Ask(state, $"{char.ToUpperInvariant(fieldLabel[0]) + fieldLabel[1..]} '{enteredValue}' not found. Did you mean '{suggestion}'? Say Yes or No.");
        }

        return Ask(state, fallbackQuestion);
    }

    private static async Task<string?> FindBestMatchAsync(string input, IQueryable<string> namesQuery, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(input)) return null;

        var candidates = await namesQuery
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Distinct()
            .Take(60)
            .ToListAsync(cancellationToken);
        if (candidates.Count == 0) return null;

        var directStartsWith = candidates
            .Where(x => x.StartsWith(input, StringComparison.OrdinalIgnoreCase))
            .OrderBy(x => x.Length)
            .FirstOrDefault();
        if (!string.IsNullOrWhiteSpace(directStartsWith))
            return directStartsWith;

        var normalizedInput = NormalizeForMatch(input);
        var exact = candidates.FirstOrDefault(x => string.Equals(NormalizeForMatch(x), normalizedInput, StringComparison.Ordinal));
        if (!string.IsNullOrWhiteSpace(exact))
            return exact;

        var normalizedCandidates = candidates
            .Select(x => new { Raw = x, Normalized = NormalizeForMatch(x) })
            .Where(x => !string.IsNullOrWhiteSpace(x.Normalized))
            .ToList();

        var contains = normalizedCandidates
            .Where(x => x.Normalized.Contains(normalizedInput, StringComparison.OrdinalIgnoreCase))
            .OrderBy(x => x.Raw.Length)
            .Select(x => x.Raw)
            .FirstOrDefault();
        if (!string.IsNullOrWhiteSpace(contains))
            return contains;

        string? best = null;
        double bestScore = 0;
        foreach (var candidate in normalizedCandidates)
        {
            var score = SimilarityScore(normalizedInput, candidate.Normalized);
            if (score > bestScore)
            {
                bestScore = score;
                best = candidate.Raw;
            }
        }

        return bestScore >= 0.55 ? best : null;
    }

    private static string NormalizeForMatch(string text)
    {
        var t = text.Trim().ToLowerInvariant();
        t = t.Replace('أ', 'ا').Replace('إ', 'ا').Replace('آ', 'ا').Replace('ٱ', 'ا').Replace('ى', 'ي').Replace('ؤ', 'و').Replace('ئ', 'ي').Replace('ة', 'ه');
        t = System.Text.RegularExpressions.Regex.Replace(t, @"[\u064B-\u065F\u0670]", string.Empty);
        t = System.Text.RegularExpressions.Regex.Replace(t, @"[^\p{L}\p{Nd}]", string.Empty);
        return t;
    }

    private static double SimilarityScore(string a, string b)
    {
        if (string.IsNullOrEmpty(a) || string.IsNullOrEmpty(b)) return 0;
        var distance = LevenshteinDistance(a, b);
        var maxLen = Math.Max(a.Length, b.Length);
        return maxLen == 0 ? 1 : 1d - ((double)distance / maxLen);
    }

    private static int LevenshteinDistance(string s, string t)
    {
        var n = s.Length;
        var m = t.Length;
        var d = new int[n + 1, m + 1];

        for (var i = 0; i <= n; i++) d[i, 0] = i;
        for (var j = 0; j <= m; j++) d[0, j] = j;

        for (var i = 1; i <= n; i++)
        {
            for (var j = 1; j <= m; j++)
            {
                var cost = s[i - 1] == t[j - 1] ? 0 : 1;
                d[i, j] = Math.Min(
                    Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
                    d[i - 1, j - 1] + cost);
            }
        }

        return d[n, m];
    }

    private static string ExtractSearchText(string input, params string[] markers)
    {
        if (string.IsNullOrWhiteSpace(input)) return string.Empty;
        var text = input.Trim();
        var lowered = text.ToLowerInvariant();
        foreach (var marker in markers)
        {
            var markerLower = marker.ToLowerInvariant();
            var idx = lowered.IndexOf(markerLower, StringComparison.Ordinal);
            if (idx >= 0)
            {
                var tail = text[(idx + marker.Length)..].Trim(new[] { ':', '-', ' ' });
                if (!string.IsNullOrWhiteSpace(tail))
                    return tail;
            }
        }
        return string.Empty;
    }

    private static Result<CopilotExecuteResult> BuildCsvExportResponse(
        string action,
        string reply,
        string csvContent,
        string fileName,
        CopilotConversationState state,
        string? altMethod = null,
        string? altBody = null,
        string? altFileName = null)
    {
        var reset = new CopilotConversationState { SessionId = state.SessionId };
        reset.Fields["lastDownloadMethod"] = "CLIENT_CSV";
        reset.Fields["lastDownloadBody"] = csvContent;
        reset.Fields["lastDocumentName"] = fileName;
        if (!string.IsNullOrWhiteSpace(altMethod) && !string.IsNullOrWhiteSpace(altBody))
        {
            reset.Fields["altDownloadMethod"] = altMethod;
            reset.Fields["altDownloadBody"] = altBody;
            reset.Fields["altDocumentName"] = string.IsNullOrWhiteSpace(altFileName) ? "export" : altFileName;
        }

        return Result<CopilotExecuteResult>.Success(new CopilotExecuteResult
        {
            Success = true,
            Action = action,
            Completed = true,
            Reply = reply,
            Result = new
            {
                downloadAction = new
                {
                    method = "CLIENT_CSV",
                    body = csvContent,
                    fileName
                }
            },
            SessionState = reset
        });
    }

    private static Result<CopilotExecuteResult> BuildClientPdfResponse(
        string action,
        string reply,
        string title,
        string[] headers,
        List<string[]> rows,
        string fileName,
        CopilotConversationState state)
    {
        var payload = JsonSerializer.Serialize(new
        {
            title,
            headers,
            rows
        });

        var reset = new CopilotConversationState { SessionId = state.SessionId };
        reset.Fields["lastDownloadMethod"] = "CLIENT_PDF";
        reset.Fields["lastDownloadBody"] = payload;
        reset.Fields["lastDocumentName"] = fileName;

        return Result<CopilotExecuteResult>.Success(new CopilotExecuteResult
        {
            Success = true,
            Action = action,
            Completed = true,
            Reply = reply,
            Result = new
            {
                downloadAction = new
                {
                    method = "CLIENT_PDF",
                    body = payload,
                    fileName
                }
            },
            SessionState = reset
        });
    }

    private static string BuildCsv(string header, IEnumerable<string> rows)
    {
        return string.Join('\n', new[] { header }.Concat(rows));
    }

    private static string CsvEscape(string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) return string.Empty;
        var escaped = value.Replace("\"", "\"\"");
        return $"\"{escaped}\"";
    }

    private static Result<CopilotExecuteResult> Ask(CopilotConversationState state, string question)
    {
        state.LastQuestion = question;
        state.LastAssistantMessage = question;
        return Result<CopilotExecuteResult>.Success(new CopilotExecuteResult
        {
            Success = true,
            Reply = question,
            NextQuestion = question,
            ConfirmationMessage = state.AwaitingConfirmation ? question : null,
            SessionState = state
        });
    }

    private static bool TryCaptureDateRangeFromInput(CopilotConversationState state, string input)
    {
        if (string.IsNullOrWhiteSpace(input)) return false;
        if (TryGetField(state, "reportFromDate", out _) && TryGetField(state, "reportToDate", out _)) return false;

        if (TryExtractDateRange(input, out var fromDate, out var toDate))
        {
            state.Fields["reportFromDate"] = fromDate.ToString("yyyy-MM-dd");
            state.Fields["reportToDate"] = toDate.ToString("yyyy-MM-dd");
            return true;
        }

        if (!TryGetField(state, "reportFromDate", out _) && TryExtractSingleDate(input, out var singleDate))
        {
            state.Fields["reportFromDate"] = singleDate.ToString("yyyy-MM-dd");
            return true;
        }

        return false;
    }

    private static bool TryExtractDateRange(string input, out DateTime fromDate, out DateTime toDate)
    {
        fromDate = default;
        toDate = default;
        if (string.IsNullOrWhiteSpace(input)) return false;

        var dates = ExtractDates(input);
        if (dates.Count < 2) return false;

        fromDate = dates[0];
        toDate = dates[1];
        return true;
    }

    private static bool TryExtractSingleDate(string input, out DateTime date)
    {
        date = default;
        if (string.IsNullOrWhiteSpace(input)) return false;
        var dates = ExtractDates(input);
        if (dates.Count == 0) return false;
        date = dates[0];
        return true;
    }

    private static List<DateTime> ExtractDates(string input)
    {
        var results = new List<DateTime>();
        if (string.IsNullOrWhiteSpace(input)) return results;

        var normalizedInput = NormalizeDateTextForParse(input);
        var patterns = new[]
        {
            @"\b\d{4}[-/]\d{1,2}[-/]\d{1,2}\b",
            @"\b\d{1,2}[-/]\d{1,2}[-/]\d{4}\b",
            @"\b\d{8}\b",
            @"\b(19|20)\d{2}\b",
            @"\b\d{1,2}\s+[A-Za-z]{3,9}\s+\d{4}\b",
            @"\b[A-Za-z]{3,9}\s+\d{1,2}(?:st|nd|rd|th)?(?:,)?\s+\d{4}\b",
            @"\b\d{2}-\d{2}\s+\d{4}\b"
        };

        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var pattern in patterns)
        {
            var matches = System.Text.RegularExpressions.Regex.Matches(normalizedInput, pattern, System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            foreach (System.Text.RegularExpressions.Match match in matches)
            {
                var token = match.Value.Trim();
                if (!seen.Add(token)) continue;
                if (TryParseIsoDate(token, out var parsed))
                    results.Add(parsed);
            }
        }

        return results;
    }

    private static bool TryParseIsoDate(string text, out DateTime date)
    {
        date = default;
        if (string.IsNullOrWhiteSpace(text)) return false;

        var cleaned = NormalizeDateTextForParse(text);
        if (System.Text.RegularExpressions.Regex.IsMatch(cleaned, @"^\d{4}$") &&
            int.TryParse(cleaned, out var yearOnly) &&
            yearOnly >= 1900 && yearOnly <= 2100)
        {
            date = new DateTime(yearOnly, 1, 1);
            return true;
        }

        var formats = new[]
        {
            "yyyy-MM-dd", "yyyy/M/d", "yyyy/MM/dd",
            "d-M-yyyy", "d/M/yyyy", "dd-MM-yyyy", "dd/MM/yyyy",
            "d MMM yyyy", "dd MMM yyyy", "d MMMM yyyy", "dd MMMM yyyy",
            "MMM d yyyy", "MMMM d yyyy", "MMM d, yyyy", "MMMM d, yyyy",
            "M/d/yy", "MM/dd/yy", "d/M/yy", "dd/MM/yy",
            "yyyyMMdd", "ddMMyyyy", "MMddyyyy"
        };

        if (DateTime.TryParseExact(cleaned, formats, CultureInfo.InvariantCulture, DateTimeStyles.AllowWhiteSpaces, out var parsed) ||
            DateTime.TryParse(cleaned, CultureInfo.InvariantCulture, DateTimeStyles.AllowWhiteSpaces, out parsed) ||
            DateTime.TryParse(cleaned, CultureInfo.CurrentCulture, DateTimeStyles.AllowWhiteSpaces, out parsed))
        {
            date = parsed.Date;
            return true;
        }

        // Voice typo fallback: "20-20 0201" => 2020-02-01
        var splitYearCompact = System.Text.RegularExpressions.Regex.Match(cleaned, @"\b(\d{2})[-/ ](\d{2})\s+(\d{2})(\d{2})\b");
        if (splitYearCompact.Success)
        {
            var yyyy = $"{splitYearCompact.Groups[1].Value}{splitYearCompact.Groups[2].Value}";
            var mm = splitYearCompact.Groups[3].Value;
            var dd = splitYearCompact.Groups[4].Value;
            if (DateTime.TryParseExact($"{yyyy}-{mm}-{dd}", "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out parsed))
            {
                date = parsed.Date;
                return true;
            }
        }

        return false;
    }

    private static string NormalizeDateTextForParse(string input)
    {
        if (string.IsNullOrWhiteSpace(input)) return string.Empty;
        var cleaned = input.Trim();
        cleaned = System.Text.RegularExpressions.Regex.Replace(cleaned, @"\b(\d{2})\s*[-/ ]\s*(\d{2})\b", "$1$2", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        cleaned = System.Text.RegularExpressions.Regex.Replace(cleaned, @"\b(\d{1,2})(st|nd|rd|th)\b", "$1", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        cleaned = System.Text.RegularExpressions.Regex.Replace(cleaned, @"\bfirst\b", "1", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        cleaned = System.Text.RegularExpressions.Regex.Replace(cleaned, @"\bsecond\b", "2", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        cleaned = System.Text.RegularExpressions.Regex.Replace(cleaned, @"\bthird\b", "3", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        cleaned = System.Text.RegularExpressions.Regex.Replace(cleaned, @"\bfourth\b", "4", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        cleaned = System.Text.RegularExpressions.Regex.Replace(cleaned, @"\bfifth\b", "5", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        cleaned = System.Text.RegularExpressions.Regex.Replace(cleaned, @"\bsixth\b", "6", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        cleaned = System.Text.RegularExpressions.Regex.Replace(cleaned, @"\bseventh\b", "7", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        cleaned = System.Text.RegularExpressions.Regex.Replace(cleaned, @"\beighth\b", "8", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        cleaned = System.Text.RegularExpressions.Regex.Replace(cleaned, @"\bninth\b", "9", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        cleaned = System.Text.RegularExpressions.Regex.Replace(cleaned, @"\btenth\b", "10", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        cleaned = cleaned.Replace(",", " ");
        cleaned = System.Text.RegularExpressions.Regex.Replace(cleaned, @"\bof\b", " ", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        cleaned = System.Text.RegularExpressions.Regex.Replace(cleaned, @"\s+", " ");
        return cleaned.Trim();
    }

    private static bool TryGetField(CopilotConversationState state, string key, out string value)
    {
        value = string.Empty;
        if (!state.Fields.TryGetValue(key, out var raw) || string.IsNullOrWhiteSpace(raw)) return false;
        value = raw.Trim();
        return true;
    }

    private static bool IsAffirmativeInput(string input)
    {
        if (string.IsNullOrWhiteSpace(input)) return false;
        var text = input.Trim().ToLowerInvariant();
        return text == "yes" || text == "y" || text == "done" ||
               text.Contains(" yes") || text.StartsWith("yes ") ||
               text.Contains("confirm") || text.Contains("ok") || text.Contains("okay") ||
               text.Contains("yeah") || text.Contains("yep");
    }

    private static bool IsNegativeInput(string input)
    {
        if (string.IsNullOrWhiteSpace(input)) return false;
        var text = input.Trim().ToLowerInvariant();
        return text == "no" || text == "n" ||
               text.Contains(" no") || text.StartsWith("no ") ||
               text.Contains("nope") || text.Contains("nah");
    }

    private static string NormalizeUnitInput(string input)
    {
        if (string.IsNullOrWhiteSpace(input)) return string.Empty;
        var value = input.Trim().ToLowerInvariant();
        return value switch
        {
            "peace" => "piece",
            "pieces" => "piece",
            "pcs" => "piece",
            "pc" => "piece",
            _ => input
        };
    }

    private static bool TryParseDecimal(string input, out decimal value)
    {
        value = 0;
        if (string.IsNullOrWhiteSpace(input)) return false;
        var cleaned = input.Trim();
        if (decimal.TryParse(cleaned, out value)) return true;
        var match = System.Text.RegularExpressions.Regex.Match(cleaned, @"-?\d+(\.\d+)?");
        return match.Success && decimal.TryParse(match.Value, out value);
    }

    private static bool ContainsAny(string input, params string[] tokens)
    {
        if (string.IsNullOrWhiteSpace(input)) return false;
        var text = input.Trim().ToLowerInvariant();
        return tokens.Any(token => text.Contains(token.ToLowerInvariant()));
    }

    private static bool IsListRequest(string input, params string[] entities)
    {
        if (string.IsNullOrWhiteSpace(input)) return false;

        var text = input.Trim().ToLowerInvariant();
        var asksForList =
            text.Contains("list") ||
            text.Contains("show") ||
            text.Contains("names") ||
            text.Contains("name") ||
            text.Contains("tell me") ||
            text.Contains("what are");

        if (!asksForList) return false;

        foreach (var entity in entities)
        {
            if (text.Contains(entity.ToLowerInvariant()))
                return true;
        }

        return false;
    }

    // ---------- Direct action methods (single-shot mode from parser params) ----------
    private async Task<Result<CopilotExecuteResult>> ExecuteCreatePurchaseOrder(Guid companyId, CopilotIntentResult intent, CancellationToken cancellationToken)
    {
        if (!TryGetString(intent, "supplierName", out var supplierName) ||
            !TryGetString(intent, "productName", out var productName) ||
            !TryGetInt(intent, "quantity", out var qty))
        {
            return Result<CopilotExecuteResult>.Failure("Need format: Create purchase order for <qty> <product> from <supplier>.");
        }

        var supplier = await _context.Suppliers.AsNoTracking()
            .FirstOrDefaultAsync(s => s.CompanyId == companyId && !s.IsDeleted && s.Name.Contains(supplierName), cancellationToken);
        if (supplier == null)
            return Result<CopilotExecuteResult>.Failure($"Supplier not found: {supplierName}");

        var product = await _context.Products.AsNoTracking()
            .FirstOrDefaultAsync(p => p.CompanyId == companyId && !p.IsDeleted && p.Name.Contains(productName), cancellationToken);
        if (product == null)
            return Result<CopilotExecuteResult>.Failure($"Product not found: {productName}");

        var create = new CreatePurchaseOrderCommand
        {
            SupplierId = supplier.Id,
            Notes = "Created by AI Copilot",
            Items = new List<CreatePurchaseOrderItemDto>
            {
                new()
                {
                    ProductId = product.Id,
                    Quantity = qty,
                    UnitPrice = product.PurchasePrice
                }
            }
        };

        var result = await _mediator.Send(create, cancellationToken);
        if (!result.Succeeded || result.Data == null)
            return Result<CopilotExecuteResult>.Failure(result.Errors);

        return Result<CopilotExecuteResult>.Success(new CopilotExecuteResult
        {
            Success = true,
            Action = intent.Action,
            Result = new
            {
                orderId = result.Data.Id,
                orderNumber = result.Data.OrderNumber,
                supplier = supplier.Name,
                item = product.Name,
                quantity = qty
            }
        });
    }

    private async Task<Result<CopilotExecuteResult>> ExecuteCreateSalesOrder(Guid companyId, CopilotIntentResult intent, CancellationToken cancellationToken)
    {
        if (!TryGetString(intent, "customerName", out var customerName) ||
            !TryGetString(intent, "productName", out var productName) ||
            !TryGetInt(intent, "quantity", out var qty))
        {
            return Result<CopilotExecuteResult>.Failure("Need format: Create sales order for <qty> <product> to <customer>.");
        }

        var customer = await _context.Customers.AsNoTracking()
            .FirstOrDefaultAsync(c => c.CompanyId == companyId && !c.IsDeleted && c.Name.Contains(customerName), cancellationToken);
        if (customer == null)
            return Result<CopilotExecuteResult>.Failure($"Customer not found: {customerName}");

        var product = await _context.Products.AsNoTracking()
            .FirstOrDefaultAsync(p => p.CompanyId == companyId && !p.IsDeleted && p.Name.Contains(productName), cancellationToken);
        if (product == null)
            return Result<CopilotExecuteResult>.Failure($"Product not found: {productName}");

        var create = new CreateSalesOrderCommand
        {
            CustomerId = customer.Id,
            Notes = "Created by AI Copilot",
            Items = new List<CreateSalesOrderItemDto>
            {
                new()
                {
                    ProductId = product.Id,
                    Quantity = qty,
                    UnitPrice = product.SalePrice
                }
            }
        };

        var result = await _mediator.Send(create, cancellationToken);
        if (!result.Succeeded || result.Data == null)
            return Result<CopilotExecuteResult>.Failure(result.Errors);

        return Result<CopilotExecuteResult>.Success(new CopilotExecuteResult
        {
            Success = true,
            Action = intent.Action,
            Result = new
            {
                orderId = result.Data.Id,
                orderNumber = result.Data.OrderNumber,
                customer = customer.Name,
                item = product.Name,
                quantity = qty
            }
        });
    }

    private async Task<Result<CopilotExecuteResult>> ExecuteCreateProduct(Guid companyId, CopilotIntentResult intent, CancellationToken cancellationToken)
    {
        if (!TryGetString(intent, "productName", out var productName))
            return Result<CopilotExecuteResult>.Failure("Need format: Add product <name> [sku <code>] [sale <price>] [purchase <price>].");

        var uom = await _context.UnitOfMeasures.AsNoTracking()
            .FirstOrDefaultAsync(u => u.CompanyId == companyId && !u.IsDeleted, cancellationToken);
        if (uom == null)
            return Result<CopilotExecuteResult>.Failure("No unit of measure found. Create one first.");

        var sku = TryGetString(intent, "sku", out var parsedSku) ? parsedSku : $"SKU-{DateTime.UtcNow:yyyyMMddHHmmss}";
        var salePrice = TryGetDecimal(intent, "salePrice", out var sale) ? sale : 0m;
        var purchasePrice = TryGetDecimal(intent, "purchasePrice", out var purchase) ? purchase : salePrice;

        var create = new CreateProductCommand
        {
            Name = productName,
            SKU = sku,
            UnitOfMeasureId = uom.Id,
            PurchasePrice = purchasePrice,
            SalePrice = salePrice,
            TrackQuantity = true
        };

        var result = await _mediator.Send(create, cancellationToken);
        if (!result.Succeeded || result.Data == null)
            return Result<CopilotExecuteResult>.Failure(result.Errors);

        return Result<CopilotExecuteResult>.Success(new CopilotExecuteResult
        {
            Success = true,
            Action = intent.Action,
            Result = new
            {
                productId = result.Data.Id,
                name = result.Data.Name,
                sku = result.Data.SKU
            }
        });
    }

    private async Task<Result<CopilotExecuteResult>> ExecuteProfitToday(Guid companyId, CopilotIntentResult intent, CancellationToken cancellationToken)
    {
        var start = DateTime.UtcNow.Date;
        var end = start.AddDays(1);
        var sales = await _context.SalesOrders.AsNoTracking()
            .Where(s => s.CompanyId == companyId && !s.IsDeleted && s.OrderDate >= start && s.OrderDate < end)
            .SumAsync(s => s.TotalAmount, cancellationToken);

        return Result<CopilotExecuteResult>.Success(new CopilotExecuteResult
        {
            Success = true,
            Action = intent.Action,
            Result = new { date = start, profit = sales }
        });
    }

    private async Task<Result<CopilotExecuteResult>> ExecuteItemsRunningOut(Guid companyId, CopilotIntentResult intent, CancellationToken cancellationToken)
    {
        var items = await _context.StockLevels.AsNoTracking()
            .Include(s => s.Product)
            .Where(s => s.CompanyId == companyId && !s.IsDeleted && s.Product != null &&
                        s.Product.ReorderPoint.HasValue && s.Quantity <= s.Product.ReorderPoint.Value)
            .OrderBy(s => s.Quantity)
            .Take(20)
            .Select(s => new
            {
                productId = s.ProductId,
                productName = s.Product!.Name,
                available = s.Quantity,
                reorderPoint = s.Product.ReorderPoint
            })
            .ToListAsync(cancellationToken);

        return Result<CopilotExecuteResult>.Success(new CopilotExecuteResult
        {
            Success = true,
            Action = intent.Action,
            Result = items
        });
    }

    private async Task<Result<CopilotExecuteResult>> ExecuteSlowProducts(Guid companyId, CopilotIntentResult intent, CancellationToken cancellationToken)
    {
        var since = DateTime.UtcNow.AddDays(-30);
        var slow = await _context.SalesOrderItems.AsNoTracking()
            .Include(i => i.Product)
            .Include(i => i.SalesOrder)
            .Where(i => i.CompanyId == companyId && !i.IsDeleted && i.SalesOrder.OrderDate >= since)
            .GroupBy(i => new { i.ProductId, i.Product!.Name })
            .Select(g => new { g.Key.ProductId, g.Key.Name, soldQty = g.Sum(x => x.Quantity) })
            .OrderBy(x => x.soldQty)
            .Take(20)
            .ToListAsync(cancellationToken);

        return Result<CopilotExecuteResult>.Success(new CopilotExecuteResult
        {
            Success = true,
            Action = intent.Action,
            Result = slow
        });
    }

    private static bool TryGetString(CopilotIntentResult intent, string key, out string value)
    {
        value = string.Empty;
        if (!intent.Parameters.TryGetValue(key, out var raw) || raw == null) return false;
        value = raw.ToString()?.Trim() ?? string.Empty;
        return !string.IsNullOrWhiteSpace(value);
    }

    private static bool TryGetInt(CopilotIntentResult intent, string key, out int value)
    {
        value = 0;
        if (!intent.Parameters.TryGetValue(key, out var raw) || raw == null) return false;
        return int.TryParse(raw.ToString(), out value);
    }

    private static bool TryGetDecimal(CopilotIntentResult intent, string key, out decimal value)
    {
        value = 0;
        if (!intent.Parameters.TryGetValue(key, out var raw) || raw == null) return false;
        return decimal.TryParse(raw.ToString(), out value);
    }
}
