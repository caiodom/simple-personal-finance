using SimplePersonalFinance.Core.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace SimplePersonalFinance.API.Requests.AccountRequests;

public class CreateAccountRequest
{
    [Required(ErrorMessage = "Account type is required")]
    public AccountTypeEnum AccountType { get; set; }

    [Required(ErrorMessage = "Name is required")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 100 characters")]
    public string Name { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "Initial balance must be a positive number")]
    public decimal InitialBalance { get; set; }
}
