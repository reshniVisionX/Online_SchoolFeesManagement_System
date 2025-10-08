import { Component ,inject,OnInit, signal} from '@angular/core';
import {SchoolService} from '../../api/school-service';
import {School} from '../../Interface/School';
import { CommonModule } from '@angular/common';
import { LoginService } from '../../api/login-service';
@Component({
  selector: 'app-navbar',
  imports: [CommonModule],
  templateUrl: './navbar.html',
  styleUrl: './navbar.css'
})

export class Navbar {

  private schSer = inject(SchoolService);
  private loginService = inject(LoginService);
   school = signal<School | null>(null);

  ngOnInit(): void {
   this.FetchSchoolInfo();
   this.preference();
  }
  
FetchSchoolInfo(){
      this.schSer.getSchoolInfo().subscribe(data => {
      console.log('school data', data);
     
      if (Array.isArray(data) && data.length > 0) {
        this.school.set(data[0]);   
      } else {
        this.school.set(null);
      }
    });
}
  theme: string = 'light';
preference(){
   const savedTheme = localStorage.getItem('Feetheme');
  if (savedTheme) {
    this.theme = savedTheme;
    document.body.classList.toggle('dark-theme', this.theme === 'dark');
  }
}

toggleTheme(event: Event) {
  event.preventDefault(); // Prevent page reload
  this.theme = this.theme === 'light' ? 'dark' : 'light';

  // Apply the class
  document.body.classList.toggle('dark-theme', this.theme === 'dark');

  // Save preference
  localStorage.setItem('Feetheme', this.theme);
}
logoutUser(){
  this.loginService.logout();
}
}
