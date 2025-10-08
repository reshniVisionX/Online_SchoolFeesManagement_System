import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map, Observable } from 'rxjs';
import { School } from '../Interface/School';

@Injectable({
  providedIn: 'root'
})
export class SchoolService {
  private baseUrl = "https://localhost:7126/api";
  private http = inject(HttpClient);


   getSchoolInfo(): Observable<School[]> {
    return this.http.get<School[]>(`${this.baseUrl}/School`).pipe(
      map((schools: School[]) => {
        return schools.map(s => ({
          ...s,
          logo: s.logo ? `data:image/jpeg;base64,${s.logo}` : '',
          image: s.image ? `data:image/jpeg;base64,${s.image}` : ''
        }));
      })
    );
  }
}
