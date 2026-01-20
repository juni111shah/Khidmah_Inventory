# Clean Architecture Implementation with Repository & CQRS Patterns

## Overview

The backend has been refactored to follow **Clean Architecture** principles with **Repository Pattern** and **CQRS (Command Query Responsibility Segregation)** pattern using MediatR.

## Architecture Layers

### 1. Domain Layer (`Khidmah_Inventory.Domain`)
- **Purpose**: Core business entities and domain logic
- **Contains**: Entities, Value Objects, Domain Events
- **No dependencies** on other layers

### 2. Application Layer (`Khidmah_Inventory.Application`)
- **Purpose**: Business logic and use cases
- **Contains**:
  - **CQRS Commands/Queries**: Write and read operations
  - **Handlers**: Business logic implementation
  - **Validators**: FluentValidation validators
  - **DTOs**: Data Transfer Objects
  - **Interfaces**: Repository and service interfaces
- **Dependencies**: Only Domain layer

### 3. Infrastructure Layer (`Khidmah_Inventory.Infrastructure`)
- **Purpose**: External concerns and implementations
- **Contains**:
  - **Repositories**: Data access implementations
  - **DbContext**: EF Core database context
  - **Services**: External service implementations (Identity, File Storage)
  - **Configurations**: Entity configurations
- **Dependencies**: Application and Domain layers

### 4. API Layer (`Khidmah_Inventory.API`)
- **Purpose**: Web API entry point
- **Contains**:
  - **Controllers**: API endpoints
  - **Middleware**: Custom middleware
  - **Configuration**: Startup and DI
- **Dependencies**: Application and Infrastructure layers

## Repository Pattern

### Interface (Application Layer)
```csharp
// Application/Common/Interfaces/IThemeRepository.cs
public interface IThemeRepository
{
    Task<ThemeDto> GetUserThemeAsync(CancellationToken cancellationToken = default);
    Task<ThemeDto> GetGlobalThemeAsync(CancellationToken cancellationToken = default);
    Task SaveUserThemeAsync(ThemeDto theme, CancellationToken cancellationToken = default);
    Task SaveGlobalThemeAsync(ThemeDto theme, CancellationToken cancellationToken = default);
    Task<string> SaveLogoAsync(IFormFile file, CancellationToken cancellationToken = default);
}
```

### Implementation (Infrastructure Layer)
```csharp
// Infrastructure/Repositories/ThemeRepository.cs
public class ThemeRepository : IThemeRepository
{
    // Implementation details...
}
```

**Benefits**:
- ✅ Separation of concerns
- ✅ Testability (easy to mock)
- ✅ Flexibility to change data access implementation
- ✅ Single Responsibility Principle

## CQRS Pattern

### Commands (Write Operations)
Commands modify state and return results.

**Structure**:
```
Features/Theme/Commands/
  SaveUserTheme/
    SaveUserThemeCommand.cs
    SaveUserThemeCommandHandler.cs
    SaveUserThemeCommandValidator.cs
  SaveGlobalTheme/
    SaveGlobalThemeCommand.cs
    SaveGlobalThemeCommandHandler.cs
    SaveGlobalThemeCommandValidator.cs
  UploadLogo/
    UploadLogoCommand.cs
    UploadLogoCommandHandler.cs
    UploadLogoCommandValidator.cs
```

**Example Command**:
```csharp
public class SaveUserThemeCommand : IRequest<Result<ThemeDto>>
{
    public ThemeDto Theme { get; set; } = null!;
}

public class SaveUserThemeCommandHandler : IRequestHandler<SaveUserThemeCommand, Result<ThemeDto>>
{
    private readonly IThemeRepository _themeRepository;

    public async Task<Result<ThemeDto>> Handle(SaveUserThemeCommand request, CancellationToken cancellationToken)
    {
        await _themeRepository.SaveUserThemeAsync(request.Theme, cancellationToken);
        return Result<ThemeDto>.Success(request.Theme);
    }
}
```

### Queries (Read Operations)
Queries read data without modifying state.

**Structure**:
```
Features/Theme/Queries/
  GetUserTheme/
    GetUserThemeQuery.cs
    GetUserThemeQueryHandler.cs
  GetGlobalTheme/
    GetGlobalThemeQuery.cs
    GetGlobalThemeQueryHandler.cs
```

**Example Query**:
```csharp
public class GetUserThemeQuery : IRequest<Result<ThemeDto>>
{
}

public class GetUserThemeQueryHandler : IRequestHandler<GetUserThemeQuery, Result<ThemeDto>>
{
    private readonly IThemeRepository _themeRepository;

    public async Task<Result<ThemeDto>> Handle(GetUserThemeQuery request, CancellationToken cancellationToken)
    {
        var theme = await _themeRepository.GetUserThemeAsync(cancellationToken);
        return Result<ThemeDto>.Success(theme);
    }
}
```

## Request Flow

```
HTTP Request
    ↓
Controller (API Layer)
    ↓
MediatR.Send(Command/Query)
    ↓
Command/Query Handler (Application Layer)
    ↓
Repository (Infrastructure Layer)
    ↓
File System / Database
```

## Controller Implementation

Controllers are thin and delegate to MediatR:

```csharp
[ApiController]
[Route("api/[controller]")]
public class ThemeController : BaseApiController
{
    [HttpGet("user")]
    public async Task<IActionResult> GetUserTheme()
    {
        var query = new GetUserThemeQuery();
        var result = await Mediator.Send(query);
        
        if (!result.Succeeded)
            return BadRequest(new { errors = result.Errors });
        
        return Ok(result.Data);
    }

    [HttpPost("user")]
    public async Task<IActionResult> SaveUserTheme([FromBody] ThemeDto theme)
    {
        var command = new SaveUserThemeCommand { Theme = theme };
        var result = await Mediator.Send(command);
        
        if (!result.Succeeded)
            return BadRequest(new { errors = result.Errors });
        
        return Ok(result.Data);
    }
}
```

## Dependency Injection

### Application Layer
```csharp
// Application/DependencyInjection.cs
services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
```

### Infrastructure Layer
```csharp
// Infrastructure/DependencyInjection.cs
services.AddScoped<IThemeRepository, ThemeRepository>();
```

## Result Pattern

All handlers return `Result<T>` for consistent error handling:

```csharp
// Success
return Result<ThemeDto>.Success(theme);

// Failure
return Result<ThemeDto>.Failure("Error message");
```

## Validation

FluentValidation is used for command validation:

```csharp
public class SaveUserThemeCommandValidator : AbstractValidator<SaveUserThemeCommand>
{
    public SaveUserThemeCommandValidator()
    {
        RuleFor(x => x.Theme)
            .NotNull()
            .WithMessage("Theme data is required");
    }
}
```

Validation runs automatically via MediatR pipeline behavior.

## Benefits of This Architecture

### 1. **Separation of Concerns**
- Each layer has a single responsibility
- Business logic is isolated from infrastructure

### 2. **Testability**
- Easy to unit test handlers
- Easy to mock repositories
- Controllers are thin and easy to test

### 3. **Maintainability**
- Clear structure and organization
- Easy to locate and modify code
- Changes in one layer don't affect others

### 4. **Scalability**
- Easy to add new features
- Consistent patterns across the codebase
- Can scale read/write operations independently (CQRS)

### 5. **Flexibility**
- Can swap implementations (e.g., different repository implementations)
- Can add cross-cutting concerns via MediatR behaviors
- Can change data access without affecting business logic

## File Structure

```
Khidmah_Inventory.API/
├── Controllers/
│   └── ThemeController.cs          ← Thin controllers using MediatR

Khidmah_Inventory.Application/
├── Common/
│   └── Interfaces/
│       └── IThemeRepository.cs     ← Repository interface
├── Features/
│   └── Theme/
│       ├── Models/
│       │   └── ThemeDto.cs         ← DTOs
│       ├── Commands/
│       │   ├── SaveUserTheme/
│       │   ├── SaveGlobalTheme/
│       │   └── UploadLogo/
│       └── Queries/
│           ├── GetUserTheme/
│           └── GetGlobalTheme/

Khidmah_Inventory.Infrastructure/
├── Repositories/
│   └── ThemeRepository.cs          ← Repository implementation
└── DependencyInjection.cs          ← DI registration
```

## Adding a New Feature

Follow these steps to add a new feature:

1. **Create Repository Interface** (Application layer)
   - `Application/Common/Interfaces/I[Feature]Repository.cs`

2. **Create Repository Implementation** (Infrastructure layer)
   - `Infrastructure/Repositories/[Feature]Repository.cs`

3. **Create Commands/Queries** (Application layer)
   - `Application/Features/[Feature]/Commands/`
   - `Application/Features/[Feature]/Queries/`

4. **Create Handlers** (Application layer)
   - Command/Query handlers with business logic

5. **Create Validators** (Application layer)
   - FluentValidation validators for commands

6. **Create Controller** (API layer)
   - Thin controller using MediatR

7. **Register Dependencies** (Infrastructure layer)
   - Register repository in `DependencyInjection.cs`

## Summary

✅ **Clean Architecture**: Proper layer separation  
✅ **Repository Pattern**: Data access abstraction  
✅ **CQRS Pattern**: Commands for writes, Queries for reads  
✅ **MediatR**: Decouples controllers from business logic  
✅ **Result Pattern**: Consistent error handling  
✅ **Validation**: FluentValidation with automatic pipeline validation  

The codebase now follows industry best practices and is maintainable, testable, and scalable.

