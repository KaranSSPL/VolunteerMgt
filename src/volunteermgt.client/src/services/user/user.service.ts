import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class UserService {

  private endpoint = '/api/users';
  private roleEndpoint = '/api/roles'; 
  constructor(private http: HttpClient) { }

  getUsers(): Observable<any> {
    return this.http.get<any>(this.endpoint).pipe(
      catchError(error => {
        console.error('Error fetching users:', error);
        return throwError(() => new Error('Failed to fetch users.'));
      })
    );
  }

  getUserById(id: string): Observable<any> {
    return this.http.get<any>(`${this.endpoint}/${id}`).pipe(
      catchError(error => {
        console.error('Error fetching user details:', error);
        return throwError(() => new Error('Failed to fetch user details.'));
      })
    );
  }

  getRoles(): Observable<any> {
    return this.http.get<any>(`${this.roleEndpoint}`).pipe(
      catchError(error => {
        console.error('Error fetching roles:', error);
        return throwError(() => new Error('Failed to fetch roles.'));
      })
    );
  }

  assignRole(userId: string, roleId: string): Observable<any> {
    const data = { userId, roleId };
    return this.http.patch<any>(`${this.endpoint}/assign-role`,data).pipe(
      catchError(error => {
        console.error('Error fetching users:', error);
        return throwError(() => new Error('Failed to fetch users.'));
      })
    );
  }
}
