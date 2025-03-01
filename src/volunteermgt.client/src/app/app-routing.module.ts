import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { LoginComponent } from './components/login/login.component';
import { DashboardComponent } from './components/dashboard/dashboard.component';
import { AuthGuard } from './services/auth.gurad';
import { VolunteertableComponent } from './components/volunteertable/volunteertable.component';
import { CreateVolunteerComponent } from './components/volunteerCRUD/create-volunteer/create-volunteer.component';
import { EditVolunteerComponent } from './components/volunteerCRUD/edit-volunteer/edit-volunteer.component';

const routes: Routes = [
  { path: '', redirectTo: '/login', pathMatch: 'full' },
  { path: 'login', component: LoginComponent, canActivate: [AuthGuard] },
  { path: 'dashboard', component: DashboardComponent },
  { path: 'volunteer', component: VolunteertableComponent },
  { path: 'createVolunteer', component: CreateVolunteerComponent },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
