import { Component, HostListener } from '@angular/core';
import { Volunteer } from '../../../Models/volunteer.model';
import { VolunteerService } from '../../../services/volunteer.service';
import { Service } from '../../../Models/voluteerService.model';
import { AssignedVolunteer } from '../../../Models/assignVolunteer.model';
import { MatSnackBar, MatSnackBarRef } from '@angular/material/snack-bar';
import { AddcoupondialogComponent } from '../../../Dialogbox/addcoupondialog/addcoupondialog.component';
import { MatDialog } from '@angular/material/dialog';
import { CouponService } from '../../../services/coupon.service';
import { CoupondialogComponent } from '../../../Dialogbox/coupondialog/coupondialog.component';
import { map, of, switchMap } from 'rxjs';

@Component({
  selector: 'app-assignservice',
  standalone: false,
  templateUrl: './assignservice.component.html',
  styleUrl: './assignservice.component.css',
})
export class AssignserviceComponent {
  @HostListener('document:click', ['$event'])
  onClickOutside(event: Event): void {
    const targetElement = event.target as HTMLElement;

    if (!targetElement.closest('.volunteer-container')) {
      this.searchSuggestions = [];
    }

    if (!targetElement.closest('.service-container')) {
      this.serviceSuggestions = [];
    }
  }

  timeSlot: string = '';
  batchNo!: number;
  coupon: number = 1;
  volunteerCoupons!: number;
  serviceVolunteerCounts: AssignedVolunteer[] = [];
  volunteers: Volunteer[] = [];
  searchQuery: string = '';
  searchSuggestions: Volunteer[] = [];
  selectedVolunteer: Volunteer | null = null;
  assignedServices: number[] = []; 

  services: Service[] = [];
  serviceQuery: string = '';
  serviceSuggestions: Service[] = [];
  selectedService: Service | null = null;
  todayDate: Date = new Date();
  totalCoupons: number = 0;
  remainingCoupons: number = 0;
  additionalCoupons: number = 0;
  selectedVolunteerIndex: number = -1;
  selectedServiceIndex: number = -1;
  totalValue: number = 0;
  isEkadashi: boolean = false;
  isFestival: boolean = false;

  constructor(private volunteerService: VolunteerService,
    private snackBar: MatSnackBar,
    private dialog: MatDialog,
    private couponService: CouponService ) {
    this.volunteerService.getVolunteers().subscribe((data) => {
      this.volunteers = data;
    });
    this.setCurrentTime();
    this.fetchCoupons();
    this.loadCheckboxState();
    this.volunteerService.getAllServices().subscribe((data) => {
      this.services = data;
    });
  }

  fetchCoupons() {
    this.couponService.getCoupons().pipe(
      switchMap((coupons) => {
        const today = new Date().toISOString().split("T")[0];
        const todayCoupons = coupons.filter(coupon => coupon.date.startsWith(today));
        this.totalCoupons = todayCoupons.reduce((sum, coupon) => sum + coupon.couponValue, 0);
        if (todayCoupons.length === 0) {
          return of(0);
        }
        return this.couponService.getAdditionalCoupons().pipe(
          map((additionalCoupons) => {
            return todayCoupons.reduce((sum, todayCoupon) => {
              const matchingCoupon = additionalCoupons.find(c => c.couponId === todayCoupon.id);
              return sum + (matchingCoupon ? matchingCoupon.totalValue : 0);
            }, 0);
          })
        );
      })
    ).subscribe((additionalCouponValue) => {
      this.additionalCoupons = additionalCouponValue;
      this.fetchServiceVolunteerCounts();
    });
  }

  fetchServiceVolunteerCounts() {
    const day = this.getSelectedDay();

    this.volunteerService.getServiceVolunteerCounts(day).subscribe((data) => {
      this.serviceVolunteerCounts = data;
      const coupons = this.serviceVolunteerCounts.map(x => x.totalCouponsToday);
      this.volunteerCoupons = coupons[0];
      this.calculateRemainingCoupons();
    });
  }

  getSelectedDay(): string {
    if (this.isFestival) {
      return "festival";
    } else if (this.isEkadashi) {
      return "ekadashi";
    }
    return this.getCurrentDay();
  }

  getCurrentDay(): string {
    const days = ["sunday", "monday", "tuesday", "wednesday", "thursday", "friday", "saturday"];
    const todayIndex = new Date().getDay();
    return days[todayIndex];
  }

  saveCheckboxState() {
    localStorage.setItem("isEkadashi", JSON.stringify(this.isEkadashi));
    localStorage.setItem("isFestival", JSON.stringify(this.isFestival));
  }

  loadCheckboxState() {
    const ekadashiStored = localStorage.getItem("isEkadashi");
    const festivalStored = localStorage.getItem("isFestival");

    this.isEkadashi = ekadashiStored ? JSON.parse(ekadashiStored) : false;
    this.isFestival = festivalStored ? JSON.parse(festivalStored) : false;
  }

  onCheckboxChange(type: 'ekadashi' | 'festival') {
    if (type === 'ekadashi') {
      this.isFestival = false; 
    } else if (type === 'festival') {
      this.isEkadashi = false; 
    }

    this.saveCheckboxState(); 
    this.fetchServiceVolunteerCounts(); 
  }

  calculateRemainingCoupons() {
    const totalVolunteers = this.getTotal('volunteerCount');
    const remaining = (this.totalCoupons || 0) - totalVolunteers - (this.additionalCoupons || 0);
    this.remainingCoupons = remaining > 0 ? remaining : 0;
  }

  searchVolunteers(): void {
    if (this.searchQuery.trim() === '') {
      this.searchSuggestions = [];
      return;
    }
    this.searchSuggestions = this.volunteers.filter(volunteer =>
      volunteer.name.toLowerCase().includes(this.searchQuery.toLowerCase()) ||
      volunteer.code.toLowerCase().includes(this.searchQuery.toLowerCase())
    );
  }

  selectSuggestion(volunteer: Volunteer): void {
    this.searchQuery = volunteer.name;
    this.searchSuggestions = [];
    this.selectedVolunteer = volunteer;
    this.selectedVolunteerIndex = -1;
    this.volunteerService.getServiceVolunteerById(volunteer.id).subscribe((response) => {      
      const services = response.data || [];
      this.assignedServices = services.map((service: { serviceId: any }) => service.serviceId);
    });
  }

  searchServices(): void {
    if (this.serviceQuery.trim() === '') {
      this.serviceSuggestions = [];
      return;
    }
    this.serviceSuggestions = this.services.filter(service =>
      service.serviceName.toLowerCase().includes(this.serviceQuery.toLowerCase()) &&
      !this.assignedServices.includes(service.id)
    );
  }

  selectService(service: Service): void {
    if (this.assignedServices.map(id => +id).includes(+service.id)) {
      this.showSnackbar("This service is already assigned to the selected volunteer.", "error");
      return;
    }

    this.serviceQuery = service.serviceName;
    this.serviceSuggestions = [];
    this.selectedServiceIndex = -1;
    this.selectedService = service;
  }


  private setCurrentTime(): void {
    const now = new Date();
    const hours = now.getHours().toString().padStart(2, '0');
    const minutes = now.getMinutes().toString().padStart(2, '0');
    this.timeSlot = `${hours}:${minutes}`;
  }

  onAssign(): void {
    if (!this.selectedVolunteer) {
      return this.showSnackbar("Please select a volunteer.", "error");
    }
    if (!this.selectedService) {
      return this.showSnackbar("Please select a service.", "error");
    }
    if (!this.timeSlot) {
      return this.showSnackbar("Please select a time slot.", "error");
    }
    if (!this.batchNo) {
      return this.showSnackbar("Please provide a Batch Number.", "error");
    }
    if (!this.coupon) {
      return this.showSnackbar("Please enter coupon value.", "error");
    }

    const [hours, minutes] = this.timeSlot.split(":").map(Number);
    const currentDate = new Date();
    currentDate.setHours(hours, minutes, 0, 0);
    const istTime = new Date(currentDate.getTime() - currentDate.getTimezoneOffset() * 60000);

    const payload = {
      volunteerId: this.selectedVolunteer.id,
      serviceId: this.selectedService.id,
      timeSlot: istTime.toISOString(),
      BatchNumber: this.batchNo,
      Coupon: this.coupon
    };

    this.volunteerService.assignService(payload).subscribe({
      next: () => {
        this.showSnackbar("Service assigned successfully!", "success");
        this.assignedServices.push(this.selectedService!.id);
        window.location.reload();
      },
      error: () => this.showSnackbar("Assignment failed. Try again.", "error")
    });
  };

  handleKeydown(event: KeyboardEvent, type: 'volunteer' | 'service') {
    if (type === 'volunteer') {
      if (this.searchSuggestions.length === 0) return;
      if (event.key === 'ArrowDown') {
        event.preventDefault();
        this.selectedVolunteerIndex = (this.selectedVolunteerIndex + 1) % this.searchSuggestions.length;
        this.scrollIntoView('volunteer');
      } else if (event.key === 'ArrowUp') {
        event.preventDefault();
        this.selectedVolunteerIndex = (this.selectedVolunteerIndex - 1 + this.searchSuggestions.length) % this.searchSuggestions.length;
        this.scrollIntoView('volunteer');
      } else if (event.key === 'Enter' && this.selectedVolunteerIndex >= 0) {
        event.preventDefault();
        this.selectSuggestion(this.searchSuggestions[this.selectedVolunteerIndex]);
      }
    } else if (type === 'service') {
      if (this.serviceSuggestions.length === 0) return;
      if (event.key === 'ArrowDown') {
        event.preventDefault();
        this.selectedServiceIndex = (this.selectedServiceIndex + 1) % this.serviceSuggestions.length;
        this.scrollIntoView('service');
      } else if (event.key === 'ArrowUp') {
        event.preventDefault();
        this.selectedServiceIndex = (this.selectedServiceIndex - 1 + this.serviceSuggestions.length) % this.serviceSuggestions.length;
        this.scrollIntoView('service');
      } else if (event.key === 'Enter' && this.selectedServiceIndex >= 0) {
        event.preventDefault();
        this.selectService(this.serviceSuggestions[this.selectedServiceIndex]);
      }
    }
  }

  scrollIntoView(type: 'volunteer' | 'service') {
    setTimeout(() => {
      let index = type === 'volunteer' ? this.selectedVolunteerIndex : this.selectedServiceIndex;
      let list = document.querySelectorAll(`.${type}-suggestions li`);
      if (list.length > 0 && index >= 0 && list[index]) {
        list[index].scrollIntoView({ behavior: 'smooth', block: 'nearest' });
      }
    });
  }
 
  onAddCoupons() {
    const dialogRef = this.dialog.open(AddcoupondialogComponent, {
      width: '400px'
    });
    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        console.log(`Coupons Added: ${result}`);     
      }
    });
  }

  openCouponDialog() {
    const dialogRef = this.dialog.open(CoupondialogComponent, {
      width: '600px',
      data: { coupons: this.additionalCoupons }
    });
    dialogRef.afterClosed().subscribe(result => {
      if (result !== undefined) {
        this.additionalCoupons = result;
      }
    });
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

  getTotal(field: keyof AssignedVolunteer): number {
    return this.serviceVolunteerCounts.reduce((total, service) => total + Number(service[field]), 0);
  }
}
