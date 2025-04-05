 import { Component, ViewEncapsulation } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { VolunteerService } from '../../../services/volunteer.service';
import { DeleteconfirmationComponent } from '../../../Dialogbox/deleteconfirmation/deleteconfirmation.component';
import { formatDate } from '@angular/common';
import { Router } from '@angular/router';

@Component({
  selector: 'app-volunteerservicetable',
  standalone: false,
  templateUrl: './volunteerservicetable.component.html',
  styleUrl: './volunteerservicetable.component.css',
  encapsulation: ViewEncapsulation.None,

})
export class VolunteerservicetableComponent {
  volunteerServiceMappings: any[] = []; 
  filteredVolunteerServiceMappings: any[] = [];

  constructor(private volunteerService: VolunteerService, public dialog: MatDialog, private router: Router) { }

  ngOnInit(): void {
    this.volunteerService.getVolunteerServiceMappings().subscribe(
      (data) => {
        this.volunteerServiceMappings = data;  
        this.filterDataByCurrentDate(); 
      },
      (error) => {
        console.error('Error fetching volunteer service mappings', error);
      }
    );
  }

  filterDataByCurrentDate() {
    const currentDate = formatDate(new Date(), 'yyyy-MM-dd', 'en-US');
    this.filteredVolunteerServiceMappings = this.volunteerServiceMappings.filter(mapping => {
      const mappingDate = formatDate(new Date(mapping.timeSlot), 'yyyy-MM-dd', 'en-US');
      return mappingDate === currentDate;  
    });
  }

  exitVolunteer(volunteerId: number): void {
    this.volunteerService.getServiceVolunteerById(volunteerId).subscribe({
      next: (services) => {
        const currentTime = this.getFormattedTime();
        const servicesToUpdate = services.filter((service: any) => !service.exitTime);
        if (servicesToUpdate.length > 0) {
          const service = servicesToUpdate[0];
          const requestData = {
            volunteerId: volunteerId,
            serviceId: service.serviceId,
            timeSlot: service.timeSlot,
            exitTime: currentTime
          };
          this.volunteerService.assignService(requestData).subscribe({
            next: () => {
              console.log(`Exit time updated for Volunteer ${volunteerId}, Service ${service.serviceId}`);
              this.getVolunteerServiceMappings();
            },
            error: (err) => console.error('Error updating exit time:', err)
          });
        } else {
          console.log(`All services for Volunteer ${volunteerId} already have exit times.`);
        }
      },
      error: (err) => console.error('Error fetching services for volunteer:', err)
    });
  }

  editVolunteerService(id: number) {
    this.router.navigate(['/volunteer-service/edit', id]);
  }

  getFormattedTime(): string {
    const now = new Date();
    let hours = now.getHours();
    const minutes = now.getMinutes();
    const ampm = hours >= 12 ? 'PM' : 'AM';
    hours = hours % 12 || 12;
    return `${hours}:${minutes.toString().padStart(2, '0')} ${ampm}`;
  }

  getVolunteerServiceMappings() {
    const currentDate = formatDate(new Date(), 'yyyy-MM-dd', 'en-US');
    this.volunteerService.getVolunteerServiceMappings().subscribe(data => {
      this.filteredVolunteerServiceMappings = data.filter(mapping => {
        const mappingDate = formatDate(new Date(mapping.timeSlot), 'yyyy-MM-dd', 'en-US');
        return mappingDate === currentDate;
      });
    });
  }

  openDeleteDialog(volunteerId: number, serviceId: number): void {
    const dialogRef = this.dialog.open(DeleteconfirmationComponent, {
      width: '350px',
      data: { message: 'Are you sure you want to delete this volunteer and all assigned services?' }
    });
    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.deleteVolunteer(volunteerId, serviceId);
      }
    });
  }

  deleteVolunteer(volunteerId: number, serviceId: number): void {
    this.volunteerService.deleteVolunteerService(volunteerId, serviceId).subscribe({
      next: () => {
        this.getVolunteerServiceMappings();
      },
      error: (err) => console.error('Error deleting volunteer:', err)
    });
  }
}
