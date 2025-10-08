using SchoolFeeManagementApi.Interface;
using SchoolFeeManagementApi.Models;
using SchoolFeeManagementApi.DTOs;

namespace SchoolFeeManagementApi.Service
{
    public class PaymentService
    {
        private readonly IPayment _paymentRepo;

        public PaymentService(IPayment paymentRepo)
        {
            _paymentRepo = paymentRepo;
        }

        public async Task<ICollection<FeePayment>> GetAllPaymentAsync()
        {
            var payments = await _paymentRepo.GetAllPayment();

            if (payments == null || !payments.Any())
                throw new Exception("No payments found.");

            return payments;
        }

        public async Task<ICollection<FeePaymentDTO>> GetAllPaymentByCourseIdAsync(int courseId)
        {
            if (courseId <= 0)
                throw new ArgumentException("Invalid courseId.");

            var payments = await _paymentRepo.GetAllPaymentByCourseId(courseId);

            if (payments == null || !payments.Any())
                throw new Exception($"No payments found for CourseId: {courseId}");

            return payments;
        }
        public async Task<FeePayment?> GetPaymentByIdAsync(int sId)
        {
            return await _paymentRepo.GetPaymentById(sId);
        }


        public async Task<bool> RefreshPaymentsForCourseAsync(int courseId)
        {
            try
            {
                return await _paymentRepo.RefreshPaymentsForCourse(courseId);
            }
            catch (Exception ex)
            {              
                throw new Exception($"Error refreshing payments for CourseId {courseId}: {ex.Message}");
            }
        }

        public async Task<bool> RefreshPaymentForStudentAsync(int studentId)
        {
            try
            {
                return await _paymentRepo.RefreshPaymentForStudent(studentId);
            }
            catch (Exception ex)
            {              
                throw new Exception($"Error refreshing payment for StudentId {studentId}: {ex.Message}");
            }
        }
    }

}
