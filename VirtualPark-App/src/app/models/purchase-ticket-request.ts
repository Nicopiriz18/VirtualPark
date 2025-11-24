import { TicketTypeValue } from './ticket-type';

export interface PurchaseTicketRequest {
  visitorId: string;
  visitDate: string; // ISO
  type: number; // numeric enum expected by backend
  specialEventId?: string | null;
}

export function buildPurchaseRequest(values: { visitorId: string; visitDate: string; type: 'General'|'SpecialEvent'; specialEventId?: string|null }): PurchaseTicketRequest {
  return {
    visitorId: values.visitorId,
    visitDate: values.visitDate,
    type: TicketTypeValue[values.type],
    specialEventId: values.specialEventId ?? null,
  };
}
