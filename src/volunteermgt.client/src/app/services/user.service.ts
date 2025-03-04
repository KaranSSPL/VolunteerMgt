import { HttpClient, HttpResponse } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class UserService {

  private apiUrl = 'https://localhost:7048/api/authentication/login';

  constructor(private http: HttpClient, private router: Router) { }

  login(credentials: { email: string; password: string; }): Observable<HttpResponse<any>> {
    return this.http.post<any>(`${this.apiUrl}`, credentials, { observe: 'response' });
  }

  logout(): void {
    sessionStorage.removeItem('authToken');
    this.router.navigate(['/login']);
  }

  isLoggedIn(): boolean {
    return !!sessionStorage.getItem('authToken');
  }
}
