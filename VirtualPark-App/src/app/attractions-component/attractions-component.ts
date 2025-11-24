import { Component, OnInit, inject, signal } from '@angular/core';
import { NgFor, NgIf } from '@angular/common';
import { Router, RouterModule } from '@angular/router';
import { AttractionService } from '../services/attraction.service';
import { AuthService } from '../services/auth.service';
import { FeedbackService } from '../services/feedback.service';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatIconModule } from '@angular/material/icon';
import { MatSnackBarModule } from '@angular/material/snack-bar';

@Component({
  selector: 'app-attractions',
  imports: [
    NgFor,
    NgIf,
    RouterModule,
    MatCardModule,
    MatButtonModule,
    MatProgressSpinnerModule,
    MatIconModule,
    MatSnackBarModule,
  ],
  template: `
    <section class="mx-auto max-w-6xl px-4 py-6">
      <header class="mb-6 flex flex-col gap-4 md:flex-row md:items-center md:justify-between">
        <div>
          <h2 class="text-3xl font-semibold text-neutral">Atracciones</h2>
          <p class="text-sm text-neutral/70">
            Gestiona las atracciones del parque. Usa las acciones para ver, editar o eliminar entradas.
          </p>
        </div>
        <button
          mat-flat-button
          color="primary"
          class="self-start md:self-auto"
          *ngIf="isAdmin"
          (click)="navigateToCreate()"
        >
          <mat-icon class="mr-2">add</mat-icon>
          Nueva atracción
        </button>
      </header>

      <div class="grid gap-4 md:grid-cols-2" *ngIf="!isLoading(); else loading">
        <ng-container *ngIf="attractionService.attractionsSignal().length; else emptyState">
          <mat-card
            *ngFor="let attraction of attractionService.attractionsSignal()"
            class="shadow-sm transition hover:-translate-y-0.5 hover:shadow-lg"
          >
            <mat-card-header>
              <mat-icon mat-card-avatar color="primary">attractions</mat-icon>
              <mat-card-title class="text-lg font-semibold text-neutral">
                {{ attraction.name }}
              </mat-card-title>
              <mat-card-subtitle class="text-xs uppercase tracking-wide text-neutral/60">
                {{ attraction.type }}
              </mat-card-subtitle>
            </mat-card-header>
            <mat-card-content class="space-y-3 text-sm text-neutral/75">
              <p *ngIf="attraction.description">{{ attraction.description }}</p>
              <div class="flex flex-wrap gap-4">
                <span class="flex items-center gap-1">
                  <mat-icon fontIcon="group" class="text-primary"></mat-icon>
                  Capacidad: {{ attraction.capacity ?? '–' }}
                </span>
                <span class="flex items-center gap-1">
                  <mat-icon fontIcon="child_care" class="text-primary"></mat-icon>
                  Edad mínima: {{ attraction.minAge ?? '–' }}
                </span>
              </div>
            </mat-card-content>
            <mat-card-actions class="flex flex-wrap gap-2">
              <button mat-stroked-button color="primary" (click)="viewDetails(attraction.id)">
                Detalles
              </button>
              <button mat-button color="accent" *ngIf="isAdmin" (click)="editAttraction(attraction.id)">
                Editar
              </button>
              <button mat-button color="warn" *ngIf="isAdmin" (click)="deleteAttraction(attraction.id)">
                Eliminar
              </button>
            </mat-card-actions>
          </mat-card>
        </ng-container>
      </div>

      <ng-template #emptyState>
        <div class="rounded-lg border border-dashed border-neutral/20 bg-white p-10 text-center shadow-sm">
          <mat-icon class="mb-4 text-4xl text-neutral/30">sentiment_dissatisfied</mat-icon>
          <h3 class="mb-2 text-xl font-semibold text-neutral">Aún no hay atracciones</h3>
          <p class="mb-6 text-neutral/60">
            Cuando se cree una atracción se mostrará aquí. Usa el botón para registrar la primera.
          </p>
          <button mat-flat-button color="primary" *ngIf="isAdmin" (click)="navigateToCreate()">
            Crear atracción
          </button>
        </div>
      </ng-template>

      <ng-template #loading>
        <div class="flex min-h-[40vh] items-center justify-center">
          <mat-progress-spinner diameter="48" mode="indeterminate"></mat-progress-spinner>
        </div>
      </ng-template>
    </section>
  `,
})
export class AttractionsComponent implements OnInit {
  private feedback = inject(FeedbackService);
  private loading = signal<boolean>(false);

  attractionService = inject(AttractionService);
  router = inject(Router);
  auth = inject(AuthService);

  isLoading = () => this.loading();

  get isAdmin(): boolean {
    return this.auth.hasRole('Administrator');
  }

  async ngOnInit(): Promise<void> {
    await this.refreshList();
  }

  viewDetails(id: string): void {
    this.router.navigate(['attractions', 'details', id]);
  }

  editAttraction(id: string): void {
    if (!this.isAdmin) return;
    this.router.navigate(['attractions', 'edit', id]);
  }

  async deleteAttraction(id: string): Promise<void> {
    if (!this.isAdmin) return;
    if (confirm('¿Estás seguro de que deseas eliminar esta atracción?')) {
      const removed = await this.attractionService.deleteAttraction(id);
      if (removed) {
        this.feedback.success('La atracción se eliminó correctamente.');
      } else {
        this.feedback.error('No se pudo eliminar la atracción. Intenta nuevamente.');
      }
      await this.refreshList();
    }
  }

  navigateToCreate(): void {
    if (!this.isAdmin) return;
    this.router.navigate(['attractions', 'create']);
  }

  private async refreshList(): Promise<void> {
    this.loading.set(true);
    try {
      await this.attractionService.fetchAttractions();
    } finally {
      this.loading.set(false);
    }
  }
}
