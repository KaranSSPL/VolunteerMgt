import { HttpClient, HttpResponse } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class UserService {

  private apiUrl = '/api/authentication'; 

  constructor(private http: HttpClient, private router: Router) { }

  login(credentials: { email: string; password: string; }): Observable<HttpResponse<any>> {
    return this.http.post<any>(`${this.apiUrl}/login`, credentials, { observe: 'response' });
  }

  register(user: any): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/register`, user);
  }

  logout(): void {
    sessionStorage.removeItem('authToken');
    this.router.navigate(['/login']);
  }

  isLoggedIn(): boolean {
    return !!sessionStorage.getItem('authToken');
  }
}
