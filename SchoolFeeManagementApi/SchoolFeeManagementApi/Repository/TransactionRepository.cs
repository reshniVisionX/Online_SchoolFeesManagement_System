using Microsoft.EntityFrameworkCore;
using SchoolFeeManagementApi.Data;
using SchoolFeeManagementApi.Interface;
using SchoolFeeManagementApi.Models;
using SchoolFeeManagementApi.Service;

namespace SchoolFeeManagementApi.Repository
{
    public class TransactionRepository : ITransaction
    {
        private readonly SchoolContext _context;
        private readonly PaymentService pser;
        public TransactionRepository(SchoolContext context,PaymentService ser)
        {
            _context = context;
            pser = ser;
        }

        public async Task<Transaction> AddTransaction(Transaction transaction)
        {
            await using var dbTransaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // 1️⃣ Fetch FeePayment
                var feePayment = await _context.FeePayments
                    .FirstOrDefaultAsync(f => f.FeeId == transaction.FeeId);

                if (feePayment == null)
                    throw new Exception("FeePayment not found for the given FeeId.");

                if (!feePayment.IsActive)
                    throw new Exception("Cannot process transaction for inactive fee payment.");

                // 2️⃣ Check if payment is still pending
                if (!feePayment.IsPending)
                    throw new Exception("Payment is already cleared. No further transaction allowed.");
               
                decimal remainingAmount = transaction.Amount;
              
                if (feePayment.Penalty == 0 && remainingAmount > feePayment.FeesToPay)
                    throw new Exception($"Payment amount exceeds total due fees ({feePayment.FeesToPay}).");

                if (remainingAmount > feePayment.FeesToPay && (feePayment.Penalty+feePayment.FeesToPay != remainingAmount))
                {
                    throw new Exception($"Payment amount exceeds total due fees ({feePayment.FeesToPay}) and penalty need to be paid fully.");
                }
                if (feePayment.FeesToPay > 0)
                {
                    var deduct = Math.Min(feePayment.FeesToPay, remainingAmount);
                    feePayment.FeesToPay -= deduct;
                    feePayment.Balance = feePayment.FeesToPay; // keep balance synced
                    feePayment.PaidAmt += deduct;
                    remainingAmount -= deduct;
                }

                // 5️⃣ Pay Penalty (full only)
                if (feePayment.Penalty > 0)
                {
                    if (remainingAmount == feePayment.Penalty)
                    {
                        feePayment.PaidAmt += feePayment.Penalty;
                        feePayment.Penalty = 0;
                        remainingAmount = 0;
                    }
                    else if (remainingAmount > 0)
                    {
                        throw new Exception($"Penalty must be paid in full. Current penalty: {feePayment.Penalty}");
                    }
                }

                // 6️⃣ If any leftover amount (transaction > fees+penalty), throw exception
                if (remainingAmount > 0)
                {
                    throw new Exception("Transaction amount exceeds total due (fees + penalty rules).");
                }

                // 7️⃣ Mark as cleared if everything is paid
                if (feePayment.FeesToPay == 0 && feePayment.Penalty == 0)
                {
                    feePayment.IsPending = false;
                }

                feePayment.UpdatedAt = DateTime.Now;
                _context.FeePayments.Update(feePayment);

                // 8️⃣ Record transaction
                transaction.Status = TransactionStatus.Success;
                transaction.DateTime = DateTime.Now;

                await _context.Transactions.AddAsync(transaction);

                // 9️⃣ Save everything
                await _context.SaveChangesAsync();
                await dbTransaction.CommitAsync();

                return transaction;
            }
            catch (Exception ex)
            {
                await dbTransaction.RollbackAsync();
                throw new Exception($"Transaction failed: {ex.Message}", ex);
            }
        }



        public async Task<ICollection<Transaction>> GetAllTransactions()
        {
            return await _context.Transactions
                .Include(t => t.Student)
                .Include(t => t.FeePayment)
                .ToListAsync();
        }

        public async Task<ICollection<Transaction>> GetByCourseId(int courseId)
        {
            return await _context.Transactions
                .Include(t => t.Student)
                .Include(t => t.FeePayment) 
                .Where(t => t.Student!.CourseId == courseId) 
                .ToListAsync();

           

        }


        public async Task<ICollection<Transaction>> GetTransactionsByStudentId(int studentId)
        {
            return await _context.Transactions
                .Where(t => t.SId == studentId)
                .Include(t => t.Student)
                .Include(t => t.FeePayment)
                .ToListAsync();
        }

   
    }
}
