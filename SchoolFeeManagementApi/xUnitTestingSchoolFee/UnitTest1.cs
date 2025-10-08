using Moq;
using SchoolFeeManagementApi.DTOs;
using Xunit;
using SchoolFeeManagementApi.Interface;
using SchoolFeeManagementApi.Models;
using SchoolFeeManagementApi.Services;
using SchoolFeeManagementApi.Service;
using SchoolFeeManagementApi.DTOs;

    namespace xUnitTestingSchoolFee
    {
  
        public class TransactionServiceTests
        {
            private readonly Mock<ITransaction> _mockTransactionRepo;
            private readonly TransactionService _service;

            public TransactionServiceTests()
            {
                _mockTransactionRepo = new Mock<ITransaction>();
                _service = new TransactionService(_mockTransactionRepo.Object);
            }

            [Fact]
            public async Task AddTransactionAsync_ThrowsException_WhenStudentIdIsInvalid()
            {
                var dto = new TransactionDTO { SId = 0, FeeId = 1, Amount = 100 };

                var ex = await Assert.ThrowsAsync<ArgumentException>(() => _service.AddTransactionAsync(dto));
                Assert.Equal("StudentId is required and must be valid.", ex.Message);
            }

            [Fact]
            public async Task AddTransactionAsync_ThrowsException_WhenFeeIdIsInvalid()
            {
                var dto = new TransactionDTO { SId = 1, FeeId = 0, Amount = 100 };

                var ex = await Assert.ThrowsAsync<ArgumentException>(() => _service.AddTransactionAsync(dto));
                Assert.Equal("FeeId is required and must be valid.", ex.Message);
            }

            [Fact]
            public async Task AddTransactionAsync_ThrowsException_WhenAmountIsInvalid()
            {
                var dto = new TransactionDTO { SId = 1, FeeId = 1, Amount = 0 };

                var ex = await Assert.ThrowsAsync<ArgumentException>(() => _service.AddTransactionAsync(dto));
                Assert.Equal("Amount must be greater than 0.", ex.Message);
            }

            [Fact]
            public async Task AddTransactionAsync_ReturnsTransaction_WhenValid()
            {
                var dto = new TransactionDTO { SId = 1, FeeId = 1, Amount = 500, PayType = PaymentType.Cash };

                var expectedTransaction = new Transaction
                {
                    SId = dto.SId.Value,
                    FeeId = dto.FeeId.Value,
                    Amount = dto.Amount,
                    PayType = dto.PayType,
                    Status = TransactionStatus.Success,
                    DateTime = DateTime.Now
                };

                _mockTransactionRepo.Setup(r => r.AddTransaction(It.IsAny<Transaction>()))
                    .ReturnsAsync(expectedTransaction);

                var result = await _service.AddTransactionAsync(dto);

                Assert.NotNull(result);
                Assert.Equal(dto.SId, result.SId);
                Assert.Equal(dto.FeeId, result.FeeId);
                Assert.Equal(dto.Amount, result.Amount);
                Assert.Equal(TransactionStatus.Success, result.Status);
            }

            [Fact]
            public async Task GetAllTransactionsAsync_ReturnsTransactions()
            {
                var transactions = new List<Transaction>
            {
                new Transaction { TransId = 1, SId = 1, Amount = 100 },
                new Transaction { TransId = 2, SId = 2, Amount = 200 }
            };

                _mockTransactionRepo.Setup(r => r.GetAllTransactions())
                                    .ReturnsAsync(transactions);

                var result = await _service.GetAllTransactionsAsync();

                Assert.NotNull(result);
                Assert.Equal(2, result.Count);
            }

            [Fact]
            public async Task GetAllTransactionsAsync_ThrowsException_WhenEmpty()
            {
                _mockTransactionRepo.Setup(r => r.GetAllTransactions())
                                    .ReturnsAsync(new List<Transaction>());

                var ex = await Assert.ThrowsAsync<Exception>(() => _service.GetAllTransactionsAsync());
                Assert.Equal("No transactions found.", ex.Message);
            }

            [Fact]
            public async Task GetTransactionByCourse_ReturnsTransactions()
            {
                var transactions = new List<Transaction>
            {
                new Transaction { TransId = 1, SId = 1, Amount = 300 }
            };

                _mockTransactionRepo.Setup(r => r.GetByCourseId(1))
                                    .ReturnsAsync(transactions);

                var result = await _service.GetTransactionByCourse(1);

                Assert.Single(result);
                Assert.Equal(300, result.First().Amount);
            }

            [Fact]
            public async Task GetTransactionByCourse_ThrowsException_WhenEmpty()
            {
                _mockTransactionRepo.Setup(r => r.GetByCourseId(5))
                                    .ReturnsAsync(new List<Transaction>());

                var ex = await Assert.ThrowsAsync<Exception>(() => _service.GetTransactionByCourse(5));
                Assert.Equal("No transactions found for CourseId: 5", ex.Message);
            }

            [Fact]
            public async Task GetTransactionsByStudentIdAsync_ReturnsTransactions()
            {
                var transactions = new List<Transaction>
            {
                new Transaction { TransId = 1, SId = 1, Amount = 1500 }
            };

                _mockTransactionRepo.Setup(r => r.GetTransactionsByStudentId(1))
                                    .ReturnsAsync(transactions);

                var result = await _service.GetTransactionsByStudentIdAsync(1);

                Assert.Single(result);
                Assert.Equal(1500, result.First().Amount);
            }

            [Fact]
            public async Task GetTransactionsByStudentIdAsync_ThrowsException_WhenEmpty()
            {
                _mockTransactionRepo.Setup(r => r.GetTransactionsByStudentId(10))
                                    .ReturnsAsync(new List<Transaction>());

                var ex = await Assert.ThrowsAsync<Exception>(() => _service.GetTransactionsByStudentIdAsync(10));
                Assert.Equal("No transactions found for StudentId: 10", ex.Message);
            }
        }
    }
