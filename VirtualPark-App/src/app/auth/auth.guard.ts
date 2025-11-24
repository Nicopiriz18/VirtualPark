import { inject } from '@angular/core';
import { CanActivateChildFn, CanActivateFn, Router } from '@angular/router';

export const authGuard: CanActivateFn = (_route, _state) => {
  const token = localStorage.getItem('auth_token');
  if (token) return true;
  const router = inject(Router);
  return router.parseUrl('/login');
};

export const authChildGuard: CanActivateChildFn = (_childRoute, _state) => {
  const token = localStorage.getItem('auth_token');
  if (token) return true;
  const router = inject(Router);
  return router.parseUrl('/login');
};

export const guestGuard: CanActivateFn = (_route, _state) => {
  const token = localStorage.getItem('auth_token');
  if (token) {
    const router = inject(Router);
    return router.parseUrl('/attractions');
  }
  return true;
};

export const roleGuard: CanActivateFn = (route, _state) => {
  const token = localStorage.getItem('auth_token');
  if (!token) {
    const router = inject(Router);
    return router.parseUrl('/login');
  }
  const required: string[] | undefined = (route.data as any)?.['roles'];
  if (!required || required.length === 0) return true;
  let roles: string[] = [];
  try {
    roles = JSON.parse(localStorage.getItem('roles') ?? '[]');
  } catch {}
  if (roles.some(r => required.includes(r))) return true;
  const router = inject(Router);
  return router.parseUrl('/');
};
