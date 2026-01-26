import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { ThemedButtonComponent } from '../../shared/components/themed-button/themed-button.component';
import { ThemedCardComponent } from '../../shared/components/themed-card/themed-card.component';
import { PosMainComponent } from './pos-main/pos-main.component';

@NgModule({
  declarations: [
    PosMainComponent
  ],
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    ThemedButtonComponent,
    ThemedCardComponent,
    RouterModule.forChild([
      { path: '', component: PosMainComponent }
    ])
  ]
})
export class PosModule { }
