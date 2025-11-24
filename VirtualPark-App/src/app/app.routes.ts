import { Routes } from '@angular/router';
import { authChildGuard, guestGuard, roleGuard } from './auth/auth.guard';
import { NavbarComponent } from './navbar/navbar.component';
import { PlaceholderComponent } from './placeholder.component';

export const routes: Routes = [
  { path: 'login', canActivate: [guestGuard], loadComponent: () => import('./auth/login.component').then(m => m.LoginComponent) },
  { path: 'register', canActivate: [guestGuard], loadComponent: () => import('./auth/register.component').then(m => m.RegisterComponent) },
  { path: '', component: NavbarComponent, canActivateChild: [authChildGuard], children: [
    { path: '', redirectTo: 'attractions', pathMatch: 'full' },
    { path: 'attractions', loadComponent: () => import('./attractions-component/attractions-component').then(m => m.AttractionsComponent) },
    { path: 'attractions/details/:id', loadComponent: () => import('./attractions-component/attraction-details.component').then(m => m.AttractionDetailsComponent) },
    { path: 'attractions/create', canActivate: [roleGuard], data: { roles: ['Administrator'] }, loadComponent: () => import('./attractions-component/create-attraction.component').then(m => m.CreateAttractionComponent) },
    { path: 'attractions/edit/:id', canActivate: [roleGuard], data: { roles: ['Administrator'] }, loadComponent: () => import('./attractions-component/edit-attraction.component').then(m => m.EditAttractionComponent) },
    { path: 'events', component: PlaceholderComponent },
    { path: 'users', canActivate: [roleGuard], data: { roles: ['Administrator'] }, loadComponent: () => import('./users-component/users-component').then(m => m.UsersComponent) },
    { path: 'users/details/:id', canActivate: [roleGuard], data: { roles: ['Administrator'] }, loadComponent: () => import('./users-component/user-details.component').then(m => m.UserDetailsComponent) },
    { path: 'users/create', canActivate: [roleGuard], data: { roles: ['Administrator'] }, loadComponent: () => import('./users-component/create-user.component').then(m => m.CreateUserComponent) },
    { path: 'users/edit/:id', canActivate: [roleGuard], data: { roles: ['Administrator'] }, loadComponent: () => import('./users-component/edit-user.component').then(m => m.EditUserComponent) },
    { path: 'tickets', canActivate: [roleGuard], data: { roles: ['Administrator'] }, component: PlaceholderComponent },
    { path: 'tickets/purchase', canActivate: [roleGuard], data: { roles: ['Visitor'] }, loadComponent: () => import('./tickets/purchase-ticket.component').then(m => m.PurchaseTicketComponent) },
    { path: 'sessions', canActivate: [roleGuard], data: { roles: ['Administrator'] }, component: PlaceholderComponent },
    { path: 'special-events', loadComponent: () => import('./special-events-component/special-events-component').then(m => m.SpecialEventsComponent) },
    { path: 'special-events/details/:id', loadComponent: () => import('./special-events-component/special-event-details.component').then(m => m.SpecialEventDetailsComponent) },
    { path: 'special-events/create', canActivate: [roleGuard], data: { roles: ['Administrator'] }, loadComponent: () => import('./special-events-component/create-special-event.component').then(m => m.CreateSpecialEventComponent) },
    { path: 'rewards/manage', canActivate: [roleGuard], data: { roles: ['Administrator'] }, loadComponent: () => import('./rewards/reward-management.component').then(m => m.RewardManagementComponent) },
    { path: 'rewards/redeem', canActivate: [roleGuard], data: { roles: ['Visitor'] }, loadComponent: () => import('./rewards/reward-redeem.component').then(m => m.RewardRedeemComponent) },
    { path: 'rewards/history', canActivate: [roleGuard], data: { roles: ['Visitor', 'Administrator'] }, loadComponent: () => import('./rewards/reward-redemption-history.component').then(m => m.RewardRedemptionHistoryComponent) },
    { path: 'maintenance', canActivate: [roleGuard], data: { roles: ['Administrator'] }, loadComponent: () => import('./maintenance/maintenance.component').then(m => m.MaintenanceComponent) },
    { path: 'reports/attraction-usage', canActivate: [roleGuard], data: { roles: ['Administrator'] }, loadComponent: () => import('./reports/attraction-usage.component').then(m => m.AttractionUsageReportComponent) },
    { path: 'score-logs', canActivate: [roleGuard], data: { roles: ['Administrator'] }, component: PlaceholderComponent },
    { path: 'scoring', canActivate: [roleGuard], data: { roles: ['Administrator'] }, loadComponent: () => import('./scoring/scoring-dashboard.component').then(m => m.ScoringDashboardComponent) },
    { path: 'visitor-profile', canActivate: [roleGuard], data: { roles: ['Visitor', 'Administrator'] }, loadComponent: () => import('./visitor/profile/visitor-profile.component').then(m => m.VisitorProfileComponent) },
    { path: 'clock', loadComponent: () => import('./clock-component/clock.component').then(m => m.ClockComponent) },
    { path: 'access', canActivate: [roleGuard], data: { roles: ['Operator'] }, loadComponent: () => import('./attraction-access/register-access.component').then(m => m.RegisterAccessComponent) },
    { path: 'access/capacity/:id', canActivate: [roleGuard], data: { roles: ['Operator'] }, loadComponent: () => import('./attraction-access/capacity.component').then(m => m.CapacityComponent) },
    { path: 'access/incidents/:id', canActivate: [roleGuard], data: { roles: ['Operator'] }, loadComponent: () => import('./attraction-access/incidents.component').then(m => m.IncidentsComponent) },
    { path: 'access/exit', canActivate: [roleGuard], data: { roles: ['Operator'] }, loadComponent: () => import('./attraction-access/exit-access.component').then(m => m.ExitAccessComponent) },
  ] },
];
