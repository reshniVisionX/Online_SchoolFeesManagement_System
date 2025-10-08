import { Component, inject, signal } from '@angular/core';
import { CourseService } from '../../api/course-service';
import {Course} from '../../Interface/Course';
import { ActivatedRoute } from '@angular/router';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-update-course',
  imports: [CommonModule,FormsModule],
  templateUrl: './update-course.html',
  styleUrl: './update-course.css'
})
export class UpdateCourse {

  
  private courseService = inject(CourseService);
  private route = inject(ActivatedRoute);
course = signal<Course | null>(null);

  courseId!: number;

  ngOnInit(): void {
    // get courseId from route param
    this.courseId = Number(this.route.snapshot.paramMap.get('id'));
console.log("The cur courseId is ",this.courseId);
    // call service and update signal
    this.courseService.getCourseById(this.courseId).subscribe({
      next: (data) => this.course.set(data),
      error: (err) => console.error('Error fetching course:', err)
    });
  }

 onSubmit(): void {
  if (!this.course()) {
    this.showToast("No course data to update", 'warn');
    return;
  }

  this.courseService.updateCourse(this.courseId, this.course()!).subscribe({
    next: (ok) => {
      if (ok) {
        this.showToast("Course updated successfully!", 'success');
      } else {
        this.showToast("Update failed!", 'error');
      }
    },
    error: (err) => {
      console.error('Error updating course:', err);
      this.showToast("Server error while updating course", 'error');
    }
  });
}
  showToast(message: string, type: 'success' | 'error' | 'warn') {
    const toast = document.createElement('div');
    toast.textContent = message;
    toast.className = `custom-toast toast-${type}`;
    document.body.appendChild(toast);

    // Show in center of screen
    toast.style.position = 'fixed';
    toast.style.top = '50%';
    toast.style.left = '50%';
    toast.style.transform = 'translate(-50%, -50%)';
    toast.style.padding = '15px 25px';
    toast.style.borderRadius = '8px';
    toast.style.color = 'white';
    toast.style.fontWeight = 'bold';
    toast.style.fontSize = '16px';
    toast.style.zIndex = '10000';
    toast.style.textAlign = 'center';
    toast.style.boxShadow = '2px 4px 8px rgba(124, 122, 122, 0.93)';

    switch(type) {
      case 'success': toast.style.backgroundColor = '#73d176ff'; break;
      case 'error': toast.style.backgroundColor = '#fc9550ff'; break;
      case 'warn': toast.style.backgroundColor = '#def227ff'; break;
    }

    setTimeout(() => {
      toast.remove();
    }, 5000); 
  }

}
