import { Component, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { AttractionService } from '../services/attraction.service';
import { FeedbackService } from '../services/feedback.service';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatSnackBarModule } from '@angular/material/snack-bar';
import { buildAttractionForm } from '../shared/forms/attraction-form.factory';
import { markAllAsTouched } from '../shared/utils/forms.util';

@Component({
  selector: 'app-create-attraction',
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatSnackBarModule,
  ],
  templateUrl: './create-attraction.component.html',

})
export class CreateAttractionComponent {
  private fb = inject(FormBuilder);
  private router = inject(Router);
  private attractionService = inject(AttractionService);
  private feedback = inject(FeedbackService);

  private saving = signal(false);

  form = buildAttractionForm(this.fb);

  isSaving = () => this.saving();

  get f() {
    return this.form.controls;
  }

  async onSubmit(): Promise<void> {
    if (this.form.invalid) {
      markAllAsTouched(this.form);
      return;
    }

    this.saving.set(true);
    try {
      const created = await this.attractionService.createAttraction(this.form.getRawValue());
      if (created) {
        this.feedback.success('La atracción se creó correctamente.');
        this.router.navigate(['/attractions']);
      } else {
        this.feedback.error('No se pudo crear la atracción. Intenta nuevamente.');
      }
    } finally {
      this.saving.set(false);
    }
  }

  goBack(): void {
    this.router.navigate(['/attractions']);
  }
}
