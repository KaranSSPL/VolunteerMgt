import { Component } from '@angular/core';
import { AuthService } from '../login/services/auth.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-login',
  standalone: false,
  templateUrl: './login.component.html',
  styleUrl: './login.component.css'
})
export class LoginComponent {
  email: string = '';
  password: string = '';
  constructor(private authService: AuthService, private router: Router) { }

  onSubmit() {
    this.authService.login(this.email, this.password).subscribe(
      response => {
        console.log('Login successful', response);
        alert('Login Successful');
   
        localStorage.setItem('token', response.payload.token); 
        this.router.navigate(['/home']); // Redirect after login
      },
      error => {
        console.error('Login failed', error);
        alert('Login Failed');
      }
    );
  }
}
