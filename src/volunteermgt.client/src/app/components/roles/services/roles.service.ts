import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root' 
})
export class RoleService {
  private readonly roleUrl = '/api/roles';
  private readonly permissionUrl = '/api/permissions';

  constructor(private http: HttpClient) { }

  // Fetch all roles
  getRoles(): Observable<any> {
    return this.http.get(`${this.roleUrl}`);
  }

  // Fetch all permissions
  getPermissions(): Observable<any> {
    return this.http.get(`${this.permissionUrl}`);
  }

  // Fetch permissions for a specific role
  getPermissionRoles(roleId: string): Observable<any> {
    return this.http.get(`${this.roleUrl}/${roleId}`);
  }

  // Assign a permission to a role
  assignPermissionToRole(roleId: string, permissionId: string): Observable<any> {
    return this.http.post(`${this.roleUrl}/assign-permission`, { roleId, permissionId });
  }
}
