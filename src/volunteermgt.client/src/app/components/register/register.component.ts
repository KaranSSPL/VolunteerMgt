import { Component } from '@angular/core';
import { VolunteerService } from '../../services/volunteer.service';
import { UserService } from '../../services/user.service';

@Component({
  selector: 'app-register',
  standalone: false,
  templateUrl: './register.component.html',
  styleUrl: './register.component.css'
})
export class RegisterComponent {
  user = {
    id: 0, 
    firstname: '',
    lastname: '',
    username: '',
    roles: 'User', 
    email: '',
    phone: '',
    password: ''
  };
  constructor(private userService: UserService) { }

  registerUser() {
    this.userService.register(this.user).subscribe({
      next: (response) => {
        console.log('User registered successfully', response);
      },
      error: (error) => {
        console.error('Registration failed', error);
      }
    });
  }
}
