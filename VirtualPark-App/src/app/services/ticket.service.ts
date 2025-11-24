import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { firstValueFrom } from 'rxjs';
import { environment } from '../environments/environment';
import { PurchaseTicketRequest } from '../models/purchase-ticket-request';
import { Ticket } from '../models/ticket';

@Injectable({ providedIn: 'root' })
export class TicketService {
  private readonly baseUrl = `${environment.apiUrl}/api/Tickets`;

  private http = inject(HttpClient);

  async purchase(request: PurchaseTicketRequest): Promise<Ticket | null> {
    try {
      const ticket = await firstValueFrom(this.http.post<Ticket>(this.baseUrl, request));
      return ticket ?? null;
    } catch (error) {
      return null;
    }
  }
}
