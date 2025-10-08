import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, CanActivate, Router } from '@angular/router';
import { LoginService } from '../api/login-service';

@Injectable({ providedIn: 'root' })

export class RoleGuard implements CanActivate {
  constructor(private auth: LoginService, private router: Router) {}

  canActivate(route: ActivatedRouteSnapshot): boolean {
    const expectedRoles: string[] = route.data['roles'];
    const user = this.auth.getCurrentUser(); // gets user_details from localStorage

    // If user not logged in or roles not defined, redirect to login
    if (!user || !user.role || !expectedRoles || expectedRoles.length === 0) {
      this.router.navigate(['/login']);
      return false;
    }

    console.log('User Role:', user.role);
    console.log('Expected Roles:', expectedRoles);

    // Check if user's role matches any of the expected roles
    if (expectedRoles.includes(user.role)) {
      return true;
    }

    // Not allowed → redirect to home
    console.log('Access denied for user:', user);
    this.router.navigate(['/']);
    return false;
  }
}
