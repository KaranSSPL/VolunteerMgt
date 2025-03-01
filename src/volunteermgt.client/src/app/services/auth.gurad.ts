import { Injectable } from '@angular/core';
import { CanActivate, Router, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';

@Injectable({
  providedIn: 'root'
})
export class AuthGuard implements CanActivate {
  constructor(private router: Router) { }

  canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): boolean {
    const isLoggedIn = !!sessionStorage.getItem('token');

    if (state.url === '/login' && isLoggedIn) {
      this.router.navigate(['/dashboard']);
      return false;
    }
    else if (state.url === '/dashboard' && !isLoggedIn) {
      this.router.navigate(['/login']);
      return false;
    }
    return true;
  }
}
