import { Component, DestroyRef, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { AuthService } from '../services/auth.service';
import { FeedbackService } from '../services/feedback.service';
import { markAllAsTouched } from '../shared/utils/forms.util';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';

@Component({
  selector: 'app-register',
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
  templateUrl: './register.component.html',

})
export class RegisterComponent {
  private fb = inject(FormBuilder);
  private auth = inject(AuthService);
  private router = inject(Router);
  private feedback = inject(FeedbackService);
  private destroyRef = inject(DestroyRef);

  private submitting = signal(false);
  private hidePass = signal(true);
  private hideConfirm = signal(true);

  form = this.fb.group({
    name: ['', [Validators.required, Validators.minLength(2)]],
    surname: ['', [Validators.required, Validators.minLength(2)]],
    email: ['', [Validators.required, Validators.email]],
    password: ['', [Validators.required, Validators.minLength(6)]],
    confirmPassword: ['', [Validators.required]],
    birthday: ['', [Validators.required]],
  });

  isSubmitting = () => this.submitting();
  hidePassword = () => this.hidePass();
  hideConfirmPassword = () => this.hideConfirm();

  constructor() {
    this.form.valueChanges.pipe(takeUntilDestroyed(this.destroyRef)).subscribe(() => {
      if (this.form.controls['confirmPassword'].hasError('mismatch') && this.passwordsMatch()) {
        this.form.controls['confirmPassword'].setErrors(null);
      }
    });
  }

  async onSubmit(): Promise<void> {
    if (this.form.invalid) {
      markAllAsTouched(this.form);
      return;
    }

    if (!this.passwordsMatch()) {
      this.form.controls['confirmPassword'].setErrors({ mismatch: true });
      this.form.controls['confirmPassword'].markAsTouched();
      return;
    }

    this.submitting.set(true);
    const { name, surname, email, password, birthday } = this.form.getRawValue();
    try {
      const response = await this.auth.register({
        name: name!.trim(),
        surname: surname!.trim(),
        email: email!.trim(),
        password: password!,
        birthday: birthday!,
      });
      if (response) {
        this.feedback.success('Registro exitoso. Ya puedes iniciar sesión con tus credenciales.');
        await this.router.navigate(['/login'], { queryParams: { email } });
      } else {
        this.feedback.error('No se pudo completar el registro. Intenta nuevamente más tarde.');
      }
    } finally {
      this.submitting.set(false);
    }
  }

  navigateToLogin(): void {
    this.router.navigate(['/login']);
  }

  togglePassword(): void {
    this.hidePass.update((value) => !value);
  }

  toggleConfirmPassword(): void {
    this.hideConfirm.update((value) => !value);
  }

  private passwordsMatch(): boolean {
    const password = this.form.controls['password'].value;
    const confirm = this.form.controls['confirmPassword'].value;
    return password === confirm;
  }
}
