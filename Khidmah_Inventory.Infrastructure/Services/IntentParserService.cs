using System.Text.RegularExpressions;
using Khidmah_Inventory.Application.Common.Services;
using Khidmah_Inventory.Application.Features.Copilot.Models;

namespace Khidmah_Inventory.Infrastructure.Services;

/// <summary>
/// Rule-based intent parser for natural language ops. No external paid AI.
/// </summary>
public class IntentParserService : IIntentParserService
{
    public Task<CopilotIntentResult> ParseAsync(string input, CancellationToken cancellationToken = default)
    {
        var text = NormalizeText(input ?? "");
        var result = new CopilotIntentResult();

        if (string.IsNullOrWhiteSpace(text))
        {
            result.Intent = "Unknown";
            return Task.FromResult(result);
        }

        if (Match(text, "hello|hi|hey|good morning|good afternoon|good evening|salam|assalam|assalamu alaikum|salam alaikum|marhaba|مرحبا|السلام عليكم|السلام عليکم", out _))
        {
            result.Intent = "Greeting";
            result.Action = "Greeting";
            return Task.FromResult(result);
        }

        if (Match(text, "thanks|thank you|thankyou|thx|ty|shukran|جزاك الله خير|شكرا|شكراً", out _))
        {
            result.Intent = "Thanks";
            result.Action = "Thanks";
            return Task.FromResult(result);
        }

        if (Match(text, "help|menu|what can you do|how can you help|how to use|guide me|show options|i need help|assist me", out _))
        {
            result.Intent = "Help";
            result.Action = "Help";
            return Task.FromResult(result);
        }

        if (Match(text, "report help|reports help|how to generate report|how to create report|show report options|what reports can i generate", out _))
        {
            result.Intent = "ReportHelp";
            result.Action = "ReportHelp";
            return Task.FromResult(result);
        }

        if (Match(text, "list customers|customer list|customer names|show customers|customers list|list customer names|عرض العملاء|قائمة العملاء|اسماء العملاء", out _))
        {
            result.Intent = "CustomerList";
            result.Action = "CustomerList";
            return Task.FromResult(result);
        }

        if (Match(text, "list suppliers|supplier list|supplier names|show suppliers|vendor list|suppliers list|list supplier names|عرض الموردين|قائمة الموردين|اسماء الموردين", out _))
        {
            result.Intent = "SupplierList";
            result.Action = "SupplierList";
            return Task.FromResult(result);
        }

        if (Match(text, "list products|product list|product names|show products|items list|products list|list item names|عرض المنتجات|قائمة المنتجات|اسماء المنتجات", out _))
        {
            result.Intent = "ProductList";
            result.Action = "ProductList";
            return Task.FromResult(result);
        }

        if (Match(text, "list warehouses|warehouse list|warehouses list|show warehouses|show warehouse|warehouse names|list warehouse names|عرض المخازن|قائمة المخازن|اسماء المخازن", out _))
        {
            result.Intent = "WarehouseList";
            result.Action = "WarehouseList";
            return Task.FromResult(result);
        }

        if (Match(text, "product details|details product|get product details|show product details|product info|details of product", out _))
        {
            result.Intent = "ProductDetails";
            result.Action = "ProductDetails";
            ExtractTailValue(text, result, "productName", "product details", "details product", "get product details", "show product details", "product info", "details of product");
            return Task.FromResult(result);
        }

        if (Match(text, "customer details|details customer|get customer details|show customer details|customer info|details of customer", out _))
        {
            result.Intent = "CustomerDetails";
            result.Action = "CustomerDetails";
            ExtractTailValue(text, result, "customerName", "customer details", "details customer", "get customer details", "show customer details", "customer info", "details of customer");
            return Task.FromResult(result);
        }

        if (Match(text, "sales details|sale details|sales order details|order details|details of sales order", out _))
        {
            result.Intent = "SalesOrderDetails";
            result.Action = "SalesOrderDetails";
            ExtractTailValue(text, result, "salesOrderRef", "sales details", "sale details", "sales order details", "order details", "details of sales order");
            return Task.FromResult(result);
        }

        if (Match(text, "export products|download products|products export|export product list|download product list", out _))
        {
            result.Intent = "ExportProducts";
            result.Action = "ExportProducts";
            return Task.FromResult(result);
        }

        if (Match(text, "export customers|download customers|customers export|export customer list|download customer list", out _))
        {
            result.Intent = "ExportCustomers";
            result.Action = "ExportCustomers";
            return Task.FromResult(result);
        }

        if (Match(text, "export sales|download sales|sales export|export sales orders|download sales orders|export orders", out _))
        {
            result.Intent = "ExportSalesOrders";
            result.Action = "ExportSalesOrders";
            return Task.FromResult(result);
        }

        if (Match(text, "product crud|products crud|product curd|products curd|crud product|crud products|manage product|manage products|products management|product management", out _))
        {
            result.Intent = "ProductCrudHelp";
            result.Action = "ProductCrudHelp";
            return Task.FromResult(result);
        }

        if (Match(text, "create customer|add customer|new customer|make customer|انشاء عميل|اضافة عميل|عميل جديد", out _))
        {
            result.Intent = "CreateCustomer";
            result.Action = "CreateCustomer";
            result.RequiresConfirmation = true;
            return Task.FromResult(result);
        }

        if (Match(text, "update customer|edit customer|modify customer|add more detail customer|add details customer|customer details update|تحديث عميل|تعديل عميل", out _))
        {
            result.Intent = "UpdateCustomer";
            result.Action = "UpdateCustomer";
            result.RequiresConfirmation = true;
            return Task.FromResult(result);
        }

        if (Match(text, "delete customer|remove customer|حذف عميل|ازالة عميل", out _))
        {
            result.Intent = "DeleteCustomer";
            result.Action = "DeleteCustomer";
            result.RequiresConfirmation = true;
            return Task.FromResult(result);
        }

        if (Match(text, "create supplier|create suplier|create supllier|add supplier|add suplier|new supplier|new suplier|add vendor|create vendor|make supplier|make suplier|انشاء مورد|اضافة مورد|مورد جديد", out _))
        {
            result.Intent = "CreateSupplier";
            result.Action = "CreateSupplier";
            result.RequiresConfirmation = true;
            return Task.FromResult(result);
        }

        if (Match(text, "update supplier|update suplier|update supllier|edit supplier|edit suplier|update vendor|edit vendor|modify supplier|modify suplier|add more detail supplier|supplier details update|suplier details update|تحديث مورد|تعديل مورد", out _))
        {
            result.Intent = "UpdateSupplier";
            result.Action = "UpdateSupplier";
            result.RequiresConfirmation = true;
            return Task.FromResult(result);
        }

        if (Match(text, "delete supplier|delete suplier|remove supplier|remove suplier|delete vendor|remove vendor|حذف مورد|ازالة مورد", out _))
        {
            result.Intent = "DeleteSupplier";
            result.Action = "DeleteSupplier";
            result.RequiresConfirmation = true;
            return Task.FromResult(result);
        }

        if (Match(text, "create warehouse|add warehouse|new warehouse|make warehouse|create wharehouse|add wharehouse|new wharehouse|create store|add store|انشاء مخزن|اضافة مخزن|مخزن جديد", out _))
        {
            result.Intent = "CreateWarehouse";
            result.Action = "CreateWarehouse";
            result.RequiresConfirmation = true;
            return Task.FromResult(result);
        }

        if (Match(text, "update warehouse|edit warehouse|modify warehouse|update wharehouse|edit wharehouse|تحديث مخزن|تعديل مخزن", out _))
        {
            result.Intent = "UpdateWarehouse";
            result.Action = "UpdateWarehouse";
            result.RequiresConfirmation = true;
            return Task.FromResult(result);
        }

        if (Match(text, "delete warehouse|remove warehouse|delete wharehouse|remove wharehouse|حذف مخزن|ازالة مخزن", out _))
        {
            result.Intent = "DeleteWarehouse";
            result.Action = "DeleteWarehouse";
            result.RequiresConfirmation = true;
            return Task.FromResult(result);
        }

        if (Match(text, "purchase order|create purchase order|create po|order from|buy from|make purchase order|make the purchase order|po|p o|talab shira|طلب شراء|امر شراء|انشاء طلب شراء", out _))
        {
            result.Intent = "CreatePurchaseOrder";
            result.Action = "CreatePurchaseOrder";
            result.RequiresConfirmation = true;
            result.ConfirmationMessage = "Create purchase order?";
            ExtractOrderPayload(text, result, "from", "supplierName");
            return Task.FromResult(result);
        }

        if (Match(text, "sales order|salse order|sell order|cell order|cells order|create sales order|create sale order|create sell order|create cell order|create cells order|create so|sell to|make sales order|make the sales order|talab bay|طلب بيع|امر بيع|انشاء طلب بيع", out _))
        {
            result.Intent = "CreateSalesOrder";
            result.Action = "CreateSalesOrder";
            result.RequiresConfirmation = true;
            result.ConfirmationMessage = "Create sales order?";
            ExtractOrderPayload(text, result, "to", "customerName");
            return Task.FromResult(result);
        }

        if (Match(text, "add product|create product|new product|make product|انشاء منتج|اضافة منتج|منتج جديد", out _))
        {
            result.Intent = "CreateProduct";
            result.Action = "CreateProduct";
            result.RequiresConfirmation = true;
            result.ConfirmationMessage = "Create product?";
            ExtractProductPayload(text, result);
            return Task.FromResult(result);
        }

        if (Match(text, "update product|edit product|modify product|products update|update products|product update|تحديث منتج|تعديل منتج", out _))
        {
            result.Intent = "UpdateProduct";
            result.Action = "UpdateProduct";
            result.RequiresConfirmation = true;
            return Task.FromResult(result);
        }

        if (Match(text, "delete product|remove product|حذف منتج|ازالة منتج", out _))
        {
            result.Intent = "DeleteProduct";
            result.Action = "DeleteProduct";
            result.RequiresConfirmation = true;
            return Task.FromResult(result);
        }

        if (Match(text, "inventory update|update inventory|adjust inventory|stock adjustment|stock update|update stock|inventory adjust|تحديث المخزون|تعديل المخزون|تحديث الستوك|تعديل الستوك", out _))
        {
            result.Intent = "InventoryUpdate";
            result.Action = "InventoryUpdate";
            result.RequiresConfirmation = true;
            result.ConfirmationMessage = "Apply inventory update?";
            return Task.FromResult(result);
        }

        if (Match(text, "price update|update price|change price|new price|update product price|purchase price update|update purchase price|update cost price|sale price update|update sale price|selling price update|تحديث السعر|تغيير السعر|تحديث سعر المنتج|تحديث سعر الشراء|تحديث سعر البيع|تحديث التكلفة", out _))
        {
            result.Intent = "PriceUpdate";
            result.Action = "PriceUpdate";
            if (Match(text, "purchase price|cost price|buy price|سعر الشراء|التكلفة", out _))
                result.Parameters["priceType"] = "purchase";
            else if (Match(text, "sale price|selling price|sell price|سعر البيع", out _))
                result.Parameters["priceType"] = "sale";
            result.RequiresConfirmation = true;
            result.ConfirmationMessage = "Apply product price update?";
            return Task.FromResult(result);
        }

        if (Match(text, "check stock|stock level|how much stock|how many stock|available stock|stock query|tell me about stock|tell me about the stock|stock info|stock status|stok|stoke|makhzon|المخزون|الستوك|كم المخزون|كمية المخزون|كم المتوفر", out _))
        {
            result.Intent = "StockQuery";
            result.Action = "StockQuery";
            return Task.FromResult(result);
        }

        if (Match(text, "how much profit|profit today|today's profit", out _))
        {
            result.Intent = "ProfitToday";
            result.Action = "GetProfitToday";
            return Task.FromResult(result);
        }

        if (Match(text, "run out|running out|stock out|will run out|items run out|run out this week", out _))
        {
            result.Intent = "ItemsRunningOut";
            result.Action = "GetItemsRunningOut";
            result.Parameters["horizonDays"] = 7;
            return Task.FromResult(result);
        }

        if (Match(text, "move stock|transfer stock|move from|transfer from", out _))
        {
            result.Intent = "MoveStock";
            result.Action = "TransferStock";
            result.RequiresConfirmation = true;
            result.ConfirmationMessage = "Confirm stock transfer?";
            ExtractLocationPair(text, result);
            return Task.FromResult(result);
        }

        if (Match(text, "slow products|slow moving|slowest", out _))
        {
            result.Intent = "SlowProducts";
            result.Action = "GetSlowProducts";
            return Task.FromResult(result);
        }

        if (Match(text, "generate invoice|genrate invoice|invoice pdf|sales invoice|print invoice|receipt invoice|generate invoices|invoice|invoices|فاتورة|طباعة فاتورة", out _))
        {
            result.Intent = "GenerateInvoice";
            result.Action = "GenerateInvoice";
            return Task.FromResult(result);
        }

        if (Match(text, "generate purchase order pdf|purchase order pdf|po pdf|print purchase order|طباعة امر شراء", out _))
        {
            result.Intent = "GeneratePurchaseOrderPdf";
            result.Action = "GeneratePurchaseOrderPdf";
            return Task.FromResult(result);
        }

        if (Match(text, "sales report|report sales|sales analytics|تقرير المبيعات", out _))
        {
            result.Intent = "SalesReport";
            result.Action = "SalesReport";
            return Task.FromResult(result);
        }

        if (Match(text, "purchase report|report purchase|تقرير المشتريات", out _))
        {
            result.Intent = "PurchaseReport";
            result.Action = "PurchaseReport";
            return Task.FromResult(result);
        }

        if (Match(text, "inventory report|stock report|تقرير المخزون", out _))
        {
            result.Intent = "InventoryReport";
            result.Action = "InventoryReport";
            return Task.FromResult(result);
        }

        if (Match(text, "profit and loss|profit loss|income statement|pnl|p&l|الارباح والخسائر|تقرير الارباح", out _))
        {
            result.Intent = "ProfitLossReport";
            result.Action = "ProfitLossReport";
            return Task.FromResult(result);
        }

        if (Match(text, "balance sheet|statement of financial position|ledger|general ledger|trial balance|الميزانية|دفتر الاستاذ|الدفتر العام", out _))
        {
            result.Intent = "BalanceSheetReport";
            result.Action = "BalanceSheetReport";
            return Task.FromResult(result);
        }

        if (Match(text, "report|reports|generate report|genrate report|make report|rpeort|repot|reporting", out _))
        {
            result.Intent = "ReportMenu";
            result.Action = "ReportMenu";
            return Task.FromResult(result);
        }

        if (Match(text, "customer|customers", out _))
        {
            result.Intent = "CustomerCrudHelp";
            result.Action = "CustomerCrudHelp";
            return Task.FromResult(result);
        }

        if (Match(text, "supplier|suppliers|suplier|supliers|vendor|vendors", out _))
        {
            result.Intent = "SupplierCrudHelp";
            result.Action = "SupplierCrudHelp";
            return Task.FromResult(result);
        }

        if (Match(text, "warehouse|warehouses|wharehouse|wharehouses|store|stores", out _))
        {
            result.Intent = "WarehouseCrudHelp";
            result.Action = "WarehouseCrudHelp";
            return Task.FromResult(result);
        }

        // Generic entity shortcuts from voice/chat ("product", "products")
        if (Match(text, "product|products", out _))
        {
            result.Intent = "ProductCrudHelp";
            result.Action = "ProductCrudHelp";
            return Task.FromResult(result);
        }

        result.Intent = "Unknown";
        return Task.FromResult(result);
    }

    private static void ExtractOrderPayload(string text, CopilotIntentResult result, string partyToken, string partyKey)
    {
        var orderMatch = Regex.Match(text, $@"for\s+(\d+)\s+(.+?)\s+{partyToken}\s+(.+)$", RegexOptions.IgnoreCase);
        if (orderMatch.Success)
        {
            result.Parameters["quantity"] = int.Parse(orderMatch.Groups[1].Value);
            result.Parameters["productName"] = orderMatch.Groups[2].Value.Trim();
            result.Parameters[partyKey] = orderMatch.Groups[3].Value.Trim();
            return;
        }

        var qtyOnlyMatch = Regex.Match(text, @"for\s+(\d+)\s+(.+)$", RegexOptions.IgnoreCase);
        if (qtyOnlyMatch.Success)
        {
            result.Parameters["quantity"] = int.Parse(qtyOnlyMatch.Groups[1].Value);
            result.Parameters["productName"] = qtyOnlyMatch.Groups[2].Value.Trim();
        }
    }

    private static void ExtractProductPayload(string text, CopilotIntentResult result)
    {
        // Example: add product gaming laptop sku GL-10 sale 1200 purchase 900
        var match = Regex.Match(text, @"(?:add|create|new)\s+product\s+(.+?)(?:\s+sku\s+([A-Za-z0-9\-_]+))?(?:\s+sale\s+([0-9]+(?:\.[0-9]+)?))?(?:\s+purchase\s+([0-9]+(?:\.[0-9]+)?))?$", RegexOptions.IgnoreCase);
        if (match.Success)
        {
            result.Parameters["productName"] = match.Groups[1].Value.Trim();
            if (!string.IsNullOrWhiteSpace(match.Groups[2].Value))
                result.Parameters["sku"] = match.Groups[2].Value.Trim();
            if (!string.IsNullOrWhiteSpace(match.Groups[3].Value) && decimal.TryParse(match.Groups[3].Value, out var sale))
                result.Parameters["salePrice"] = sale;
            if (!string.IsNullOrWhiteSpace(match.Groups[4].Value) && decimal.TryParse(match.Groups[4].Value, out var purchase))
                result.Parameters["purchasePrice"] = purchase;
        }
    }

    private static void ExtractLocationPair(string text, CopilotIntentResult result)
    {
        var match = Regex.Match(text, @"(?:from|at)\s*([A-Za-z0-9\-]+)\s*(?:to|into)\s*([A-Za-z0-9\-]+)", RegexOptions.IgnoreCase);
        if (match.Success)
        {
            result.Parameters["fromLocation"] = match.Groups[1].Value.Trim();
            result.Parameters["toLocation"] = match.Groups[2].Value.Trim();
        }
    }

    private static void ExtractTailValue(string text, CopilotIntentResult result, string key, params string[] markers)
    {
        if (string.IsNullOrWhiteSpace(text) || markers == null || markers.Length == 0) return;

        foreach (var marker in markers)
        {
            if (string.IsNullOrWhiteSpace(marker)) continue;
            var pattern = $@"\b{Regex.Escape(marker)}\b\s+(.+)$";
            var match = Regex.Match(text, pattern, RegexOptions.IgnoreCase);
            if (match.Success)
            {
                var value = match.Groups[1].Value.Trim();
                if (!string.IsNullOrWhiteSpace(value))
                {
                    result.Parameters[key] = value;
                    return;
                }
            }
        }
    }

    private static bool Match(string text, string patterns, out string? matched)
    {
        matched = null;
        foreach (var p in patterns.Split('|'))
        {
            var token = p.Trim();
            if (TokenMatch(text, token))
            {
                matched = token;
                return true;
            }
        }
        return false;
    }

    private static bool TokenMatch(string text, string token)
    {
        if (string.IsNullOrWhiteSpace(text) || string.IsNullOrWhiteSpace(token))
            return false;

        // Prevent short tokens (hi/po/so) from matching inside other words (e.g., "t-shirt").
        if (IsAsciiWord(token) && token.Length <= 3)
        {
            return Regex.IsMatch(text, $@"\b{Regex.Escape(token)}\b", RegexOptions.IgnoreCase);
        }

        return text.Contains(token, StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsAsciiWord(string value)
    {
        foreach (var ch in value)
        {
            if (!((ch >= 'a' && ch <= 'z') || (ch >= 'A' && ch <= 'Z') || (ch >= '0' && ch <= '9')))
                return false;
        }
        return value.Length > 0;
    }

    private static string NormalizeText(string input)
    {
        var text = (input ?? string.Empty).Trim().ToLowerInvariant();
        if (string.IsNullOrWhiteSpace(text)) return text;

        text = text
            .Replace('أ', 'ا')
            .Replace('إ', 'ا')
            .Replace('آ', 'ا')
            .Replace('ٱ', 'ا')
            .Replace('ى', 'ي')
            .Replace('ؤ', 'و')
            .Replace('ئ', 'ي')
            .Replace('ة', 'ه');

        text = Regex.Replace(text, @"[\u064B-\u065F\u0670]", string.Empty);
        text = Regex.Replace(text, @"[^\p{L}\p{Nd}\s]", " ");
        text = Regex.Replace(text, @"\s+", " ").Trim();

        // Canonicalize common English typos from voice/text input.
        text = CanonicalizeTypos(text);

        return text;
    }

    private static string CanonicalizeTypos(string text)
    {
        if (string.IsNullOrWhiteSpace(text)) return text;

        var typoMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["gerate"] = "generate",
            ["genrate"] = "generate",
            ["rpeort"] = "report",
            ["repot"] = "report",
            ["detials"] = "details",
            ["deatils"] = "details",
            ["inovice"] = "invoice",
            ["invocie"] = "invoice",
            ["invoce"] = "invoice",
            ["sunpplier"] = "supplier",
            ["suplier"] = "supplier",
            ["supliers"] = "suppliers",
            ["supllier"] = "supplier",
            ["suppllier"] = "supplier",
            ["custmer"] = "customer",
            ["costumer"] = "customer",
            ["prodcut"] = "product",
            ["prodcuts"] = "products",
            ["wharehouse"] = "warehouse",
            ["warehosue"] = "warehouse",
            ["wherehouse"] = "warehouse",
            ["wharehouses"] = "warehouses"
        };

        foreach (var kv in typoMap)
        {
            text = Regex.Replace(text, $@"\b{Regex.Escape(kv.Key)}\b", kv.Value, RegexOptions.IgnoreCase);
        }

        return text;
    }
}
