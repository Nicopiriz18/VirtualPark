import { Component, OnInit, inject, signal } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { CommonModule, DatePipe } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { MatSnackBarModule } from '@angular/material/snack-bar';
import { TicketService } from '../services/ticket.service';
import { SpecialEventService } from '../services/special-event.service';
import { SpecialEvent } from '../models/special-event';
import { buildPurchaseRequest } from '../models/purchase-ticket-request';
import { Ticket } from '../models/ticket';
import { FeedbackService } from '../services/feedback.service';
import { markAllAsTouched } from '../shared/utils/forms.util';
import { AuthService } from '../services/auth.service';
import { ClockService } from '../services/clock.service';

@Component({
  selector: 'app-purchase-ticket',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatButtonModule,
    MatSnackBarModule,
    DatePipe,
  ],
  templateUrl: './purchase-ticket.component.html',
  styleUrls: ['./purchase-ticket.component.css'],
})
export class PurchaseTicketComponent implements OnInit {
  private fb = inject(FormBuilder);
  private ticketService = inject(TicketService);
  private eventService = inject(SpecialEventService);
  private feedback = inject(FeedbackService);
  private authService = inject(AuthService);
  private clockService = inject(ClockService);

  events: SpecialEvent[] = [];
  currentUserEmail = '';

  private processing = signal(false);
  private currentUserId: string | null = null;
  private serverNowIso: string | null = null;
  ticket = signal<Ticket | null>(null);

  form: FormGroup = this.fb.group({
    visitorId: [{ value: '', disabled: true }, Validators.required],
    visitDate: [''],
    ticketType: ['General', Validators.required],
    specialEventId: [''],
  });

  get f() {
    return this.form.controls;
  }

  isProcessing = () => this.processing();

  async ngOnInit(): Promise<void> {
    this.setCurrentUser();
    await Promise.all([this.loadServerTime(), this.loadEvents()]);
  }

  private setCurrentUser(): void {
    this.currentUserId = this.authService.getUserId();
    if (!this.currentUserId) {
      this.feedback.error('No se pudo obtener el usuario actual.');
      return;
    }

    this.form.get('visitorId')?.setValue(this.currentUserId);
    this.form.get('visitorId')?.disable();
    this.currentUserEmail = this.authService.getUserEmail() ?? '';
  }

  private async loadEvents(): Promise<void> {
    await this.eventService.fetchEvents();
    this.events = this.eventService.eventsSignal();
  }

  private async loadServerTime(): Promise<void> {
    const clock = await this.clockService.getClock();
    if (clock?.currentDateTime) {
      this.serverNowIso = clock.currentDateTime;
      const formatted = this.formatForInput(clock.currentDateTime);
      if (formatted) {
        this.form.get('visitDate')?.setValue(formatted);
      }
    }
  }

  onTicketTypeChange(): void {
    if (this.form.value.ticketType === 'SpecialEvent') {
      this.form.get('specialEventId')?.setValidators([Validators.required]);
    } else {
      this.form.get('specialEventId')?.clearValidators();
      this.form.get('specialEventId')?.setValue('');
    }
    this.form.get('specialEventId')?.updateValueAndValidity();
  }

  resetForm(): void {
    this.form.reset({
      visitorId: this.currentUserId ?? '',
      visitDate: this.serverNowIso ? this.formatForInput(this.serverNowIso) : '',
      ticketType: 'General',
      specialEventId: '',
    });
    if (this.currentUserId) {
      this.form.get('visitorId')?.setValue(this.currentUserId);
    }
    this.form.get('visitorId')?.disable();
    this.ticket.set(null);
  }

  async onSubmit(): Promise<void> {
    if (!this.currentUserId) {
      this.feedback.error('Debes iniciar sesion para comprar una entrada.');
      return;
    }

    if (this.form.invalid) {
      markAllAsTouched(this.form);
      return;
    }

    this.processing.set(true);
    try {
      const { visitorId, visitDate, ticketType, specialEventId } = this.form.getRawValue();
      const visitIso = visitDate ? new Date(visitDate).toISOString() : this.getDefaultVisitDate();

      const request = buildPurchaseRequest({
        visitorId,
        visitDate: visitIso,
        type: ticketType,
        specialEventId: specialEventId || undefined,
      });

      const result = await this.ticketService.purchase(request);
      if (result) {
        this.ticket.set(result);
        this.feedback.success('Entrada comprada con éxito.');
      } else {
        this.feedback.error('No se pudo procesar la compra.');
      }
    } finally {
      this.processing.set(false);
    }
  }

  private getDefaultVisitDate(): string {
    return this.serverNowIso ?? new Date().toISOString();
  }

  private formatForInput(value: string): string {
    const date = new Date(value);
    if (Number.isNaN(date.getTime())) {
      return '';
    }
    const pad = (v: number) => v.toString().padStart(2, '0');
    return `${date.getFullYear()}-${pad(date.getMonth() + 1)}-${pad(date.getDate())}T${pad(
      date.getHours()
    )}:${pad(date.getMinutes())}`;
  }
}
