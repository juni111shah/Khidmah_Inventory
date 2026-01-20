# Khidmah Inventory Management System

A comprehensive multi-tenant inventory management system built with .NET 8 and Angular.

## Architecture

This project follows **Clean Architecture** principles with a clear separation of concerns:

### Project Structure

```
Khidmah_Inventory/
├── Khidmah_Inventory.Domain/          # Domain Layer (Entities, Value Objects, Domain Events)
├── Khidmah_Inventory.Application/     # Application Layer (CQRS, Use Cases, DTOs)
├── Khidmah_Inventory.Infrastructure/  # Infrastructure Layer (Data Access, External Services)
├── Khidmah_Inventory.API/            # Presentation Layer (Controllers, Middleware)
└── khidmah_inventory.client/         # Angular Frontend
```

### Code Flow

**Repository => CQRS => Controller**

1. **Domain Layer**: Contains business entities, value objects, and domain logic
2. **Application Layer**: Contains CQRS commands/queries, handlers, and DTOs
3. **Infrastructure Layer**: Contains EF Core DbContext, repositories, and external service implementations
4. **API Layer**: Contains controllers that use MediatR to dispatch CQRS commands/queries

### Key Features

- ✅ **Multi-Tenancy**: Complete data isolation by CompanyId
- ✅ **Authentication & Authorization**: JWT-based auth with RBAC
- ✅ **CQRS Pattern**: Commands and Queries separation using MediatR
- ✅ **Clean Architecture**: Proper layer separation and dependency inversion
- ✅ **Audit Trail**: Automatic tracking of created/updated/deleted records
- ✅ **Soft Delete**: Entities are soft-deleted, not physically removed

## Modules

### 1. Authentication & User Management
- User registration (Admin only)
- Login/Logout/Refresh token
- Multi-role user management
- Role-based access control (RBAC)
- Permissions mapping

### 2. Company / Multi-Tenant
- Create companies/businesses
- Assign users to companies
- Data isolation by CompanyId
- Subscription/plan management

### 3. Product Management
- Products CRUD
- Categories (hierarchical)
- Brands
- Units of Measure
- Product variants
- Barcode generation
- Product images

### 4. Warehouse & Location
- Warehouse CRUD
- Warehouse zones
- Bins/locations
- Default warehouse settings

### 5. Inventory / Stock Management
- Opening stock
- Stock In / Stock Out
- Inventory adjustments
- Stock transfers between warehouses
- Batch/lot tracking
- Expiry date management
- Stock valuation (FIFO/LIFO/WAVG)

### 6. Purchase Management
- Supplier management
- Purchase requisition
- Purchase order (PO)
- Goods Received Note (GRN)
- Purchase invoice
- Purchase return

### 7. Sales Management
- Customer management
- Quotations
- Sales orders (SO)
- Delivery orders / challan
- Sales invoices
- Sales returns

### 8. POS (Point of Sale)
- Counter sales
- Barcode/QR scanning
- Quick checkout
- Cash/POS/card payment
- Cash register opening/closing

### 9. Accounting & Finance (Basic)
- Chart of accounts mapping
- Purchase invoice posting
- Sales invoice posting
- Tax & VAT settings
- Payment tracking

### 10. Reports & Analytics
- Stock valuation report
- Fast/slow moving product report
- Purchase vs Sales comparison
- Inventory aging report
- Expiry report
- Low stock report
- Profitability report

### 11. Notification & Alert
- Low stock alerts
- Expiry alerts
- Purchase approval notifications
- Sales order status alerts
- Real-time stock changes (SignalR)

### 12. File & Document
- Product images
- PO/SO PDFs
- GRN PDF
- Delivery challan
- Invoice PDFs

### 13. Audit & Logging
- Record every change (who, when, what)
- Track stock value history
- Track login history

### 14. Settings
- Currency configuration
- Company profile
- Business rules
- Email server settings (SMTP)
- Numbering sequences (PO, SO, Invoice)

## Getting Started

### Prerequisites

- .NET 8 SDK
- SQL Server (LocalDB or full instance)
- Node.js and npm
- Angular CLI

### Setup

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd Khidmah_Inventory
   ```

2. **Update connection string**
   - Edit `Khidmah_Inventory.API/appsettings.json`
   - Update the `DefaultConnection` string

3. **Restore packages**
   ```bash
   dotnet restore
   ```

4. **Run database migrations**
   ```bash
   cd Khidmah_Inventory.API
   dotnet ef database update
   ```

5. **Run the API**
   ```bash
   dotnet run --project Khidmah_Inventory.API
   ```

6. **Run the Angular app**
   ```bash
   cd khidmah_inventory.client
   npm install
   npm start
   ```

## API Documentation

Once the API is running, Swagger documentation is available at:
- `https://localhost:5001/swagger`

## Authentication

The API uses JWT Bearer authentication. Include the token in the Authorization header:

```
Authorization: Bearer <your-token>
```

For multi-tenant requests, include the company ID in the header:

```
X-Company-Id: <company-guid>
```

## Development Guidelines

### Adding a New Feature

1. **Domain Layer**: Create entity in `Khidmah_Inventory.Domain/Entities/`
2. **Application Layer**: 
   - Create command/query in `Khidmah_Inventory.Application/Features/[Feature]/`
   - Create handler, validator, and DTOs
3. **Infrastructure Layer**: 
   - Add entity configuration in `Khidmah_Inventory.Infrastructure/Data/Configurations/`
   - Add DbSet to ApplicationDbContext
4. **API Layer**: Create controller in `Khidmah_Inventory.API/Controllers/`

### CQRS Pattern

- **Commands**: For write operations (Create, Update, Delete)
- **Queries**: For read operations (Get, List, Search)

Example:
```csharp
// Command
public class CreateProductCommand : IRequest<Result<ProductDto>>
{
    public string Name { get; set; }
    // ...
}

// Handler
public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Result<ProductDto>>
{
    // Implementation
}

// Controller
[HttpPost]
public async Task<IActionResult> Create([FromBody] CreateProductCommand command)
{
    var result = await Mediator.Send(command);
    return Ok(result);
}
```

## License

[Your License Here]

