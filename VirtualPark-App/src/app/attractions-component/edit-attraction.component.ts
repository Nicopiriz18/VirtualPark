import { Component, OnInit, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { AttractionService } from '../services/attraction.service';
import { FeedbackService } from '../services/feedback.service';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatSnackBarModule } from '@angular/material/snack-bar';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { buildAttractionForm } from '../shared/forms/attraction-form.factory';
import { markAllAsTouched } from '../shared/utils/forms.util';

@Component({
  selector: 'app-edit-attraction',
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatSnackBarModule,
    MatProgressSpinnerModule,
  ],
  templateUrl: './edit-attraction.component.html',

})
export class EditAttractionComponent implements OnInit {
  private fb = inject(FormBuilder);
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private attractionService = inject(AttractionService);
  private feedback = inject(FeedbackService);

  private loading = signal(true);
  private saving = signal(false);
  private attractionId: string | null = null;

  form = buildAttractionForm(this.fb);

  isLoading = () => this.loading();
  isSaving = () => this.saving();

  get f() {
    return this.form.controls;
  }

  async ngOnInit(): Promise<void> {
    this.attractionId = this.route.snapshot.params['id'];
    if (!this.attractionId) {
      this.feedback.error('Identificador de atracción inválido.');
      this.goBack();
      return;
    }

    try {
      const attraction = await this.attractionService.fetchAttractionById(this.attractionId);
      if (!attraction) {
        this.feedback.error('No se encontró la atracción solicitada.');
        this.goBack();
        return;
      }
      this.form.patchValue(attraction);
    } finally {
      this.loading.set(false);
    }
  }

  async onSubmit(): Promise<void> {
    if (this.form.invalid || !this.attractionId) {
      markAllAsTouched(this.form);
      return;
    }

    this.saving.set(true);
    try {
      const updated = await this.attractionService.updateAttraction(this.attractionId, this.form.getRawValue());
      if (updated) {
        this.feedback.success('La atracción se actualizó correctamente.');
        this.goBack();
      } else {
        this.feedback.error('No se pudo actualizar la atracción.');
      }
    } finally {
      this.saving.set(false);
    }
  }

  goBack(): void {
    this.router.navigate(['/attractions']);
  }
}
