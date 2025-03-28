import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { UserService } from '../../../services/user/user.service';
import Swal from 'sweetalert2';

@Component({
  selector: 'app-home',
  standalone: false,
  templateUrl: './home.component.html',
  styleUrl: './home.component.css'
})
export class HomeComponent implements OnInit {
  users: any[] = [];

  constructor(private userService: UserService) { }
  ngOnInit() {
    this.loadVolunteers();
  }

  loadVolunteers() {
    this.userService.getVolunteers().subscribe({
      next: (response) => {
        this.users = response.payload;
      },
      error: (error) => {
        console.error('Error fetching users:', error);
        Swal.fire('Error!', 'Failed to fetch users.', 'error');
      }
    });
  }
}








