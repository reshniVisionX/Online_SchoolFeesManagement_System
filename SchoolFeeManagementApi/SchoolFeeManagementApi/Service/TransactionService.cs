using SchoolFeeManagementApi.DTOs;
using SchoolFeeManagementApi.Interface;
using SchoolFeeManagementApi.Models;

namespace SchoolFeeManagementApi.Service
{
    public class TransactionService
    {
        private readonly ITransaction _transactionRepo;

        public TransactionService(ITransaction transactionRepo)
        {
            _transactionRepo = transactionRepo;
        }

        public async Task<Transaction> AddTransactionAsync(TransactionDTO dto)
        {
            if (dto.SId == null || dto.SId <= 0)
                throw new ArgumentException("StudentId is required and must be valid.");

            if (dto.FeeId == null || dto.FeeId <= 0)
                throw new ArgumentException("FeeId is required and must be valid.");

            if (dto.Amount <= 0)
                throw new ArgumentException("Amount must be greater than 0.");

            var newTransaction = new Transaction
            {
                SId = dto.SId.Value,
                FeeId = dto.FeeId.Value,
                PayType = dto.PayType,
                Amount = dto.Amount,
                Status = TransactionStatus.Success,
                DateTime = DateTime.Now
            };

            return await _transactionRepo.AddTransaction(newTransaction);
        }

        public async Task<ICollection<Transaction>> GetAllTransactionsAsync()
        {
            var transactions = await _transactionRepo.GetAllTransactions();
            if (!transactions.Any())
                throw new Exception("No transactions found.");
            return transactions;
        }

        public async Task<IEnumerable<Transaction>> GetTransactionByCourse(int courseId)
        {
            if (courseId <= 0)
                throw new ArgumentException("Invalid courseId.");
            var transactions = await _transactionRepo.GetByCourseId(courseId);
            if (!transactions.Any())
                throw new Exception($"No transactions found for CourseId: {courseId}");
            return transactions;
        }

        public async Task<ICollection<Transaction>> GetTransactionsByStudentIdAsync(int studentId)
        {
            if (studentId <= 0)
                throw new ArgumentException("Invalid studentId.");

            var transactions = await _transactionRepo.GetTransactionsByStudentId(studentId);
            if (!transactions.Any())
                throw new Exception($"No transactions found for StudentId: {studentId}");
            return transactions;
        }
    }
}
