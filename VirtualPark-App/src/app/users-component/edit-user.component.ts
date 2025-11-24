import { Component, OnDestroy, OnInit, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { MatSnackBarModule } from '@angular/material/snack-bar';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { UserService } from '../services/user.service';
import { AuthService } from '../services/auth.service';
import { FeedbackService } from '../services/feedback.service';
import { buildUserForm } from '../shared/forms/user-form.factory';
import { markAllAsTouched } from '../shared/utils/forms.util';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-edit-user',
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
    MatProgressSpinnerModule,
  ],
  templateUrl: './edit-user.component.html',

})
export class EditUserComponent implements OnInit, OnDestroy {
  private fb = inject(FormBuilder);
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private userService = inject(UserService);
  private auth = inject(AuthService);
  private feedback = inject(FeedbackService);

  private loading = signal(true);
  private saving = signal(false);
  private userId: string | null = null;
  private originalRole: string | null = null;
  private roleChangeSub: Subscription | null = null;

  roles: string[] = ['Visitor', 'Operator', 'Administrator'];

  form = buildUserForm(this.fb, {}, { requirePassword: false });

  isLoading = () => this.loading();
  isSaving = () => this.saving();

  get f() {
    return this.form.controls;
  }

  get isAdmin(): boolean {
    return this.auth.hasRole('Administrator');
  }

  async ngOnInit(): Promise<void> {
    if (!this.isAdmin) {
      this.goBack();
      return;
    }

    this.userId = this.route.snapshot.paramMap.get('id');
    if (!this.userId) {
      this.feedback.error('Identificador de usuario inválido.');
      this.goBack();
      return;
    }

    try {
      const user = await this.userService.fetchUserById(this.userId);
      if (!user) {
        this.feedback.error('No se encontró el usuario.');
        this.goBack();
        return;
      }
      this.originalRole = user.roles?.[0] ?? null;
      this.form.patchValue({
        name: user.name,
        surname: user.surname,
        email: user.email,
        role: this.originalRole ?? 'Visitor',
      });
      this.configureBirthDateValidators(this.form.controls['role'].value);
      this.roleChangeSub = this.form.controls['role'].valueChanges.subscribe((value) =>
        this.configureBirthDateValidators(value)
      );
    } finally {
      this.loading.set(false);
    }
  }

  async onSubmit(): Promise<void> {
    if (this.form.invalid || !this.userId) {
      markAllAsTouched(this.form);
      return;
    }

    this.saving.set(true);
    try {
      const { name, surname, email, password, role } = this.form.getRawValue();

      const updatePayload: any = { name, surname, email };
      if (password) {
        updatePayload.password = password;
      }

      const updated = await this.userService.updateUser(this.userId, updatePayload);
      if (!updated) {
        this.feedback.error('No se pudo actualizar el usuario.');
        return;
      }

      if (role && role !== this.originalRole) {
        const birthDateValue = this.form.controls['birthDate'].value;
        const birthDateIso =
          role === 'Visitor' && birthDateValue ? new Date(birthDateValue).toISOString() : undefined;
        const assigned = await this.userService.assignRole(this.userId, role, birthDateIso);
        if (!assigned) {
          this.feedback.error('No se pudo actualizar el rol del usuario.');
          return;
        }
      }

      this.feedback.success('El usuario se actualizó correctamente.');
      this.goBack();
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

  get showBirthDateField(): boolean {
    return this.form.controls['role'].value === 'Visitor';
  }

  private configureBirthDateValidators(role: string | null): void {
    const control = this.form.get('birthDate');
    if (!control) return;

    if (role === 'Visitor' && this.originalRole !== 'Visitor') {
      control.setValidators([Validators.required]);
    } else {
      control.clearValidators();
    }

    control.updateValueAndValidity({ emitEvent: false });
  }
}
