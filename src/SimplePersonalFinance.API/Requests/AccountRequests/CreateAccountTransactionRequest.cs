using SimplePersonalFinance.Core.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace SimplePersonalFinance.API.Requests.AccountRequests
{
    public class CreateAccountTransactionRequest
    {
        [Required(ErrorMessage = "Category is required")]
        public CategoryEnum CategoryId { get; set; }

        [Required(ErrorMessage = "Transaction type is required")]
        public TransactionTypeEnum TransactionTypeId { get; set; }

        [Required(ErrorMessage = "Description is required")]
        [StringLength(250, MinimumLength = 2, ErrorMessage = "Description must be between 2 and 250 characters")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Amount is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than zero")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "Date is required")]
        public DateTime Date { get; set; }
    }
}
