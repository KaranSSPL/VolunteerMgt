import { Component } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import Swal from 'sweetalert2';
import { FormBuilder, FormGroup, Validators, AbstractControl, ValidationErrors } from '@angular/forms';

@Component({
  selector: 'app-signup',
  standalone: false,
  templateUrl: './signup.component.html',
  styleUrl: './signup.component.css'
})
export class SignupComponent {
  signUpForm!: FormGroup;
  loading = false;
  constructor(private http: HttpClient, private router: Router, private formBuilder: FormBuilder) { }

  ngOnInit() {
    this.signUpForm = this.formBuilder.group({
      email: ['', [Validators.required, Validators.email]],
      username: ['', Validators.required],
      phoneNumber: ['', [Validators.required, Validators.pattern('^[0-9]{10}$')]],
      role: [{ value: 'User', disabled: true }],
      password: ['',  
        [Validators.required,
        Validators.pattern('^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[!@#$%^&*])[A-Za-z\\d!@#$%^&*]{8,}$')
        ]],
      confirmPassword: ['', Validators.required]
    }, { validators: this.passwordsMatchValidator });
  }
  private passwordsMatchValidator(group: FormGroup): ValidationErrors | null {
    const password = group.get('password')?.value;
    const confirmPassword = group.get('confirmPassword')?.value;
    return password === confirmPassword ? null : { passwordMismatch: true };
  }

  onSignup() {
    if (this.signUpForm.invalid) {
      this.signUpForm.markAllAsTouched();
      return;
    }

    this.loading = true;
    const signupData = this.signUpForm.getRawValue();

    this.http.post('/api/volunteers', signupData).subscribe({
      next: (response: any) => {
        if (response.statusCode === 400) {
          Swal.fire("Signup Failed!", response.errors.join("\n"), "error");
        } else {
          Swal.fire({ position: "center", icon: "success", title: "Signup successful!", showConfirmButton: false, timer: 2000 });
          this.router.navigate(['/login']);
        }
      },
      error: (error) => {
        console.error('Signup failed:', error);
        Swal.fire("Signup Failed!", "Something went wrong. Please try again.", "error");
      },
      complete: () => (this.loading = false)
    });
  }
}
