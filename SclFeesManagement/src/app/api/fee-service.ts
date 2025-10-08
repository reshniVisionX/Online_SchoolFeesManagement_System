import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { FeePaymentDTO } from '../Interface/FeePayDTO';  
import { FeePayments } from '../Interface/FeePayments';

@Injectable({
  providedIn: 'root'
})
export class FeeService {
  private baseUrl = "https://localhost:7126/api";
  private http = inject(HttpClient);

  getFeeDetailsByCourse(courseId: number): Observable<FeePaymentDTO[]> {
    return this.http.get<FeePaymentDTO[]>(`${this.baseUrl}/Payment/course/${courseId}`);
  }

  getAllPayments(): Observable<FeePayments[]> {
    return this.http.get<FeePayments[]>(`${this.baseUrl}/Payment`);
  }

  getPaymentByStudentId(sId: number): Observable<FeePayments> {
    return this.http.get<FeePayments>(`${this.baseUrl}/Payment/studId/${sId}`);
  }

}
