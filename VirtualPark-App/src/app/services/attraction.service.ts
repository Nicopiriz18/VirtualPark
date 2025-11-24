import { Injectable, signal, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { firstValueFrom } from 'rxjs';
import { environment } from '../environments/environment';
import { Attraction } from '../models/attraction';

@Injectable({
  providedIn: 'root',
})
export class AttractionService {
  // Private mutable signal
  private readonly privateAttractionsSignal = signal<Attraction[]>([]);
  // Public readonly signal
  readonly attractionsSignal = this.privateAttractionsSignal.asReadonly();

  private readonly baseUrl = `${environment.apiUrl}/api/attractions`;

  private http = inject(HttpClient);

  constructor() {
    // Eager initialization
    this.fetchAttractions();
  }

  /** Fetch all attractions from the API */
  async fetchAttractions(): Promise<Attraction[]> {
    try {
      const data = await firstValueFrom(this.http.get<Attraction[]>(this.baseUrl));
      this.privateAttractionsSignal.set(data ?? []);
      return data ?? [];
    } catch (error) {
      console.warn('Failed to fetch attractions (likely unauthorized)', error);
      this.privateAttractionsSignal.set([]);
      return [];
    }
  }

  /** Fetch a single attraction by ID */
  async fetchAttractionById(id: string): Promise<Attraction | null> {
    try {
      const attraction = await firstValueFrom(
        this.http.get<Attraction>(`${this.baseUrl}/${id}`)
      );
      return attraction ?? null;
    } catch (error) {
      console.error(`Failed to fetch attraction with ID ${id}`, error);
      return null;
    }
  }

  /** Create a new attraction */
  async createAttraction(attraction: Omit<Attraction, 'id'>): Promise<Attraction | null> {
    try {
      const createdAttraction = await firstValueFrom(
        this.http.post<Attraction>(this.baseUrl, attraction)
      );
      if (!createdAttraction) {
        return null;
      }
      // Update the signal with the new attraction
      this.privateAttractionsSignal.update((attractions) => [...attractions, createdAttraction]);
      return createdAttraction;
    } catch (error) {
      console.error('Failed to create attraction', error);
      return null;
    }
  }

  /** Update an existing attraction */
  async updateAttraction(id: string, updatedAttraction: Partial<Attraction>): Promise<boolean> {
    try {
      await firstValueFrom(this.http.put<void>(`${this.baseUrl}/${id}`, updatedAttraction));
      // Update the signal with the modified attraction
      this.privateAttractionsSignal.update((attractions) =>
        attractions.map((a) => (a.id === id ? { ...a, ...updatedAttraction } : a))
      );
      return true;
    } catch (error) {
      console.error(`Failed to update attraction with ID ${id}`, error);
      return false;
    }
  }

  /** Delete an attraction */
  async deleteAttraction(id: string): Promise<boolean> {
    try {
      await firstValueFrom(this.http.delete(`${this.baseUrl}/${id}`));
      // Remove the deleted attraction from the signal
      this.privateAttractionsSignal.update((attractions) => attractions.filter((a) => a.id !== id));
      return true;
    } catch (error) {
      console.error(`Failed to delete attraction with ID ${id}`, error);
      return false;
    }
  }
}
