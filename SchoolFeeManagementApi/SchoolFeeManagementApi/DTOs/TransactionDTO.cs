using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SchoolFeeManagementApi.DTOs
{
    public class TransactionDTO
    {
        public int? SId { get; set; }

        public int? FeeId { get; set; }

        public PaymentType PayType { get; set; } = PaymentType.Cash;
     
        public decimal Amount { get; set; }

    }
}
