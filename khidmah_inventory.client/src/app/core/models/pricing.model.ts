export interface PriceHistoryItem {
  date: string;
  price: number;
  type: string;
}

export interface PriceOptimization {
  productId: string;
  productName: string;
  productSKU: string;
  currentPrice: number;
  competitorPrice?: number;
  recommendedPrice?: number;
  minPrice?: number;
  maxPrice?: number;
  currentMargin: number;
  recommendedMargin?: number;
  optimalMargin?: number;
  recommendation: string;
  priceChangeAmount?: number;
  priceChangePercentage?: number;
  priceHistory: PriceHistoryItem[];
}
