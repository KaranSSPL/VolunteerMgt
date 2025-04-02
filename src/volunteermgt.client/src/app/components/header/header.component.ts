import { Component, EventEmitter, Output } from '@angular/core';
import { UserService } from '../../services/user.service';

@Component({
  selector: 'app-header',
  standalone:false,
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.css']
})
export class HeaderComponent {
  dropdownOpen = false;

  @Output() menuToggle = new EventEmitter<void>();

  constructor(private userService: UserService) { }

  toggleSidebar() {
    this.menuToggle.emit(); 
  }

  toggleDropdown(event: Event) {
    event.preventDefault();
    this.dropdownOpen = !this.dropdownOpen;
  }

  logout() {
    this.userService.logout();
  }
}
