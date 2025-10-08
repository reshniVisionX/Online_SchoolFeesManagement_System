import { Component, inject, signal, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { FeePaymentDTO } from '../../Interface/FeePayDTO';
import { FeeService } from '../../api/fee-service';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';

@Component({
  selector: 'app-fee-payment',
  imports: [CommonModule, FormsModule],
  templateUrl: './fee-payment.html',
  styleUrl: './fee-payment.css'
})
export class FeePayment implements OnInit {
  private feeService = inject(FeeService);
  private route = inject(ActivatedRoute);

  feeDetails = signal<FeePaymentDTO[]>([]);     // full data from API
  displayedFees = signal<FeePaymentDTO[]>([]);  // filtered & sorted list
  courseId = signal<number | null>(null);

   private router: Router = inject(Router);
  // filter & sort states
  statusFilter: 'All' | 'Pending' | 'Paid' = 'All';
  sortBalanceAsc: boolean | null = null;  // null = no sorting yet

  ngOnInit(): void {
    const courseId = Number(this.route.snapshot.paramMap.get('id'));
    if (courseId) {
      this.courseId.set(courseId);
      this.loadFeeDetails(courseId);
    }
  }
viewTransaction(){
  this.router.navigate(['/transaction',this.courseId()]);
}
   loadFeeDetails(courseId: number) {
    this.feeService.getFeeDetailsByCourse(courseId).subscribe({
      next: (res) => {
        this.feeDetails.set(res);
        this.applyFilterAndSort(); // initialize
      },
      error: (err) => console.error('Error loading fee details', err)
    });
  }

  // Call this method from both status filter & balance sort
  applyFilterAndSort(sortBalance?: boolean) {
    let fees = [...this.feeDetails()];

    // 1️⃣ Apply status filter if not 'All'
    if (this.statusFilter !== 'All') {
      const isPending = this.statusFilter === 'Pending';
      fees = fees.filter(f => f.isPending === isPending);
    }

    // 2️⃣ Apply balance sort if requested
    if (sortBalance !== undefined) {
      this.sortBalanceAsc = this.sortBalanceAsc === null ? true : !this.sortBalanceAsc; // toggle
    }

    if (this.sortBalanceAsc !== null) {
      fees.sort((a, b) =>
        this.sortBalanceAsc ? a.balance - b.balance : b.balance - a.balance
      );
    }

    // 3️⃣ Update displayed list
    this.displayedFees.set(fees);
  }

  // Methods called by UI
  setStatusFilter(status: 'All' | 'Pending' | 'Paid') {
    this.statusFilter = status;
    this.applyFilterAndSort(); // apply current filters + sort
  }

  toggleBalanceSort() {
    this.applyFilterAndSort(true); // pass true to indicate sorting by balance
  }
}
