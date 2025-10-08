using System.ComponentModel.DataAnnotations;

namespace SchoolFeeManagementApi.Models
{
    public class Feedback
    {
        [Key]
        public int FeedId { get; set; }

        [Required]
        [EmailAddress] 
        [StringLength(100)]
        public string EmailId { get; set; } = string.Empty;

        [Required]
        [StringLength(500)] 
        public string Query { get; set; } = string.Empty;

        [Range(1, 5)]
        public int Rating { get; set; } = 5;

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.Now; 
    }
}
