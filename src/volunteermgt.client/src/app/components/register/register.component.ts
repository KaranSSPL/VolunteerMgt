import { Component } from '@angular/core';
import { UserService } from '../../services/user.service';
import { NgForm } from '@angular/forms';
import { Router } from '@angular/router';

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

  constructor(private userService: UserService, private router: Router) { }

  registerUser(form: NgForm) {
    if (form.invalid) {
      return;
    }

    this.userService.register(this.user).subscribe({
      next: (response) => {
        console.log('User registered successfully', response);
        this.router.navigate(['/login']);
      },
      error: (error) => {
        console.error('Registration failed', error);
      }
    });
  }
}
