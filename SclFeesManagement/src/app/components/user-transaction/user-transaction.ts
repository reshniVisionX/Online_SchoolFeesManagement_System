import { Component, inject, signal, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { FeePayments } from '../../Interface/FeePayments';
import { FeeService } from '../../api/fee-service';
import { TransactionService } from '../../api/transactionservice';
import { Transaction, PaymentType } from '../../Interface/Transaction';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { LoginService } from '../../api/login-service';

@Component({
  selector: 'app-user-transaction',
  imports: [CommonModule, FormsModule],
  templateUrl: './user-transaction.html',
  styleUrl: './user-transaction.css'
})

export class UserTransaction implements OnInit {
  userId = signal<number | null>(null);
  userName = signal<string>('');
  feesId = signal<number | null>(null);
  feePayment = signal<FeePayments | null>(null);

  private feeService = inject(FeeService);
  private transactionService = inject(TransactionService);
  private loginService = inject(LoginService);

  selectedUPI = signal<string | null>(null);
  pin = signal<string>('');

  amount: number = 0;

  toastMessage = signal<string | null>(null);
  toastSuccess = signal<boolean>(false);

  constructor() {
    const username= this.loginService.getUserName();
    const userId = this.loginService.getUserId();
    this.userId.set(userId);
    if (username) {
     this.userName.set(username);
}

  }

ngOnInit(): void {
    this.fetchFeePayment();
  }

  private fetchFeePayment() {
    const sId = this.userId();
    if (!sId) return;

    this.feeService.getPaymentByStudentId(sId).subscribe({
      next: (res: FeePayments) => {
        this.feePayment.set(res);
        this.feesId.set(res.feeId); 
        console.log('Fetched FeePayment:', res);
      },
      error: (err) => console.error('Error fetching FeePayment', err)
    });
  }


submitPayment() {
  const sId = this.userId();
  const feeId = this.feesId();


if (!this.selectedUPI()) {
  this.showToast('Please select a UPI option', false);
  return;
}

if (!this.pin() || this.pin().length < 4) {
  this.showToast('PIN must be at least 4 digits', false);
  return;
}

if (this.pin().length > 8) {
  this.showToast('PIN must not exceed 8 digits', false);
  return;
}

if (!/^\d+$/.test(this.pin())) {
  this.showToast('PIN must only contain numbers', false);
  return;
}


if (!sId || !feeId) {
  this.showToast('Invalid student or fee ID', false);
  return;
}
  const payload: Partial<Transaction> = {
    sId,
    feeId,
    payType: PaymentType.UPI, 
    amount: this.amount
  };

  this.transactionService.insertIntoTransaction(payload).subscribe({
    next: (res) => {
      console.log('Payment success:', res);
      this.showToast('✔ Payment Successful!', true);
      this.fetchFeePayment(); 
    },
    error: (err) => {
      console.error('Payment failed', err);
      this.showToast('❌ Payment Failed', false);
    }
  });
}


  private showToast(message: string, success: boolean) {
    this.toastMessage.set(message);
    this.toastSuccess.set(success);

    setTimeout(() => {
      this.toastMessage.set(null);
    }, 5000); 
  }
}
