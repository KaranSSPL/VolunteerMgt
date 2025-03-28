import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { ActivatedRoute, Router } from '@angular/router';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { AuthService } from '../../../services/auth/auth.service';
import { UserService } from '../../../services/user/user.service';
import Swal from 'sweetalert2';

@Component({
  selector: 'app-edit-profile',
  standalone: false,
  templateUrl: './edit.component.html',
  styleUrls: ['./edit.component.css'],
})
export class EditComponent implements OnInit {
  editForm!: FormGroup;
  loading = false;
  isAdmin = false;
  constructor(private route: ActivatedRoute, private http: HttpClient, private formBuilder: FormBuilder, private router: Router,
    private authService: AuthService, private userService: UserService) { }

  ngOnInit() {
    this.isAdmin = this.authService.getRole() === 'Admin';
    this.initForm();

    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.getUserDetails(id);
    }
  }

  initForm() {
    this.editForm = this.formBuilder.group({
      id: [''],
      email: ['', [Validators.required, Validators.email]],
      username: ['', [Validators.required, Validators.minLength(3)]],
      phoneNumber: ['', [Validators.required, Validators.pattern('^[0-9]{10}$')]],
      role: [{ value: '', disabled: !this.isAdmin }, Validators.required],
    });
  }
  getUserDetails(id: string) {
    this.loading = true;

    this.http.get(`/api/volunteers/${id}`).subscribe({
      next: (data: any) => {
        if (data && data?.payload) {
          this.editForm.patchValue(data.payload);
        } else {
          this.editForm.patchValue(data);
        }
      },
      error: (error) => {
        Swal.fire("Error!", "Failed to load user details.", "error");
      },
      complete: () => (this.loading = false),
    });
  }

  onUpdate() {
    if (this.editForm.invalid) {
      this.editForm.markAllAsTouched();
      return;
    }

    this.loading = true;
    this.http.put('/api/volunteers/', this.editForm.getRawValue()).subscribe({
      next: (response: any) => {
        Swal.fire({ position: "center", icon: "success", title: "Profile updated successfully.", showConfirmButton: false, timer: 1500 });
        this.router.navigate(['/home']);
      },
      error: (error) => {
        Swal.fire("Update Failed!", error.message || "Something went wrong.", "error");
      },
      complete: () => (this.loading = false),
    });
  }
  openRolePopup(userId: string) {
    this.userService.getRoles().subscribe({
      next: (response) => {
        if (!Array.isArray(response.payload)) {
          Swal.fire('Error!', 'Roles data is not in the expected format.', 'error');
          return;
        }

        const roleOptions = response.payload.reduce((acc: { [key: string]: string }, role: any) => {
          if (role?.name) {
            acc[role.id] = role.name;
          }
          return acc;
        }, {});

        Swal.fire({
          title: 'Assign Role',
          input: 'select',
          inputOptions: roleOptions,
          inputPlaceholder: 'Select a role',
          showCancelButton: true,
          confirmButtonText: 'Assign',
          cancelButtonText: 'Cancel',
          preConfirm: (selectedRole) => {
            if (!selectedRole) {
              Swal.showValidationMessage('Please select a role.');
            }
            return selectedRole;
          }
        }).then((result) => {
          if (result.isConfirmed) {
            this.assignRole(userId, result.value);
          }
        });
      },
      error: () => {
        Swal.fire('Error!', 'Failed to load roles.', 'error');
      }
    });
  }

  assignRole(userId: string, roleId: string) {
    this.userService.assignRole(userId, roleId).subscribe({
      next: (response) => {
        if (response.statusCode === 400) {
          Swal.fire("Login Failed!", "Invalid credentials. Please try again.", "error");
        }
        else if (response.statusCode === 409) {
          Swal.fire("Assign Failed!", "Already assign", "error");
        }
        else if (response.statusCode === 500) {
          Swal.fire("Failed!", "Failed to assign roles to the user.", "error");
        }
        else {
          Swal.fire('Success!', 'Role assigned successfully.', 'success');
        }
      },
      error: () => {
        Swal.fire('Error!', 'Failed to assign role.', 'error');
      },
      complete: () => this.ngOnInit(),
    });
  }
}
