import { Student } from './Student';

export interface Role {
  roleId: number;
  roleName: string;
  students?: Student[]; // optional navigation property
}

