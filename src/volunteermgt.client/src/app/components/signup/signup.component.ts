import { Component } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import Swal from 'sweetalert2';

@Component({
  selector: 'app-signup',
  standalone: false,
  templateUrl: './signup.component.html',
  styleUrl: './signup.component.css'
})
export class SignupComponent {
  loading = false;
  confirmPassword = '';
  roles: { id: string, name: string }[] = [];

  volunteer = {
    email: '',
    username: '',
    phoneNumber: '',
    role: 'User',
    password: ''
  };
  constructor(private http: HttpClient, private router: Router) { }
    
  get passwordMismatch(): boolean {
    return this.volunteer.password !== this.confirmPassword && this.confirmPassword !== '';
  }

  onSignup() {

    if (this.passwordMismatch) {
      alert('Passwords do not match!');
      return;
    }

    this.loading = true;

    this.http.post('/api/volunteers', this.volunteer).subscribe({
      next: (response : any) => {
        if (response.statusCode == 400) {
          console.log(response.errors);
          Swal.fire("Signup Failed!", response.errors.join("\n"), "error");

        } else {
        alert('Signup successful!');
        this.router.navigate(['/login']);
        }
      },
      error: (error) => {
        console.error('Signup failed:', error);
        alert('Signup failed. Please try again.');
      }
    }).add(() => {
      this.loading = false;
    });;
  }
}
