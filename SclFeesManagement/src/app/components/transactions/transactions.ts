import { Component, signal, inject } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { TransactionService } from '../../api/transactionservice';
import { Transaction } from '../../Interface/Transaction';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-transactions',
  standalone: true,
  imports: [CommonModule,FormsModule],
  templateUrl: './transactions.html',
  styleUrl: './transactions.css'
})
export class Transactions {
  
  private transactionService = inject(TransactionService);
  private route = inject(ActivatedRoute);

  courseId = signal<number | null>(null);
  transactions = signal<Transaction[]>([]);
  loading = signal<boolean>(false);
  error = signal<string | null>(null);

  ngOnInit(): void {
    const id = Number(this.route.snapshot.paramMap.get('id'));
    if (id) {
      this.courseId.set(id);
      this.loadTransactions(id);
    } else {
      this.error.set('Invalid course id');
    }
  }

  loadTransactions(courseId: number) {
    this.loading.set(true);
    this.error.set(null);

    this.transactionService.getTransactionByCourseId(courseId).subscribe({
      next: (data) => {
        this.transactions.set(data);
        console.log(this.transactions());
        this.loading.set(false);
      },
      error: (err) => {
        this.error.set('Failed to load transactions');
        console.error(err);
        this.loading.set(false);
      }
    });
  }
}
