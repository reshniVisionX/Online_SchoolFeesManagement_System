using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SchoolFeeManagementApi.Models
{
    public class Student
    {
        [Key]
        public int SId { get; set; }

        [Required]
        [StringLength(100)]
        public string SName { get; set; } = string.Empty;

        [Required]
        public int CourseId { get; set; }

        [Required]
        public int RoleId { get; set; }

        [Required]
        public int SdId { get; set; }

        public byte[]? SImage { get; set; }
        [Required]
        [StringLength(10)]
        public string Password { get; set; }

        [Required]
        public DateOnly Dob { get; set; }

        [Required]
        [StringLength(5)] 
        public string BloodGrp { get; set; } = string.Empty;

        [Required]
        [StringLength(50)] 
        public string AdmissionId { get; set; } = string.Empty;

        [Required]
        [StringLength(255)]
        public string ParAddress { get; set; } = string.Empty;

        [Required]
        [Phone]
        [StringLength(10)]
        public string ParPhone { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string ParEmail { get; set; } = string.Empty;

        // 🔗 Navigation Properties
        public Role? Role { get; set; }
        public Course? Course { get; set; }
        public StudentDetails? StudentDetails { get; set; }
        public ICollection<FeePayment> FeePayments { get; set; } = new List<FeePayment>();
    }
   

    }
