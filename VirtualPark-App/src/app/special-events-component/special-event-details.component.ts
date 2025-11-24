import { Component, OnInit, inject, signal } from '@angular/core';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { CurrencyPipe, DatePipe, NgIf } from '@angular/common';
import { SpecialEventService } from '../services/special-event.service';
import { FeedbackService } from '../services/feedback.service';
import { SpecialEvent } from '../models/special-event';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';

@Component({
  selector: 'app-special-event-details',
  standalone: true,
  styleUrls: ['./special-event-details.component.css'],
  imports: [
    RouterModule,
    NgIf,
    DatePipe,
    CurrencyPipe,
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatProgressSpinnerModule,
  ],
  templateUrl: './special-event-details.component.html',

})
export class SpecialEventDetailsComponent implements OnInit {
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private eventService = inject(SpecialEventService);
  private feedback = inject(FeedbackService);

  private loading = signal(true);

  event = signal<SpecialEvent | null>(null);

  isLoading = () => this.loading();

  async ngOnInit(): Promise<void> {
    const id = this.route.snapshot.paramMap.get('id');
    if (!id) {
      this.feedback.error('Identificador de evento inválido.');
      this.goBack();
      return;
    }

    try {
      const data = await this.eventService.fetchEventById(id);
      this.event.set(data ?? null);
    } finally {
      this.loading.set(false);
    }
  }

  goBack(): void {
    this.router.navigate(['special-events']);
  }
}
