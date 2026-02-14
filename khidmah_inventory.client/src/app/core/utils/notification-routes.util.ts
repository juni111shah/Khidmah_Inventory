/**
 * Maps entity type and id to the Angular route for notification click-through.
 */
export function getRouteForNotification(entityType: string | undefined, entityId: string | undefined): string | null {
  if (!entityType || !entityId) return null;
  const type = entityType.toLowerCase();
  switch (type) {
    case 'purchaseorder':
      return `/purchase-orders/${entityId}`;
    case 'salesorder':
      return `/sales-orders/${entityId}`;
    case 'product':
      return `/products/${entityId}`;
    case 'workflowinstance':
      return `/workflows/inbox`;
    case 'customer':
      return `/customers/${entityId}`;
    case 'supplier':
      return `/suppliers/${entityId}`;
    case 'warehouse':
      return `/warehouses/${entityId}`;
    default:
      return null;
  }
}
