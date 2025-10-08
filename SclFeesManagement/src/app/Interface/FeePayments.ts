import { Student } from './Student';
import { Course } from './Course';
import { Transaction } from './Transaction';

export interface FeePayments {
  feeId: number;
  sId: number;
  courseId: number;
  penalty: number;
  totalFees: number;
  totWaiver: number;
  feesToPay: number;
  paidAmt: number;
  balance: number;
  isPending: boolean;
  isActive: boolean;
  updatedAt: string; 
  academicYear: string;

  student?: Student;
  course?: Course;
  transactions?: Transaction[];
}
