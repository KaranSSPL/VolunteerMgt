import { Component, ViewEncapsulation } from '@angular/core';
import { FormArray, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { VolunteerService } from '../../../../services/volunteer.service';
import { MatSnackBar, MatSnackBarRef } from '@angular/material/snack-bar';


@Component({
  selector: 'app-create-volunteer',
  standalone: false,
  templateUrl: './create-volunteer.component.html',
  styleUrls: ['./create-volunteer.component.css'],
  encapsulation: ViewEncapsulation.None,
})
export class CreateVolunteerComponent {
  volunteerForm: FormGroup;
  allDays: string[] = ['Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday', 'Sunday'];
  photoPreview: string | ArrayBuffer | null = null;
  uploadedPhoto: File | null = null;
  suggestedDays: string[][] = [];
  selectedDayIndex: number[] = [];

  constructor(private fb: FormBuilder, private router: Router, private volunteerService: VolunteerService, private snackBar: MatSnackBar) {
    this.volunteerForm = this.fb.group({
      name: ['', Validators.required],
      mobileNo: ['', [Validators.required, Validators.pattern('^[0-9]{10}$')]],
      address: ['', Validators.required],
      occupation: ['', Validators.required],
      image: ['', Validators.required],
      code: ['', Validators.required],
      availabilities: this.fb.array([this.createAvailability()]),
    });
  }

  onFileSelect(event: Event): void {
    const fileInput = event.target as HTMLInputElement;
    if (fileInput.files && fileInput.files[0]) {
      this.uploadedPhoto = fileInput.files[0];
      const reader = new FileReader();
      reader.onload = (e) => {
        this.photoPreview = e.target?.result as string | ArrayBuffer;
      };
      reader.readAsDataURL(this.uploadedPhoto);
    }
  }

  capturePhoto(): void {
    navigator.mediaDevices.getUserMedia({ video: true }).then((stream) => {
      const video = document.createElement('video');
      video.srcObject = stream;
      video.play();
      const canvas = document.createElement('canvas');
      const context = canvas.getContext('2d');
      setTimeout(() => {
        canvas.width = video.videoWidth;
        canvas.height = video.videoHeight;
        context?.drawImage(video, 0, 0, canvas.width, canvas.height);
        this.photoPreview = canvas.toDataURL('image/png');
        fetch(this.photoPreview as string)
          .then(res => res.blob())
          .then(blob => {
            this.uploadedPhoto = new File([blob], 'captured-photo.png', { type: 'image/png' });
            console.log('Captured Photo File:', this.uploadedPhoto); 
          });
        stream.getTracks().forEach((track) => track.stop());
      }, 1000);
    });
  }

  removePhoto(): void {
    this.photoPreview = null;
    this.uploadedPhoto = null;
    const fileInput = document.querySelector('input[type="file"]') as HTMLInputElement;
    if (fileInput) {
      fileInput.value = ''; 
    }
  }

  get availabilities() {
    return this.volunteerForm.get('availabilities') as FormArray;
  }

  createAvailability(): FormGroup {
    return this.fb.group({
      day: ['', Validators.required],
      timeSlot: ['', Validators.required],
    });
  }

  addAvailability() {
    this.availabilities.push(this.createAvailability());
  }

  removeAvailability(index: number) {
    if (this.availabilities.length > 1) {
      this.availabilities.removeAt(index);
      this.suggestedDays.splice(index, 1);
    }
  }

  filterDays(index: number) {
    const inputDay = this.availabilities.at(index).get('day')?.value.toLowerCase();
    if (inputDay) {
      this.suggestedDays[index] = this.allDays.filter(day =>
        day.toLowerCase().startsWith(inputDay)
      );
      this.selectedDayIndex[index] = -1;
    } else {
      this.suggestedDays[index] = [];
    }
  }

  handleKeydown(event: KeyboardEvent, index: number) {
    if (this.suggestedDays[index]?.length) {
      if (event.key === 'ArrowDown') {
        event.preventDefault();
        this.selectedDayIndex[index] = (this.selectedDayIndex[index] + 1) % this.suggestedDays[index].length;
      } else if (event.key === 'ArrowUp') {
        event.preventDefault();
        this.selectedDayIndex[index] = (this.selectedDayIndex[index] - 1 + this.suggestedDays[index].length) % this.suggestedDays[index].length;
      } else if (event.key === 'Enter' && this.selectedDayIndex[index] !== -1) {
        event.preventDefault();
        this.selectDay(index, this.suggestedDays[index][this.selectedDayIndex[index]]);
      }
    }
  }

  selectDay(index: number, day: string) {
    this.availabilities.at(index).get('day')?.setValue(day);
    this.suggestedDays[index] = [];
    this.selectedDayIndex[index] = -1;
  }

  hideSuggestions(index: number) {
    setTimeout(() => {
      this.suggestedDays[index] = [];
    }, 200); 
  }

  submitForm() {
    if (this.volunteerForm.valid) {
      const volunteerData = new FormData();
      volunteerData.append('name', this.volunteerForm.value.name);
      volunteerData.append('mobileNo', this.volunteerForm.value.mobileNo);
      volunteerData.append('address', this.volunteerForm.value.address);
      volunteerData.append('occupation', this.volunteerForm.value.occupation);
      volunteerData.append('code', this.volunteerForm.value.code);
      volunteerData.append('availabilities', JSON.stringify(this.volunteerForm.value.availabilities));

      if (this.uploadedPhoto) {
        volunteerData.append('image', this.uploadedPhoto);
      }
      this.volunteerService.addVolunteer(volunteerData).subscribe({
        next: (response) => {
          this.showSnackbar('Volunteer Created Successfully!', 'success');
          this.volunteerForm.reset();
          this.availabilities.clear();
          this.availabilities.push(this.createAvailability());
          this.photoPreview = null;
          this.uploadedPhoto = null;
        },
        error: (err) => {
          console.error('Error:', err);
          this.showSnackbar('Something went wrong! Please try again.', 'error');
        },
      });
    } else {
      this.showSnackbar('Please fill all required fields correctly.', 'error');
    }
  }

  cancel(): void {
    this.router.navigate(['/volunteer']);
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
