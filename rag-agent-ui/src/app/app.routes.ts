import { Routes } from '@angular/router';
import { HomeComponent } from './features/home/home.component';
import { JobComponent } from './features/job/job.component';

export const routes: Routes = [
  { path: '', component: HomeComponent },
  { path: 'documents', component: JobComponent },
  { path: '**', redirectTo: '' }
];
