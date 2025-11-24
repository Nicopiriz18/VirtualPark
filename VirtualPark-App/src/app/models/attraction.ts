export interface Attraction {
  id: string; // GUID del backend
  name: string;
  type: string;
  description?: string;
  capacity?: number;
  minAge?: number;
}