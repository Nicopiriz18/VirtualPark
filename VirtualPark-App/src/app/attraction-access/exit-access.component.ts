import { Component, inject } from '@angular/core';
import { NgIf, NgFor } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AttractionAccessService } from '../services/attraction-access.service';
import { AttractionService } from '../services/attraction.service';
import { UserService } from '../services/user.service';
import { Attraction } from '../models/attraction';
import { User } from '../models/user';
import { ActivatedRoute, Router } from '@angular/router';

@Component({
  selector: 'app-exit-access',
  standalone: true,
  imports: [NgIf, NgFor, FormsModule],
  templateUrl: './exit-access.component.html',

})
export class ExitAccessComponent {
  private service = inject(AttractionAccessService);
  private attractionService = inject(AttractionService);
  private userService = inject(UserService);
  private route = inject(ActivatedRoute);
  private router = inject(Router);

  attractions: Attraction[] = [];
  visitors: User[] = [];

  selectedAttractionId = this.route.snapshot.queryParamMap.get('attractionId') ?? '';
  selectedVisitorId = '';

  message = '';
  messageType: 'success' | 'error' | 'warning' | '' = '';
  isSubmitting = false;

  constructor() {
    this.loadAttractions();
    this.loadVisitors();
  }

  async loadAttractions() {
    await this.attractionService.fetchAttractions();
    this.attractions = this.attractionService.attractionsSignal();
  }

  async refreshAttractions() {
    await this.loadAttractions();
  }

  async loadVisitors() {
    await this.userService.fetchUsers();
    // filter visitor role
    this.visitors = this.userService.usersSignal().filter(u => (u.roles ?? []).includes('Visitor'));
  }

  async refreshVisitors() {
    await this.loadVisitors();
  }

  async submit() {
    if (!this.selectedAttractionId) {
      this.message = 'Seleccione una atracción';
      return;
    }
    if (!this.selectedVisitorId) {
      this.message = 'Seleccione un visitante';
      return;
    }

    this.isSubmitting = true;
    const res = await this.service.registerExit(this.selectedAttractionId, this.selectedVisitorId);
    this.isSubmitting = false;

    if (res) {
      this.setMessage('Salida registrada con éxito', 'success');
    } else {
      this.setMessage('Error al registrar la salida', 'error');
    }
  }

  goBack() {
    this.router.navigate(['access']);
  }

  private setMessage(message: string, type: 'success' | 'error' | 'warning' | '') {
    this.message = message;
    this.messageType = type;
  }
}
