
using SimplePersonalFinance.Core.Domain.Entities;

namespace SimplePersonalFinance.Application.ViewModels.Accounts;

public class AccountTransactionsViewModel : AccountViewModel
{
    public List<TransactionViewModel> Transactions { get; private set; }
    public AccountTransactionsViewModel(Guid id, 
                                        Guid userId, 
                                        int accountTypeId, 
                                        string name, 
                                        string accountTypeName, 
                                        decimal initialBalance, 
                                        decimal currentBalance, 
                                        List<TransactionViewModel> transactions) 
        : base(id, userId, accountTypeId, name, accountTypeName, initialBalance, currentBalance)
    {
        Transactions = transactions;
    }

    public static new AccountTransactionsViewModel MapToViewModel(Account account)
    {
        var transactions = account.Transactions.Select(x => TransactionViewModel.ToViewModel(x)).ToList();
        return new (account.Id,
                    account.UserId,
                    account.AccountTypeId,
                    account.Name,
                    account.AccountType.Name,
                    account.InitialBalance,
                    account.CurrentBalance,
                    transactions);
    }
}
