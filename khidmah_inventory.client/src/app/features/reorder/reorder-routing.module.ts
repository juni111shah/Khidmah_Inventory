import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ReorderDashboardComponent } from './reorder-dashboard/reorder-dashboard.component';
import { ReorderListComponent } from './reorder-list/reorder-list.component';
import { ReorderReviewComponent } from './reorder-review/reorder-review.component';
import { GeneratePoFromSuggestionComponent } from './generate-po-from-suggestion/generate-po-from-suggestion.component';
import { AuthGuard } from '../../core/guards/auth.guard';
import { PermissionGuard } from '../../core/guards/permission.guard';

const routes: Routes = [
  {
    path: '',
    component: ReorderDashboardComponent,
    canActivate: [AuthGuard, PermissionGuard],
    data: { permission: 'Reordering:Suggestions:Read', header: { title: 'Reorder Dashboard', description: 'Low stock and reorder suggestions' } }
  },
  {
    path: 'list',
    component: ReorderListComponent,
    canActivate: [AuthGuard, PermissionGuard],
    data: { permission: 'Reordering:Suggestions:Read', header: { title: 'Reorder List', description: 'View and select items to reorder' } }
  },
  {
    path: 'review',
    component: ReorderReviewComponent,
    canActivate: [AuthGuard, PermissionGuard],
    data: { permission: 'Reordering:Suggestions:Read', header: { title: 'Review reorder', description: 'Review suggestion and generate PO' } }
  },
  {
    path: 'generate-po',
    component: GeneratePoFromSuggestionComponent,
    canActivate: [AuthGuard, PermissionGuard],
    data: { permission: 'Reordering:GeneratePO:Create', header: { title: 'Generate PO', description: 'Create purchase order from suggestions' } }
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class ReorderRoutingModule {}
