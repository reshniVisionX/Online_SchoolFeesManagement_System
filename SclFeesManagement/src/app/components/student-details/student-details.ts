import { CommonModule } from '@angular/common';
import { Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { StudentDetail } from '../../Interface/StudentDetails';
import { ActivatedRoute, Router } from '@angular/router';
import { StudentServices } from '../../api/student-services';
import {Student} from '../../Interface/Student';
import { UpdateStudents } from '../../Interface/UpdateStudents';


@Component({
  selector: 'app-student-details',
  imports: [CommonModule,FormsModule],
  templateUrl: './student-details.html',
  styleUrl: './student-details.css'
})
export class StudentDetails {
   private studentService = inject(StudentServices);
   private router: Router = inject(Router);
    students = signal<Student | null>(null); 
   private route = inject(ActivatedRoute);
  sId!: number;

     ngOnInit(): void {
      const sId = Number(this.route.snapshot.paramMap.get('id'));
    if (sId) {
      // Call service to fetch a single student
      this.studentService.getStudentDetailsById(sId).subscribe({
        next: (data: Student) => {this.students.set(data); console.log("Fetched student: ",data);}, // set single student
        error: (err) => console.error('Error fetching student', err)
      });
     }
     }

onSubmit(): void {
  if (!this.students()) {
    this.showToast("No data to upload", 'warn');
    return;
  }

  const id = this.students()!.sId;

  const payload: UpdateStudents = {
    sName: this.students()!.sName,
    courseId: this.students()!.courseId,
    dob: this.students()!.dob,
    bloodGrp: this.students()!.bloodGrp,
    parAddress: this.students()!.parAddress,
    parPhone: this.students()!.parPhone,
    parEmail: this.students()!.parEmail,
    category: this.students()!.studentDetails?.category,
    isSports: this.students()!.studentDetails?.isSports,
    isMerit: this.students()!.studentDetails?.isMerit,
    isFG: this.students()!.studentDetails?.isFG,
    isWaiver: this.students()!.studentDetails?.isWaiver,
    isActive: this.students()!.studentDetails?.isActive
  };

  this.studentService.updateStudent(id, payload).subscribe({
    next: (ok) => {
      if (ok) {
        this.showToast("Student updated successfully!", 'success');
      } else {
        this.showToast("Update failed.", 'error');
      }
    },
    error: (err) => {
      console.error('Error updating student', err);
      this.showToast("Server error while updating", 'error');
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
