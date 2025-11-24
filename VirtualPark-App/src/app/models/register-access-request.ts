import { TicketType } from './ticket-type';

export interface RegisterAccessRequest {
  entryMethod: 'QR' | 'NFC';
  qrCode?: string; // GUID as string
  visitorId?: string; // GUID
  visitDate: string; // ISO date
  type: TicketType;
  specialEventId?: string;
}
