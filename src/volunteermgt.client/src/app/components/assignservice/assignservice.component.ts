import { Component, HostListener, ViewEncapsulation } from '@angular/core';
import { Volunteer } from '../../Models/volunteer.model';
import { VolunteerService } from '../../services/volunteer.service';
import { Service } from '../../Models/voluteerService.model';
import { AssignedVolunteer } from '../../Models/assignVolunteer.model';
import { MatSnackBar, MatSnackBarRef } from '@angular/material/snack-bar';

@Component({
  selector: 'app-assignservice',
  standalone: false,
  templateUrl: './assignservice.component.html',
  styleUrl: './assignservice.component.css',
  encapsulation: ViewEncapsulation.None,
})
export class AssignserviceComponent {
  @HostListener('document:click', ['$event'])
  onClickOutside(event: Event): void {
    const targetElement = event.target as HTMLElement;

    if (!targetElement.closest('.volunteer-container')) {
      this.searchSuggestions = [];
    }

    if (!targetElement.closest('.service-container')) {
      this.serviceSuggestions = [];
    }
  }
  timeSlot: string = '';
  serviceVolunteerCounts: AssignedVolunteer[] = [];
  volunteers: Volunteer[] = [];
  searchQuery: string = '';
  searchSuggestions: Volunteer[] = [];
  selectedVolunteer: Volunteer | null = null;
  assignedServices: number[] = []; 

  services: Service[] = [];
  serviceQuery: string = '';
  serviceSuggestions: Service[] = [];
  selectedService: Service | null = null;

  constructor(private volunteerService: VolunteerService, private snackBar: MatSnackBar) {
    this.volunteerService.getVolunteers().subscribe((data) => {
      this.volunteers = data;
    });

    this.setCurrentTime();
    this.fetchServiceVolunteerCounts();
    this.volunteerService.getAllServices().subscribe((data) => {
      this.services = data;
    });
  }

  fetchServiceVolunteerCounts() {
    this.volunteerService.getServiceVolunteerCounts().subscribe((data) => {
      this.serviceVolunteerCounts = data;
    });
  }

  searchVolunteers(): void {
    if (this.searchQuery.trim() === '') {
      this.searchSuggestions = [];
      return;
    }

    this.searchSuggestions = this.volunteers.filter(volunteer =>
      volunteer.name.toLowerCase().includes(this.searchQuery.toLowerCase()) ||
      volunteer.code.toLowerCase().includes(this.searchQuery.toLowerCase())
    );
  }

  selectSuggestion(volunteer: Volunteer): void {
    this.searchQuery = volunteer.name;
    this.searchSuggestions = [];
    this.selectedVolunteer = volunteer;

    this.volunteerService.getServiceVolunteerById(volunteer.id).subscribe((assignedServices) => {
      this.assignedServices = assignedServices.map((service: { serviceId: any; }) => service.serviceId);
    });
  }

  searchServices(): void {
    if (this.serviceQuery.trim() === '') {
      this.serviceSuggestions = [];
      return;
    }

    this.serviceSuggestions = this.services.filter(service =>
      service.serviceName.toLowerCase().includes(this.serviceQuery.toLowerCase()) &&
      !this.assignedServices.includes(service.id)
    );
  }

  selectService(service: Service): void {
    if (this.assignedServices.includes(service.id)) {
      this.showSnackbar("This service is already assigned to the selected volunteer.", "error");
      return;
    }

    this.serviceQuery = service.serviceName;
    this.serviceSuggestions = [];
    this.selectedService = service;
  }

  private setCurrentTime(): void {
    const now = new Date();
    const hours = now.getHours().toString().padStart(2, '0');
    const minutes = now.getMinutes().toString().padStart(2, '0');
    this.timeSlot = `${hours}:${minutes}`;
  }

  onAssign() {
    if (!this.selectedVolunteer || !this.selectedService || !this.timeSlot) {
      this.showSnackbar("Please fill all fields before assigning.", "error");
      return;
    }

    if (this.assignedServices.includes(this.selectedService.id)) {
      this.showSnackbar("This service is already assigned to the selected volunteer.", "error");
      return;
    }

    const [hours, minutes] = this.timeSlot.split(":").map(Number);
    const amPm = hours >= 12 ? "PM" : "AM";
    const formattedHours = hours % 12 || 12;
    const formattedTimeSlot = `${formattedHours}:${minutes.toString().padStart(2, "0")} ${amPm}`;

    const assignedData = {
      volunteerId: this.selectedVolunteer.id,
      serviceId: this.selectedService.id,
      timeSlot: formattedTimeSlot
    };

    this.volunteerService.assignService(assignedData).subscribe({
      next: (response) => {
        console.log("Assignment successful:", response);
        this.showSnackbar("Service assigned successfully!", "success");
        this.assignedServices.push(this.selectedService!.id); 
      },
      error: (error) => {
        console.error("Error assigning service:", error);
        this.showSnackbar("Failed to assign service. Please try again.", "error");
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

  getTotal(field: keyof AssignedVolunteer): number {
    return this.serviceVolunteerCounts.reduce((total, service) => total + Number(service[field]), 0);
  }
}
