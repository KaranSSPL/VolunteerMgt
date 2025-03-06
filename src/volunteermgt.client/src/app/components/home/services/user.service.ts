import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class UserService {

  private endpoint = '/api/volunteer/get-volunteers';
  private roleEndpoint = '/api/role'; 
  constructor(private http: HttpClient) { }

  getUsers(): Observable<any> {
    return this.http.get<any>(this.endpoint).pipe(
      catchError(error => {
        console.error('Error fetching users:', error);
        return throwError(() => new Error('Failed to fetch users.'));
      })
    );
  }

  getVolunteerById(id: string): Observable<any> {
    return this.http.get<any>(`${this.endpoint}/${id}`).pipe(
      catchError(error => {
        console.error('Error fetching volunteer details:', error);
        return throwError(() => new Error('Failed to fetch volunteer details.'));
      })
    );
  }

  getRoles(): Observable<any> {
    return this.http.get<any>(`${this.roleEndpoint}/get-roles`).pipe(
      catchError(error => {
        console.error('Error fetching roles:', error);
        return throwError(() => new Error('Failed to fetch roles.'));
      })
    );
  }

  assignRole(userId: string, roleIds: string[]): Observable<any> {
    const data = { userId, roleIds };
    return this.http.post<any>(`${this.roleEndpoint}/assign-user-roles`,data).pipe(
      catchError(error => {
        console.error('Error fetching users:', error);
        return throwError(() => new Error('Failed to fetch users.'));
      })
    );
  }
}
