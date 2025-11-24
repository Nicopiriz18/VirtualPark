import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { AuthService } from '../services/auth.service';
import { FeedbackService } from '../services/feedback.service';
import { markAllAsTouched } from '../shared/utils/forms.util';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    ReactiveFormsModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatIconModule,
    MatProgressSpinnerModule,
  ],
  templateUrl: './login.component.html',

})
export class LoginComponent {
  private fb = inject(FormBuilder);
  private auth = inject(AuthService);
  private router = inject(Router);
  private feedback = inject(FeedbackService);
  private route = inject(ActivatedRoute);

  private submitting = signal(false);
  private hidden = signal(true);

  form = this.fb.group({
    email: ['', [Validators.required, Validators.email]],
    password: ['', [Validators.required, Validators.minLength(6)]],
  });

  isSubmitting = () => this.submitting();
  hidePassword = () => this.hidden();

  constructor() {
    const prefilledEmail = this.route.snapshot.queryParamMap.get('email');
    if (prefilledEmail) {
      this.form.patchValue({ email: prefilledEmail });
    }
  }

  async onSubmit(): Promise<void> {
    if (this.form.invalid) {
      markAllAsTouched(this.form);
      return;
    }

    this.submitting.set(true);
    const { email, password } = this.form.getRawValue();

    try {
      const token = await this.auth.login(email!.trim(), password!);
      if (token) {
        this.feedback.success('Inicio de sesión exitoso.');
        await this.router.navigate(['/attractions']);
      } else {
        this.feedback.error('No pudimos validar tus credenciales. Verifica los datos e intenta nuevamente.');
      }
    } finally {
      this.submitting.set(false);
    }
  }

  togglePasswordVisibility(): void {
    this.hidden.update((value) => !value);
  }
}
