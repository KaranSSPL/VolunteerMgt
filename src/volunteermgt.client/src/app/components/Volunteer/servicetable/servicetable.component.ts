
import { Component } from '@angular/core';
import { VolunteerService } from '../../../services/volunteer.service';
import { Service } from '../../../Models/voluteerService.model';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatDialog } from '@angular/material/dialog';
import { DeleteconfirmationComponent } from '../../../Dialogbox/deleteconfirmation/deleteconfirmation.component';
import { MatSnackBar, MatSnackBarRef } from '@angular/material/snack-bar';
@Component({
  selector: 'app-servicetable',
  standalone: false,
  templateUrl: './servicetable.component.html',
  styleUrl: './servicetable.component.css'
})
export class ServicetableComponent {

  services: Service[] = [];
  isLoading: boolean = true;
  showModal: boolean = false;
  serviceForm: FormGroup;
  editMode: boolean = false;
  selectedServiceId: number | null = null;

  constructor(private volunteerService: VolunteerService, private fb: FormBuilder, private dialog: MatDialog, private snackBar: MatSnackBar) {
    this.serviceForm = this.fb.group({
      serviceName: ['', Validators.required],
      saturdayVolunteerRequirement: ['', Validators.required],
      sundayVolunteerRequirement: ['', Validators.required],
      ekadashiVolunteerRequirement: ['', Validators.required],
      festivalVolunteerRequirement: ['', Validators.required],
      code: ['', Validators.required],
      defaultTime: ['', Validators.required],
    });
  }

  ngOnInit(): void {
    this.fetchServices();
  }

  fetchServices(): void {
    this.volunteerService.getAllServices().subscribe({
      next: (data) => {
        this.services = data;
        this.isLoading = false;
      },
      error: (err) => {
        this.showSnackbar("Error Fetching Services", "error");
        this.isLoading = false;
      }
    });
  }

  openModal(service?: Service): void {
    this.showModal = true;
    if (service) {
      this.editMode = true;
      this.selectedServiceId = service.id;
      const localDateTime = new Date(service.defaultTime);
      const formattedTime = localDateTime.getHours().toString().padStart(2, '0') + ':' +
        localDateTime.getMinutes().toString().padStart(2, '0');

      this.serviceForm.patchValue({
        serviceName: service.serviceName,
        saturdayVolunteerRequirement: service.saturdayVolunteerRequirement,
        sundayVolunteerRequirement: service.sundayVolunteerRequirement,
        ekadashiVolunteerRequirement: service.ekadashiVolunteerRequirement,
        festivalVolunteerRequirement: service.festivalVolunteerRequirement,
        code: service.code,
        defaultTime: formattedTime
      });
    } else {
      this.editMode = false;
      this.selectedServiceId = null;
      this.serviceForm.reset();
    }
  }

  closeModal(): void {
    this.showModal = false;
    this.editMode = false;
    this.selectedServiceId = null;
    this.serviceForm.reset();
  }

  addService(): void {
    if (this.serviceForm.invalid) return;
    const today = new Date();
    const dateString = today.getFullYear() + '-' +
      String(today.getMonth() + 1).padStart(2, '0') + '-' +
      String(today.getDate()).padStart(2, '0');
    const timeString = this.serviceForm.value.defaultTime + ':00';
    const newService: Service = {
      id: 0,
      serviceName: this.serviceForm.value.serviceName,
      saturdayVolunteerRequirement: this.serviceForm.value.saturdayVolunteerRequirement,
      sundayVolunteerRequirement: this.serviceForm.value.sundayVolunteerRequirement,
      ekadashiVolunteerRequirement: this.serviceForm.value.ekadashiVolunteerRequirement,
      festivalVolunteerRequirement: this.serviceForm.value.festivalVolunteerRequirement,
      code: this.serviceForm.value.code,
      defaultTime: `${dateString}T${timeString}`
    };
    this.volunteerService.addService(newService).subscribe({
      next: (service) => {
        this.services.push(service);
        this.closeModal();
        this.showSnackbar("Service Added Successfully", "success");
        this.serviceForm.reset();
        this.fetchServices();
      },
      error: (err) => this.showSnackbar("Error Adding Service", "error")
    });
  }

  updateService(): void {
    if (this.serviceForm.invalid || this.selectedServiceId === null) return;
    const today = new Date();
    const dateString = today.getFullYear() + '-' +
      String(today.getMonth() + 1).padStart(2, '0') + '-' +
      String(today.getDate()).padStart(2, '0');
    const timeString = this.serviceForm.value.defaultTime + ':00';
    const updatedService: Service = {
      id: this.selectedServiceId,
      serviceName: this.serviceForm.value.serviceName,
      saturdayVolunteerRequirement: this.serviceForm.value.saturdayVolunteerRequirement,
      sundayVolunteerRequirement: this.serviceForm.value.sundayVolunteerRequirement,
      ekadashiVolunteerRequirement: this.serviceForm.value.ekadashiVolunteerRequirement,
      festivalVolunteerRequirement: this.serviceForm.value.festivalVolunteerRequirement,
      code: this.serviceForm.value.code,
      defaultTime: `${dateString}T${timeString}`
    };
    this.volunteerService.updateService(this.selectedServiceId, updatedService).subscribe({
      next: () => {
        this.fetchServices();
        this.closeModal();
        this.showSnackbar("Service Updated Successfully", "success");
      },
      error: (err) => this.showSnackbar("Error Updating Service", "error")
    });
  }

  deleteService(serviceId: number): void {
    const message = `Are you sure you want to delete this service?`;
    const dialogRef = this.dialog.open(DeleteconfirmationComponent, {
      width: '350px',
      data: { id: serviceId, message }
    });
    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.volunteerService.deleteService(serviceId).subscribe({
          next: () => {
            this.services = this.services.filter(service => service.id !== serviceId);
            this.showSnackbar("Service Deleted Successfully", "success");
          },
          error: (err) => this.showSnackbar("Error Deleting Service", "error")
        });
      }
    });
  }

  showSnackbar(message: string, type: "success" | "error") {
    const snackbarRef: MatSnackBarRef<any> = this.snackBar.open(message, "close", {
      duration: 3000,
      verticalPosition: "top",
      horizontalPosition: "center",
    });

    snackbarRef.afterOpened().subscribe(() => {
      const snackbarElement = document.querySelector('.mat-mdc-snack-bar-container');
      if (snackbarElement) {
        snackbarElement.classList.add(type === "success" ? "snackbar-success" : "snackbar-error");
      }
    });
  }
}
