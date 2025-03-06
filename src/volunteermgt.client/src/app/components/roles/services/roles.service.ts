import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root' 
})
export class RoleService {
  private readonly roleUrl = '/api/role';
  private readonly permissionUrl = '/api/permission';

  constructor(private http: HttpClient) { }

  // Fetch all roles
  getRoles(): Observable<any> {
    return this.http.get(`${this.roleUrl}/get-roles`);
  }

  // Fetch all permissions
  getPermissions(): Observable<any> {
    return this.http.get(`${this.permissionUrl}/get-permissions`);
  }

  // Fetch permissions for a specific role
  getPermissionRoles(roleId: string): Observable<any> {
    return this.http.get(`${this.roleUrl}/get-permission-roles/${roleId}`);
  }

  // Assign a permission to a role
  assignPermissionToRole(roleId: string, permissionId: string): Observable<any> {
    return this.http.post(`${this.permissionUrl}/add-permission-roles`, { roleId, permissionId });
  }
}
