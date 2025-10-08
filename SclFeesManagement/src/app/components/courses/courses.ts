import { Component, inject, signal } from '@angular/core';
import {Course} from '../../Interface/Course';
import {CourseService} from '../../api/course-service';
import { CommonModule } from '@angular/common';
import { debounceTime, Subject, switchMap } from 'rxjs';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';

@Component({
  selector: 'app-courses',
  imports: [CommonModule,FormsModule],
  templateUrl: './courses.html',
  styleUrl: './courses.css'
})

export class Courses {
  private courseService = inject(CourseService);
  
  courses = signal<Course[]>([]);
  noCoursesMessage = signal<string>(''); 

  searchName: string = '';
  selectedActive: boolean = true;
   private router: Router = inject(Router);

  ngOnInit(): void {
    this.loadCourses();
  }

  loadCourses() {
    this.courseService.filterCourses(this.searchName, this.selectedActive)
      .subscribe({
        next: (data) => {
          this.courses.set(data);
          this.noCoursesMessage.set(data.length > 0 ? '' : 'No courses available');
        },
        error: (err) => {
          if (err.status === 404) {
            this.courses.set([]);
            this.noCoursesMessage.set('No courses available');
          } else {
            console.error(err);
            this.noCoursesMessage.set('Error loading courses');
          }
        }
      });
  }

  onSearchInput() {
    this.loadCourses();
  }

  fetchStudents(courseId : number){
      this.router.navigate(['/students', courseId]);
  }

  toggleStatus(courseId: number) {
  this.courseService.toggleCourseStatus(courseId).subscribe({
    next: (result: boolean) => {
      if (result) {
        console.log(`Course ${courseId} status toggled successfully.`);        
        this.loadCourses(); 
      } else {
        console.warn(`Failed to toggle status for Course ${courseId}.`);
      }
    },
    error: (err) => {
      console.error('Error toggling course status:', err);
    }
  });
}
updateCourse(course:number){
  this.router.navigate(['/update-course', course]);
}
}

