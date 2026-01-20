# Settings Implementation - Complete Guide

This document describes the complete implementation of the settings flow for both backend and frontend.

## Overview

The settings system provides a comprehensive solution for managing all application settings including:
- Company Settings
- User Settings
- System Settings
- Notification Settings
- UI Settings
- Report Settings

## Backend Implementation

### 1. Domain Layer

#### Settings Entity
**Location**: `Khidmah_Inventory.Domain/Entities/Settings.cs`

- Stores settings as JSON in the database
- Supports multiple settings types per company
- Uses `SettingsType` and `SettingsKey` for identification
- Inherits from `BaseEntity` for multi-tenancy and audit

### 2. Application Layer

#### DTOs
**Location**: `Khidmah_Inventory.Application/Features/Settings/Models/`

- `CompanySettingsDto`
- `UserSettingsDto`
- `SystemSettingsDto`
- `NotificationSettingsDto`
- `UISettingsDto`
- `ReportSettingsDto`
- `SettingsInfoDto`

#### Repository Interface
**Location**: `Khidmah_Inventory.Application/Common/Interfaces/ISettingsRepository.cs`

```csharp
public interface ISettingsRepository
{
    Task<T?> GetSettingsAsync<T>(Guid companyId, string settingsType, string settingsKey, ...);
    Task SaveSettingsAsync<T>(Guid companyId, string settingsType, string settingsKey, T settings, ...);
    Task<bool> DeleteSettingsAsync(Guid companyId, string settingsType, string settingsKey, ...);
    Task<List<SettingsInfoDto>> GetAllSettingsAsync(Guid companyId, string? settingsType = null, ...);
}
```

#### CQRS Commands & Queries

**Queries** (Read operations):
- `GetCompanySettingsQuery` + Handler
- `GetUserSettingsQuery` + Handler
- `GetSystemSettingsQuery` + Handler
- `GetNotificationSettingsQuery` + Handler
- `GetUISettingsQuery` + Handler
- `GetReportSettingsQuery` + Handler

**Commands** (Write operations):
- `SaveCompanySettingsCommand` + Handler + Validator
- `SaveUserSettingsCommand` + Handler + Validator
- `SaveSystemSettingsCommand` + Handler + Validator
- `SaveNotificationSettingsCommand` + Handler
- `SaveUISettingsCommand` + Handler
- `SaveReportSettingsCommand` + Handler

### 3. Infrastructure Layer

#### Repository Implementation
**Location**: `Khidmah_Inventory.Infrastructure/Repositories/SettingsRepository.cs`

- Serializes/deserializes settings to/from JSON
- Handles create/update logic (upsert pattern)
- Filters by CompanyId for multi-tenancy
- Returns default settings if not found

#### Entity Configuration
**Location**: `Khidmah_Inventory.Infrastructure/Data/Configurations/SettingsConfiguration.cs`

- Unique index on `CompanyId`, `SettingsType`, `SettingsKey`
- Query filter for soft deletes

### 4. API Layer

#### Settings Controller
**Location**: `Khidmah_Inventory.API/Controllers/SettingsController.cs`

**Endpoints**:
- `GET /api/settings/company` - Get company settings
- `POST /api/settings/company` - Save company settings
- `GET /api/settings/user` - Get user settings
- `POST /api/settings/user` - Save user settings
- `GET /api/settings/system` - Get system settings
- `POST /api/settings/system` - Save system settings
- `GET /api/settings/notifications` - Get notification settings
- `POST /api/settings/notifications` - Save notification settings
- `GET /api/settings/ui` - Get UI settings
- `POST /api/settings/ui` - Save UI settings
- `GET /api/settings/reports` - Get report settings
- `POST /api/settings/reports` - Save report settings

All endpoints:
- Require authentication (`[Authorize]`)
- Return standardized `ApiResponse<T>` structure
- Use CQRS pattern via MediatR

## Frontend Implementation

### 1. API Service

#### SettingsApiService
**Location**: `khidmah_inventory.client/src/app/core/services/settings-api.service.ts`

Provides methods for all settings operations:
- `getCompanySettings()` / `saveCompanySettings()`
- `getUserSettings()` / `saveUserSettings()`
- `getSystemSettings()` / `saveSystemSettings()`
- `getNotificationSettings()` / `saveNotificationSettings()`
- `getUISettings()` / `saveUISettings()`
- `getReportSettings()` / `saveReportSettings()`

### 2. Settings Component

#### Integration
**Location**: `khidmah_inventory.client/src/app/features/settings/settings.component.ts`

**Features**:
- ✅ Loads settings from API on initialization
- ✅ Falls back to localStorage if API unavailable
- ✅ Saves settings to API
- ✅ Caches in localStorage for offline access
- ✅ Shows loading states
- ✅ Error handling with user-friendly messages
- ✅ Toast notifications for success/error

**Methods**:
- `loadSettings()` - Loads all settings from API
- `loadCompanySettings()` - Loads company settings
- `loadUserSettings()` - Loads user settings
- `loadSystemSettings()` - Loads system settings
- `loadNotificationSettings()` - Loads notification settings
- `loadUISettings()` - Loads UI settings
- `loadReportSettings()` - Loads report settings
- `saveCompanySettings()` - Saves company settings
- `saveUserSettings()` - Saves user settings
- `saveSystemSettings()` - Saves system settings
- `saveNotificationSettings()` - Saves notification settings
- `saveUISettings()` - Saves UI settings
- `saveReportSettings()` - Saves report settings

## Data Flow

### Loading Settings

```
Frontend Component
    ↓
SettingsApiService.getCompanySettings()
    ↓
HTTP GET /api/settings/company
    ↓
SettingsController.GetCompanySettings()
    ↓
MediatR.Send(GetCompanySettingsQuery)
    ↓
GetCompanySettingsQueryHandler
    ↓
ISettingsRepository.GetSettingsAsync()
    ↓
Database (Settings table)
    ↓
Deserialize JSON → CompanySettingsDto
    ↓
Return ApiResponse<CompanySettingsDto>
    ↓
Frontend updates component state
```

### Saving Settings

```
Frontend Component (user clicks Save)
    ↓
SettingsApiService.saveCompanySettings(settings)
    ↓
HTTP POST /api/settings/company
    ↓
SettingsController.SaveCompanySettings()
    ↓
MediatR.Send(SaveCompanySettingsCommand)
    ↓
SaveCompanySettingsCommandValidator (validation)
    ↓
SaveCompanySettingsCommandHandler
    ↓
ISettingsRepository.SaveSettingsAsync()
    ↓
Serialize to JSON → Database (Settings table)
    ↓
Return ApiResponse<CompanySettingsDto>
    ↓
Frontend shows success message
```

## Settings Types & Keys

| Settings Type | Settings Key | Scope | Description |
|---------------|--------------|-------|-------------|
| `Company` | `company` | Company-wide | Company information and business details |
| `User` | `user-{userId}` | User-specific | Personal preferences and profile |
| `System` | `system` | Company-wide | System configuration and defaults |
| `Notification` | `notification` | Company-wide | Notification preferences |
| `UI` | `ui-{userId}` | User-specific | UI preferences and appearance |
| `Report` | `report` | Company-wide | Report settings and formats |

## Multi-Tenancy

- All settings are filtered by `CompanyId`
- User-specific settings use `user-{userId}` key pattern
- Company-wide settings use simple keys like `company`, `system`

## Error Handling

### Backend
- Validation errors via FluentValidation
- Exception handling via `ExceptionHandlingBehavior`
- Returns `Result<T>` with error messages

### Frontend
- API errors caught and handled gracefully
- Falls back to localStorage if API unavailable
- Shows user-friendly error messages
- Maintains data consistency

## Caching Strategy

1. **Primary**: Database (via API)
2. **Secondary**: localStorage (fallback and cache)
3. **On Save**: Updates both API and localStorage
4. **On Load**: Tries API first, falls back to localStorage

## Example Usage

### Backend (CQRS)

```csharp
// Get settings
var query = new GetCompanySettingsQuery();
var result = await Mediator.Send(query);
if (result.Succeeded)
{
    var settings = result.Data;
}

// Save settings
var command = new SaveCompanySettingsCommand 
{ 
    Settings = companySettingsDto 
};
var result = await Mediator.Send(command);
```

### Frontend (Angular)

```typescript
// Load settings
this.settingsApiService.getCompanySettings()
  .subscribe(response => {
    if (response.success && response.data) {
      this.companySettings = response.data;
    }
  });

// Save settings
this.settingsApiService.saveCompanySettings(this.companySettings)
  .subscribe(response => {
    if (response.success) {
      this.showToast('Settings saved!', 'success');
    }
  });
```

## API Response Format

### Success Response
```json
{
  "success": true,
  "message": "Company settings retrieved successfully.",
  "statusCode": 200,
  "data": {
    "name": "Acme Corp",
    "email": "contact@acme.com",
    "currency": "USD",
    "timeZone": "America/New_York"
  },
  "errors": [],
  "timestamp": "2024-01-15T10:30:00Z"
}
```

### Error Response
```json
{
  "success": false,
  "message": "Operation failed",
  "statusCode": 400,
  "data": null,
  "errors": [
    "Company name is required"
  ],
  "timestamp": "2024-01-15T10:30:00Z"
}
```

## Database Schema

```sql
CREATE TABLE Settings (
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    CompanyId UNIQUEIDENTIFIER NOT NULL,
    SettingsType NVARCHAR(50) NOT NULL,
    SettingsKey NVARCHAR(200) NOT NULL,
    JsonData NVARCHAR(MAX) NOT NULL,
    Description NVARCHAR(500),
    CreatedAt DATETIME2 NOT NULL,
    UpdatedAt DATETIME2,
    CreatedBy UNIQUEIDENTIFIER,
    UpdatedBy UNIQUEIDENTIFIER,
    IsDeleted BIT NOT NULL DEFAULT 0,
    DeletedAt DATETIME2,
    DeletedBy UNIQUEIDENTIFIER,
    
    UNIQUE INDEX IX_Settings_CompanyId_SettingsType_SettingsKey 
        (CompanyId, SettingsType, SettingsKey) 
        WHERE IsDeleted = 0
);
```

## Testing

### Backend Testing
- Unit tests for handlers
- Integration tests for repository
- API endpoint tests

### Frontend Testing
- Component tests
- Service tests
- E2E tests for settings flow

## Future Enhancements

- [ ] Settings versioning/history
- [ ] Settings import/export
- [ ] Settings templates
- [ ] Bulk settings operations
- [ ] Settings audit log
- [ ] Settings permissions (who can change what)

## Summary

✅ **Complete Backend Implementation**:
- Domain entity
- Repository pattern
- CQRS commands/queries
- API controller
- Validation
- Error handling

✅ **Complete Frontend Implementation**:
- API service
- Component integration
- Error handling
- Caching strategy
- User feedback

✅ **Features**:
- Multi-tenant support
- User-specific and company-wide settings
- JSON storage for flexibility
- Offline support (localStorage fallback)
- Standardized API responses

The settings system is now fully functional and ready for use!

