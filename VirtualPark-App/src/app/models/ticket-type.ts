export type TicketType = 'General' | 'SpecialEvent';

// helper mapping to numeric values if you need to send numbers instead of strings
export const TicketTypeValue: Record<TicketType, number> = {
  General: 0,
  SpecialEvent: 1,
};
