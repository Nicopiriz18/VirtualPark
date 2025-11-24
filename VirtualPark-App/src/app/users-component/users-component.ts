import { Component, OnInit, inject, signal } from '@angular/core';
import { Router, RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSnackBarModule } from '@angular/material/snack-bar';
import { UserService } from '../services/user.service';
import { AuthService } from '../services/auth.service';
import { FeedbackService } from '../services/feedback.service';
import { User } from '../models/user';

@Component({
  selector: 'app-users',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    MatCardModule,
    MatTableModule,
    MatButtonModule,
    MatIconModule,
    MatProgressSpinnerModule,
    MatSnackBarModule,
  ],
  template: `
    <section class="mx-auto max-w-6xl px-4 py-6">
      <mat-card class="shadow-sm">
        <mat-card-header class="flex flex-col gap-2 md:flex-row md:items-center md:justify-between">
          <div>
            <mat-card-title class="text-2xl font-semibold text-neutral">Usuarios</mat-card-title>
            <mat-card-subtitle class="text-sm text-neutral/70">
              Gestiona los usuarios del sistema y sus roles asignados.
            </mat-card-subtitle>
          </div>
          <button
            mat-flat-button
            color="primary"
            class="self-start md:self-auto"
            *ngIf="isAdmin"
            (click)="navigateToCreate()"
          >
            <mat-icon class="mr-2">person_add</mat-icon>
            Nuevo usuario
          </button>
        </mat-card-header>
        <mat-card-content>
          <ng-container *ngIf="!isLoading(); else loading">
            <div *ngIf="users.length; else emptyState" class="overflow-x-auto">
              <table mat-table [dataSource]="users" class="min-w-full">
                <ng-container matColumnDef="name">
                  <th mat-header-cell *matHeaderCellDef>Nombre</th>
                  <td mat-cell *matCellDef="let user">
                    {{ user.name }} {{ user.surname }}
                  </td>
                </ng-container>

                <ng-container matColumnDef="email">
                  <th mat-header-cell *matHeaderCellDef>Email</th>
                  <td mat-cell *matCellDef="let user">
                    {{ user.email }}
                  </td>
                </ng-container>

                <ng-container matColumnDef="roles">
                  <th mat-header-cell *matHeaderCellDef>Roles</th>
                  <td mat-cell *matCellDef="let user">
                    {{ user.roles.join(', ') }}
                  </td>
                </ng-container>

                <ng-container matColumnDef="actions">
                  <th mat-header-cell *matHeaderCellDef class="w-32 text-right">Acciones</th>
                  <td mat-cell *matCellDef="let user" class="w-32 text-right">
                    <button
                      mat-icon-button
                      color="primary"
                      (click)="viewDetails(user.id)"
                      matTooltip="Detalles"
                    >
                      <mat-icon>visibility</mat-icon>
                    </button>
                    <button
                      mat-icon-button
                      color="accent"
                      *ngIf="isAdmin"
                      (click)="editUser(user.id)"
                      matTooltip="Editar"
                    >
                      <mat-icon>edit</mat-icon>
                    </button>
                  </td>
                </ng-container>

                <tr mat-header-row *matHeaderRowDef="displayedColumns"></tr>
                <tr mat-row *matRowDef="let row; columns: displayedColumns"></tr>
              </table>
            </div>
          </ng-container>
        </mat-card-content>
      </mat-card>

      <ng-template #loading>
        <div class="flex min-h-[40vh] items-center justify-center">
          <mat-progress-spinner diameter="48" mode="indeterminate"></mat-progress-spinner>
        </div>
      </ng-template>

      <ng-template #emptyState>
        <div class="rounded-lg border border-dashed border-neutral/20 bg-white p-10 text-center shadow-sm">
          <mat-icon class="mb-4 text-4xl text-neutral/30">group_off</mat-icon>
          <h3 class="mb-2 text-xl font-semibold text-neutral">No se encontraron usuarios</h3>
          <p class="mb-6 text-neutral/60">
            Cuando registres usuarios aparecerán listados aquí.
          </p>
          <button mat-flat-button color="primary" (click)="navigateToCreate()" *ngIf="isAdmin">
            Crear primer usuario
          </button>
        </div>
      </ng-template>
    </section>
  `,
})
export class UsersComponent implements OnInit {
  private userService = inject(UserService);
  private router = inject(Router);
  private auth = inject(AuthService);
  private feedback = inject(FeedbackService);

  private loading = signal(true);

  users: User[] = [];
  displayedColumns: string[] = ['name', 'email', 'roles', 'actions'];

  isLoading = () => this.loading();

  get isAdmin(): boolean {
    return this.auth.hasRole('Administrator');
  }

  async ngOnInit(): Promise<void> {
    await this.refreshUsers();
  }

  private async refreshUsers(): Promise<void> {
    this.loading.set(true);
    try {
      const data = await this.userService.fetchUsers();
      this.users = data;
    } finally {
      this.loading.set(false);
    }
  }

  viewDetails(id: string): void {
    this.router.navigate(['users', 'details', id]);
  }

  editUser(id: string): void {
    if (!this.isAdmin) {
      this.feedback.error('No tienes permisos para editar usuarios.');
      return;
    }
    this.router.navigate(['users', 'edit', id]);
  }

  navigateToCreate(): void {
    if (!this.isAdmin) {
      this.feedback.error('No tienes permisos para crear usuarios.');
      return;
    }
    this.router.navigate(['users', 'create']);
  }
}
