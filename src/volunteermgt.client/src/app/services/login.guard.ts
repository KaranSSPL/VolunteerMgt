import { Injectable } from '@angular/core';
import { CanActivate, Router } from '@angular/router';
import { UserService } from './user.service';

@Injectable({
  providedIn: 'root'
})
export class LoginGuard implements CanActivate {

  constructor(private userService: UserService, private router: Router) { }

  canActivate(): boolean {
    if (this.userService.isLoggedIn()) {
      this.router.navigate(['/dashboard']); 
      return false; 
    }
    return true;
  }
}
