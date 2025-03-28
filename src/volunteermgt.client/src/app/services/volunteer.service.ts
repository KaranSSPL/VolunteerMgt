import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Volunteer } from '../Models/volunteer.model';
import { Service } from '../Models/voluteerService.model';

@Injectable({
  providedIn: 'root',
})
export class VolunteerService {
  private apiUrl = `/api`;

  constructor(private http: HttpClient) { }

  getVolunteers(): Observable<Volunteer[]> {
    return this.http.get<Volunteer[]>(`${this.apiUrl}/volunteers/all`);
  }

  getVolunteerById(id: number): Observable<Volunteer> {
    return this.http.get<Volunteer>(`${this.apiUrl}/volunteers/${id}`);
  }

  addVolunteer(volunteerData: FormData): Observable<any> {
  return this.http.post(`${this.apiUrl}/volunteers/add`, volunteerData);
  }

  updateVolunteer(id: number, volunteer: FormData): Observable<Volunteer> {
    return this.http.put<Volunteer>(`${this.apiUrl}/volunteers/update/${id}`, volunteer);
  }

  deleteVolunteer(id: number): Observable<boolean> {
    return this.http.delete<boolean>(`${this.apiUrl}/volunteers/delete/${id}`);
  }

  //Service

  getAllServices(): Observable<Service[]> {
    return this.http.get<Service[]>(`${this.apiUrl}/services/allService`);
  }

  addService(service: Service): Observable<Service> {
    return this.http.post<Service>(`${this.apiUrl}/services/add`, service);
  }

  updateService(id: number, service: Service): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/services/update/${id}`, service);
  }

  deleteService(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/services/delete/${id}`);
  }

  //Volunteer Service

  getVolunteerServiceMappings(): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/volunteer-service/volunteer-service-mappings`);
  }

  assignService(data: any): Observable<any> {
    return this.http.post(`${this.apiUrl}/volunteer-service/assign`, data);
  }

  getServiceVolunteerById(volunteerId: number): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/volunteer-service/volunteer/${ volunteerId }/services`);
  }

  getVolunteerServiceById(Id: number): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/volunteer-service/volunteer-service-mappings/${Id}`);
  }

  deleteVolunteerService(volunteerId: number, serviceId: number): Observable<any> {
    return this.http.delete(`${this.apiUrl}/volunteer-service/volunteer/${volunteerId}/service/${serviceId}`);
  }

  deleteVolunteerandService(volunteerId: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/volunteer-service/volunteer/${volunteerId}`);
  }

  getServiceVolunteerCounts(): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/volunteer-service/service-volunteer-counts`);
  }
}
