export interface Clock {
  currentDateTime: string; // ISO string
  formattedDateTime: string;
  isSystemTime: boolean;
  lastModified?: string;
  timeZone?: string;
}
