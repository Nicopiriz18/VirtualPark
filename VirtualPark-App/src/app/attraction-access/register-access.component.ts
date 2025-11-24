import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule, NgIf } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, ActivatedRoute, RouterModule } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonToggleModule } from '@angular/material/button-toggle';
import { MatSnackBarModule } from '@angular/material/snack-bar';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatDividerModule } from '@angular/material/divider';
import { AttractionAccessService } from '../services/attraction-access.service';
import { AttractionService } from '../services/attraction.service';
import { FeedbackService } from '../services/feedback.service';
import { Attraction } from '../models/attraction';
import { TicketType, TicketTypeValue } from '../models/ticket-type';
import { markAllAsTouched } from '../shared/utils/forms.util';
import { RegisterAccessRequest } from '../models/register-access-request';

@Component({
  selector: 'app-register-access',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    ReactiveFormsModule,
    NgIf,
    MatCardModule,
    MatFormFieldModule,
    MatSelectModule,
    MatInputModule,
    MatButtonModule,
    MatIconModule,
    MatButtonToggleModule,
    MatSnackBarModule,
    MatProgressSpinnerModule,
    MatDividerModule,
  ],
  templateUrl: './register-access.component.html',

})
export class RegisterAccessComponent implements OnInit {
  private fb = inject(FormBuilder);
  private router = inject(Router);
  private route = inject(ActivatedRoute);
  private accessService = inject(AttractionAccessService);
  private attractionService = inject(AttractionService);
  private feedback = inject(FeedbackService);

  private loadingAttractions = signal(false);
  private submitting = signal(false);

  attractions = signal<Attraction[]>([]);
  ticketTypes: TicketType[] = ['General', 'SpecialEvent'];

  form = this.fb.group({
    attractionId: ['', Validators.required],
    entryMethod: ['QR' as 'QR' | 'NFC', Validators.required],
    qrCode: [''],
    visitorId: [''],
    visitDate: [''],
    ticketType: ['General' as TicketType, Validators.required],
    specialEventId: [''],
  });

  isLoadingAttractions = () => this.loadingAttractions();
  isSubmitting = () => this.submitting();

  async ngOnInit(): Promise<void> {
    await this.loadAttractions();
    this.configureEntryMethodValidators(this.form.value.entryMethod ?? 'QR');

    const fromQuery = this.route.snapshot.queryParamMap.get('attractionId');
    if (fromQuery) {
      this.form.patchValue({ attractionId: fromQuery });
    }
  }

  async refreshAttractions(): Promise<void> {
    await this.loadAttractions();
    this.feedback.success('Listado de atracciones actualizado.');
  }

  onEntryMethodChange(method: 'QR' | 'NFC'): void {
    this.configureEntryMethodValidators(method);
  }

  async onSubmit(): Promise<void> {
    if (this.form.invalid) {
      markAllAsTouched(this.form);
      return;
    }

    const method = this.form.value.entryMethod ?? 'QR';
    if (method === 'QR' && !this.form.value.qrCode?.trim()) {
      this.form.controls['qrCode'].setErrors({ required: true });
      markAllAsTouched(this.form);
      return;
    }
    if (method === 'NFC' && !this.form.value.visitorId?.trim()) {
      this.form.controls['visitorId'].setErrors({ required: true });
      markAllAsTouched(this.form);
      return;
    }

    this.submitting.set(true);
    try {
      const request: RegisterAccessRequest = {
        entryMethod: method,
        visitDate: this.toIsoDate(this.form.value.visitDate),
        type: this.form.value.ticketType ?? 'General',
      };

      if (method === 'QR') {
        request.qrCode = this.form.value.qrCode?.trim();
      } else {
        request.visitorId = this.form.value.visitorId?.trim();
      }

      if (this.form.value.specialEventId?.trim()) {
        request.specialEventId = this.form.value.specialEventId.trim();
      }

      // API expects numeric ticket type, convert before sending
      const payload = {
        ...request,
        type: TicketTypeValue[request.type],
      };

      const result = await this.accessService.registerAccess(this.form.value.attractionId!, payload as any);
      if (result !== null) {
        this.feedback.success('El acceso se registró correctamente.');
        this.resetForm(false);
      } else {
        this.feedback.error('No se pudo registrar el acceso. Intenta nuevamente.');
      }
    } finally {
      this.submitting.set(false);
    }
  }

  resetForm(clearAttraction = false): void {
    const attractionId = clearAttraction ? '' : this.form.value.attractionId ?? '';
    this.form.reset({
      attractionId,
      entryMethod: this.form.value.entryMethod ?? 'QR',
      qrCode: '',
      visitorId: '',
      visitDate: '',
      ticketType: this.form.value.ticketType ?? 'General',
      specialEventId: '',
    });
    this.configureEntryMethodValidators(this.form.value.entryMethod ?? 'QR');
  }

  goToCapacity(): void {
    const attractionId = this.form.value.attractionId;
    if (!attractionId) return;
    this.router.navigate(['access', 'capacity', attractionId]);
  }

  goToIncidents(): void {
    const attractionId = this.form.value.attractionId;
    if (!attractionId) return;
    this.router.navigate(['access', 'incidents', attractionId]);
  }

  private async loadAttractions(): Promise<void> {
    this.loadingAttractions.set(true);
    try {
      await this.attractionService.fetchAttractions();
      this.attractions.set(this.attractionService.attractionsSignal());
    } finally {
      this.loadingAttractions.set(false);
    }
  }

  private configureEntryMethodValidators(method: 'QR' | 'NFC'): void {
    if (method === 'QR') {
      this.form.controls['qrCode'].setValidators([Validators.required]);
      this.form.controls['visitorId'].clearValidators();
      this.form.controls['visitorId'].setValue('');
    } else {
      this.form.controls['visitorId'].setValidators([Validators.required]);
      this.form.controls['qrCode'].clearValidators();
      this.form.controls['qrCode'].setValue('');
    }

    this.form.controls['qrCode'].updateValueAndValidity({ emitEvent: false });
    this.form.controls['visitorId'].updateValueAndValidity({ emitEvent: false });
  }

  private toIsoDate(dateValue: string | null | undefined): string {
    if (!dateValue) {
      return new Date().toISOString();
    }
    const date = new Date(dateValue);
    if (Number.isNaN(date.getTime())) {
      return new Date().toISOString();
    }
    return date.toISOString();
  }
}
