import { CanActivate, CanActivateFn, Router } from '@angular/router';
import { LoginService } from '../api/login-service';
import { Injectable } from '@angular/core';

@Injectable({ providedIn: 'root' })
export class authGuard implements CanActivate {
  constructor(private auth: LoginService, private router: Router) {}

  canActivate(): boolean {
    if (this.auth.isLoggedIn()) return true;
    console.log('Token:', this.auth.getToken());
    this.router.navigate(['/login']);
    return false;
  }
  
}
