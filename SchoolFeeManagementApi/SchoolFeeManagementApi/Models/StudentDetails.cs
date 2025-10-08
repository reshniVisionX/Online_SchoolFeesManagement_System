using System.ComponentModel.DataAnnotations;

namespace SchoolFeeManagementApi.Models
{
    public class StudentDetails
    {
        [Key]
        public int SdId { get; set; }

        [Required]
        [StringLength(20)] 
        public StudentCategory Category { get; set; } = StudentCategory.DayScholar;

        [Required]
        public bool IsSports { get; set; } = false;

        [Required]
        public bool IsMerit { get; set; } = false;

        [Required]
        public bool IsFG { get; set; } = false;

        [Required]
        public bool IsWaiver { get; set; } = false;

        [Required]
        public DateTime AdmissionDate { get; set; }

        [Required]
        public bool IsActive { get; set; } = true;

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // 🔗 Navigation
        public Student? Student { get; set; }
    }
}

public enum StudentCategory
{
    DayScholar,
    Hostel
}