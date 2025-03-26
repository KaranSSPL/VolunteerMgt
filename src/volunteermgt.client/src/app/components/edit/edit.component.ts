import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { ActivatedRoute } from '@angular/router';
import Swal from 'sweetalert2';

@Component({
  selector: 'app-edit-profile',
  standalone: false,
  templateUrl: './edit.component.html',
  styleUrls: ['./edit.component.css'],
})
export class EditComponent implements OnInit {
  loading = false;
  confirmPassword = '';

  volunteer = {
    id : '',
    email: '',
    username: '',
    phoneNumber: '',
    role: '',
  };

  constructor(private route: ActivatedRoute, private http: HttpClient) { }

  ngOnInit() {
      const id = this.route.snapshot.paramMap.get('id'); 
      if (id) {
        this.volunteer.id = id;
        this.getUserDetails(id);
      }
  } 

  getUserDetails(id:string) {
    this.loading = true;
    const url = `/api/volunteers/${id}`;  

    this.http.get(url).subscribe({
      next: (data: any) => {
        if (data && data.payload) {
          console.log(data);
          this.volunteer = data.payload;  
        } else {
          this.volunteer = data;  
        }
        this.loading = false;
      },
      error: (error) => {
        console.error('Error fetching user details:', error);
        Swal.fire("Error!", "Failed to load user details.", "error");
        this.loading = false;
      }
    });
  }

  onUpdate() {
    this.loading = true;

    this.http.put('/api/volunteers/', this.volunteer).subscribe({
      next: (response: any) => {
        Swal.fire({ position: "center", icon: "info", title: "Profile updated successfully.", showConfirmButton: false, timer: 1500 });
        this.loading = false;
      },
      error: (error) => {
        console.error('Update failed:', error);
        Swal.fire("Update Failed!", error.message || "Something went wrong.", "error");
        this.loading = false;
      }
    });
  }
}
