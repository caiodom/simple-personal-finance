using SimplePersonalFinance.Core.Domain.Entities;
using SimplePersonalFinance.Core.Domain.Enums;
using SimplePersonalFinance.Core.Interfaces.Domain.Strategies;

namespace SimplePersonalFinance.Core.Interfaces.Domain.Services;

public interface IBalanceManager
{
    void ApplyNewTransaction(Account account, decimal amount, TransactionTypeEnum type);
    void RevertTransaction(Account account, Transaction transaction);
    void UpdateBalanceForEdit(
        Account account,
        decimal originalValue,
        decimal newValue,
        TransactionTypeEnum originalType,
        TransactionTypeEnum newType,
        IBalanceUpdateStrategy strategy);
}
