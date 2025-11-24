import { Component, OnInit, inject } from '@angular/core';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { NgIf } from '@angular/common';
import { UserService } from '../services/user.service';
import { AuthService } from '../services/auth.service';
import { User } from '../models/user';

@Component({
  selector: 'app-user-details',
  standalone: true,
  imports: [NgIf, RouterModule],
  templateUrl: './user-details.component.html',

})
export class UserDetailsComponent implements OnInit {
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private userService = inject(UserService);
  private auth = inject(AuthService);

  user: User | null = null;

  get isAdmin(): boolean {
    return this.auth.hasRole('Administrator');
  }

  async ngOnInit() {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.user = await this.userService.fetchUserById(id);
    }
  }

  editUser() {
    if (!this.isAdmin || !this.user) return;
    this.router.navigate(['users', 'edit', this.user.id]);
  }

  goBack() {
    this.router.navigate(['users']);
  }
}
