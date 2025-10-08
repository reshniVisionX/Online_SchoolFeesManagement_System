import { Routes } from '@angular/router';
import { Courses } from './components/courses/courses';
import { Home} from './components/home/home';
import { Students } from './components/students/students';
import { StudentDetails} from './components/student-details/student-details';
import { FeePayment } from './components/fee-payment/fee-payment';
import { ExcelUpload } from './components/excel-upload/excel-upload';
import { UserTransaction } from './components/user-transaction/user-transaction';
import { LoginPage } from './components/login-page/login-page';
import { authGuard } from './guards/auth-gaurd-guard';
import { Feedback } from './components/feedback/feedback';
import { Profile } from './components/profile/profile';
import { Transactions } from './components/transactions/transactions';
import { UpdateCourse } from './components/update-course/update-course';


export const routes: Routes = [
  { path: 'courses', component: Courses},
  { path : 'home', component: Home, canActivate:[authGuard]},
  { path: 'students/:id', component: Students }, 
  { path: 'studentDetails/:id', component: StudentDetails },
  { path:  'feeDetailsCourse/:id', component:FeePayment}, 
  { path: 'feedback', component:Feedback},
  { path: 'update-course/:id', component: UpdateCourse},
  { path:'profile',component:Profile},
  { path:'transaction/:id',component:Transactions},
  { path: 'login', component:LoginPage},
  { path:'addStudents/:id', component: ExcelUpload},
  { path: 'userTransaction', component:UserTransaction},
  { path: '', redirectTo: 'home', pathMatch: 'full' },
  { path: '**', redirectTo: 'home' }
];
