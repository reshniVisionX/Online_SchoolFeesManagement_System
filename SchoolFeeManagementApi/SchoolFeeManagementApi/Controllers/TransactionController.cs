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
    public class TransactionController : ControllerBase
    {
        private readonly TransactionService _transactionService;

        public TransactionController(TransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        // GET: api/transaction
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Transaction>>> GetAllTransactions()
        {
            try
            {
                var transactions = await _transactionService.GetAllTransactionsAsync();
                return Ok(transactions);
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        // GET: api/transaction/student/5
        [HttpGet("student/{studentId}")]
        public async Task<ActionResult<IEnumerable<Transaction>>> GetTransactionsByStudentId(int studentId)
        {
            try
            {
                var transactions = await _transactionService.GetTransactionsByStudentIdAsync(studentId);
                return Ok(transactions);
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        // POST: api/transaction
        [HttpPost]
        public async Task<ActionResult<Transaction>> AddTransaction([FromBody] TransactionDTO dto)
        {
            try
            {
                var transaction = await _transactionService.AddTransactionAsync(dto);
                return CreatedAtAction(nameof(GetAllTransactions), new { id = transaction.TransId }, transaction);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("course/{courseId}")]
        public async Task<ActionResult<IEnumerable<Transaction>>> GetTransactionsByCourseId(int courseId)
        {
           
            try
            {
                var transactions = await _transactionService.GetTransactionByCourse(courseId);
                return Ok(transactions);
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
    }
}
