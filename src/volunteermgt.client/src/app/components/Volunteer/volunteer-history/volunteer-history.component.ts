import { Component } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { VolunteerService } from '../../../services/volunteer.service';
import { DeleteconfirmationComponent } from '../../../Dialogbox/deleteconfirmation/deleteconfirmation.component';
import { MatSnackBar, MatSnackBarRef } from '@angular/material/snack-bar';
@Component({
  selector: 'app-volunteer-history',
  standalone: false,
  templateUrl: './volunteer-history.component.html',
  styleUrl: './volunteer-history.component.css',
})
export class VolunteerHistoryComponent {
  volunteerServiceMappings: any[] = [];
  defaultTime: string = '10:00 PM';
  searchQuery: string = '';
  filteredMappings: any[] = [];
  sortColumn: string = '';
  sortDirection: 'asc' | 'desc' = 'asc';
  startDate: Date | null = null;
  endDate: Date | null = null;
  couponValues: number[] = Array.from({ length: 21 }, (_, i) => i);
  selectedEqualCoupon: number | null = null;
  selectedGreaterThanCoupon: number | null = null;

  constructor(private volunteerService: VolunteerService, public dialog: MatDialog, private snackBar: MatSnackBar) { }

  ngOnInit(): void {
    this.getVolunteerServiceMappings();
    this.updateExitTimeForPastSlots();
  }

  getVolunteerServiceMappings() {
    this.volunteerService.getVolunteerServiceMappings().subscribe(data => {
      this.volunteerServiceMappings = data;
      this.filteredMappings = [...data];
    });
  }

  applyFilters(): void {
    let query = this.searchQuery.toLowerCase();
    this.filteredMappings = this.volunteerServiceMappings.filter(mapping => {
      let matchesSearch = mapping.volunteerName?.toLowerCase().includes(query) ||
        (mapping.serviceName && mapping.serviceName.toLowerCase().includes(query));
      let matchesDateRange = true;
      if (this.startDate && this.endDate) {
        let slotDate = new Date(mapping.timeSlot);

        let endOfDay = new Date(this.endDate);
        endOfDay.setHours(23, 59, 59, 999);

        matchesDateRange = slotDate >= this.startDate && slotDate <= endOfDay;
      }
      return matchesSearch && matchesDateRange;
    });
  }

  filterByCoupon(): void {
    this.filteredMappings = this.volunteerServiceMappings.filter(mapping => {
      const equalMatch =
        this.selectedEqualCoupon === null || mapping.coupon === this.selectedEqualCoupon;

      const greaterThanMatch =
        this.selectedGreaterThanCoupon === null || mapping.coupon > this.selectedGreaterThanCoupon;

      return equalMatch && greaterThanMatch;
    });
  }

  sortTable(column: string): void {
    if (this.sortColumn === column) {
      this.sortDirection = this.sortDirection === 'asc' ? 'desc' : 'asc';
    } else {
      this.sortColumn = column;
      this.sortDirection = 'asc';
    }
    this.filteredMappings.sort((a, b) => {
      const valueA = a[column];
      const valueB = b[column];
      if (typeof valueA === 'string' && typeof valueB === 'string') {
        return this.sortDirection === 'asc'
          ? valueA.localeCompare(valueB)
          : valueB.localeCompare(valueA);
      }
      if (typeof valueA === 'number' && typeof valueB === 'number') {
        return this.sortDirection === 'asc' ? valueA - valueB : valueB - valueA;
      }
      if (valueA instanceof Date && valueB instanceof Date) {
        return this.sortDirection === 'asc'
          ? valueA.getTime() - valueB.getTime()
          : valueB.getTime() - valueA.getTime();
      }
      return 0;
    });
  }

  updateExitTimeForPastSlots(): void {
    const now = new Date();
    const [currHour, currMin] = [now.getHours(), now.getMinutes()];
    const currDate = now.toISOString().split('T')[0];
    this.volunteerService.getAllServices().subscribe({
      next: (services) => {
        services.forEach(({ id, defaultTime }) => {
          const [defHour, defMin] = defaultTime.split('T')[1].split(':').map(Number);
          this.volunteerServiceMappings.forEach(({ volunteerId, serviceId, timeSlot, exitTime }) => {
            if (serviceId === id && !exitTime && currDate !== timeSlot.split('T')[0]) {
              const formattedExitTime = `${defHour % 12 || 12}:${defMin.toString().padStart(2, '0')} ${defHour >= 12 ? 'PM' : 'AM'}`;
              this.volunteerService.assignService({ volunteerId, serviceId, timeSlot, exitTime: formattedExitTime }).subscribe({
                next: () => {
                  this.getVolunteerServiceMappings()
                  this.showSnackbar("Updated Exit Time For Past Slots","success")
                },
                error: (err) => this.showSnackbar("Error Updating Exit Time For Past Slots","error"),
              });
            }});
        });
      },
      error: (err) => console.error('Error fetching services:', err),
    });
  }

  openDeleteDialog(volunteerId: number, serviceId: number): void {
    const dialogRef = this.dialog.open(DeleteconfirmationComponent, {
      width: '350px',
      data: { message: 'Are you sure you want to delete this volunteer and all assigned services?' }
    });
    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.deleteVolunteer(volunteerId, serviceId);
      }
    });
  }

  deleteVolunteer(volunteerId: number, serviceId: number): void {
    this.volunteerService.deleteVolunteerService(volunteerId, serviceId).subscribe({
      next: () => {
        this.getVolunteerServiceMappings();
        this.showSnackbar("Assigned Service is Deleted Successfully", "success");
      },
      error: (err) => {
        this.showSnackbar("Error Deleting Assigned Service", "error");
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
}
