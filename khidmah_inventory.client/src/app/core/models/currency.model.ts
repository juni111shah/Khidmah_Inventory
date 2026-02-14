export interface CurrencyDto {
  id: string;
  companyId: string;
  code: string;
  name: string;
  symbol: string;
  isBase: boolean;
}

export interface ExchangeRateDto {
  id: string;
  fromCurrencyId: string;
  fromCurrencyCode: string;
  toCurrencyId: string;
  toCurrencyCode: string;
  rate: number;
  date: string;
}

export interface GetCurrenciesListResult {
  items: CurrencyDto[];
}

export interface GetExchangeRatesResult {
  items: ExchangeRateDto[];
}
