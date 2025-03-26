import { Component, OnInit } from '@angular/core';
import { RoleService } from './services/roles.service'; // Import the service
import Swal from 'sweetalert2';

@Component({
  selector: 'app-roles',
  standalone: false,
  templateUrl: './roles.component.html',
  styleUrl: './roles.component.css'
})
export class RolesComponent implements OnInit {
  roles: any[] = [];
  permissions: any[] = [];
  loading = false;

  constructor(private roleService: RoleService) { } // Inject the service

  ngOnInit() {
    this.getRoles();
    this.getPermissions();
  }

  getRoles() {
    this.loading = true;
    this.roleService.getRoles().subscribe({
      next: (response: any) => {
        this.roles = response.payload.map((role: any) => ({
          ...role,
          permissions: role.permissions || []
        }));
        this.loading = false;
      },
      error: (error) => {
        console.error('Error fetching roles:', error);
        Swal.fire('Error!', 'Failed to fetch roles.', 'error');
        this.loading = false;
      }
    });
  }

  getPermissions() {
    this.roleService.getPermissions().subscribe({
      next: (response: any) => {
        this.permissions = response.payload;
      },
      error: (error) => {
        console.error('Error fetching permissions:', error);
      }
    });
  }

  getPermissionRoles(roleId: string) {
    this.roleService.getPermissionRoles(roleId).subscribe({
      next: (response: any) => {
        const role = this.roles.find((r) => r.id === roleId);
        if (role) {
          role.permissions = response.payload || [];
        }
      },
      error: (error) => {
        console.error(`Error fetching permissions for role ${roleId}:`, error);
        Swal.fire('Error!', 'Failed to fetch permissions.', 'error');
      }
    });
  }

  getPermissionNames(role: any): string {
    return role.permissions.length > 0 ? role.permissions.map((p: any) => p.name).join(', ') : 'No Permissions';
  }

  assignPermission(roleId: string) {
    if (this.permissions.length === 0) {
      Swal.fire('Error!', 'No permissions available to assign.', 'error');
      return;
    }

    const permissionOptions = this.permissions.reduce((acc: any, perm) => {
      acc[perm.id] = perm.name;
      return acc;
    }, {});

    Swal.fire({
      title: 'Assign Permission',
      input: 'select',
      inputOptions: permissionOptions,
      inputPlaceholder: 'Select a permission',
      showCancelButton: true,
      confirmButtonText: 'Assign',
      cancelButtonText: 'Cancel',
      preConfirm: (selectedPermissionId) => {
        if (!selectedPermissionId) {
          Swal.showValidationMessage('Please select a permission.');
        }
        return selectedPermissionId;
      }
    }).then((result) => {
      if (result.isConfirmed) {
        this.assignPermissionToRole(roleId, result.value);
      }
    });
  }

  removePermission(roleId: string) {
    if (this.permissions.length === 0) {
      Swal.fire('Error!', 'No permissions available to assign.', 'error');
      return;
    }
    const role = this.roles.find(role => role.id === roleId);

    if (!role) {
      console.error("Role not found!");
      return;
    }

    const permissionOptions = role.permissions.reduce((acc: any, perm:any ) => {
      acc[perm.id] = perm.name;
      return acc;
    }, {});

    Swal.fire({
      title: 'Remove Permission',
      input: 'select',
      inputOptions: permissionOptions,
      inputPlaceholder: 'Select a permission',
      showCancelButton: true,
      confirmButtonText: 'Remove',
      cancelButtonText: 'Cancel',
      preConfirm: (selectedPermissionId) => {
        if (!selectedPermissionId) {
          Swal.showValidationMessage('Please select a permission.');
        }
        return selectedPermissionId;
      }
    }).then((result) => {
      if (result.isConfirmed) {
        this.removePermissionToRole(roleId, result.value);
      }
    });
  }

  deleteRole(id: string) {
    Swal.fire({
      title: "Are you sure?",
      text: "This action cannot be undone!",
      icon: "warning",
      showCancelButton: true,
      confirmButtonText: "Yes, delete it!",
      cancelButtonText: "No, cancel",
      reverseButtons: true
    }).then((result) => {
      if (result.isConfirmed) {
        this.roleService.deleteRole(id).subscribe({
          next: ( response ) => {
            if (response.statusCode === 400) {
              Swal.fire('Delete Failed!', "Internal server error.", "error");
            }
            else (response.statusCode !== 400)
            {
              Swal.fire("Success!", "Successfully Deleted.", "success");
            }
            debugger
            this.ngOnInit();
          },
          error: (error) => {
            console.error("Error deleting role:", error);
            Swal.fire("Error!", "Failed to delete role.", "error");
          }
        });
      }
    });
  }

  assignPermissionToRole(roleId: string, permissionId: string) {
    this.roleService.assignPermissionToRole(roleId, permissionId).subscribe({
      next: (response) => {
        if (response.statusCode == 400) {
          Swal.fire("Failed!", "Failed to assign permission.", "error");
        }
        else if (response.statusCode == 409) {
          Swal.fire("Failed!", "Already assigned permission.", "error");
        } else {
          Swal.fire('Success!', 'Permission assigned successfully.', 'success');
          /*this.getPermissionRoles(roleId);*/
          this.ngOnInit();
        }
      },
      error: (error) => {
        console.error('Error assigning permission:', error);
        Swal.fire('Error!', 'Failed to assign permission.', 'error');
      }
    });
  }

  removePermissionToRole(roleId: string, permissionId: string) {
    this.roleService.removePermissionToRole(roleId, permissionId).subscribe({
      next: (response) => {
        if (response.statusCode == 400) {
          Swal.fire("Failed!", "Failed to remove permission.", "error");
        }
        else if (response.statusCode == 409) {
          Swal.fire("Failed!", "Already remove permission.", "error");
        } else {
          Swal.fire({ position: "center", icon: "success", title: "Permission remove successfully.", showConfirmButton: false, timer: 1500 });
          /*this.getPermissionRoles(roleId);*/
          this.ngOnInit();
        }
      },
      error: (error) => {
        console.error('Error remove permission:', error);
        Swal.fire('Error!', 'Failed to remove permission.', 'error');
      }
    });
  }
}
