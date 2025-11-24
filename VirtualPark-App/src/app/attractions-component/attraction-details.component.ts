import { Component, OnInit, inject } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { AttractionService } from '../services/attraction.service';
import { CommonModule, NgIf } from '@angular/common';

@Component({
  selector: 'app-attraction-details',
  standalone: true,
  imports: [CommonModule, NgIf],
  templateUrl: './attraction-details.component.html',

})
export class AttractionDetailsComponent implements OnInit {
  attractionService = inject(AttractionService);
  route = inject(ActivatedRoute);
  attraction: any;

  async ngOnInit() {
    const id = this.route.snapshot.params['id'];
    this.attraction = await this.attractionService.fetchAttractionById(id);
  }
}
