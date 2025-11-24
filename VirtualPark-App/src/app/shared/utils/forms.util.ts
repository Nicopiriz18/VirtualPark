import { AbstractControl, FormArray, FormControl, FormGroup } from '@angular/forms';

/**
 * Marks all controls within a form group/array as touched to trigger validation messages.
 */
export function markAllAsTouched(control: AbstractControl): void {
  if (control instanceof FormGroup || control instanceof FormArray) {
    Object.values(control.controls).forEach(markAllAsTouched);
  } else if (control instanceof FormControl) {
    control.markAsTouched({ onlySelf: true });
  }
  control.updateValueAndValidity({ onlySelf: true });
}
