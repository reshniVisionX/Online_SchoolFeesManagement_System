using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SchoolFeeManagementApi.DTOs;
using SchoolFeeManagementApi.Models;
using SchoolFeeManagementApi.Service;

namespace SchoolFeeManagementApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PaymentController : ControllerBase
    {
        private readonly PaymentService _paymentService;

        public PaymentController(PaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        // GET: api/payment
        [HttpGet]
        public async Task<ActionResult<ICollection<FeePayment>>> GetAllPayments()
        {
            try
            {
                var payments = await _paymentService.GetAllPaymentAsync();
                return Ok(payments);
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        // GET: api/payment/course/5
        [HttpGet("course/{courseId}")]
        public async Task<ActionResult<ICollection<FeePaymentDTO>>> GetPaymentsByCourseId(int courseId)
        {
            try
            {
                var payments = await _paymentService.GetAllPaymentByCourseIdAsync(courseId);
                return Ok(payments);
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
    
        [HttpGet("studId/{sId}")]
        public async Task<ActionResult<FeePayment>> GetPaymentById(int sId)
        {
            var payment = await _paymentService.GetPaymentByIdAsync(sId);
            if (payment == null)
                return NotFound(new { Message = $"No FeePayment found for Student Id = {sId}" });

            return Ok(payment);
        }


        [HttpPatch("refresh-course/{courseId}")]
        public async Task<ActionResult<bool>> RefreshPaymentsForCourse(int courseId)
        {
            try
            {
                var result = await _paymentService.RefreshPaymentsForCourseAsync(courseId);
                if (!result) return NotFound(new { message = "Course not found or failed to update payments" });

                return Ok(new { message = "Payments refreshed successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPatch("refresh-student/{studentId}")]
        public async Task<ActionResult<bool>> RefreshPaymentForStudent(int studentId)
        {
            try
            {
                var result = await _paymentService.RefreshPaymentForStudentAsync(studentId);
                if (!result) return NotFound(new { message = "Student not found or failed to update payment" });

                return Ok(new { message = "Payment refreshed successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
