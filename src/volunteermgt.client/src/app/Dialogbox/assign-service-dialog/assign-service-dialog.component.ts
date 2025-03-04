import { HttpClient } from '@angular/common/http';
import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { VolunteerService } from '../../services/volunteer.service';
import { MatDialogRef } from '@angular/material/dialog';

@Component({
  selector: 'app-assign-service-dialog',
  standalone: false,
  templateUrl: './assign-service-dialog.component.html',
  styleUrl: './assign-service-dialog.component.css'
})
export class AssignServiceDialogComponent {
  assignForm: FormGroup;
  services: any[] = [];
  volunteers: any[] = [];
  selectedVolunteers: any[] = [];
  selectedService: any | null = null;
  isVolunteerDropdownOpen = false;
  isServiceDropdownOpen = false;

  constructor(
    private fb: FormBuilder,
    private volunteerService: VolunteerService,
    private dialogRef: MatDialogRef<AssignServiceDialogComponent>
  ) {
    this.assignForm = this.fb.group({
      timeSlot: ['', [Validators.required, Validators.pattern(/^\d{1,2}(:\d{2})?(am|pm) to \d{1,2}(:\d{2})?(am|pm)$/)]],
      date: [new Date().toISOString().split('T')[0]]
    });

    this.loadDropdownData();
  }

  loadDropdownData(): void {
    this.volunteerService.getVolunteers().subscribe(data => this.volunteers = data);
    this.volunteerService.getAllServices().subscribe(data => this.services = data);
  }

  selectVolunteer(volunteer: any): void {
    if (!this.selectedVolunteers.some(v => v.id === volunteer.id)) {
      this.selectedVolunteers.push(volunteer);
    }
  }

  removeVolunteer(volunteerId: number): void {
    this.selectedVolunteers = this.selectedVolunteers.filter(v => v.id !== volunteerId);
  }

  selectService(service: any): void {
    this.selectedService = service;
    this.isServiceDropdownOpen = false;
  }

  assignService(): void {
    if (this.assignForm.valid && this.selectedVolunteers.length > 0 && this.selectedService) {
      const timeSlot = this.assignForm.value.timeSlot;
      const createdDate = new Date().toISOString();

      const payload = this.selectedVolunteers.map(volunteer => ({
        volunteerId: volunteer.id,
        serviceId: this.selectedService.id,
        timeSlot,
        createdDate
      }));

      this.volunteerService.assignService(payload).subscribe(
        () => {
          alert('Service Assigned Successfully');
          this.assignForm.reset();
          this.selectedVolunteers = [];
          this.selectedService = null;
          this.dialogRef.close();
        },
        () => {
          alert('Failed to assign service');
        }
      );
    } else {
      alert('Please select at least one volunteer and one service.');
    }
  }


  closeDialog(): void {
    this.dialogRef.close();
  }

  toggleDropdown(type: string): void {
    if (type === 'volunteer') {
      this.isVolunteerDropdownOpen = !this.isVolunteerDropdownOpen;
    } else {
      this.isServiceDropdownOpen = !this.isServiceDropdownOpen;
    }
  }

  isVolunteerSelected(volunteerId: number): boolean {
    return this.selectedVolunteers.some(v => v.id === volunteerId);
  }
}
