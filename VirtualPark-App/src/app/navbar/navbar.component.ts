import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterModule, RouterLinkActive } from '@angular/router';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatTabsModule } from '@angular/material/tabs';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { AuthService } from '../services/auth.service';

type NavLink = { path: string; label: string; roles?: string[] };

@Component({
  selector: 'app-navbar',
  templateUrl: './navbar.component.html',
  styleUrls: ['./navbar.component.css'],
  imports: [
    CommonModule,
    RouterModule,
    RouterLinkActive,
    MatToolbarModule,
    MatTabsModule,
    MatButtonModule,
    MatIconModule,
  ],
})
export class NavbarComponent {
  private router = inject(Router);
  public auth = inject(AuthService);

  navLinks: NavLink[] = [
    { path: '/attractions', label: 'Attractions' },
    { path: '/access', label: 'Attraction Access', roles: ['Operator'] },
    { path: '/users', label: 'Users', roles: ['Administrator'] },
    { path: '/tickets/purchase', label: 'Tickets' },
    { path: '/rewards/redeem', label: 'Rewards', roles: ['Visitor'] },
    { path: '/rewards/manage', label: 'Rewards (Admin)', roles: ['Administrator'] },
    { path: '/maintenance', label: 'Maintenance', roles: ['Administrator'] },
    { path: '/scoring', label: 'Scoring', roles: ['Administrator'] },
    { path: '/reports/attraction-usage', label: 'Reports', roles: ['Administrator'] },
    { path: '/special-events', label: 'Special Events', roles: ['Administrator'] },
    { path: '/visitor-profile', label: 'Profile', roles: ['Visitor', 'Administrator'] },
  ];

  get loggedIn(): boolean {
    return !!this.auth.getToken();
  }

  get displayedLinks(): NavLink[] {
    const roles = this.auth.getRoles();
    return this.navLinks.filter((link) => !link.roles?.length || link.roles.some((role) => roles.includes(role)));
  }

  goLogin(): void {
    this.router.navigate(['/login']);
  }

  logout(): void {
    this.auth.logout();
    this.router.navigate(['/login']);
  }

  goClock(): void {
    this.router.navigate(['/clock']);
  }
}
