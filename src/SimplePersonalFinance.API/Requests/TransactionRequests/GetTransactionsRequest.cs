using SimplePersonalFinance.API.Requests.BaseRequests;
using System.ComponentModel.DataAnnotations;

namespace SimplePersonalFinance.API.Requests.TransactionRequests;

public class GetTransactionsRequest:BasePaginantionRequest
{
    [Required(ErrorMessage = "Account ID is required")]
    public Guid AccountId { get; set; }
}
