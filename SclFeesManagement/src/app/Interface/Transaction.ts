import { Student } from './Student';
import { FeePayments } from './FeePayments';

export interface Transaction {
  transId: number;
  sId: number;
  feeId: number;
  payType: PaymentType;
  amount: number;
  status: TransactionStatus;
  dateTime: string; // ISO string

  // Navigation
  student?: Student;
  feePayment?: FeePayments;
}

// Enums
export enum PaymentType {
  UPI = 'UPI',
  Cash = 'Cash',
  Card = 'Card'
}

export enum TransactionStatus {
  Success = 'Success',
  Pending = 'Pending',
  Failed = 'Failed'
}
