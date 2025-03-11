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

  constructor(private volunteerService: VolunteerService, public dialog: MatDialog) { }

  ngOnInit(): void {
    this.fetchVolunteerServiceMappings();
  }

  fetchVolunteerServiceMappings(): void {
    this.volunteerService.getVolunteerServiceMappings().subscribe({
      next: (data) => {
        const groupedData = Object.values(
          data.reduce((acc: Record<number, any>, curr: any) => {
            const { id, volunteerId, volunteerName, serviceName, timeSlot, createdDate } = curr;

            if (!acc[volunteerId]) {
              acc[volunteerId] = {id,volunteerId,volunteerName,services: [],timeSlots: [],createdDate};
            }

            if (serviceName && !acc[volunteerId].services.includes(serviceName)) {
              acc[volunteerId].services.push(serviceName);
            }
            if (timeSlot && !acc[volunteerId].timeSlots.includes(timeSlot)) {
              acc[volunteerId].timeSlots.push(timeSlot);
            }

            return acc;
          }, {})
        );

        this.volunteerServiceMappings = groupedData;
      },
      error: (err) => console.error('Error fetching volunteer service mappings:', err)
    });
  }

  openEditDialog(id: number): void {
    const dialogRef = this.dialog.open(EditvolunteerdialogComponent, {
      width: '500px',
      data: { id } 
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.fetchVolunteerServiceMappings();
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
        this.fetchVolunteerServiceMappings();
      },
      error: (err) => console.error('Error deleting volunteer:', err)
    });
  }
}
