import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators, FormArray } from '@angular/forms';
import Swal from 'sweetalert2';
import { RoleService } from '../../../../services/role/roles.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-add-role',
  standalone: false,
  templateUrl: './add-role.component.html',
  styleUrl: './add-role.component.css'
})
export class AddRoleComponent implements OnInit {
  addRole!: FormGroup;
  permissions: any[] = [];
  loading: boolean = false;
  constructor(
    private roleService: RoleService,
    private router: Router,
    private formBuilder: FormBuilder
  ) { }

  ngOnInit() {
    this.addRole = this.formBuilder.group({
      roleName: ['', Validators.required],
      permissions: this.formBuilder.array([])
    });

    // Fetch permissions
    this.fetchPermissions();
  }
  get permissionsControls() {
    return this.addRole.get('permissions') as FormArray;
  }

  fetchPermissions() {
    this.loading = true;
    this.roleService.getPermissions().subscribe(
      (response) => {
        if (response?.statusCode == 400) {
          Swal.fire("Fetching  Failed!", response.errors?.join("\n") || "Unknown error", "error");
        } else if (response?.payload && Array.isArray(response.payload)) {
          this.permissions = response.payload;

          this.permissionsControls.clear();

          this.permissions.forEach(() => {
            this.permissionsControls.push(this.formBuilder.control(false));
          });
        } else {
          console.warn("Unexpected response format:", response);
          Swal.fire("Error!", "Invalid response received.", "error");
        }
        this.loading = false;
      },
      (error) => {
        Swal.fire("Error!", "Failed to load permissions. Please try again.", "error");
        this.loading = false;
      }
    );
  }

  onSubmit() {

    if (this.addRole.invalid) {
      this.addRole.markAllAsTouched();
      return;
    }
    this.loading = true;
    const selectedPermissions = this.permissionsControls.value
      .map((checked: boolean, i: number) => (checked ? { id: this.permissions[i].id } : null))
      .filter((p: any) => p !== null);

    if (selectedPermissions.length === 0) {
      Swal.fire("Validation Error!", "At least one permission must be selected!", "warning");
      this.loading = false;
      return;
    }

    const newRole = {
      name: this.addRole.value.roleName,
      permissions: selectedPermissions
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
            title: `Role '${newRole.name}' created successfully!`,
            showConfirmButton: false,
            timer: 2000
          });
          this.router.navigate(['/roles']);
        }
        this.loading = false;
      },
      (error) => {
        this.loading = false;
        Swal.fire("Error!", "Failed to create role. Try again!", "error");
        this.loading = false;
      }
    );
  }
}
