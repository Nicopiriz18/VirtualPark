import { FormBuilder, Validators } from '@angular/forms';
import { Attraction } from '../../models/attraction';

export function buildAttractionForm(fb: FormBuilder, initial: Partial<Attraction> = {}) {
  return fb.nonNullable.group({
    name: [initial.name ?? '', [Validators.required, Validators.minLength(3)]],
    type: [initial.type ?? '', [Validators.required, Validators.minLength(3)]],
    description: [initial.description ?? ''],
    capacity: [initial.capacity ?? 0, [Validators.required, Validators.min(1)]],
    minAge: [initial.minAge ?? 0, [Validators.required, Validators.min(0)]],
  });
}

export type AttractionFormGroup = ReturnType<typeof buildAttractionForm>;
