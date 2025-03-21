import { Component, Input, Output, EventEmitter, OnInit, ViewEncapsulation } from '@angular/core';
import { FormBuilder, FormGroup, FormArray } from '@angular/forms';
import { Volunteer } from '../../../../Models/volunteer.model';
import { VolunteerService } from '../../../../services/volunteer.service';
import { MatSnackBar, MatSnackBarRef } from '@angular/material/snack-bar';


@Component({
  selector: 'app-edit-volunteer',
  standalone: false,
  templateUrl: './edit-volunteer.component.html',
  styleUrls: ['./edit-volunteer.component.css'],
  encapsulation: ViewEncapsulation.None,
})
export class EditVolunteerComponent implements OnInit {
  @Input() volunteer: Volunteer | null = null; 
  @Output() cancelEdit = new EventEmitter<void>();
  photoPreview: string | ArrayBuffer | null = null;
  uploadedPhoto: File | null = null;
  volunteerForm!: FormGroup;
  allDays: string[] = ['Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday','Saturday', 'Sunday'];
  suggestedDays: string[][] = [];

  constructor(
    private fb: FormBuilder,
    private volunteerService: VolunteerService,
    private snackBar: MatSnackBar
  ) { }

  ngOnInit(): void {
    if (this.volunteer) {
      this.initializeForm();
    }
  }

  initializeForm(): void {
    this.volunteerForm = this.fb.group({
      name: [this.volunteer?.name || ''],
      mobileNo: [this.volunteer?.mobileNo || ''],
      address: [this.volunteer?.address || ''],
      occupation: [this.volunteer?.occupation || ''],
      image: [this.volunteer?.image || ''],
      code: [this.volunteer?.code || ''],
      availabilities: this.fb.array(this.volunteer?.availabilities?.map(avail => this.createAvailabilityGroup(avail)) || [])
    });
  }

  createAvailabilityGroup(avail?: any): FormGroup {
    return this.fb.group({
      day: [avail?.day || ''],
      timeSlot: [avail?.timeSlot || '']
    });
  }
  get availabilities(): FormArray {
    return this.volunteerForm.get('availabilities') as FormArray;
  }

  addAvailability(): void {

    this.availabilities.push(this.createAvailabilityGroup());
  }

  removeAvailability(index: number): void {
    this.availabilities.removeAt(index);
  }

 updateVolunteer(): void {
  if (!this.volunteer || !this.volunteerForm.valid) return;

  const formData = new FormData();
  formData.append("name", this.volunteerForm.value.name);
  formData.append("mobileNo", this.volunteerForm.value.mobileNo);
  formData.append("address", this.volunteerForm.value.address);
   formData.append("occupation", this.volunteerForm.value.occupation);
   formData.append("code", this.volunteerForm.value.code);

  if (this.uploadedPhoto) {
    formData.append("image", this.uploadedPhoto);
  }
  formData.append("availabilities", JSON.stringify(this.volunteerForm.value.availabilities));

  this.volunteerService.updateVolunteer(this.volunteer.id, formData).subscribe(
    () => {
      this.showSnackbar("Volunteer updated successfully!", "success");
      this.cancelEdit.emit();
      window.location.reload();
    },
    (error) => console.error("Error updating volunteer:", error)
  );
}


  cancel(): void {
    this.cancelEdit.emit();  
  }

  filterDays(index: number) {
    const inputDay = this.availabilities.at(index).get('day')?.value.toLowerCase();

    if (inputDay) {
      this.suggestedDays[index] = this.allDays.filter(day =>
        day.toLowerCase().startsWith(inputDay)
      );
    } else {
      this.suggestedDays[index] = [];
    }
  }

  selectDay(index: number, day: string) {
    this.availabilities.at(index).get('day')?.setValue(day);
    this.suggestedDays[index] = [];
  }

  onFileSelected(event: Event): void {
    const fileInput = event.target as HTMLInputElement;
    if (fileInput.files && fileInput.files.length > 0) {
      this.uploadedPhoto = fileInput.files[0];

      const reader = new FileReader();
      reader.onload = () => {
        this.photoPreview = reader.result;
      };
      reader.readAsDataURL(this.uploadedPhoto);
    }
  }

  hideSuggestions(index: number) {
    setTimeout(() => {
      this.suggestedDays[index] = [];
    }, 200);
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
