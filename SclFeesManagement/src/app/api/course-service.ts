import { HttpClient, HttpParams } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import {Course} from '../Interface/Course';

@Injectable({
  providedIn: 'root'
})
export class CourseService {
    private baseUrl = "https://localhost:7126/api";
  private http = inject(HttpClient);

   getAllCourses(): Observable<Course[]> {
    return this.http.get<Course[]>(`${this.baseUrl}/Course`);
  }
  getCourseById(id: number): Observable<Course> {
    return this.http.get<Course>(`${this.baseUrl}/Course/Id/${id}`);
  }

    filterCourses(name: string, active: boolean): Observable<Course[]> {
    let params = new HttpParams().set('active', active.toString()); // always send true or false

    if (name) {
      params = params.set('name', name);
    }

    return this.http.get<Course[]>(`${this.baseUrl}/Course/filter`, { params });
  }

 updateCourse(id: number, course: Course): Observable<boolean> {
  return this.http.patch<boolean>(`${this.baseUrl}/Course/${id}`, course);
}


toggleCourseStatus(id: number): Observable<boolean> {
  return this.http.patch<boolean>(`${this.baseUrl}/Course/toggle-status/${id}`, null);
}



}
