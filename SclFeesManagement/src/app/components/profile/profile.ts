import { Component, inject, signal } from '@angular/core';
import { UserTransaction } from '../user-transaction/user-transaction';
import { StudentServices } from '../../api/student-services';
import { ActivatedRoute, Router } from '@angular/router';
import { Student } from '../../Interface/Student';
import { LoginService } from '../../api/login-service';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import jsPDF from 'jspdf';
import { Transaction } from '../../Interface/Transaction';
import { TransactionService } from '../../api/transactionservice';
import {SchoolService} from '../../api/school-service';
import {School} from '../../Interface/School';
import { TransactionStatus } from '../../Interface/Transaction';
import { PaymentType } from '../../Interface/Transaction';

@Component({
  selector: 'app-profile',
  imports: [UserTransaction, CommonModule, FormsModule],
  templateUrl: './profile.html',
  styleUrls: ['./profile.css']
})
export class Profile {
  private studentService = inject(StudentServices);
  private loginService = inject(LoginService);
  private router: Router = inject(Router);
  private route = inject(ActivatedRoute);
  private transactionService = inject(TransactionService);
  private schSer = inject(SchoolService);
   school = signal<School | null>(null);

  students = signal<Student | null>(null);
  sId = signal<number | null>(null);
  transaction = signal<Transaction[]>([]); // <-- Array of transactions

  ngOnInit(): void {
     
    const userId = this.loginService.getUserId();
    if (userId) {
      this.sId.set(userId);
      this.studentService.getStudentDetailsById(userId).subscribe({
        next: (data: Student) => {
          this.students.set(data);
          console.log('Fetched student: ', data);
        },
        error: (err) => console.error('Error fetching student', err)
      });
    }
  }

/** Fetch school info as a Promise */
FetchSchoolInfo(): Promise<School | null> {
  return new Promise((resolve) => {
    this.schSer.getSchoolInfo().subscribe({
      next: (data) => {
        if (Array.isArray(data) && data.length > 0) {
          this.school.set(data[0]);
          resolve(data[0]);
        } else {
          this.school.set(null);
          resolve(null);
        }
      },
      error: (err) => {
        console.error('Error fetching school info', err);
        this.school.set(null);
        resolve(null);
      }
    });
  });
}

  /** Fetch transactions for a student and update the signal */
  fetchTransactions(studentId: number): Promise<Transaction[]> {
    return new Promise((resolve) => {
      this.transactionService.getTransactionByStudentId(studentId).subscribe({
        next: (data: Transaction[]) => {
          this.transaction.set(data); // signal updated
          resolve(data);
        },
        error: (err) => {
          console.error('Error fetching transactions', err);
          this.transaction.set([]);
          resolve([]);
        }
      });
    });
  }

generatePDF(transactions: Transaction[]): jsPDF {
  const doc = new jsPDF();

  const school = this.school(); // get school info from the signal

transactions.forEach((transaction, index) => {
  if (index > 0) doc.addPage(); // new page for each transaction

  if (school) {
     if (school.logo) {
        // since already base64, just add
        doc.addImage(school.logo, 'JPEG', 90, 5, 30, 30); 
        // (x=90, y=5, width=30, height=30 → adjust as needed)
      }
 doc.setFontSize(16);
      doc.text(school.name ?? '-', 105, 45, { align: 'center' });
      doc.setFontSize(12);
      doc.text(`Address: ${school.address ?? '-'}`, 105, 52, { align: 'center' });
      doc.text(`City: ${school.city ?? '-'}`, 105, 59, { align: 'center' });
      doc.text(`Phone: ${school.phone ?? '-'}`, 105, 66, { align: 'center' });

      doc.setLineWidth(0.5);
      doc.line(14, 70, 196, 70); // underline
  }

  // --- Header for Receipt ---
  doc.setFontSize(20);
  doc.text('Payment Receipt', 105, 55, { align: 'center' });
  doc.setLineWidth(0.5);
  doc.line(14, 58, 196, 58);

  // --- Student Info ---
  doc.setFontSize(12);
  doc.text(`Student Name: ${transaction.student?.sName ?? '-'}`, 14, 70);
  doc.text(`Admission ID: ${transaction.student?.admissionId ?? '-'}`, 14, 77);
  doc.text(`Phone: ${transaction.student?.parPhone ?? '-'}`, 14, 84);
  doc.text(`Email: ${transaction.student?.parEmail ?? '-'}`, 14, 91);
  doc.text(`Academic Year: ${transaction.feePayment?.academicYear ?? '-'}`, 14, 98);

  doc.text(`Total Fees: ₹${transaction.feePayment?.totalFees ?? 0}`, 14, 110);
  doc.text(`Fees to Pay: ₹${transaction.feePayment?.feesToPay ?? 0}`, 14, 117);
  doc.text(`Total Amount Paid: ₹${transaction.feePayment?.paidAmt ?? 0}`, 14, 124);

  doc.text(`Payment Type: ${transaction.payType === PaymentType.UPI? 'UPI' : 'Cash'}`, 14, 136);
  doc.text(`Amount Now Transferred: ₹${transaction.amount?.toFixed(2) ?? '0.00'}`, 14, 143);
  doc.text(`Status:  ${transaction.status === TransactionStatus.Success ? 'Pending' : 'Success'}`, 14, 150);

  doc.text(`Date/Time: ${new Date(transaction.dateTime).toLocaleString()}`, 14, 160);

  // Footer line
  doc.setLineWidth(0.5);
  doc.line(14, 177, 196, 177);
});


  return doc;
}

/** View PDF in a new tab */
async viewPayReceipt(studentId: number) {
  const [transactions, school] = await Promise.all([
    this.fetchTransactions(studentId),
    this.FetchSchoolInfo()
  ]);

  if (!transactions.length) {
    console.warn(`❌ No transactions found for studentId: ${studentId}`);
    return;
  }

  if (!school) {
    console.warn(`❌ School info not available while generating receipt`);
    return;
  }

  const doc = this.generatePDF(transactions);
  window.open(doc.output('bloburl'), '_blank');
}

/** Download PDF locally */
async downloadPayReceipt(studentId: number) {
  const [transactions, school] = await Promise.all([
    this.fetchTransactions(studentId),
    this.FetchSchoolInfo()
  ]);

  if (!transactions.length) {
    console.warn(`❌ No transactions found for studentId: ${studentId}`);
    return;
  }

  if (!school) {
    console.warn(`❌ School info not available while generating receipt`);
    return;
  }

  const doc = this.generatePDF(transactions);
  doc.save(`Receipt_${studentId}.pdf`);
}

}
