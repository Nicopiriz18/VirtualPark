import { Component, OnInit, inject, signal } from '@angular/core';
import { Router, RouterModule } from '@angular/router';
import { CurrencyPipe, DatePipe, NgFor, NgIf } from '@angular/common';
import { SpecialEventService } from '../services/special-event.service';
import { AuthService } from '../services/auth.service';
import { FeedbackService } from '../services/feedback.service';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatSnackBarModule } from '@angular/material/snack-bar';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';

@Component({
  selector: 'app-special-events',
  standalone: true,
  imports: [
    NgFor,
    NgIf,
    RouterModule,
    DatePipe,
    CurrencyPipe,
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatSnackBarModule,
    MatProgressSpinnerModule,
  ],
  template: `
    <section class="mx-auto max-w-6xl px-4 py-6">
      <header class="mb-6 flex flex-col gap-4 md:flex-row md:items-center md:justify-between">
        <div>
          <h2 class="text-3xl font-semibold text-neutral">Eventos especiales</h2>
          <p class="text-sm text-neutral/70">
            Consulta y administra los eventos especiales del parque y su capacidad disponible.
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
          Nuevo evento
        </button>
      </header>

      <div class="grid gap-4 md:grid-cols-2" *ngIf="!isLoading(); else loading">
        <ng-container *ngIf="eventService.eventsSignal().length; else emptyState">
          <mat-card
            *ngFor="let event of eventService.eventsSignal()"
            class="shadow-sm transition hover:-translate-y-0.5 hover:shadow-lg"
          >
            <mat-card-header>
              <mat-icon mat-card-avatar color="primary">event</mat-icon>
              <mat-card-title class="text-lg font-semibold text-neutral">
                {{ event.name }}
              </mat-card-title>
              <mat-card-subtitle class="text-xs uppercase tracking-wide text-neutral/60">
                {{ event.date | date: 'fullDate' }}
              </mat-card-subtitle>
            </mat-card-header>
            <mat-card-content class="space-y-3 text-sm text-neutral/75">
              <div class="flex flex-wrap gap-4">
                <span class="flex items-center gap-1">
                  <mat-icon fontIcon="people" class="text-primary"></mat-icon>
                  Capacidad máxima: {{ event.maxCapacity }}
                </span>
                <span class="flex items-center gap-1">
                  <mat-icon fontIcon="attach_money" class="text-primary"></mat-icon>
                  Costo adicional: {{ event.additionalCost | currency: 'USD' }}
                </span>
              </div>
            </mat-card-content>
            <mat-card-actions class="flex flex-wrap gap-2">
              <button mat-stroked-button color="primary" (click)="viewDetails(event.id)">
                Ver detalles
              </button>
              <button mat-button color="warn" *ngIf="isAdmin" (click)="deleteEvent(event.id)">
                Eliminar
              </button>
            </mat-card-actions>
          </mat-card>
        </ng-container>
      </div>

      <ng-template #emptyState>
        <div class="rounded-lg border border-dashed border-neutral/20 bg-white p-10 text-center shadow-sm">
          <mat-icon class="mb-4 text-4xl text-neutral/30">event_busy</mat-icon>
          <h3 class="mb-2 text-xl font-semibold text-neutral">No hay eventos registrados</h3>
          <p class="mb-6 text-neutral/60">
            Crea un evento especial para comenzar a promocionarlo en el parque.
          </p>
          <button mat-flat-button color="primary" *ngIf="isAdmin" (click)="navigateToCreate()">
            Crear primer evento
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
export class SpecialEventsComponent implements OnInit {
  private loading = signal(false);
  private feedback = inject(FeedbackService);

  eventService = inject(SpecialEventService);
  router = inject(Router);
  auth = inject(AuthService);

  isLoading = () => this.loading();

  get isAdmin(): boolean {
    return this.auth.hasRole('Administrator');
  }

  async ngOnInit(): Promise<void> {
    await this.refresh();
  }

  viewDetails(id: string): void {
    this.router.navigate(['special-events', 'details', id]);
  }

  async deleteEvent(id: string): Promise<void> {
    if (!this.isAdmin) return;
    if (confirm('¿Estás seguro de que deseas eliminar este evento especial?')) {
      const removed = await this.eventService.deleteEvent(id);
      if (removed) {
        this.feedback.success('El evento especial se eliminó correctamente.');
      } else {
        this.feedback.error('No se pudo eliminar el evento especial. Intenta nuevamente.');
      }
      await this.refresh();
    }
  }

  navigateToCreate(): void {
    if (!this.isAdmin) return;
    this.router.navigate(['special-events', 'create']);
  }

  private async refresh(): Promise<void> {
    this.loading.set(true);
    try {
      await this.eventService.fetchEvents();
    } finally {
      this.loading.set(false);
    }
  }
}
