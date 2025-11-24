import { Component, inject } from '@angular/core';
import { NgIf, NgFor } from '@angular/common';
import { ActivatedRoute } from '@angular/router';
import { AttractionAccessService } from '../services/attraction-access.service';
import { AttractionService } from '../services/attraction.service';
import { FormsModule } from '@angular/forms';
import { Attraction } from '../models/attraction';
import { Router } from '@angular/router';

@Component({
  selector: 'app-capacity',
  standalone: true,
  imports: [NgIf, NgFor, FormsModule],
  templateUrl: './capacity.component.html',

})
export class CapacityComponent {
  private route = inject(ActivatedRoute);
  private service = inject(AttractionAccessService);
  private attractionService = inject(AttractionService);

  data: { attractionId: string; currentOccupancy: number; remainingCapacity: number } | null = null;
  attractions: Attraction[] = [];
  selectedAttractionId = this.route.snapshot.paramMap.get('id') ?? '';

  constructor() {
    this.loadAttractions();
    if (this.selectedAttractionId) {
      this.load(this.selectedAttractionId);
    }
  }

  private router = inject(Router);

  async loadAttractions() {
    await this.attractionService.fetchAttractions();
    this.attractions = this.attractionService.attractionsSignal();
  }

  async refreshAttractions() {
    await this.loadAttractions();
  }

  async loadSelected() {
    if (!this.selectedAttractionId) return;
    await this.load(this.selectedAttractionId);
  }

  async load(id: string) {
    this.data = await this.service.getCapacity(id);
  }

  goToRegister() {
    if (!this.selectedAttractionId) return;
    this.router.navigate(['access'], { queryParams: { attractionId: this.selectedAttractionId } });
  }

  goToIncidents() {
    if (!this.selectedAttractionId) return;
    this.router.navigate(['access', 'incidents', this.selectedAttractionId]);
  }

  goToExit() {
    if (!this.selectedAttractionId) return;
    this.router.navigate(['access', 'exit'], { queryParams: { attractionId: this.selectedAttractionId } });
  }

  get selectedAttractionLabel(): string {
    return this.attractions.find((a) => a.id === this.selectedAttractionId)?.name ?? 'N/A';
  }
}
