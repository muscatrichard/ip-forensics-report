import { Routes } from '@angular/router';
import { authGuard } from './core/guards/auth.guard';
import { guestGuard } from './core/guards/guest.guard';

export const routes: Routes = [
  { path: '', pathMatch: 'full', redirectTo: 'reports' },

  {
    path: '',
    loadComponent: () =>
      import('./features/auth/auth-layout/auth-layout').then((m) => m.AuthLayout),
    children: [
      {
        path: 'login',
        canActivate: [guestGuard],
        title: 'Sign in | IP Forensics',
        loadComponent: () => import('./features/auth/login/login').then((m) => m.Login),
      },
      {
        path: 'register',
        canActivate: [guestGuard],
        title: 'Create an account | IP Forensics',
        loadComponent: () =>
          import('./features/auth/registration/registration').then((m) => m.Registration),
      },
    ],
  },

  {
    path: '',
    canActivate: [authGuard],
    loadComponent: () => import('./features/private/layout/layout').then((m) => m.Layout),
    children: [
      {
        path: 'reports',
        title: 'Reports | IP Forensics',
        loadComponent: () => import('./features/private/reports/reports').then((m) => m.Reports),
      },
      {
        path: 'reports/:id',
        title: 'Report | IP Forensics',
        loadComponent: () =>
          import('./features/private/report-detail/report-detail').then((m) => m.ReportDetail),
      },
      {
        path: 'generate',
        title: 'Generate report | IP Forensics',
        loadComponent: () =>
          import('./features/private/report-generation/report-generation').then(
            (m) => m.ReportGeneration,
          ),
      },
    ],
  },

  { path: '**', redirectTo: 'reports' },
];
