import { Component } from '@angular/core';
import { AuthService } from './services/auth.service';
import { Router } from '@angular/router';
import Swal from 'sweetalert2';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrl: './login.component.css',
  standalone: false
})
export class LoginComponent {
  email: string = '';
  password: string = '';
  loading: boolean = false;

  constructor(private authService: AuthService, private router: Router) { }

  onSubmit() {
    this.loading = true;

    this.authService.login(this.email, this.password).subscribe(
      response => {
        if (response.payload) {
          sessionStorage.setItem('authToken', response.payload.token);
          Swal.fire({ position: "center", icon: "success", title: "Successfully Login", showConfirmButton: false, timer: 1500 });
          this.router.navigate(['/home']); // Redirect after login
        } else {
          Swal.fire("Login Failed!", "Invalid credentials. Please try again.", "error");
        }
      },
      error => {
        Swal.fire("Login Failed!", "Invalid credentials. Please try again.", "error");
      }
    ).add(() => {
      this.loading = false;
    });
  }
}

