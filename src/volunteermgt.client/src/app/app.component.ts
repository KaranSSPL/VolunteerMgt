import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  standalone: false,
  styleUrl: './app.component.css'
})
export class AppComponent {
  title = 'volunteermgt.client';

  showNavbar: boolean = true;
  isModalOpen: boolean = false;
  isLoading: boolean = false;

  constructor(private router: Router) {
    // Listen to route changes
    this.router.events.subscribe(() => {
      this.showNavbar = this.router.url !== '/login'; 
    });
  }

  openModal() {
    this.isModalOpen = true;
  }

  // Close modal
  closeModal() {
    this.isModalOpen = false;
  }
  logout() {
    this.isLoading = true;

    setTimeout(() => {
      sessionStorage.removeItem('authToken');
      this.isModalOpen = false;
      this.router.navigate(['/login']).then(() => {
        this.isLoading = false; 
      });
    }, 1500); 
  }
}
