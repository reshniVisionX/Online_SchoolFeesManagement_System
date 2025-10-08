using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SchoolFeeManagementApi.DTOs
{
    public class CourseDTO
    {
       
        public string CourseName { get; set; } = string.Empty;
        public int? SclId { get; set; } = 1;

        public decimal TutionFees { get; set; }

        public decimal BusFees { get; set; }

        public decimal HostelFees { get; set; }

        public decimal DonationFee { get; set; }

        public decimal Sports { get; set; }

        public decimal Merit { get; set; }
        public decimal FG { get; set; }
        public decimal Waiver { get; set; }
    }
}
