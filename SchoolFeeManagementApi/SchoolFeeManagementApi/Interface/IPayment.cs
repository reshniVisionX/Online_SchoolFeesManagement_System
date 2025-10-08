using SchoolFeeManagementApi.Models;
using SchoolFeeManagementApi.DTOs;
namespace SchoolFeeManagementApi.Interface
{
    public interface IPayment
    {
        Task<ICollection<FeePaymentDTO>> GetAllPaymentByCourseId(int courseId);
        Task<ICollection<FeePayment>> GetAllPayment();
        
        Task<FeePayment> GetPaymentById(int sId);
        Task<bool> RefreshPaymentsForCourse(int courseId);
        Task<bool> RefreshPaymentForStudent(int studentId);
    }
}
