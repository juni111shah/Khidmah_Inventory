# Khidmah Inventory – Enterprise Expansion

This document tracks the multi-country, compliant, enterprise platform expansion. All work follows Clean Architecture, CQRS, multi-tenant, permission system, and `Result`/`ApiResponse` patterns.

---

## PART 1 – MULTI-CURRENCY & FX ENGINE ✅

### Delivered

**Domain**
- `Currency`: Code, Name, Symbol, IsBase (company-scoped, soft-delete).
- `ExchangeRate`: FromCurrencyId, ToCurrencyId, Rate, Date (company-scoped).
- `JournalEntry`: TransactionCurrencyId, BaseCurrencyId, ConversionRateToBase.
- `JournalLine`: BaseDebit, BaseCredit (optional; for base-currency amounts).

**Application**
- `ICurrencyConversionService`: ConvertAsync, GetRateAsync, GetBaseCurrencyAsync, ToBaseCurrencyAsync.
- Currency CRUD: CreateCurrency, UpdateCurrency, DeleteCurrency, GetCurrency, GetCurrenciesList.
- Exchange rates: CreateExchangeRate, GetExchangeRates (with date/currency filters).
- Validators for create/update commands.

**Infrastructure**
- `CurrencyConversionService` implementation (company-scoped rates, direct/inverse lookup).
- EF configurations: Currency, ExchangeRate; JournalEntry/JournalLine precision for new fields.
- `ApplicationDbContext`: Currencies, ExchangeRates DbSets.
- Infrastructure references Domain (for entity types in configs).

**API**
- `api/currency`: GET (list), GET `{id}`, POST, PUT `{id}`, DELETE `{id}`.
- `api/exchange-rates`: GET (list with fromDate, toDate, fromCurrencyId, toCurrencyId), POST.
- Permissions: Currency (List, Read, Create, Update, Delete), ExchangeRates (List, Create).
- Validation codes and `CurrencyController`, `ExchangeRatesController`.

**Frontend**
- `core/models/currency.model.ts`, `core/services/currency-api.service.ts`.
- `api-validation-codes` and `api-code.service` mappings for currency and exchange-rates.
- **Currencies** page: list, add/edit in drawer (currency-form component).
- **Exchange rates** page: list with date filter, add rate in drawer (add-exchange-rate-form).
- Routes: `/currency`, `/exchange-rates`.
- Finance sidebar: “Currencies” and “Exchange rates” under Finance.

### After deploy

1. **Run migration** (with API/IDE not locking bin):
   ```bash
   dotnet ef migrations add AddCurrencyAndExchangeRate --project Khidmah_Inventory.Infrastructure --startup-project Khidmah_Inventory.API --context ApplicationDbContext
   dotnet ef database update --project Khidmah_Inventory.Infrastructure --startup-project Khidmah_Inventory.API
   ```
2. **Seed or add permissions** for Currency and ExchangeRates (e.g. Currency:List, Currency:Read, Currency:Create, Currency:Update, Currency:Delete, ExchangeRates:List, ExchangeRates:Create) and assign to roles.
3. **Reports in transaction vs base currency**: Use `ICurrencyConversionService` in statement/report handlers; add a “report in base currency” (or “transaction currency”) parameter where needed.

---

## PART 2 – TAXATION ENGINE (VAT / GST) – Pending

- Domain: Tax (Name, Rate, Type inclusive/exclusive, AccountId), TaxGroup (multiple taxes).
- Apply to sales, purchase, POS; auto-post to tax payable/receivable.
- Tax reports by period.

---

## PART 3 – ASSET MANAGEMENT – Pending

- Asset entity (purchase date, cost, useful life, depreciation method, residual value, expense account).
- Monthly depreciation journal.
- API: api/assets; Frontend: asset register, depreciation schedule.

---

## PART 4 – PAYROLL FOUNDATION – Pending

- Employee, salary structure, allowance, deduction.
- Monthly payroll run; expense + payable journals.

---

## PART 5 – MULTI-COMPANY CONSOLIDATION – Pending

- Group reporting: combined revenue, profit, eliminations placeholder.
- API: api/group-reporting.

---

## PART 6 – FRAUD & RISK MONITORING – Pending

- Checks: unusual refunds, price override abuse, manual stock edits, late night sales, rapid shrinkage.
- Return risk score; api/risk; command center.

---

## PART 7 – APPROVAL GOVERNANCE – Pending

- Extend workflows: large discounts, stock adjustments, high-value purchase, vendor changes.
- Escalation timers.

---

## PART 8 – FULL AUDIT & TRACEABILITY – Pending

- Store who, when, before, after for changes.
- Audit explorer page.

---

## PART 9 – DOCUMENT CONTROL – Pending

- Version invoices and POs; no silent edits after approval.

---

## PART 10 – ENTERPRISE DASHBOARD – Pending

- CEO: financial health, cash prediction, risk alerts, compliance issues, group performance.

---

## PART 11 – PERFORMANCE – Pending

- Aggregates, projections, indexes.

---

## PART 12 – PERMISSIONS – Pending

- New finance/asset/payroll/risk permissions (extend AuthorizePermissions and seed).
