
export interface FeePaymentDTO {
  sId: number;
  studentName: string;
  courseName: string;
  academicYear: string;
  penalty: number;
  totalFees: number;
  totWaiver: number;
  feesToPay: number;
  paidAmt: number;
  balance: number;
  isPending: boolean;
  updatedAt: string; // ISO date string
}
