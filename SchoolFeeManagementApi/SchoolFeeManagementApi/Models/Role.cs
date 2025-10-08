using System.ComponentModel.DataAnnotations;

namespace SchoolFeeManagementApi.Models
{
    public class Role
    {
        [Key]
        public int RoleId { get; set; }
        [Required]
        public string RoleName { get; set; } = string.Empty; 
      
        public ICollection<Student> Students { get; set; } = new List<Student>();
    }

}
