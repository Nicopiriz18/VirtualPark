import { Component, inject, signal, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { VisitorProfileService } from '../../services/visitor/visitor-profile.service';
import { ScoreHistoryService, ScoreHistoryEntry } from '../../services/visitor/score-history.service';
import { AuthService } from '../../services/auth.service';
import { FeedbackService } from '../../services/feedback.service';
import { markAllAsTouched } from '../../shared/utils/forms.util';

@Component({
  selector: 'app-visitor-profile',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatIconModule,
    MatProgressSpinnerModule,
  ],
  templateUrl: './visitor-profile.component.html',
})
export class VisitorProfileComponent implements OnInit {
  private fb = inject(FormBuilder);
  private visitorProfileService = inject(VisitorProfileService);
  private feedback = inject(FeedbackService);
  private scoreHistoryService = inject(ScoreHistoryService);
  private auth = inject(AuthService);

  private updating = signal(false);
  private historyLoading = signal(false);
  history = signal<ScoreHistoryEntry[]>([]);

  async ngOnInit(): Promise<void> {
    const storedId = this.auth.getUserId();
    const storedEmail = this.auth.getUserEmail();

    if (storedId) {
      this.form.controls['visitorId'].setValue(storedId);
    }

    if (storedEmail) {
      this.form.controls['email'].setValue(storedEmail);
    }

    if (storedId) {
      await this.loadHistory(false);
    }
  }

  form = this.fb.group({
    visitorId: ['', Validators.required],
    name: ['', Validators.required],
    surname: ['', Validators.required],
    email: ['', [Validators.required, Validators.email]],
    password: ['', Validators.minLength(6)],
  });

  isUpdating = () => this.updating();
  isHistoryLoading = () => this.historyLoading();

  canFetchHistory(): boolean {
    const visitorId = this.form.controls['visitorId'].value?.trim();
    return !!visitorId && !this.isHistoryLoading();
  }

  refreshHistory(): void {
    void this.loadHistory();
  }

  resetForm(): void {
    this.form.reset({
      visitorId: '',
      name: '',
      surname: '',
      email: '',
      password: '',
    });

    const storedId = this.auth.getUserId();
    const storedEmail = this.auth.getUserEmail();
    if (storedId) {
      this.form.controls['visitorId'].setValue(storedId);
    }
    if (storedEmail) {
      this.form.controls['email'].setValue(storedEmail);
    }
  }

  async onSubmit(): Promise<void> {
    if (this.form.invalid) {
      markAllAsTouched(this.form);
      return;
    }

    const { visitorId, name, surname, email, password } = this.form.getRawValue();
    if (!visitorId || !name || !surname || !email) return;

    this.updating.set(true);
    try {
      const success = await this.visitorProfileService.updateProfile(visitorId.trim(), {
        name: name.trim(),
        surname: surname.trim(),
        email: email.trim(),
        password: password ? password : null,
      });
      if (success) {
        this.feedback.success('Perfil actualizado correctamente.');
        await this.loadHistory(false);
      } else {
        this.feedback.error('No fue posible actualizar el perfil. Verifica los datos e intenta nuevamente.');
      }
    } finally {
      this.updating.set(false);
    }
  }

  private async loadHistory(showMissingIdAlert = true): Promise<void> {
    const visitorId = this.form.controls['visitorId'].value?.trim();
    if (!visitorId) {
      if (showMissingIdAlert) {
        this.feedback.error('Debes ingresar el identificador del visitante para consultar el historial.');
      }
      return;
    }

    this.historyLoading.set(true);
    try {
      const entries = await this.scoreHistoryService.getVisitorHistory(visitorId);
      this.history.set(
        [...entries].sort(
          (a, b) => new Date(b.date).getTime() - new Date(a.date).getTime(),
        ),
      );
    } catch {
      this.history.set([]);
      this.feedback.error('No fue posible obtener el historial de puntos. Intenta nuevamente mǭs tarde.');
    } finally {
      this.historyLoading.set(false);
    }
  }
}
