import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Volunteer } from '../Models/volunteer.model';

@Injectable({
  providedIn: 'root',
})
export class VolunteerService {
  private apiUrl = 'https://localhost:7048/api/volunteers/';

  constructor(private http: HttpClient) { }

  getVolunteers(): Observable<Volunteer[]> {
    return this.http.get<Volunteer[]>(`${this.apiUrl}all`);
  }

  getVolunteerById(id: number): Observable<Volunteer> {
    return this.http.get<Volunteer>(`${this.apiUrl}${id}`);
  }

  addVolunteer(volunteer: Volunteer): Observable<Volunteer> {
    return this.http.post<Volunteer>(`${this.apiUrl}add`, volunteer);
  }

  updateVolunteer(id: number, volunteer: Volunteer): Observable<Volunteer> {
    return this.http.put<Volunteer>(`${this.apiUrl}update/${id}`, volunteer);
  }

  deleteVolunteer(id: number): Observable<boolean> {
    return this.http.delete<boolean>(`${this.apiUrl}delete/${id}`);
  }
}
