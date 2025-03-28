import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable,timeout  } from 'rxjs';
import { jwtDecode } from 'jwt-decode';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private apiUrl = '/api/authentication';
  private role: string | null = null;
  constructor(private http: HttpClient) {
    this.loadUserRole();
}

  login(email: string, password: string): Observable<any> {
    const body = { email, password };
    return this.http.post<any>(this.apiUrl, body).pipe(timeout(10000));
  }

  decodeToken(token: string): any {
    try {
      return jwtDecode(token);
    } catch (error) {
      console.error('Invalid token', error);
      return null;
    }
  }

  saveToken(token: string): void {
    sessionStorage.setItem('authToken', token);
    this.loadUserRole();
  }

  private loadUserRole(): void {
    const token = sessionStorage.getItem('authToken');
    if (token) {
      const decodedToken: any = this.decodeToken(token);
      this.role = decodedToken?.['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'] || null;
    }
  }
  getRole(): string | null {
    return this.role;
  }
  logout(): void {
    sessionStorage.removeItem('authToken');
    this.role = null;
  }
}
