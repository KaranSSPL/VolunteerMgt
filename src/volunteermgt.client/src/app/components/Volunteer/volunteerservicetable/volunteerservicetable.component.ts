 import { Component } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { VolunteerService } from '../../../services/volunteer.service';
import { EditvolunteerdialogComponent } from '../../../Dialogbox/editvolunteerdialog/editvolunteerdialog.component';
import { DeleteconfirmationComponent } from '../../../Dialogbox/deleteconfirmation/deleteconfirmation.component';

@Component({
  selector: 'app-volunteerservicetable',
  standalone: false,
  templateUrl: './volunteerservicetable.component.html',
  styleUrl: './volunteerservicetable.component.css'
})
export class VolunteerservicetableComponent {
  volunteerServiceMappings: any[] = [];
  defaultTime: string = '10:00 PM';
  searchQuery: string = '';
  filteredMappings: any[] = [];

  constructor(private volunteerService: VolunteerService, public dialog: MatDialog) { }

  ngOnInit(): void {
    this.loadDefaultTime();
    this.setDefaultTimeForInput();
    this.getVolunteerServiceMappings();
  }

  loadDefaultTime(): void {
    const savedTime = localStorage.getItem('defaultExitTime');
    this.defaultTime = savedTime ? savedTime : '10:00 PM';
  }

  setDefaultTimeForInput(): void {
    const [hours, minutes, ampm] = this.defaultTime.split(/[: ]/);
    const hour24 = ampm === 'PM' ? (parseInt(hours) % 12) + 12 : parseInt(hours) % 12;
    this.defaultTime = `${hour24.toString().padStart(2, '0')}:${minutes}`;
  }

  convertTo12HourFormat(time: string): string {
    const [hour, minute] = time.split(':').map(Number);
    const ampm = hour >= 12 ? 'PM' : 'AM';
    const hour12 = hour % 12 || 12;
    return `${hour12}:${minute.toString().padStart(2, '0')} ${ampm}`;
  }

  getVolunteerServiceMappings() {
    this.volunteerService.getVolunteerServiceMappings().subscribe(data => {
      this.volunteerServiceMappings = data;
      this.filteredMappings = [...data]; 
      this.updateExitTimeForPastSlots();
    });
  }

  filterTable(): void {
    const query = this.searchQuery.toLowerCase();
    this.filteredMappings = this.volunteerServiceMappings.filter(mapping =>
      mapping.volunteerName?.toLowerCase().includes(query) || 
      (mapping.serviceName && mapping.serviceName.toLowerCase().includes(query)) || 
      (mapping.timeSlot && new Date(mapping.timeSlot).toLocaleDateString().includes(query))
    );
  }

  updateExitTimeForPastSlots(): void {
    const today = new Date().toISOString().split('T')[0];
    this.volunteerServiceMappings.forEach((mapping) => {
      if (!mapping.timeSlots || !Array.isArray(mapping.timeSlots)) return;  
      mapping.timeSlots.forEach((timeSlot: string, index: number) => {
        if (!timeSlot) return; 
        const slotDate = new Date(timeSlot).toISOString().split('T')[0];
        if (slotDate !== today && (!mapping.exitTime || !mapping.exitTime[index])) {
          const requestData = {
            volunteerId: mapping.volunteerId,
            serviceId: mapping.serviceId,
            timeSlot: timeSlot,
            exitTime: this.convertTo12HourFormat(this.defaultTime)
          };
          this.volunteerService.assignService(requestData).subscribe({
            next: () => {
              this.deleteService(mapping.volunteerId, mapping.serviceId);
              console.log(`Exit time set for volunteer ${mapping.volunteerId} at ${this.defaultTime}`)
            },
            error: (err) => console.error('Error updating exit time:', err)
          });
        }
      });
    });
  }

  deleteService(volunteerId: number, serviceId: number): void {
    this.volunteerService.deleteVolunteerService(volunteerId, serviceId).subscribe({
      next: () => {
      },
      error: (err) => {
        console.error('Error deleting service:', err);
      }
    });
  }

  onDefaultTimeChange(event: any): void {
    const [hours, minutes] = event.target.value.split(':').map(Number);
    const ampm = hours >= 12 ? 'PM' : 'AM';
    const hour12 = hours % 12 || 12;
    this.defaultTime = `${hour12}:${minutes.toString().padStart(2, '0')} ${ampm}`;
    localStorage.setItem('defaultExitTime', this.defaultTime); 
    window.location.reload();
    console.log(`Default exit time updated to: ${this.defaultTime}`);
  }

  openEditDialog(id: number): void {
    const dialogRef = this.dialog.open(EditvolunteerdialogComponent, {
      width: '500px',
      data: { id } 
    });
    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.getVolunteerServiceMappings();
      }
    });
  }

  openDeleteDialog(volunteerId: number): void {
    const dialogRef = this.dialog.open(DeleteconfirmationComponent, {
      width: '350px',
      data: { message: 'Are you sure you want to delete this volunteer and all assigned services?'}
    });
    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.deleteVolunteer(volunteerId);
      }
    });
  }

  deleteVolunteer(volunteerId: number): void {
    this.volunteerService.deleteVolunteerandService(volunteerId).subscribe({
      next: () => {
        this.getVolunteerServiceMappings();
      },
      error: (err) => console.error('Error deleting volunteer:', err)
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

  getFormattedTime(): string {
    const now = new Date();
    let hours = now.getHours();
    const minutes = now.getMinutes();
    const ampm = hours >= 12 ? 'PM' : 'AM';
    hours = hours % 12 || 12;
    return `${hours}:${minutes.toString().padStart(2, '0')} ${ampm}`;
  }
}
