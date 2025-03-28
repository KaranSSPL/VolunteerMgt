import { Component } from '@angular/core';
import { AuthService } from '../../../services/auth/auth.service';

import { Router } from '@angular/router';
import Swal from 'sweetalert2';
import { FormControl, FormGroup, FormBuilder, Validators } from '@angular/forms';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrl: './login.component.css',
  standalone: false
})
export class LoginComponent {
  loginForm!: FormGroup;
  loading: boolean = false;

  constructor(private authService: AuthService, private router: Router, private formBuilder: FormBuilder) {
    this.init();
  }
  init() {
    this.loginForm = this.formBuilder.group({
      email: new FormControl('', [Validators.required, Validators.email, Validators.maxLength(256)]),
      password: new FormControl('', [Validators.required, Validators.minLength(8)]),
    });
  }
  onSubmit() {
    if (this.loginForm.invalid) {
      this.loginForm.markAllAsTouched();
      return;
    }

    this.loading = true;
    const { email, password } = this.loginForm.value;

    this.authService.login(email, password).subscribe(
      response => {
        if (response.payload) {
          this.authService.saveToken(response.payload.token);
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

