import { Component, OnInit, inject } from '@angular/core';
import { NgIf, DatePipe } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { ClockService } from '../services/clock.service';
import { AuthService } from '../services/auth.service';
import { Clock } from '../models/clock';

@Component({
  selector: 'app-clock',
  imports: [NgIf, ReactiveFormsModule, DatePipe],
  templateUrl: './clock.component.html',

  styles: [`.clock-container { padding: 2rem; max-width: 400px; margin: 0 auto; background: #fff; border-radius: 8px; box-shadow: 0 2px 8px #0001; }`]
})
export class ClockComponent implements OnInit {
  private clockService = inject(ClockService);
  private auth = inject(AuthService);
  private fb = inject(FormBuilder);

  clock: Clock | null = null;
  form = this.fb.group({
    newValue: ['', [Validators.required, Validators.pattern(/^\d{4}-\d{2}-\d{2}T\d{2}:\d{2}$/)]],
  });
  message = '';

  get isAdmin(): boolean {
    return this.auth.hasRole('Administrator');
  }

  async ngOnInit() {
    this.clock = await this.clockService.getClock();
  }

  async setClock() {
    if (this.form.invalid) return;
    const newValue = this.form.value.newValue as string;
    const ok = await this.clockService.setClock(newValue);
    if (ok) {
      this.message = 'Hora actualizada correctamente';
      this.clock = await this.clockService.getClock();
      this.form.reset();
    } else {
      this.message = 'Error al actualizar la hora';
    }
  }
}
