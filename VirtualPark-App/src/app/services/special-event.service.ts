import { Injectable, signal, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { firstValueFrom } from 'rxjs';
import { environment } from '../environments/environment';
import { SpecialEvent } from '../models/special-event';

@Injectable({
  providedIn: 'root',
})
export class SpecialEventService {
  private readonly privateEventsSignal = signal<SpecialEvent[]>([]);
  readonly eventsSignal = this.privateEventsSignal.asReadonly();

  private readonly baseUrl = `${environment.apiUrl}/api/SpecialEvent`;

  private http = inject(HttpClient);

  constructor() {
    this.fetchEvents();
  }

  async fetchEvents(): Promise<SpecialEvent[]> {
    try {
      const data = await firstValueFrom(this.http.get<SpecialEvent[]>(this.baseUrl));
      this.privateEventsSignal.set(data ?? []);
      return data ?? [];
    } catch (error) {
      this.privateEventsSignal.set([]);
      return [];
    }
  }

  async fetchEventById(id: string): Promise<SpecialEvent | null> {
    try {
      const event = await firstValueFrom(this.http.get<SpecialEvent>(`${this.baseUrl}/${id}`));
      return event ?? null;
    } catch (error) {
      return null;
    }
  }

  async createEvent(event: Omit<SpecialEvent, 'id'>): Promise<SpecialEvent | null> {
    try {
      const created = await firstValueFrom(this.http.post<SpecialEvent>(this.baseUrl, event));
      if (!created) return null;
      this.privateEventsSignal.update((events) => [...events, created]);
      return created;
    } catch (error) {
      return null;
    }
  }

  async updateEvent(id: string, updated: Partial<SpecialEvent>): Promise<boolean> {
    try {
      await firstValueFrom(this.http.put<void>(`${this.baseUrl}/${id}`, updated));
      this.privateEventsSignal.update((events) =>
        events.map((e) => (e.id === id ? { ...e, ...updated } : e))
      );
      return true;
    } catch (error) {
      return false;
    }
  }

  async deleteEvent(id: string): Promise<boolean> {
    try {
      await firstValueFrom(this.http.delete(`${this.baseUrl}/${id}`));
      this.privateEventsSignal.update((events) => events.filter((e) => e.id !== id));
      return true;
    } catch (error) {
      return false;
    }
  }
}
