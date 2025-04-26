using SimplePersonalFinance.Core.Domain.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace SimplePersonalFinance.API.Requests.BudgetRequests
{
    public class CreateBudgetRequest
    {

        [Required(ErrorMessage = "Category is required")]
        public CategoryEnum Category { get; set; }

        [Required(ErrorMessage = "Limit amount is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Limit amount must be greater than zero")]
        public decimal LimitAmount { get; set; }

        [Required(ErrorMessage = "Month is required")]
        [Range(1, 12, ErrorMessage = "Month must be between 1 and 12")]
        public int Month { get; set; }

        [Required(ErrorMessage = "Year is required")]
        [Range(2000, 2100, ErrorMessage = "Year must be between 2000 and 2100")]
        public int Year { get; set; }
    }
}
