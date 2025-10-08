using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SchoolFeeManagementApi.Models
{
    public class Course
    {
        [Key]
        public int CourseId { get; set; }

        [Required]
        [StringLength(50)]   
        public string CourseName { get; set; } = string.Empty;

        [Required]
        [ForeignKey("School")]
        public int SclId { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Required]
        public bool IsActive { get; set; } = true;

        [Column(TypeName = "decimal(10,2)")]
        [Range(0, 100000)]
        public decimal TutionFees { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        [Range(0, 50000)]
        public decimal BusFees { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        [Range(0, 100000)]
        public decimal HostelFees { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        [Range(0, 500000)]
        public decimal DonationFee { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        [Range(0, 200000)]
        public decimal Sports { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        [Range(0, 200000)]
        public decimal Merit { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        [Range(0, 50000)]
        public decimal FG { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        [Range(0, 55000)]
        public decimal Waiver { get; set; }

        // Navigation properties
        public School? School { get; set; }
        public ICollection<Student> Students { get; set; } = new List<Student>();
        public ICollection<FeePayment> FeePayments { get; set; } = new List<FeePayment>();
    }
}
