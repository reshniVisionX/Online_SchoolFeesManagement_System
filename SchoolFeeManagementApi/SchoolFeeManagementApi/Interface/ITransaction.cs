using SchoolFeeManagementApi.Models;

namespace SchoolFeeManagementApi.Interface
{
    public interface ITransaction
    {
        Task<ICollection<Transaction>> GetAllTransactions();
        Task<ICollection<Transaction>> GetTransactionsByStudentId(int studentId);
        Task<Transaction> AddTransaction(Transaction transaction);
        Task<ICollection<Transaction>> GetByCourseId(int courseId);

    }
}
