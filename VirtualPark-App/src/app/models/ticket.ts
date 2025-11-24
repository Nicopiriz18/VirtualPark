export interface Ticket {
  id: string;
  visitorId: string;
  visitDate: string; // ISO
  type: 'General' | 'SpecialEvent' | number;
  qrCode: string; // GUID
  specialEventId?: string | null;
}
