
export interface UpdateStudents {
  sName: string;
  courseId: number;
  dob: string; // or Date if you're handling date objects
  bloodGrp: string;
  parAddress: string;
  parPhone: string;
  parEmail: string;
  category?: number;
  isSports?: boolean;
  isMerit?: boolean;
  isFG?: boolean;
  isWaiver?: boolean;
  isActive?: boolean;
}
