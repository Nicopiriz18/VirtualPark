import { Component, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { SpecialEventService } from '../services/special-event.service';
import { AuthService } from '../services/auth.service';
import { FeedbackService } from '../services/feedback.service';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatSnackBarModule } from '@angular/material/snack-bar';
import { buildSpecialEventForm } from '../shared/forms/special-event-form.factory';
import { markAllAsTouched } from '../shared/utils/forms.util';

@Component({
  selector: 'app-create-special-event',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    ReactiveFormsModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatSnackBarModule,
  ],
  templateUrl: './create-special-event.component.html',

})
export class CreateSpecialEventComponent {
  private fb = inject(FormBuilder);
  private router = inject(Router);
  private eventService = inject(SpecialEventService);
  private auth = inject(AuthService);
  private feedback = inject(FeedbackService);

  private saving = signal(false);

  form = buildSpecialEventForm(this.fb);

  isSaving = () => this.saving();

  get f() {
    return this.form.controls;
  }

  get isAdmin(): boolean {
    return this.auth.hasRole('Administrator');
  }

  async onSubmit(): Promise<void> {
    if (!this.isAdmin) return;
    if (this.form.invalid) {
      markAllAsTouched(this.form);
      return;
    }

    this.saving.set(true);
    try {
      const created = await this.eventService.createEvent(this.form.getRawValue());
      if (created) {
        this.feedback.success('El evento especial se creó correctamente.');
        this.router.navigate(['special-events']);
      } else {
        this.feedback.error('No se pudo crear el evento especial. Intenta nuevamente.');
      }
    } finally {
      this.saving.set(false);
    }
  }

  goBack(): void {
    this.router.navigate(['special-events']);
  }
}
