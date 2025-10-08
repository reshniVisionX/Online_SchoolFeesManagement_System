import { Component, inject, signal } from '@angular/core';
import { StudentServices } from '../../api/student-services';
import {Student} from '../../Interface/Student';
import { ActivatedRoute, Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-students',
  imports: [CommonModule,FormsModule],
  templateUrl: './students.html',
  styleUrl: './students.css'
})
export class Students {
 students = signal<Student[]>([]);
allStudents = signal<Student[]>([]);
  // Inject services
  private studentService = inject(StudentServices);
  private route = inject(ActivatedRoute);
  
   private router: Router = inject(Router);

  searchName: string = '';
  searchAid: string = '';
  selectedActive: boolean | null = null;
  selectedCategory: number | null = null;
  selectedBloodGroup: string | null = null;
  selectedDob: string | null = null;

couId = signal<number | null>(null);

  ngOnInit(): void {
    const id = Number(this.route.snapshot.paramMap.get('id'));
    if (id) {
      this.couId.set(id);   // store courseId once
      this.loadStudentsByCourse(id);
    }
  }

  loadStudentsByCourse(courseId: number) {
    this.studentService.getStudentsByCourse(courseId).subscribe({
      next: (data: Student[]) => {
        this.allStudents.set(data);   // store original list
        this.students.set(data);      // initialize rendered list
      },
      error: (err) => console.error('Error fetching students', err)
    });
  }

insertStudents(){
  console.log(this.couId());
   this.router.navigate(['/addStudents', this.couId()]);
}

onSearchInput() {
  const filtered = this.allStudents().filter(s => {
    let match = true;

    if (this.searchName?.trim()) {
      match = match && s.sName.toLowerCase().includes(this.searchName.toLowerCase());
    }

    if (this.searchAid?.trim()) {
      match = match && s.admissionId.toLowerCase().includes(this.searchAid.toLowerCase());
    }

    if (this.selectedActive !== null) {
      match = match && s.studentDetails?.isActive === this.selectedActive;
    }

    if (this.selectedCategory !== null) {
      match = match && s.studentDetails?.category === this.selectedCategory;
    }

    if (this.selectedBloodGroup?.trim()) {
      match = match && s.bloodGrp?.toLowerCase() === this.selectedBloodGroup.toLowerCase();
    }

    if (this.selectedDob) {
      match = match && s.dob.startsWith(this.selectedDob);
    }

    return match;
  });

  this.students.set(filtered);
}

  viewFeeDetails(student: Student) {
    // You can implement routing or modal popup for fee details
    console.log('Show fee details for', student.sName);
  }
  fetchStudentDetails(sid:number){
    this.router.navigate(['/studentDetails', sid]);
  }

 getFeeDetails() {
    const id = this.couId();  
    if (id) {
      this.router.navigate(['/feeDetailsCourse', id]);
    } else {
      console.error('CourseId not found!');
    }
 }

}
