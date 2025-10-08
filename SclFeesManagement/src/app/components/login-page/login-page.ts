import { Component, inject } from '@angular/core';
import { Router } from '@angular/router';
import { Login } from '../../Interface/Login';
import {LoginService} from '../../api/login-service';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-login-page',
  imports: [CommonModule,FormsModule],
  templateUrl: './login-page.html',
  styleUrl: './login-page.css'
})
export class LoginPage {
 loginData: Login = {
    admissionId: '',
    password: ''
  };

  errorMessage: string = ''
  private loginService = inject(LoginService);
  private router = inject(Router);

  onLogin(): void {
    this.loginService.login(this.loginData).subscribe({
      next: () => {
        if(this.loginService.isLoggedIn())
           this.router.navigate(['/home']);
        else
          this.router.navigate(['/login']);
      },
      error: () => {
        this.errorMessage = 'Invalid Admission ID or Password';
      }
    });
  }

  showPassword = false;

togglePassword() {
  this.showPassword = !this.showPassword;
}

}
