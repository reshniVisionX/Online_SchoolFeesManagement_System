import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable, tap } from 'rxjs';
import { Login } from '../Interface/Login';

@Injectable({
  providedIn: 'root'
})
export class LoginService {
  private baseUrl = "https://localhost:7126/api";
  private http = inject(HttpClient);

  login(credentials: Login): Observable<any> {
    return this.http.post<any>(`${this.baseUrl}/User/login`, credentials).pipe(
      tap(response => {
        if (response && response.token) {
          // Store token
          localStorage.setItem('jwt_token', response.token);

          // Store user details
          const userDetails = {
            username: response.username,
            admissionId: response.admissionId,
            userId: response.userId,
            role: response.role
          };
          localStorage.setItem('user_details', JSON.stringify(userDetails));
        }
      })
    );
  }
getUserName(): string | null {
  const userDetails = localStorage.getItem('user_details');
  return userDetails ? JSON.parse(userDetails).username : null;
}
getUserId():number | null{
  const userDetails = localStorage.getItem('user_details');
  return userDetails ? JSON.parse(userDetails).userId : null;
}

  logout(): void {
  localStorage.removeItem('jwt_token');
  localStorage.removeItem('user_details');
}

getToken(): string | null {
  return localStorage.getItem('jwt_token');
}

getCurrentUser(): any | null {
  const v = localStorage.getItem('user_details');
  return v ? JSON.parse(v) : null;
}

isLoggedIn(): boolean {
  return !!this.getToken();
}

getCurrentUserRole(): string | null {
  const user = this.getCurrentUser();
  return user ? user.role || null : null;  
}

isAdmin(): boolean {
  const user = this.getCurrentUser();
  return user?.role === 'Admin';
}

isStudent(): boolean {
  const user = this.getCurrentUser();
  return user?.role === 'Student';
}

}
