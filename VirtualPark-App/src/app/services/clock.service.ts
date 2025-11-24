import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../environments/environment';
import { Clock } from '../models/clock';

@Injectable({ providedIn: 'root' })
export class ClockService {
  private readonly baseUrl = `${environment.apiUrl}/api/Clock`;

  private http = inject(HttpClient);

  async getClock(): Promise<Clock | null> {
    try {
      const result = await this.http.get<Clock>(this.baseUrl).toPromise();
      return result ?? null;
    } catch {
      return null;
    }
  }

  async setClock(value: string): Promise<boolean> {
    try {
      await this.http.put(this.baseUrl, { value }).toPromise();
      return true;
    } catch {
      return false;
    }
  }
}
