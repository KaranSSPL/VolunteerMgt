import { Component } from '@angular/core';
import { FormArray, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { VolunteerService } from '../../../../services/volunteer.service';

@Component({
  selector: 'app-create-volunteer',
  standalone: false,
  templateUrl: './create-volunteer.component.html',
  styleUrls: ['./create-volunteer.component.css'],
})
export class CreateVolunteerComponent {
  volunteerForm: FormGroup;

  allDays: string[] = ['Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday', 'Sunday'];

  suggestedDays: string[][] = [];

  constructor(private fb: FormBuilder, private router: Router, private volunteerService: VolunteerService) {
    this.volunteerForm = this.fb.group({
      name: ['', Validators.required],
      mobileNo: ['', [Validators.required, Validators.pattern('^[0-9]{10}$')]],
      address: ['', Validators.required],
      occupation: ['', Validators.required],
      availabilities: this.fb.array([this.createAvailability()]),
    });
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
    } else {
      this.suggestedDays[index] = [];
    }
  }

  selectDay(index: number, day: string) {
    this.availabilities.at(index).get('day')?.setValue(day);
    this.suggestedDays[index] = []; 
  }

  hideSuggestions(index: number) {
    setTimeout(() => {
      this.suggestedDays[index] = [];
    }, 200); 
  }

  submitForm() {
    if (this.volunteerForm.valid) {
      const volunteerData = this.volunteerForm.value;
      this.volunteerService.addVolunteer(volunteerData).subscribe({
        next: (response) => {
          console.log('Success:', response);
          alert('Volunteer Created Successfully!');
          this.volunteerForm.reset();
          this.availabilities.clear();
          this.availabilities.push(this.createAvailability());
        },
        error: (err) => {
          console.error('Error:', err);
          alert('Something went wrong! Please try again.');
        },
      });
    } else {
      alert('Please fill all required fields correctly.');
    }
  }



  cancel(): void {
    this.router.navigate(['/volunteer']);
  }
}
