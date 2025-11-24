import { Component, OnDestroy, OnInit, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { MatSnackBarModule } from '@angular/material/snack-bar';
import { UserService } from '../services/user.service';
import { AuthService } from '../services/auth.service';
import { FeedbackService } from '../services/feedback.service';
import { buildUserForm } from '../shared/forms/user-form.factory';
import { markAllAsTouched } from '../shared/utils/forms.util';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-create-user',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    ReactiveFormsModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatButtonModule,
    MatSnackBarModule,
  ],
  templateUrl: './create-user.component.html',
})
export class CreateUserComponent implements OnInit, OnDestroy {
  private fb = inject(FormBuilder);
  private router = inject(Router);
  private userService = inject(UserService);
  private auth = inject(AuthService);
  private feedback = inject(FeedbackService);
  private roleChangeSub: Subscription | null = null;

  private saving = signal(false);

  roles: string[] = ['Visitor', 'Operator', 'Administrator'];

  form = buildUserForm(this.fb, {}, { requirePassword: true });

  isSaving = () => this.saving();

  get f() {
    return this.form.controls;
  }

  get isAdmin(): boolean {
    return this.auth.hasRole('Administrator');
  }

  get showBirthDateField(): boolean {
    return this.form.controls['role'].value === 'Visitor';
  }

  ngOnInit(): void {
    this.configureBirthDateValidators(this.form.controls['role'].value);
    this.roleChangeSub = this.form.controls['role'].valueChanges.subscribe((value) =>
      this.configureBirthDateValidators(value)
    );
  }

  async onSubmit(): Promise<void> {
    if (!this.isAdmin) return;
    if (this.form.invalid) {
      markAllAsTouched(this.form);
      return;
    }

    this.saving.set(true);
    try {
      const formValue = this.form.getRawValue();
      const birthDateIso =
        formValue.role === 'Visitor' && formValue.birthDate
          ? new Date(formValue.birthDate).toISOString()
          : undefined;

      const created = await this.userService.createUser({
        name: formValue.name,
        surname: formValue.surname,
        email: formValue.email,
        password: formValue.password,
        role: formValue.role,
        birthDate: birthDateIso,
      });
      if (created) {
        this.feedback.success('El usuario se creó correctamente.');
        this.goBack();
      } else {
        this.feedback.error('No se pudo crear el usuario. Intenta nuevamente.');
      }
    } finally {
      this.saving.set(false);
    }
  }

  goBack(): void {
    this.router.navigate(['users']);
  }

  ngOnDestroy(): void {
    this.roleChangeSub?.unsubscribe();
  }

  private configureBirthDateValidators(role: string | null): void {
    const control = this.form.get('birthDate');
    if (!control) return;

    if (role === 'Visitor') {
      control.setValidators([Validators.required]);
    } else {
      control.clearValidators();
      control.setValue('', { emitEvent: false });
    }

    control.updateValueAndValidity({ emitEvent: false });
  }
}
