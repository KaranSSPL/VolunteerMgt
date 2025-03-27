import { Component, OnInit, ViewEncapsulation } from '@angular/core';
import { VolunteerService } from '../../../services/volunteer.service';
import { Volunteer } from '../../../Models/volunteer.model';
import { Router } from '@angular/router';
import { DeleteconfirmationComponent } from '../../../Dialogbox/deleteconfirmation/deleteconfirmation.component';
import { MatDialog } from '@angular/material/dialog';
import { environment } from '../../../../environments/environment';

@Component({
  selector: 'app-volunteertable',
  standalone: false,
  templateUrl: './volunteertable.component.html',
  styleUrls: ['./volunteertable.component.css'],
  encapsulation: ViewEncapsulation.None,
})
export class VolunteertableComponent implements OnInit {
  volunteers: Volunteer[] = [];
  selectedVolunteer: Volunteer | null = null;
  isEditing: boolean = false;  

  constructor(private volunteerService: VolunteerService, private router: Router, private dialog: MatDialog)
 { }

  ngOnInit(): void {
    this.loadVolunteers();
  }

  loadVolunteers(): void {
    this.volunteerService.getVolunteers().subscribe((data) => {
      this.volunteers = data.map(volunteer => ({
        ...volunteer,
        imagePath: volunteer.imagePath ? `${environment.apiUrl}/${volunteer.imagePath}` : ''
      }));
    });
  }

  editVolunteer(volunteer: Volunteer): void {
    this.selectedVolunteer = volunteer;
    this.isEditing = true;  
  }

  cancelEdit(): void {
    this.selectedVolunteer = null;
    this.isEditing = false; 
  }

  deleteVolunteer(id: number): void {
    const message = `Are you sure you want to delete this Volunteer?`;
    const dialogRef = this.dialog.open(DeleteconfirmationComponent, {
      width: '300px',
      data: { message }
    });
    dialogRef.afterClosed().subscribe((result) => {
      if (result) {
        this.volunteerService.deleteVolunteer(id).subscribe(() => {
          this.volunteers = this.volunteers.filter(v => v.id !== id);
        });
      }
    });
  }
}
