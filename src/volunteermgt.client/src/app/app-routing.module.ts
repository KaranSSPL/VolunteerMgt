import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { LoginComponent } from './components/login/login.component';
import { DashboardComponent } from './components/dashboard/dashboard.component';
import { VolunteertableComponent } from './components/Volunteer/volunteertable/volunteertable.component';
import { CreateVolunteerComponent } from './components/Volunteer/volunteerCRUD/create-volunteer/create-volunteer.component';
import { AuthGuard } from './services/auth.gurad';
import { LoginGuard } from './services/login.guard';
import { VolunteerservicetableComponent } from './components/Volunteer/volunteerservicetable/volunteerservicetable.component';
import { ServicetableComponent } from './components/Volunteer/servicetable/servicetable.component';
import { AssignserviceComponent } from './components/assignservice/assignservice.component';

const routes: Routes = [
  { path: '', redirectTo: '/login', pathMatch: 'full' },
  { path: 'login', component: LoginComponent, canActivate: [LoginGuard] },
  { path: 'assignService', component: AssignserviceComponent, canActivate: [AuthGuard] },
  { path: 'dashboard', component: DashboardComponent, canActivate: [AuthGuard] },
  { path: 'volunteer', component: VolunteertableComponent, canActivate: [AuthGuard] },
  { path: 'createVolunteer', component: CreateVolunteerComponent, canActivate: [AuthGuard] },
  { path: 'volunteerService', component: VolunteerservicetableComponent, canActivate: [AuthGuard] },
  { path: 'service', component: ServicetableComponent, canActivate: [AuthGuard] },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
