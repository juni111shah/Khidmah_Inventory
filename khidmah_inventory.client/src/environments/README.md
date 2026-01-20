# Environment Configuration

This folder contains environment-specific configuration files for the Angular application.

## Files

- **environment.ts** - Production environment configuration
- **environment.development.ts** - Development environment configuration

## Configuration Properties

Each environment file contains:

- `production`: Boolean indicating if running in production mode
- `apiUrl`: Full API URL including `/api` path (e.g., `https://localhost:5001/api`)
- `apiBaseUrl`: Base API URL without `/api` path (e.g., `https://localhost:5001`)
- `enableDebug`: Boolean to enable/disable debug features

## Usage

### In Services

Import and use the `ApiConfigService`:

```typescript
import { ApiConfigService } from '../services/api-config.service';

constructor(private apiConfig: ApiConfigService) {}

// Get full API URL
const url = this.apiConfig.getApiUrl('auth/login');
// Returns: 'https://localhost:5001/api/auth/login'

// Get base API URL
const baseUrl = this.apiConfig.getBaseApiUrl();
// Returns: 'https://localhost:5001/api'
```

### Direct Import (Not Recommended)

You can also import environment directly, but using `ApiConfigService` is preferred:

```typescript
import { environment } from '../../../environments/environment';

const apiUrl = environment.apiUrl;
```

## Updating API Endpoint

To change the API endpoint for different environments:

1. **Development**: Edit `environment.development.ts`
2. **Production**: Edit `environment.ts`

Example:
```typescript
export const environment = {
  production: false,
  apiUrl: 'https://your-api-server.com/api',
  apiBaseUrl: 'https://your-api-server.com',
  enableDebug: true
};
```

## Build Configuration

The `angular.json` file is configured to automatically replace:
- `environment.ts` with `environment.development.ts` when building for development
- `environment.ts` remains as-is when building for production

## Notes

- The API URL should include the `/api` path
- For CORS to work, ensure your backend allows requests from your frontend origin
- In development, the proxy configuration (`proxy.conf.js`) may be used instead of direct API calls

