namespace Khidmah_Inventory.Application.Features.Reports.Models;

public class CustomReportDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ReportType { get; set; } = string.Empty;
    public string ReportDefinition { get; set; } = string.Empty;
    public bool IsPublic { get; set; }
    public Guid? CreatedByUserId { get; set; }
    public string? CreatedByUserName { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class ReportDefinitionDto
{
    public List<ReportField> Fields { get; set; } = new();
    public List<ReportFilter> Filters { get; set; } = new();
    public List<ReportGroupBy> GroupBy { get; set; } = new();
    public List<ReportSort> Sorts { get; set; } = new();
    public List<ReportFormula> Formulas { get; set; } = new();
}

public class ReportField
{
    public string Name { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty; // String, Number, Date, Currency
    public string DataSource { get; set; } = string.Empty; // Entity field path
    public bool IsVisible { get; set; } = true;
    public int Order { get; set; }
}

public class ReportFilter
{
    public string Field { get; set; } = string.Empty;
    public string Operator { get; set; } = string.Empty; // Equals, Contains, GreaterThan, etc.
    public object? Value { get; set; }
    public string Logic { get; set; } = "AND"; // AND, OR
}

public class ReportGroupBy
{
    public string Field { get; set; } = string.Empty;
    public string Order { get; set; } = "ASC"; // ASC, DESC
}

public class ReportSort
{
    public string Field { get; set; } = string.Empty;
    public string Direction { get; set; } = "ASC"; // ASC, DESC
}

public class ReportFormula
{
    public string Name { get; set; } = string.Empty;
    public string Expression { get; set; } = string.Empty; // e.g., "Sum(Quantity) * Average(Price)"
    public string Type { get; set; } = string.Empty; // Sum, Average, Count, Custom
}

