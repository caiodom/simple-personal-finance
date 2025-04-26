using System.ComponentModel.DataAnnotations;

namespace SimplePersonalFinance.API.Requests.BaseRequests
{
    public abstract class BasePaginantionRequest
    {
        [Range(1, int.MaxValue, ErrorMessage = "Page size must be greater than 0")]
        public int PageSize { get; set; } = 10;

        [Range(1, int.MaxValue, ErrorMessage = "Page number must be greater than 0")]
        public int PageNumber { get; set; } = 1;
    }
}
