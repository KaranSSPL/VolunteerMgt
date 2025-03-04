import { Component, Input, Output, EventEmitter, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, FormArray } from '@angular/forms';
import { Volunteer } from '../../../../Models/volunteer.model';
import { VolunteerService } from '../../../../services/volunteer.service';

@Component({
  selector: 'app-edit-volunteer',
  standalone: false,
  templateUrl: './edit-volunteer.component.html',
  styleUrls: ['./edit-volunteer.component.css'],
})
export class EditVolunteerComponent implements OnInit {
  @Input() volunteer: Volunteer | null = null; 
  @Output() cancelEdit = new EventEmitter<void>();

  volunteerForm!: FormGroup;
  allDays: string[] = ['Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday','Saturday', 'Sunday'];
  suggestedDays: string[][] = [];

  constructor(
    private fb: FormBuilder,
    private volunteerService: VolunteerService
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

    const updatedVolunteer = { ...this.volunteer, ...this.volunteerForm.value };

    this.volunteerService.updateVolunteer(this.volunteer.id, updatedVolunteer).subscribe(
      () => {
        alert('Volunteer updated successfully!');
        this.cancelEdit.emit();
      },
      (error) => console.error('Error updating volunteer:', error)
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

  hideSuggestions(index: number) {
    setTimeout(() => {
      this.suggestedDays[index] = [];
    }, 200);
  }
}
