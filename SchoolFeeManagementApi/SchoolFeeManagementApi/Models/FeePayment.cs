using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SchoolFeeManagementApi.Models
{
    public class FeePayment
    {
        [Key]
        public int FeeId { get; set; }

        [Required]
        public int SId { get; set; }

        [Required]
        public int CourseId { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        [Range(0, 10000)]
        public decimal Penalty { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        [Range(0, 500000)]
        public decimal TotalFees { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        [Range(0, 9000000)]
        public decimal TotWaiver { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        [Range(0, 500000)]
        public decimal FeesToPay { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        [Range(0, 9000000)]
        public decimal PaidAmt { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        [Range(0, 500000)]
        public decimal Balance { get; set; }

        [Required]
        public bool IsPending { get; set; } = true;

        [Required]
        public bool IsActive { get; set; } = true;

        [Required]
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        [Required]
        [StringLength(9)] 
        public string AcademicYear { get; set; } = string.Empty;
        [Required]
        public DateTime DueDate { get; set; } = DateTime.Now.AddMonths(3);

        [ForeignKey("SId")]
        public Student? Student { get; set; }

        [ForeignKey("CourseId")]
        public Course? Course { get; set; }

        public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
    }
}
