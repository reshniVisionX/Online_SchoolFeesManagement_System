using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Transactions;

namespace SchoolFeeManagementApi.Models
{
    public class Transaction
    {
        [Key]
        public int TransId { get; set; }

        [Required]
        public int SId { get; set; }

        [Required]
        public int FeeId { get; set; }

        [Required]
        public PaymentType PayType { get; set; } = PaymentType.Cash;

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        [Range(0, 9000000)]
        public decimal Amount { get; set; }

        [Required]
        public TransactionStatus Status { get; set; }= TransactionStatus.Pending;

        [Required]
        public DateTime DateTime { get; set; } = DateTime.Now;

        // Navigation
        [ForeignKey("SId")]
        public Student? Student { get; set; }

        [ForeignKey("FeeId")]
        public FeePayment? FeePayment { get; set; }
    }
}
public enum PaymentType
{
    UPI,
    Cash,
    Card
}

public enum TransactionStatus
{
    Success,
    Pending,
    Failed
}