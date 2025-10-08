using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SchoolFeeManagementApi.Models
{
    public class School
    {
        [Key]
        public int SclId { get; set; }

        [Required]
        [StringLength(100)] 
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string City { get; set; } = string.Empty;

      
        public byte[]? Logo { get; set; } 

        [Required]
        [StringLength(255)]
        public string Address { get; set; } = string.Empty;

        [Required]
        [Phone]
        [StringLength(10)] 
        public string Phone { get; set; } = string.Empty;

        [Required]
        public DateOnly FoundedAt { get; set; }

        [Range(0, 5)]
        public double Rating { get; set; }

       
        public byte[]? Image { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // 🔗 Navigation
        public ICollection<Course> Courses { get; set; } = new List<Course>();
    }
}
