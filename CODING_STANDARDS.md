# Coding Standards & Best Practices

This document defines the coding standards, architectural principles, and best practices that **MUST** be followed throughout the Khidmah Inventory Management System project.

## Table of Contents

1. [SOLID Principles](#solid-principles)
2. [Clean Architecture](#clean-architecture)
3. [CQRS Pattern](#cqrs-pattern)
4. [Repository Pattern](#repository-pattern)
5. [Naming Conventions](#naming-conventions)
6. [Code Organization](#code-organization)
7. [Error Handling](#error-handling)
8. [Logging](#logging)
9. [Validation](#validation)
10. [Dependency Injection](#dependency-injection)
11. [Testing Guidelines](#testing-guidelines)
12. [Code Review Checklist](#code-review-checklist)

---

## SOLID Principles

### 1. Single Responsibility Principle (SRP)
**Each class should have only one reason to change.**

✅ **DO:**
```csharp
// Good: Each class has a single responsibility
public class FileValidationService : IFileValidationService
{
    public void ValidateFile(IFormFile file, long maxSize, string[] allowedExtensions) { }
}

public class FileStorageService : IFileStorageService
{
    public Task<string> SaveFileAsync(IFormFile file, string subdirectory) { }
}
```

❌ **DON'T:**
```csharp
// Bad: Class does too many things
public class FileService
{
    public void ValidateFile() { }
    public Task<string> SaveFile() { }
    public Task DeleteFile() { }
    public void SendEmail() { } // Wrong responsibility!
}
```

### 2. Open/Closed Principle (OCP)
**Classes should be open for extension but closed for modification.**

✅ **DO:**
```csharp
// Good: Use interfaces and dependency injection
public interface IFileStorageService
{
    Task<string> SaveFileAsync(IFormFile file, string subdirectory);
}

// Can extend with new implementations without modifying existing code
public class AzureBlobStorageService : IFileStorageService { }
public class LocalFileStorageService : IFileStorageService { }
```

❌ **DON'T:**
```csharp
// Bad: Modifying existing code to add new features
public class FileStorageService
{
    public Task<string> SaveFileAsync(IFormFile file, string subdirectory)
    {
        if (storageType == "Azure") { }
        else if (storageType == "Local") { }
        else if (storageType == "S3") { } // Adding new type requires modification
    }
}
```

### 3. Liskov Substitution Principle (LSP)
**Derived classes must be substitutable for their base classes.**

✅ **DO:**
```csharp
// Good: Derived class can replace base class without breaking functionality
public abstract class BaseRepository<T> : IRepository<T>
{
    public abstract Task<T> GetByIdAsync(Guid id);
}

public class ProductRepository : BaseRepository<Product>
{
    public override Task<Product> GetByIdAsync(Guid id) { }
}
```

### 4. Interface Segregation Principle (ISP)
**Clients should not be forced to depend on interfaces they don't use.**

✅ **DO:**
```csharp
// Good: Small, focused interfaces
public interface IFileValidationService
{
    void ValidateFile(IFormFile file, long maxSize, string[] allowedExtensions);
}

public interface IFileStorageService
{
    Task<string> SaveFileAsync(IFormFile file, string subdirectory);
}
```

❌ **DON'T:**
```csharp
// Bad: Large interface forcing clients to implement unused methods
public interface IFileService
{
    void ValidateFile();
    Task<string> SaveFile();
    Task DeleteFile();
    void SendEmail(); // Not all clients need this
    void GenerateThumbnail(); // Not all clients need this
}
```

### 5. Dependency Inversion Principle (DIP)
**Depend on abstractions, not concretions.**

✅ **DO:**
```csharp
// Good: Depend on interface
public class SaveUserThemeCommandHandler
{
    private readonly IThemeRepository _themeRepository; // Interface, not concrete class
    
    public SaveUserThemeCommandHandler(IThemeRepository themeRepository)
    {
        _themeRepository = themeRepository;
    }
}
```

❌ **DON'T:**
```csharp
// Bad: Depend on concrete class
public class SaveUserThemeCommandHandler
{
    private readonly ThemeRepository _themeRepository; // Concrete class
    
    public SaveUserThemeCommandHandler(ThemeRepository themeRepository)
    {
        _themeRepository = themeRepository;
    }
}
```

---

## Clean Architecture

### Layer Responsibilities

#### Domain Layer (`Khidmah_Inventory.Domain`)
- **Purpose**: Core business logic and entities
- **Dependencies**: None (pure business logic)
- **Contains**: Entities, Value Objects, Domain Events, Domain Interfaces

**Rules:**
- ✅ No dependencies on other layers
- ✅ No infrastructure concerns (no EF Core, no HTTP)
- ✅ Pure C# classes

#### Application Layer (`Khidmah_Inventory.Application`)
- **Purpose**: Business logic orchestration
- **Dependencies**: Domain layer only
- **Contains**: Commands, Queries, Handlers, DTOs, Interfaces, Validators

**Rules:**
- ✅ Depend only on Domain layer
- ✅ Define interfaces (repositories, services)
- ✅ Implement business use cases
- ❌ No infrastructure implementations

#### Infrastructure Layer (`Khidmah_Inventory.Infrastructure`)
- **Purpose**: External concerns implementation
- **Dependencies**: Application and Domain layers
- **Contains**: Repositories, DbContext, External Services, Configurations

**Rules:**
- ✅ Implement Application layer interfaces
- ✅ Handle external concerns (database, file system, APIs)
- ✅ Entity Framework configurations

#### API Layer (`Khidmah_Inventory.API`)
- **Purpose**: Web API entry point
- **Dependencies**: Application and Infrastructure layers
- **Contains**: Controllers, Middleware, Configuration

**Rules:**
- ✅ Thin controllers (delegate to MediatR)
- ✅ HTTP concerns only
- ❌ No business logic

---

## CQRS Pattern

### Commands (Write Operations)
- **Naming**: `[Action][Entity]Command` (e.g., `CreateProductCommand`, `UpdateUserCommand`)
- **Location**: `Application/Features/[Feature]/Commands/[CommandName]/`
- **Returns**: `Result<T>` or `Result`

**Structure:**
```
Commands/
  CreateProduct/
    CreateProductCommand.cs
    CreateProductCommandHandler.cs
    CreateProductCommandValidator.cs
```

**Example:**
```csharp
// Command
public class CreateProductCommand : IRequest<Result<ProductDto>>
{
    public string Name { get; set; } = string.Empty;
    public string SKU { get; set; } = string.Empty;
}

// Handler
public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Result<ProductDto>>
{
    private readonly IProductRepository _repository;
    private readonly ILogger<CreateProductCommandHandler> _logger;

    public CreateProductCommandHandler(
        IProductRepository repository,
        ILogger<CreateProductCommandHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<Result<ProductDto>> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        // Business logic here
        var product = await _repository.CreateAsync(request, cancellationToken);
        _logger.LogInformation("Product created: {ProductId}", product.Id);
        
        return Result<ProductDto>.Success(mappedDto);
    }
}

// Validator
public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Product name is required")
            .MaximumLength(200)
            .WithMessage("Product name must not exceed 200 characters");

        RuleFor(x => x.SKU)
            .NotEmpty()
            .WithMessage("SKU is required");
    }
}
```

### Queries (Read Operations)
- **Naming**: `Get[Entity]Query` or `Get[Entity]ListQuery`
- **Location**: `Application/Features/[Feature]/Queries/[QueryName]/`
- **Returns**: `Result<T>`

**Structure:**
```
Queries/
  GetProduct/
    GetProductQuery.cs
    GetProductQueryHandler.cs
  GetProductList/
    GetProductListQuery.cs
    GetProductListQueryHandler.cs
```

**Example:**
```csharp
// Query
public class GetProductQuery : IRequest<Result<ProductDto>>
{
    public Guid Id { get; set; }
}

// Handler
public class GetProductQueryHandler : IRequestHandler<GetProductQuery, Result<ProductDto>>
{
    private readonly IProductRepository _repository;
    private readonly ILogger<GetProductQueryHandler> _logger;

    public GetProductQueryHandler(
        IProductRepository repository,
        ILogger<GetProductQueryHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<Result<ProductDto>> Handle(GetProductQuery request, CancellationToken cancellationToken)
    {
        var product = await _repository.GetByIdAsync(request.Id, cancellationToken);
        
        if (product == null)
        {
            return Result<ProductDto>.Failure("Product not found");
        }

        return Result<ProductDto>.Success(mappedDto);
    }
}
```

---

## Repository Pattern

### Interface (Application Layer)
```csharp
// Application/Common/Interfaces/IProductRepository.cs
public interface IProductRepository
{
    Task<ProductDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<ProductDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<ProductDto> CreateAsync(CreateProductCommand command, CancellationToken cancellationToken = default);
    Task<ProductDto> UpdateAsync(UpdateProductCommand command, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
```

### Implementation (Infrastructure Layer)
```csharp
// Infrastructure/Repositories/ProductRepository.cs
public class ProductRepository : IProductRepository
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IMapper _mapper;

    public ProductRepository(
        IApplicationDbContext context,
        ICurrentUserService currentUser,
        IMapper mapper)
    {
        _context = context;
        _currentUser = currentUser;
        _mapper = mapper;
    }

    public async Task<ProductDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var product = await _context.Products
            .Where(p => p.Id == id && p.CompanyId == _currentUser.CompanyId && !p.IsDeleted)
            .FirstOrDefaultAsync(cancellationToken);

        return _mapper.Map<ProductDto>(product);
    }

    // ... other methods
}
```

**Rules:**
- ✅ Repository interface in Application layer
- ✅ Repository implementation in Infrastructure layer
- ✅ Use dependency injection for dependencies
- ✅ Always filter by CompanyId for multi-tenancy
- ✅ Always check IsDeleted for soft deletes

---

## Naming Conventions

### Classes
- **Commands**: `[Action][Entity]Command` (e.g., `CreateProductCommand`)
- **Queries**: `Get[Entity]Query` or `Get[Entity]ListQuery` (e.g., `GetProductQuery`)
- **Handlers**: `[Command/Query]Handler` (e.g., `CreateProductCommandHandler`)
- **Validators**: `[Command]Validator` (e.g., `CreateProductCommandValidator`)
- **Repositories**: `I[Entity]Repository` (interface), `[Entity]Repository` (implementation)
- **Services**: `I[Entity]Service` (interface), `[Entity]Service` (implementation)
- **DTOs**: `[Entity]Dto` (e.g., `ProductDto`)
- **Controllers**: `[Entity]Controller` (e.g., `ProductController`)

### Methods
- **Async methods**: End with `Async` (e.g., `GetByIdAsync`)
- **Boolean methods**: Use `Is`, `Has`, `Can` prefix (e.g., `IsValid`, `HasPermission`)
- **Commands**: Use verbs (e.g., `Create`, `Update`, `Delete`)
- **Queries**: Use `Get`, `Find`, `List` (e.g., `GetById`, `FindByName`)

### Variables
- **Private fields**: Use `_camelCase` (e.g., `_repository`)
- **Properties**: Use `PascalCase` (e.g., `ProductName`)
- **Local variables**: Use `camelCase` (e.g., `productId`)
- **Constants**: Use `PascalCase` (e.g., `MaxFileSize`)

### Files and Folders
- **One class per file**: File name matches class name
- **Folders**: Use PascalCase (e.g., `Features/Products/Commands/`)

---

## Code Organization

### Project Structure
```
Khidmah_Inventory.API/
├── Controllers/
│   ├── BaseApiController.cs
│   └── [Entity]Controller.cs
├── Middleware/
│   └── [MiddlewareName].cs
└── Program.cs

Khidmah_Inventory.Application/
├── Common/
│   ├── Behaviors/
│   ├── Interfaces/
│   ├── Mappings/
│   └── Models/
└── Features/
    └── [Feature]/
        ├── Commands/
        ├── Queries/
        └── Models/

Khidmah_Inventory.Infrastructure/
├── Data/
│   ├── Configurations/
│   └── Interceptors/
├── Repositories/
└── Services/

Khidmah_Inventory.Domain/
├── Common/
└── Entities/
```

### File Organization
- **Group related code**: Keep related classes together
- **Use regions sparingly**: Only for very large files
- **Order members**: Fields → Properties → Constructors → Methods

---

## Error Handling

### Result Pattern
Always use `Result<T>` for operation results:

```csharp
// Success
return Result<ProductDto>.Success(productDto);

// Failure
return Result<ProductDto>.Failure("Product not found");
return Result<ProductDto>.Failure("Invalid input", "Name is required");
```

### Exception Handling
- **Use pipeline behaviors** for global exception handling
- **Don't catch exceptions in handlers** unless you can handle them meaningfully
- **Let ArgumentException bubble up** - validation behavior will catch it
- **Log exceptions** at appropriate levels

```csharp
// ExceptionHandlingBehavior handles exceptions globally
// Handlers should focus on business logic
public async Task<Result<ProductDto>> Handle(CreateProductCommand request, CancellationToken cancellationToken)
{
    // No try-catch needed - behavior handles it
    var product = await _repository.CreateAsync(request, cancellationToken);
    return Result<ProductDto>.Success(mappedDto);
}
```

### Controller Error Handling
Use `BaseApiController.HandleResult()`:

```csharp
[HttpPost]
public async Task<IActionResult> Create([FromBody] CreateProductCommand command)
{
    var result = await Mediator.Send(command);
    return HandleResult(result);
}
```

---

## Logging

### Log Levels
- **Trace**: Very detailed information (rarely used)
- **Debug**: Detailed information for debugging
- **Information**: General informational messages (successful operations)
- **Warning**: Warning messages (validation errors, recoverable errors)
- **Error**: Error messages (exceptions, failures)
- **Critical**: Critical failures (system failures)

### Logging Guidelines
```csharp
public class CreateProductCommandHandler
{
    private readonly ILogger<CreateProductCommandHandler> _logger;

    public async Task<Result<ProductDto>> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating product: {ProductName}", request.Name);
        
        try
        {
            var product = await _repository.CreateAsync(request, cancellationToken);
            _logger.LogInformation("Product created successfully: {ProductId}", product.Id);
            return Result<ProductDto>.Success(mappedDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create product: {ProductName}", request.Name);
            throw; // Let behavior handle it
        }
    }
}
```

**Rules:**
- ✅ Log at Information level for successful operations
- ✅ Log at Warning level for validation errors
- ✅ Log at Error level for exceptions
- ✅ Use structured logging with parameters
- ✅ Don't log sensitive information (passwords, tokens)

---

## Validation

### FluentValidation
Always create validators for commands:

```csharp
public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Product name is required")
            .MaximumLength(200)
            .WithMessage("Product name must not exceed 200 characters");

        RuleFor(x => x.SKU)
            .NotEmpty()
            .WithMessage("SKU is required")
            .Matches(@"^[A-Z0-9-]+$")
            .WithMessage("SKU must contain only uppercase letters, numbers, and hyphens");
    }
}
```

**Rules:**
- ✅ Create validator for every command
- ✅ Use descriptive error messages
- ✅ Validate all required fields
- ✅ Validate format and constraints

---

## Dependency Injection

### Registration
- **Interfaces**: Register in `Infrastructure/DependencyInjection.cs`
- **Application services**: Register in `Application/DependencyInjection.cs`
- **Lifetime**: Use `Scoped` for repositories and services, `Transient` for behaviors

```csharp
// Infrastructure/DependencyInjection.cs
services.AddScoped<IProductRepository, ProductRepository>();
services.AddScoped<IFileStorageService, FileStorageService>();

// Application/DependencyInjection.cs
services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ExceptionHandlingBehavior<,>));
```

### Constructor Injection
Always use constructor injection:

```csharp
public class CreateProductCommandHandler
{
    private readonly IProductRepository _repository;
    private readonly ILogger<CreateProductCommandHandler> _logger;

    public CreateProductCommandHandler(
        IProductRepository repository,
        ILogger<CreateProductCommandHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }
}
```

---

## Testing Guidelines

### Unit Tests
- Test handlers in isolation
- Mock dependencies
- Test success and failure scenarios

```csharp
[Fact]
public async Task Handle_ValidCommand_ReturnsSuccess()
{
    // Arrange
    var repository = new Mock<IProductRepository>();
    var logger = new Mock<ILogger<CreateProductCommandHandler>>();
    var handler = new CreateProductCommandHandler(repository.Object, logger.Object);
    var command = new CreateProductCommand { Name = "Test Product" };

    // Act
    var result = await handler.Handle(command, CancellationToken.None);

    // Assert
    Assert.True(result.Succeeded);
    Assert.NotNull(result.Data);
}
```

### Integration Tests
- Test API endpoints
- Test database operations
- Test multi-tenancy isolation

---

## Code Review Checklist

Before submitting code for review, ensure:

- [ ] Follows SOLID principles
- [ ] Follows Clean Architecture layers
- [ ] Uses CQRS pattern correctly
- [ ] Repository pattern implemented correctly
- [ ] Naming conventions followed
- [ ] Error handling with Result pattern
- [ ] Logging added where appropriate
- [ ] Validation added for commands
- [ ] Dependency injection used correctly
- [ ] No business logic in controllers
- [ ] No infrastructure concerns in Application layer
- [ ] Multi-tenancy considered (CompanyId filtering)
- [ ] Soft delete considered (IsDeleted checking)
- [ ] Code is testable
- [ ] No hardcoded values (use configuration)
- [ ] Async/await used correctly
- [ ] CancellationToken used in async methods

---

## Examples

### Complete Feature Example

**1. Command:**
```csharp
// Application/Features/Products/Commands/CreateProduct/CreateProductCommand.cs
public class CreateProductCommand : IRequest<Result<ProductDto>>
{
    public string Name { get; set; } = string.Empty;
    public string SKU { get; set; } = string.Empty;
    public decimal Price { get; set; }
}
```

**2. Validator:**
```csharp
// Application/Features/Products/Commands/CreateProduct/CreateProductCommandValidator.cs
public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.SKU).NotEmpty();
        RuleFor(x => x.Price).GreaterThan(0);
    }
}
```

**3. Handler:**
```csharp
// Application/Features/Products/Commands/CreateProduct/CreateProductCommandHandler.cs
public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Result<ProductDto>>
{
    private readonly IProductRepository _repository;
    private readonly ILogger<CreateProductCommandHandler> _logger;

    public CreateProductCommandHandler(
        IProductRepository repository,
        ILogger<CreateProductCommandHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<Result<ProductDto>> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var product = await _repository.CreateAsync(request, cancellationToken);
        _logger.LogInformation("Product created: {ProductId}", product.Id);
        return Result<ProductDto>.Success(product);
    }
}
```

**4. Controller:**
```csharp
// API/Controllers/ProductsController.cs
[HttpPost]
public async Task<IActionResult> Create([FromBody] CreateProductCommand command)
{
    var result = await Mediator.Send(command);
    return HandleResult(result);
}
```

---

## Summary

This document defines the standards for:
- ✅ **SOLID Principles**: Single Responsibility, Open/Closed, Liskov Substitution, Interface Segregation, Dependency Inversion
- ✅ **Clean Architecture**: Proper layer separation and dependencies
- ✅ **CQRS Pattern**: Commands for writes, Queries for reads
- ✅ **Repository Pattern**: Data access abstraction
- ✅ **Naming Conventions**: Consistent naming across the codebase
- ✅ **Error Handling**: Result pattern and exception handling
- ✅ **Logging**: Structured logging with appropriate levels
- ✅ **Validation**: FluentValidation for commands
- ✅ **Dependency Injection**: Constructor injection and proper registration

**All code in this project MUST follow these standards.**

