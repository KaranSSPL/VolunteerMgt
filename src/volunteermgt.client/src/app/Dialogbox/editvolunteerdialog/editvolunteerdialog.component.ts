import { Component, Inject } from '@angular/core';
import { VolunteerService } from '../../services/volunteer.service';
import { MAT_DIALOG_DATA, MatDialog, MatDialogRef } from '@angular/material/dialog';
import { DeleteconfirmationComponent } from '../deleteconfirmation/deleteconfirmation.component';

@Component({
  selector: 'app-editvolunteerdialog',
  standalone: false,
  templateUrl: './editvolunteerdialog.component.html',
  styleUrl: './editvolunteerdialog.component.css'
})
export class EditvolunteerdialogComponent {
  volunteerData: any = null;
  selectedService: any = null;
  timeSlot: string = '';

  constructor(
    private volunteerService: VolunteerService,
    private dialog: MatDialog,
    public dialogRef: MatDialogRef<EditvolunteerdialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: { id: number }
  ) {
  }

  ngOnInit(): void {
    this.fetchVolunteerDetails();
  }

  fetchVolunteerDetails(): void {
    this.volunteerService.getServiceVolunteerById(this.data.id).subscribe({
      next: (response) => {
        this.volunteerData = response;
      },
      error: (err) => {
        console.error('Error fetching volunteer data:', err);
      }
    });
  }

  closeDialog(): void {
    this.dialogRef.close();
    this.resetDropdown();
  }

  resetDropdown(): void {
    this.selectedService = null; 
    this.timeSlot = '';
  }

  openDeleteDialog(volunteerId: number, serviceId: number, index: number): void {
    const message = `Are you sure you want to delete this volunteer service?`;
    const dialogRef = this.dialog.open(DeleteconfirmationComponent, {
      width: '350px',
      data: { volunteerId, serviceId, message }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.deleteService(volunteerId, serviceId, index);
      }
    });
  }

  deleteService(volunteerId: number, serviceId: number, index: number): void {
    this.volunteerService.deleteVolunteerService(volunteerId, serviceId).subscribe({
      next: () => {
        this.volunteerData.assignedServices.splice(index, 1);
      },
      error: (err) => {
        console.error('Error deleting service:', err);
      }
    });
  }
}
