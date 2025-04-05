import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { VolunteerService } from '../../../services/volunteer.service';
import { FormBuilder, FormGroup } from '@angular/forms';
import { MatSnackBar, MatSnackBarRef } from '@angular/material/snack-bar';

@Component({
  selector: 'app-edit-volunteer-service',
  standalone: false,
  templateUrl: './edit-volunteer-service.component.html',
  styleUrl: './edit-volunteer-service.component.css'
})
export class EditVolunteerServiceComponent implements OnInit {
  form!: FormGroup;
  id!: number;

  constructor(  
    private fb: FormBuilder,
    private route: ActivatedRoute,
    private volunteerService: VolunteerService,
    private router: Router,
    private snackBar: MatSnackBar
  ) { }

  ngOnInit(): void {
    this.id = +this.route.snapshot.paramMap.get('id')!;
    this.getVolunteerServiceData(this.id);
  }

  getVolunteerServiceData(id: number) {
    this.volunteerService.getVolunteerServiceById(id).subscribe(response => {
      const data = response.volunteerDetails;

      this.form = this.fb.group({
        volunteerId: [data.volunteerId],
        serviceId:[data.serviceId],
        volunteerName: [data.volunteerName],
        serviceName: [data.serviceName],
        timeSlot: [data.timeSlot],
        exitTime: [data.exitTime],
        batchNumber: [data.batchNumber],
        coupon: [data.coupon]
      });
    });
  }

  onSubmit() {
    if (this.form.valid) {
      const updatedData = { ...this.form.value, id: this.id };
      this.volunteerService.updateVolunteerService(this.id, updatedData).subscribe(() => {
        this.showSnackbar('Volunteer Service Updated Successfully!', 'success');
        this.router.navigate(['/volunteerService']);
      });
    }
  }

  cancel() {
    this.router.navigate(['/volunteerService']);
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
