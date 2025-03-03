import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class UserService {
  private getVolunteers = '/api/get-volunteers'; 
  private getVunteer = '/api/get-volunteers'; // Replace with your actual API URL

  constructor(private http: HttpClient) { }

  getUsers(): Observable<any> {
    return this.http.get<any>(this.getVolunteers);
  }

  getVolunteerById(id: string): Observable<any> {
    return this.http.get<any>(`https://localhost:7048/api/get-volunteer?Id=${id}`);
  }

}
