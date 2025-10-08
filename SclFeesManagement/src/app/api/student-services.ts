import { inject, Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { map, Observable } from 'rxjs';
import { Student } from '../Interface/Student';
import { UpdateStudents } from '../Interface/UpdateStudents';
@Injectable({
  providedIn: 'root'
})
export class StudentServices {
    private baseUrl = "https://localhost:7126/api";
  private http = inject(HttpClient);

    getStudentsByCourse(courseId: number): Observable<Student[]> {
    return this.http.get<Student[]>(`${this.baseUrl}/Student/course/${courseId}`);
  }
  getStudentDetailsById(id: number): Observable<Student> {
    return this.http.get<Student>(`${this.baseUrl}/Student/${id}`);
  }

  getAllStudents(): Observable<Student[]> {
    return this.http.get<Student[]>(`${this.baseUrl}/Student`);
  }

updateStudent(studentId: number, dto: UpdateStudents) {
  return this.http.patch<boolean>(`${this.baseUrl}/Student/${studentId}`, dto);
}

  // Filter students dynamically
  filterStudents(filters: {
    name?: string;
    aid?: string;
    isActive?: boolean | null;
    category?: number | null;
    bloodGroup?: string | null;
    dob?: string | null;
  }): Observable<Student[]> {
    let params = new HttpParams();
    if (filters.name) params = params.set('name', filters.name);
    if (filters.aid) params = params.set('aid', filters.aid);
    if (filters.isActive !== null && filters.isActive !== undefined)
      params = params.set('isActive', filters.isActive.toString());
    if (filters.category !== null && filters.category !== undefined)
      params = params.set('category', filters.category.toString());
    if (filters.bloodGroup) params = params.set('bloodGroup', filters.bloodGroup);
    if (filters.dob) params = params.set('dob', filters.dob);

    return this.http.get<Student[]>(`${this.baseUrl}/Student/filter`, { params });
  }
}
