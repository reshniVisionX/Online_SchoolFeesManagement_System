namespace SchoolFeeManagementApi.DTOs
{
    public class FeePaymentDTO
    {
        public int SId { get; set; }                
        public string StudentName { get; set; } = string.Empty;
        public string CourseName { get; set; } = string.Empty;
        public string AcademicYear { get; set; } = string.Empty;
        public decimal Penalty { get; set; }
        public decimal TotalFees { get; set; }
        public decimal TotWaiver { get; set; }
        public DateTime DueDate { get; set; }
        public decimal FeesToPay { get; set; }
        public decimal PaidAmt { get; set; }
        public decimal Balance { get; set; }
        public bool IsPending { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
