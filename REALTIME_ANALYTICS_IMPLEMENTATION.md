# Real-Time Analytics with SignalR Implementation

## Overview

This document describes the comprehensive real-time analytics system implemented with SignalR, featuring time range filters, multiple analytics types, and automatic chart updates without page refresh.

## Features Implemented

### 1. Backend SignalR Infrastructure

#### SignalR Hub (`AnalyticsHub`)
- **Location**: `Khidmah_Inventory.API/Hubs/AnalyticsHub.cs`
- **Features**:
  - JWT-authenticated connections
  - Company-based group management
  - Subscribe/Unsubscribe to specific analytics streams
  - Real-time data broadcasting

#### SignalR Configuration
- **JWT Authentication**: Configured to work with SignalR WebSocket connections
- **Hub Endpoint**: `/hubs/analytics`
- **Background Service**: `AnalyticsBroadcastService` for periodic updates

### 2. Enhanced Analytics Queries

#### Analytics Types
1. **Sales Analytics** (`GetSalesAnalyticsQuery`)
   - Total sales, cost, profit, profit margin
   - Time series data (Day/Week/Month grouping)
   - Category breakdown
   - Top products and customers
   - Average order value

2. **Inventory Analytics** (`GetInventoryAnalyticsQuery`)
   - Total stock value
   - Low stock and out-of-stock items
   - Category and warehouse breakdown
   - Fast-moving and slow-moving products

3. **Profit Analytics** (`GetProfitAnalyticsQuery`)
   - Revenue, cost, profit calculations
   - Profit margin and gross profit margin
   - Profit trend over time
   - Category-wise profit analysis

#### Time Range Filters
- **Predefined Ranges**:
  - Today, Yesterday
  - Last 7 Days, Last 30 Days
  - This Month, Last Month
  - This Quarter, Last Quarter
  - This Year, Last Year
- **Custom Range**: User-defined date range

#### Grouping Options
- **Day**: Daily aggregation
- **Week**: Weekly aggregation
- **Month**: Monthly aggregation

### 3. Frontend Components

#### Chart Component (`ChartComponent`)
- **Location**: `khidmah_inventory.client/src/app/shared/components/chart/chart.component.ts`
- **Features**:
  - Chart.js integration
  - Support for multiple chart types (line, bar, pie, etc.)
  - Real-time data updates
  - Responsive design

#### Time Range Filter Component (`TimeRangeFilterComponent`)
- **Location**: `khidmah_inventory.client/src/app/shared/components/time-range-filter/`
- **Features**:
  - Dropdown for predefined ranges
  - Custom date picker for custom ranges
  - Event emission on change

#### SignalR Service (`SignalRService`)
- **Location**: `khidmah_inventory.client/src/app/core/services/signalr.service.ts`
- **Features**:
  - Automatic connection management
  - JWT token authentication
  - Reconnection handling
  - Subscribe/Unsubscribe to analytics streams
  - Event handlers for real-time updates

#### Analytics Components
- **Sales Analytics Component**: Complete sales analysis with charts and tables
- **Dashboard Component**: Updated with real-time charts and SignalR integration

### 4. Real-Time Updates

#### How It Works
1. **Connection**: Client connects to SignalR hub on component initialization
2. **Subscription**: Client subscribes to specific analytics types
3. **Broadcasting**: Server broadcasts updates every 30 seconds (configurable)
4. **Reception**: Client receives updates and automatically updates charts
5. **Visual Feedback**: Toast notifications inform users of real-time updates

#### Update Triggers
- Periodic updates (every 30 seconds)
- Manual refresh
- Data changes (when orders, sales, or inventory changes occur)

## Installation & Setup

### Backend
1. SignalR package is already added to `Khidmah_Inventory.API.csproj`
2. SignalR is configured in `Program.cs`
3. Hub is mapped at `/hubs/analytics`

### Frontend
1. **Install Required Packages**:
   ```bash
   cd khidmah_inventory.client
   npm install @microsoft/signalr@^8.0.0
   npm install chart.js@^4.4.0
   npm install ng2-charts@^5.0.0
   ```

2. **Package.json** already includes these dependencies

## Usage

### Dashboard with Real-Time Updates
```typescript
// Dashboard automatically connects to SignalR
// Charts update in real-time without page refresh
```

### Sales Analytics Component
```typescript
// Navigate to /analytics/sales
// Select time range from dropdown
// Charts and tables update automatically
// Real-time updates every 30 seconds
```

### Adding Charts to Other Components
```typescript
import { ChartComponent, ChartData } from '../../shared/components/chart/chart.component';

// Create chart data
chartData: ChartData = {
  labels: ['Jan', 'Feb', 'Mar'],
  datasets: [{
    label: 'Sales',
    data: [100, 200, 300],
    backgroundColor: 'rgba(54, 162, 235, 0.2)',
    borderColor: 'rgba(54, 162, 235, 1)'
  }]
};

// Use in template
<app-chart type="line" [data]="chartData"></app-chart>
```

### Using SignalR Service
```typescript
import { SignalRService } from '../../core/services/signalr.service';

constructor(private signalRService: SignalRService) {}

async ngOnInit() {
  await this.signalRService.startConnection();
  this.signalRService.subscribeToAnalytics('Sales');
  this.signalRService.onAnalyticsUpdate((type, data) => {
    // Handle real-time update
  });
}
```

## API Endpoints

### Analytics Controller
- `POST /api/analytics/sales` - Get sales analytics
- `GET /api/analytics/inventory` - Get inventory analytics
- `POST /api/analytics/profit` - Get profit analytics

### SignalR Hub
- **Endpoint**: `/hubs/analytics`
- **Methods**:
  - `SubscribeToAnalytics(analyticsType)` - Subscribe to analytics stream
  - `UnsubscribeFromAnalytics(analyticsType)` - Unsubscribe from stream
- **Events**:
  - `DashboardUpdated` - Dashboard data updated
  - `AnalyticsUpdated` - Analytics data updated

## Permissions Required

- `Dashboard:Read` - View dashboard
- `Analytics:Sales:Read` - View sales analytics
- `Analytics:Inventory:Read` - View inventory analytics
- `Analytics:Profit:Read` - View profit analytics

## Formulas & Calculations

### Sales Analytics
- **Total Sales**: Sum of all sales order totals
- **Total Cost**: Sum of (quantity × average cost) for all items
- **Total Profit**: Total Sales - Total Cost
- **Profit Margin**: (Total Profit / Total Sales) × 100
- **Average Order Value**: Total Sales / Total Orders

### Inventory Analytics
- **Stock Value**: Sum of (quantity × average cost) for all stock levels
- **Average Stock Value**: Total Stock Value / Total Products
- **Fast Moving**: Products with high sales in last 30 days
- **Slow Moving**: Products with low sales and high stock value

### Profit Analytics
- **Gross Profit Margin**: ((Revenue - Cost) / Revenue) × 100
- **Category Profit**: Revenue - Cost per category
- **Profit Trend**: Daily profit calculation over time range

## Chart Types Supported

1. **Line Chart**: For time series data (sales trends, profit trends)
2. **Bar Chart**: For category breakdowns, comparisons
3. **Pie Chart**: For percentage distributions (can be added)
4. **Area Chart**: For cumulative data (can be configured)

## Configuration

### Update Interval
- **Default**: 30 seconds
- **Location**: `AnalyticsBroadcastService._updateInterval`
- **Modify**: Change `TimeSpan.FromSeconds(30)` to desired interval

### Chart Options
- Customizable via `options` input in `ChartComponent`
- Supports all Chart.js configuration options
- Responsive by default

## Best Practices

1. **Connection Management**: Always unsubscribe and disconnect in `ngOnDestroy`
2. **Error Handling**: Handle connection failures gracefully
3. **Performance**: Limit real-time updates to essential data
4. **User Experience**: Show loading states and update indicators
5. **Security**: All SignalR connections require JWT authentication

## Future Enhancements

1. **More Analytics Types**: Purchase analytics, customer analytics
2. **Export Functionality**: PDF/Excel export of analytics
3. **Custom Dashboards**: User-configurable dashboard layouts
4. **Alerts**: Real-time alerts for low stock, high sales, etc.
5. **Comparison Mode**: Compare different time periods
6. **Drill-Down**: Click charts to see detailed data

## Troubleshooting

### SignalR Connection Issues
- Check JWT token is valid
- Verify CORS configuration
- Check network connectivity
- Review browser console for errors

### Chart Not Updating
- Verify SignalR connection is established
- Check subscription to analytics type
- Ensure data format matches Chart.js requirements
- Review component lifecycle hooks

### Performance Issues
- Reduce update frequency
- Limit data points in charts
- Use pagination for large datasets
- Implement virtual scrolling for tables

