import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { Transaction } from '../Interface/Transaction';

@Injectable({
  providedIn: 'root'
})
export class TransactionService {
  private baseUrl = "https://localhost:7126/api";
  private http = inject(HttpClient);


  insertIntoTransaction(transaction: Partial<Transaction>): Observable<Transaction> {
    return this.http.post<Transaction>(`${this.baseUrl}/Transaction`, transaction);
  }

  getTransactionByStudentId(sId: number): Observable<Transaction[]> {
    return this.http.get<Transaction[]>(`${this.baseUrl}/Transaction/student/${sId}`);
  }

  getTransactionByCourseId(cId: number): Observable<Transaction[]> {
    return this.http.get<Transaction[]>(`${this.baseUrl}/Transaction/course/${cId}`);
  }
}
