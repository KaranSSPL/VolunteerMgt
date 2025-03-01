import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { UserService } from '../../services/user.service';

@Component({
  selector: 'app-login',
  standalone:false,
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit {
  loginForm!: FormGroup;
  errorMessage: string = '';

  constructor(private fb: FormBuilder, private userService: UserService, private router: Router) { }

  ngOnInit(): void {
    this.loginForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(6)]]
    });
  }

  get f() {
    return this.loginForm.controls;
  }

  onSubmit(): void {
    if (this.loginForm.invalid) return;

    const credentials = this.loginForm.value;

    this.userService.login(credentials).subscribe({
      next: (response) => {
        if (response.body.statusCode === 200) {
          sessionStorage.setItem('authToken', response.body.payload.token);
          this.router.navigate(['/dashboard']);
        } else {
          this.errorMessage = 'Login failed. Please try again.';
        }
      },
      error: () => {
        this.errorMessage = 'Invalid email or password.';
      }
    });
  }
}
