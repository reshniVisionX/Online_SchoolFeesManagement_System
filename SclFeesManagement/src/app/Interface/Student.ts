import { Role } from './Role';
import { Course } from './Course';
import { StudentDetail } from './StudentDetails';
import { FeePayments } from './FeePayments';

export interface Student {
  sId: number;
  sName: string;
  courseId: number;
  roleId: number;
  sdId: number;
  sImage?: string; // base64 string for image
  password:string;
  dob: string; // ISO string for DateOnly
  bloodGrp: string;
  admissionId: string;
  parAddress: string;
  parPhone: string;
  parEmail: string;

  // Navigation properties
  role?: Role;
  course?: Course;
  studentDetails?: StudentDetail;
  feePayments?: FeePayments[];
}
