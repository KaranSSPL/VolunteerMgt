import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../services/auth/auth.service';


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

  constructor(private router: Router, private authService: AuthService) {
    // Listen to route changes
    this.router.events.subscribe(() => {
      this.showNavbar = !['/login', '/signup', '/404'].includes(this.router.url);
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
      this.authService.logout();
      this.isModalOpen = false;
      this.router.navigate(['/login']).then(() => {
        this.isLoading = false; 
      });
    }, 1500); 
  }
}
