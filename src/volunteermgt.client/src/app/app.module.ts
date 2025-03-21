import { HttpClientModule } from '@angular/common/http';
import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { LoginComponent } from './components/login/login.component';
import { DashboardComponent } from './components/dashboard/dashboard.component';
import { SidebarComponent } from './components/sidebar/sidebar.component';
import { RegisterComponent } from './components/register/register.component';
import { HeaderComponent } from './components/header/header.component';
import { VolunteertableComponent } from './components/Volunteer/volunteertable/volunteertable.component';
import { CreateVolunteerComponent } from './components/Volunteer/volunteerCRUD/create-volunteer/create-volunteer.component';
import { EditVolunteerComponent } from './components/Volunteer/volunteerCRUD/edit-volunteer/edit-volunteer.component';
import { CommonModule, DatePipe } from '@angular/common';
import { DeleteconfirmationComponent } from './Dialogbox/deleteconfirmation/deleteconfirmation.component';
import { MatDialogModule } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatAutocompleteModule } from '@angular/material/autocomplete';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { VolunteerservicetableComponent } from './components/Volunteer/volunteerservicetable/volunteerservicetable.component';
import { ServicetableComponent } from './components/Volunteer/servicetable/servicetable.component';
import { EditvolunteerdialogComponent } from './Dialogbox/editvolunteerdialog/editvolunteerdialog.component';
import { AssignserviceComponent } from './components/Volunteer/assignservice/assignservice.component';
import { MatSnackBarModule } from '@angular/material/snack-bar';
import { AddcoupondialogComponent } from './Dialogbox/addcoupondialog/addcoupondialog.component';
import { CoupondialogComponent } from './Dialogbox/coupondialog/coupondialog.component';
import { CouponComponent } from './components/Volunteer/coupon/coupon.component';

@NgModule({
  declarations: [
    AppComponent,
    LoginComponent,
    DashboardComponent,
    SidebarComponent,
    RegisterComponent,
    HeaderComponent,
    VolunteertableComponent,
    CreateVolunteerComponent,
    EditVolunteerComponent,
    DeleteconfirmationComponent,
    VolunteerservicetableComponent,
    ServicetableComponent,
    EditvolunteerdialogComponent,
    AssignserviceComponent,
    AddcoupondialogComponent,
    CoupondialogComponent,
    CouponComponent,
  ],
  imports: [
    BrowserModule,
    HttpClientModule,
    AppRoutingModule,
    FormsModule,
    ReactiveFormsModule,
    CommonModule,
    MatDialogModule,
    MatFormFieldModule,
    MatInputModule,
    MatAutocompleteModule,
    BrowserAnimationsModule,
    MatSnackBarModule
  ],
  providers: [DatePipe],
  bootstrap: [AppComponent]
})
export class AppModule { }
