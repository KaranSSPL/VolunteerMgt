import { Component, OnInit } from '@angular/core';
import Swal from 'sweetalert2';
import { RoleService } from '../services/roles.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-add-role',
  standalone: false,
  templateUrl: './add-role.component.html',
  styleUrl: './add-role.component.css'
})
export class AddRoleComponent implements OnInit {
  roleName: string = '';
  loading: boolean = false;
  permissions: any[] = [];
  constructor(private roleService: RoleService, private router: Router) { }

  ngOnInit() {
    this.fetchPermissions();
    setTimeout(() => {
      this.loading = false;
    }, 5000);
  }

  fetchPermissions() {
    //this.loading = true;
    this.roleService.getPermissions().subscribe(
      (response) => {
        if (response.statusCode == 400) {
          Swal.fire("Fetching  Failed!", response.errors.join("\n"), "error");
          this.loading = false;
        } else {
          this.permissions = response.payload;
          this.loading = false;
        }
      },
      (error) => {
        console.error('Error fetching permissions:', error);
        this.loading = false;
      }
    );
  }

  onSubmit() {
    if (!this.roleName) {
      Swal.fire("Validation Error!", "Role name is required!", "warning");
      return;
    }
    this.loading = true;

    //if (selectedPermissions.length === 0) {
    //  Swal.fire("Validation Error!", "At least one permission must be selected!", "warning");
    //  return;
    //}

    const newRole = {
      name: this.roleName,
      permissions: this.permissions
        .filter(p => p.selected)
        .map(p => ({ id: p.id }))
    };

    this.roleService.createRole(newRole.name, newRole.permissions).subscribe(
      (response) => {
        console.log(response);
        if (response.statusCode == 500) {
          Swal.fire("Error!", "Failed to create role. Try again!", "error");
        }
        else if (response.statusCode == 409) {
          Swal.fire("Error!", "Role already exist!", "error");
        }
        else {
          Swal.fire({
            position: "center",
            icon: "success",
            title: `Role '${this.roleName}' created successfully!`,
            showConfirmButton: false,
            timer: 2000
          });
        }
        this.loading = true;
        this.router.navigate(['/roles']);
      },
      (error) => {
        this.loading = false;
        Swal.fire("Error!", "Failed to create role. Try again!", "error");
      }
    );
  }
}
