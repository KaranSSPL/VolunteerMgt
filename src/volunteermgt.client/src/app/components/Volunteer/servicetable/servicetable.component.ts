
import { Component } from '@angular/core';
import { VolunteerService } from '../../../services/volunteer.service';
import { Service } from '../../../Models/voluteerService.model';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatDialog } from '@angular/material/dialog';
import { DeleteconfirmationComponent } from '../../../Dialogbox/deleteconfirmation/deleteconfirmation.component';
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

  constructor(private volunteerService: VolunteerService, private fb: FormBuilder, private dialog: MatDialog) {
    this.serviceForm = this.fb.group({
      serviceName: ['', Validators.required],
      requiredVolunteer: ['', Validators.required],
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
        console.error('Error fetching services:', err);
        this.isLoading = false;
      }
    });
  }

  openModal(service?: Service): void {
    this.showModal = true;
    if (service) {
      this.editMode = true;
      this.selectedServiceId = service.id;
      this.serviceForm.patchValue({ serviceName: service.serviceName, requiredVolunteer: service.requiredVolunteer });
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
    const newService: Service = {
      id: 0,
      serviceName: this.serviceForm.value.serviceName,
      requiredVolunteer: this.serviceForm.value.requiredVolunteer
    };
    this.volunteerService.addService(newService).subscribe({
      next: (service) => {
        this.services.push(service);
        this.closeModal();
        this.serviceForm.reset();
      },
      error: (err) => console.error('Error adding service:', err)
    });
  }

  updateService(): void {
    if (this.serviceForm.invalid || this.selectedServiceId === null) return;
    const updatedService: Service = {
      id: this.selectedServiceId,
      serviceName: this.serviceForm.value.serviceName,
      requiredVolunteer: this.serviceForm.value.requiredVolunteer
    };
    this.volunteerService.updateService(this.selectedServiceId, updatedService).subscribe({
      next: () => {
        const index = this.services.findIndex(s => s.id === this.selectedServiceId);
        if (index !== -1) {
          this.services[index].serviceName = updatedService.serviceName;
        }
        this.closeModal();
      },
      error: (err) => console.error('Error updating service:', err)
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
          },
          error: (err) => console.error('Error deleting service:', err)
        });
      }
    });
  }
}
