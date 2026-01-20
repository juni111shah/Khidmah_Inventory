# Environment Setup Guide

## Overview

The Angular application now uses environment configuration files to manage API endpoints and other environment-specific settings.

## Files Created

1. **`src/environments/environment.ts`** - Production environment
2. **`src/environments/environment.development.ts`** - Development environment
3. **`src/app/core/services/api-config.service.ts`** - Service for accessing environment config
4. **`angular.json`** - Updated with file replacements for environments

## Current Configuration

### Development Environment
- **API URL**: `https://localhost:5001/api`
- **Base URL**: `https://localhost:5001`
- **Production**: `false`
- **Debug**: `true`

### Production Environment
- **API URL**: `https://localhost:5001/api` (⚠️ Update this for production!)
- **Base URL**: `https://localhost:5001` (⚠️ Update this for production!)
- **Production**: `true`
- **Debug**: `false`

## How to Update API Endpoint

### For Development

Edit `src/environments/environment.development.ts`:

```typescript
export const environment = {
  production: false,
  apiUrl: 'https://your-dev-server.com/api',
  apiBaseUrl: 'https://your-dev-server.com',
  enableDebug: true
};
```

### For Production

Edit `src/environments/environment.ts`:

```typescript
export const environment = {
  production: true,
  apiUrl: 'https://your-production-server.com/api',
  apiBaseUrl: 'https://your-production-server.com',
  enableDebug: false
};
```

## Using in Services

The `AuthService` has been updated as an example. To update other services:

1. Import `ApiConfigService`:
```typescript
import { ApiConfigService } from '../services/api-config.service';
```

2. Inject it in constructor:
```typescript
constructor(
  private http: HttpClient,
  private apiConfig: ApiConfigService
) {}
```

3. Use it for API URLs:
```typescript
// Instead of: private apiUrl = '/api/products';
// Use:
private apiUrl = this.apiConfig.getApiUrl('products');

// Or directly in methods:
this.http.get(this.apiConfig.getApiUrl('products/list'))
```

## Services to Update

The following services still use hardcoded `/api` paths and should be updated:

- `product-api.service.ts`
- `category-api.service.ts`
- `warehouse-api.service.ts`
- `supplier-api.service.ts`
- `customer-api.service.ts`
- `purchase-order-api.service.ts`
- `sales-order-api.service.ts`
- `inventory-api.service.ts`
- `user-api.service.ts`
- `role-api.service.ts`
- `dashboard-api.service.ts`
- `analytics-api.service.ts`
- `report-api.service.ts`
- `theme-api.service.ts`
- `settings-api.service.ts`
- `permission-api.service.ts`
- `sync.service.ts`

## Build Commands

- **Development**: `ng serve` (uses `environment.development.ts`)
- **Production**: `ng build --configuration production` (uses `environment.ts`)

## Proxy Configuration

The `proxy.conf.js` is still configured for development. If you're using the proxy, you can keep relative paths (`/api/...`) in development. However, using the environment configuration is recommended for consistency across environments.

## Next Steps

1. ✅ Environment files created
2. ✅ Angular.json updated
3. ✅ ApiConfigService created
4. ✅ AuthService updated (example)
5. ⚠️ Update remaining services to use ApiConfigService
6. ⚠️ Update production API URLs before deploying

