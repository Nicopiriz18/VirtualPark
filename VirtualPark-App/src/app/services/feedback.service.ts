import { Injectable, inject } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';

@Injectable({
  providedIn: 'root',
})
export class FeedbackService {
  private snackBar = inject(MatSnackBar);

  success(message: string): void {
    this.open(message, ['bg-green-600', 'text-white']);
  }

  info(message: string): void {
    this.open(message, ['bg-primary', 'text-white']);
  }

  error(message: string): void {
    this.open(message, ['bg-red-600', 'text-white']);
  }

  private open(message: string, panelClass: string[]): void {
    this.snackBar.open(message, 'Cerrar', {
      duration: 4000,
      horizontalPosition: 'right',
      verticalPosition: 'top',
      panelClass,
    });
  }
}
