import { Component } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  standalone: false,
  styleUrl: './app.component.css'
})
export class AppComponent {
  showHeader: boolean = true;
  isSidebarOpen = true;

  constructor(private router: Router) {
    this.router.events.subscribe(() => {
      this.showHeader = this.router.url !== '/login' && this.router.url !== '/register';
    });
  }

  toggleSidebar() {
    this.isSidebarOpen = !this.isSidebarOpen;
  }

  title = 'volunteermgt.client';
}
