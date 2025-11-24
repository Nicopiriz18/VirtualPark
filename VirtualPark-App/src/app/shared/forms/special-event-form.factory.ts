import { FormBuilder, Validators } from '@angular/forms';
import { SpecialEvent } from '../../models/special-event';

export function buildSpecialEventForm(
  fb: FormBuilder,
  initial: Partial<SpecialEvent> = {}
) {
  return fb.nonNullable.group({
    name: [initial.name ?? '', [Validators.required, Validators.minLength(3)]],
    date: [initial.date ?? '', Validators.required],
    maxCapacity: [initial.maxCapacity ?? 0, [Validators.required, Validators.min(1)]],
    additionalCost: [initial.additionalCost ?? 0, [Validators.required, Validators.min(0)]],
  });
}

export type SpecialEventFormGroup = ReturnType<typeof buildSpecialEventForm>;
