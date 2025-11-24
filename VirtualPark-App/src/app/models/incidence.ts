export interface Incidence {
  id: string;
  title: string;
  description: string;
  status: boolean;
  date: string; // ISO
  attractionId: string;
  attractionName?: string;
}
