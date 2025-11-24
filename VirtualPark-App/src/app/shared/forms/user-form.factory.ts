import { FormBuilder, Validators } from '@angular/forms';
import { User } from '../../models/user';

interface UserFormOptions {
  requirePassword?: boolean;
}

interface UserFormInitial extends Partial<User & { password?: string }> {
  birthDate?: string;
}

export function buildUserForm(
  fb: FormBuilder,
  initial: UserFormInitial = {},
  options: UserFormOptions = {}
) {
  const requirePassword = options.requirePassword ?? true;
  const passwordValidators = requirePassword
    ? [Validators.required, Validators.minLength(6)]
    : [Validators.pattern(/^$|.{6,}$/)];
  const role = initial.roles?.[0] ?? 'Visitor';

  return fb.nonNullable.group({
    name: [initial.name ?? '', [Validators.required, Validators.minLength(2)]],
    surname: [initial.surname ?? '', [Validators.required, Validators.minLength(2)]],
    email: [initial.email ?? '', [Validators.required, Validators.email]],
    password: [initial.password ?? '', passwordValidators],
    role: [role, Validators.required],
    birthDate: [initial.birthDate ?? ''],
  });
}

export type UserFormGroup = ReturnType<typeof buildUserForm>;
