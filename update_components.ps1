# PowerShell script to update remaining listing components
$components = @(
    "categories-list",
    "roles-list",
    "purchase-orders-list",
    "sales-orders-list"
)

foreach ($component in $components) {
    $tsFile = "d:\Khidmah_Inventory\khidmah_inventory.client\src\app\features\$component\$component.component.ts"
    $htmlFile = "d:\Khidmah_Inventory\khidmah_inventory.client\src\app\features\$component\$component.component.html"

    Write-Host "Updating $component..."

    # Update TypeScript file
    if (Test-Path $tsFile) {
        $content = Get-Content $tsFile -Raw

        # Add import
        $content = $content -replace "import \{ FilterFieldComponent \} from '\.\./\.\./\.\./shared/components/filter-field/filter-field\.component';", "import { FilterFieldComponent } from '../../../shared/components/filter-field/filter-field.component';
import { FilterPanelComponent, FilterPanelConfig, FilterPanelField } from '../../../shared/components/filter-panel/filter-panel.component';"

        # Add to imports array
        $content = $content -replace "(ListingContainerComponent,\s*FilterFieldComponent)", "`$1,
    FilterPanelComponent"

        # Add filter panel properties after showFilterPanel
        $content = $content -replace "(showFilterPanel = false;)", "`$1

  // Filter panel configuration
  filterPanelConfig: FilterPanelConfig = {
    fields: [],
    showResetButton: true,
    showApplyButton: true,
    layout: 'row'
  };

  filterPanelValues: { [key: string]: any } = {};"

        # Update ngOnInit to call initializeFilterPanel
        $content = $content -replace "(ngOnInit\(\): void \{\s*[^}]*?load\w+\(\);\s*\})", "`${`$1.Replace('load', 'initializeFilterPanel();
    load')}`"

        # Add initializeFilterPanel method
        $content = $content -replace "(load\w+\(\);\s*\})", "`$1

  private initializeFilterPanel(): void {
    this.filterPanelConfig = {
      fields: [
        {
          key: 'isActive',
          label: 'Status',
          type: 'select',
          placeholder: 'All Status',
          colSize: 'col-md-4',
          defaultValue: null,
          options: [
            { label: 'All Status', value: null },
            { label: 'Active', value: true },
            { label: 'Inactive', value: false }
          ]
        }
      ],
      showResetButton: true,
      showApplyButton: true,
      layout: 'row'
    };

    // Initialize filter values
    this.filterPanelValues = {
      isActive: null
    };
  }"

        # Update resetFilters method
        $content = $content -replace "(resetFilters\(\): void \{\s*[^}]*?\})", "  // Filter panel event handlers
  onFilterApplied(filters: { [key: string]: any }): void {
    this.isActiveFilter = filters['isActive'] !== undefined ? filters['isActive'] : null;

    if (this.filterRequest.pagination) {
      this.filterRequest.pagination.pageNo = 1;
    }
    this.load$($component.Split('-')[0])();
  }

  onFilterReset(): void {
    this.isActiveFilter = null;
    this.searchTerm = '';
    if (this.filterRequest.search) {
      this.filterRequest.search.term = '';
    }
    if (this.filterRequest.pagination) {
      this.filterRequest.pagination.pageNo = 1;
    }
    this.load$($component.Split('-')[0])();
  }

  resetFilters(): void {
    this.onFilterReset();
  }"

        Set-Content $tsFile $content
        Write-Host "Updated $tsFile"
    }

    # Update HTML file
    if (Test-Path $htmlFile) {
        $content = Get-Content $htmlFile -Raw

        # Replace filter panel content
        $content = $content -replace "(<div filter-panel>[\s\S]*?</div>)", "  <div filter-panel>
    <app-filter-panel
      [config]=""filterPanelConfig""
      [filterValues]=""filterPanelValues""
      (filterApplied)=""onFilterApplied(`$event)""
      (filterReset)=""onFilterReset()"">
    </app-filter-panel>
  </div>"

        Set-Content $htmlFile $content
        Write-Host "Updated $htmlFile"
    }
}

Write-Host "All components updated!"