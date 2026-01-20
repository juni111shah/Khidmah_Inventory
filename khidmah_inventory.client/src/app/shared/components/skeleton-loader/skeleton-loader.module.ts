import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SkeletonLoaderComponent } from './skeleton-loader.component';
import { SkeletonButtonComponent } from '../skeleton-button/skeleton-button.component';
import { SkeletonFieldComponent } from '../skeleton-field/skeleton-field.component';
import { SkeletonCardComponent } from '../skeleton-card/skeleton-card.component';
import { SkeletonTableComponent } from '../skeleton-table/skeleton-table.component';
import { SkeletonListComponent } from '../skeleton-list/skeleton-list.component';
import { SkeletonFormComponent } from '../skeleton-form/skeleton-form.component';

const COMPONENTS = [
  SkeletonLoaderComponent,
  SkeletonButtonComponent,
  SkeletonFieldComponent,
  SkeletonCardComponent,
  SkeletonTableComponent,
  SkeletonListComponent,
  SkeletonFormComponent
];

@NgModule({
  declarations: [],
  imports: [CommonModule, ...COMPONENTS],
  exports: COMPONENTS
})
export class SkeletonLoaderModule { }

