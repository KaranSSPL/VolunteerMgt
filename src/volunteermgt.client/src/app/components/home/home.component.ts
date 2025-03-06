import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { UserService } from './services/user.service';
import Swal from 'sweetalert2';

@Component({
  selector: 'app-home',
  standalone: false,
  templateUrl: './home.component.html',
  styleUrl: './home.component.css'
})
export class HomeComponent implements OnInit {
  users: any[] = [];

  constructor(private userService: UserService) { }
  ngOnInit() {
    this.loadVolunteers();
  }

  loadVolunteers() {
    this.userService.getVolunteers().subscribe({
      next: (response) => {
        this.users = response.payload;
      },
      error: (error) => {
        console.error('Error fetching users:', error);
        Swal.fire('Error!', 'Failed to fetch users.', 'error');
      }
    });
  }
  openRolePopup(userId: string) {
    this.userService.getRoles().subscribe({
      next: (response) => {
        console.log(response.payload);
        if (!Array.isArray(response.payload)) {
          Swal.fire('Error!', 'Roles data is not in the expected format.', 'error');
          return;
        }

        const roleOptions = response.payload.reduce((acc: { [key: string]: string }, role: any) => {
          if (role?.name) {
            acc[role.id] = role.name;
          } return acc;
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
      error: (error) => {
        console.error('Error fetching roles:', error);
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
        } else {
          this.loadVolunteers();
          Swal.fire('Success!', `Role assigned successfully.`, 'success');
        }
      },
      error: (error) => {
        console.error('Error assigning role:', error);
        Swal.fire('Error!', 'Failed to assign role.', 'error');
      }
    });
  }
}








