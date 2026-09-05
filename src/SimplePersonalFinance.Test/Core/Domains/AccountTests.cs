using SimplePersonalFinance.Core.Domain.Entities;
using SimplePersonalFinance.Core.Domain.Enums;
using SimplePersonalFinance.Core.Domain.Events;
using SimplePersonalFinance.Core.Domain.Exceptions;

namespace SimplePersonalFinance.Test.Core.Domain.Entities;

public class AccountTests
{
    [Fact]
    public void Constructor_ShouldInitializeCorrectly()
    {
        var userId = Guid.NewGuid();
        var name = "Test Account";
        var initialBalance = 1000m;

        var account = new Account(userId, AccountTypeEnum.CHECKING, name, initialBalance);

        Assert.Equal(userId, account.UserId);
        Assert.Equal((int)AccountTypeEnum.CHECKING, account.AccountTypeId);
        Assert.Equal(name, account.Name);
        Assert.Equal(initialBalance, account.InitialBalance);
        Assert.Equal(initialBalance, account.CurrentBalance);
        Assert.Empty(account.Transactions);
    }

    [Fact]
    public void Constructor_WithNegativeInitialBalance_ShouldThrowDomainException()
    {
        var exception = Assert.Throws<DomainException>(() =>
            new Account(Guid.NewGuid(), AccountTypeEnum.CHECKING, "Test Account", -1m));

        Assert.Equal("Initial balance cannot be negative", exception.Message);
    }

    [Fact]
    public void AddTransaction_IncomeShouldIncreaseBalance()
    {
        var account = new Account(Guid.NewGuid(), AccountTypeEnum.CHECKING, "Test Account", 1000m);
        var initialBalance = account.CurrentBalance;
        var transactionAmount = 500m;

        var transaction = account.AddTransaction(
            "Salary",
            transactionAmount,
            CategoryEnum.SALARY,
            TransactionTypeEnum.INCOME,
            DateTime.Now);

        Assert.Equal(initialBalance + transactionAmount, account.CurrentBalance);
        Assert.Contains(transaction, account.Transactions);
        Assert.Equal("Salary", transaction.Description);
        Assert.Equal(transactionAmount, transaction.Amount);
        Assert.Equal((int)CategoryEnum.SALARY, transaction.CategoryId);
        Assert.Equal((int)TransactionTypeEnum.INCOME, transaction.TransactionTypeId);
    }

    [Fact]
    public void AddTransaction_ExpenseShouldDecreaseBalance()
    {
        var account = new Account(Guid.NewGuid(), AccountTypeEnum.CHECKING, "Test Account", 1000m);
        var initialBalance = account.CurrentBalance;
        var transactionAmount = 300m;

        var transaction = account.AddTransaction(
            "Groceries",
            transactionAmount,
            CategoryEnum.FOOD,
            TransactionTypeEnum.EXPENSE,
            DateTime.Now);

        Assert.Equal(initialBalance - transactionAmount, account.CurrentBalance);
        Assert.Contains(transaction, account.Transactions);
        Assert.Equal("Groceries", transaction.Description);
        Assert.Equal(transactionAmount, transaction.Amount);
        Assert.Equal((int)CategoryEnum.FOOD, transaction.CategoryId);
        Assert.Equal((int)TransactionTypeEnum.EXPENSE, transaction.TransactionTypeId);
    }

    [Fact]
    public void AddTransaction_WithZeroAmount_ShouldNotChangeBalance()
    {
        var account = new Account(Guid.NewGuid(), AccountTypeEnum.CHECKING, "Test Account", 1000m);
        var initialBalance = account.CurrentBalance;

        var transaction = account.AddTransaction(
            "Zero Transaction",
            0m,
            CategoryEnum.OTHERS,
            TransactionTypeEnum.EXPENSE,
            DateTime.Now);

        Assert.Equal(initialBalance, account.CurrentBalance);
        Assert.Contains(transaction, account.Transactions);
        Assert.Equal(0m, transaction.Amount);
        Assert.Equal((int)TransactionTypeEnum.EXPENSE, transaction.TransactionTypeId);
    }

    [Fact]
    public void AddTransaction_WithEmptyDescription_ShouldThrowDomainException()
    {
        var account = new Account(Guid.NewGuid(), AccountTypeEnum.CHECKING, "Test Account", 1000m);

        var exception = Assert.Throws<DomainException>(() =>
            account.AddTransaction(
                "",
                100m,
                CategoryEnum.FOOD,
                TransactionTypeEnum.EXPENSE,
                DateTime.Now));

        Assert.Contains("description cannot be empty", exception.Message);
    }

    [Fact]
    public void AddTransaction_WithNegativeAmount_ShouldThrowDomainException()
    {
        var account = new Account(Guid.NewGuid(), AccountTypeEnum.CHECKING, "Test Account", 1000m);

        var exception = Assert.Throws<DomainException>(() =>
            account.AddTransaction(
                "Test Transaction",
                -100m,
                CategoryEnum.FOOD,
                TransactionTypeEnum.EXPENSE,
                DateTime.Now));

        Assert.Contains("Transaction amount cannot be negative", exception.Message);
    }

    [Fact]
    public void AddTransaction_WithLargeAmount_ShouldUpdateBalanceCorrectly()
    {
        var account = new Account(Guid.NewGuid(), AccountTypeEnum.CHECKING, "Test Account", 1000m);
        var initialBalance = account.CurrentBalance;
        var largeAmount = 999999999.99m;

        var transaction = account.AddTransaction(
            "Large Transaction",
            largeAmount,
            CategoryEnum.OTHERS,
            TransactionTypeEnum.INCOME,
            DateTime.Now);

        Assert.Equal(initialBalance + largeAmount, account.CurrentBalance);
        Assert.Equal(largeAmount, transaction.Amount);
    }

    [Fact]
    public void AddTransaction_MultipleTransactions_ShouldUpdateBalanceCorrectly()
    {
        var account = new Account(Guid.NewGuid(), AccountTypeEnum.CHECKING, "Test Account", 1000m);
        var initialBalance = account.CurrentBalance;

        account.AddTransaction("Income 1", 200m, CategoryEnum.SALARY, TransactionTypeEnum.INCOME, DateTime.Now);
        account.AddTransaction("Expense 1", 50m, CategoryEnum.FOOD, TransactionTypeEnum.EXPENSE, DateTime.Now);
        account.AddTransaction("Income 2", 300m, CategoryEnum.OTHERS, TransactionTypeEnum.INCOME, DateTime.Now);
        account.AddTransaction("Expense 2", 150m, CategoryEnum.ENTERTAINMENT, TransactionTypeEnum.EXPENSE, DateTime.Now);

        var expectedBalance = initialBalance + 200m - 50m + 300m - 150m;

        Assert.Equal(expectedBalance, account.CurrentBalance);
        Assert.Equal(4, account.Transactions.Count);
    }

    [Fact]
    public void AddTransaction_WithFutureDate_ShouldStillAddTransaction()
    {
        var account = new Account(Guid.NewGuid(), AccountTypeEnum.CHECKING, "Test Account", 1000m);
        var futureDate = DateTime.Now.AddMonths(1);

        var transaction = account.AddTransaction(
            "Future Transaction",
            100m,
            CategoryEnum.OTHERS,
            TransactionTypeEnum.INCOME,
            futureDate);

        Assert.Equal(1100m, account.CurrentBalance);
        Assert.Equal(futureDate, transaction.Date);
    }

    [Fact]
    public void AddTransaction_WithPastDate_ShouldAddTransaction()
    {
        var account = new Account(Guid.NewGuid(), AccountTypeEnum.CHECKING, "Test Account", 1000m);
        var pastDate = DateTime.Now.AddYears(-1);

        var transaction = account.AddTransaction(
            "Past Transaction",
            100m,
            CategoryEnum.OTHERS,
            TransactionTypeEnum.EXPENSE,
            pastDate);

        Assert.Equal(900m, account.CurrentBalance);
        Assert.Equal(pastDate, transaction.Date);
    }

    [Fact]
    public void AddTransaction_ShouldAddDomainEvent()
    {
        var userId = Guid.NewGuid();
        var account = new Account(userId, AccountTypeEnum.CHECKING, "Test Account", 1000m);

        account.AddTransaction(
            "Test Transaction",
            100m,
            CategoryEnum.FOOD,
            TransactionTypeEnum.EXPENSE,
            DateTime.Now);

        Assert.Single(account.DomainEvents);
        var budgetEvent = Assert.IsType<BudgetEvaluationRequestedDomainEvent>(account.DomainEvents.First());
        Assert.Equal(account.Id, budgetEvent.AccountId);
        Assert.Equal(userId, budgetEvent.UserId);
        Assert.Equal(CategoryEnum.FOOD, budgetEvent.Category);
    }

    [Fact]
    public void AddTransaction_MaxTransactionAmount_ShouldUpdateBalanceCorrectly()
    {
        var account = new Account(Guid.NewGuid(), AccountTypeEnum.CHECKING, "Test Account", 1000m);
        var maxAmount = decimal.MaxValue / 1000000;

        var transaction = account.AddTransaction(
            "Max Transaction",
            maxAmount,
            CategoryEnum.OTHERS,
            TransactionTypeEnum.INCOME,
            DateTime.Now);

        Assert.Equal(1000m + maxAmount, account.CurrentBalance);
        Assert.Equal(maxAmount, transaction.Amount);
    }

    [Fact]
    public void EditTransaction_ShouldUpdateBalanceCorrectly()
    {
        var account = new Account(Guid.NewGuid(), AccountTypeEnum.CHECKING, "Test Account", 1000m);
        var transaction = account.AddTransaction(
            "Groceries",
            300m,
            CategoryEnum.FOOD,
            TransactionTypeEnum.EXPENSE,
            DateTime.Now);

        Assert.Equal(700m, account.CurrentBalance);

        account.EditTransaction(
            transaction.Id,
            200m,
            "Updated Groceries",
            CategoryEnum.FOOD,
            TransactionTypeEnum.EXPENSE);

        Assert.Equal(800m, account.CurrentBalance);
        Assert.Equal("Updated Groceries", transaction.Description);
        Assert.Equal(200m, transaction.Amount);
    }

    [Fact]
    public void EditTransaction_ChangingTypeFromExpenseToIncome_ShouldUpdateBalanceCorrectly()
    {
        var account = new Account(Guid.NewGuid(), AccountTypeEnum.CHECKING, "Test Account", 1000m);
        var transaction = account.AddTransaction(
            "Refund",
            300m,
            CategoryEnum.OTHERS,
            TransactionTypeEnum.EXPENSE,
            DateTime.Now);

        Assert.Equal(700m, account.CurrentBalance);

        account.EditTransaction(
            transaction.Id,
            300m,
            "Corrected Refund",
            CategoryEnum.OTHERS,
            TransactionTypeEnum.INCOME);

        Assert.Equal(1300m, account.CurrentBalance);
        Assert.Equal((int)TransactionTypeEnum.INCOME, transaction.TransactionTypeId);
    }

    [Fact]
    public void EditTransaction_ChangingTypeFromIncomeToExpense_ShouldUpdateBalanceCorrectly()
    {
        var account = new Account(Guid.NewGuid(), AccountTypeEnum.CHECKING, "Test Account", 1000m);
        var transaction = account.AddTransaction(
            "Purchase",
            300m,
            CategoryEnum.OTHERS,
            TransactionTypeEnum.INCOME,
            DateTime.Now);

        Assert.Equal(1300m, account.CurrentBalance);

        account.EditTransaction(
            transaction.Id,
            300m,
            "Corrected Purchase",
            CategoryEnum.OTHERS,
            TransactionTypeEnum.EXPENSE);

        Assert.Equal(700m, account.CurrentBalance);
        Assert.Equal((int)TransactionTypeEnum.EXPENSE, transaction.TransactionTypeId);
    }

    [Fact]
    public void EditTransaction_IncreaseIncomeAmount_ShouldUpdateBalanceCorrectly()
    {
        var account = new Account(Guid.NewGuid(), AccountTypeEnum.CHECKING, "Test Account", 1000m);
        var transaction = account.AddTransaction(
            "Income",
            300m,
            CategoryEnum.OTHERS,
            TransactionTypeEnum.INCOME,
            DateTime.Now);

        account.EditTransaction(
            transaction.Id,
            500m,
            "Updated Income",
            CategoryEnum.OTHERS,
            TransactionTypeEnum.INCOME);

        Assert.Equal(1500m, account.CurrentBalance);
    }

    [Fact]
    public void EditTransaction_IncreaseExpenseAmount_ShouldUpdateBalanceCorrectly()
    {
        var account = new Account(Guid.NewGuid(), AccountTypeEnum.CHECKING, "Test Account", 1000m);
        var transaction = account.AddTransaction(
            "Expense",
            300m,
            CategoryEnum.OTHERS,
            TransactionTypeEnum.EXPENSE,
            DateTime.Now);

        account.EditTransaction(
            transaction.Id,
            500m,
            "Updated Expense",
            CategoryEnum.OTHERS,
            TransactionTypeEnum.EXPENSE);

        Assert.Equal(500m, account.CurrentBalance);
    }

    [Fact]
    public void EditTransaction_SameTypeButDifferentAmount_ShouldUpdateBalanceCorrectly()
    {
        var account = new Account(Guid.NewGuid(), AccountTypeEnum.CHECKING, "Test Account", 1000m);
        var transaction = account.AddTransaction(
            "Original Transaction",
            300m,
            CategoryEnum.OTHERS,
            TransactionTypeEnum.INCOME,
            DateTime.Now);

        account.EditTransaction(
            transaction.Id,
            350m,
            "Updated Transaction",
            CategoryEnum.OTHERS,
            TransactionTypeEnum.INCOME);

        Assert.Equal(1350m, account.CurrentBalance);
        Assert.Equal("Updated Transaction", transaction.Description);
        Assert.Equal(350m, transaction.Amount);
    }

    [Fact]
    public void EditTransaction_ChangingBothTypeAndAmount_ShouldUpdateBalanceCorrectly()
    {
        var account = new Account(Guid.NewGuid(), AccountTypeEnum.CHECKING, "Test Account", 1000m);
        var transaction = account.AddTransaction(
            "Original Transaction",
            300m,
            CategoryEnum.OTHERS,
            TransactionTypeEnum.EXPENSE,
            DateTime.Now);

        account.EditTransaction(
            transaction.Id,
            500m,
            "Updated Transaction",
            CategoryEnum.SALARY,
            TransactionTypeEnum.INCOME);

        Assert.Equal(1500m, account.CurrentBalance);
        Assert.Equal("Updated Transaction", transaction.Description);
        Assert.Equal(500m, transaction.Amount);
        Assert.Equal((int)TransactionTypeEnum.INCOME, transaction.TransactionTypeId);
        Assert.Equal((int)CategoryEnum.SALARY, transaction.CategoryId);
    }

    [Fact]
    public void EditTransaction_ToZeroAmount_ShouldUpdateBalanceCorrectly()
    {
        var account = new Account(Guid.NewGuid(), AccountTypeEnum.CHECKING, "Test Account", 1000m);
        var transaction = account.AddTransaction(
            "Original Transaction",
            300m,
            CategoryEnum.OTHERS,
            TransactionTypeEnum.EXPENSE,
            DateTime.Now);

        account.EditTransaction(
            transaction.Id,
            0m,
            "Updated Transaction",
            CategoryEnum.OTHERS,
            TransactionTypeEnum.EXPENSE);

        Assert.Equal(1000m, account.CurrentBalance);
        Assert.Equal(0m, transaction.Amount);
    }

    [Fact]
    public void EditTransaction_FromZeroToNonZero_ShouldUpdateBalanceCorrectly()
    {
        var account = new Account(Guid.NewGuid(), AccountTypeEnum.CHECKING, "Test Account", 1000m);
        var transaction = account.AddTransaction(
            "Original Transaction",
            0m,
            CategoryEnum.OTHERS,
            TransactionTypeEnum.EXPENSE,
            DateTime.Now);

        account.EditTransaction(
            transaction.Id,
            250m,
            "Updated Transaction",
            CategoryEnum.OTHERS,
            TransactionTypeEnum.EXPENSE);

        Assert.Equal(750m, account.CurrentBalance);
        Assert.Equal(250m, transaction.Amount);
    }

    [Fact]
    public void EditTransaction_SameAmountButDifferentCategory_ShouldNotAffectBalance()
    {
        var account = new Account(Guid.NewGuid(), AccountTypeEnum.CHECKING, "Test Account", 1000m);
        var transaction = account.AddTransaction(
            "Original Transaction",
            300m,
            CategoryEnum.ENTERTAINMENT,
            TransactionTypeEnum.EXPENSE,
            DateTime.Now);

        account.EditTransaction(
            transaction.Id,
            300m,
            "Updated Transaction",
            CategoryEnum.FOOD,
            TransactionTypeEnum.EXPENSE);

        Assert.Equal(700m, account.CurrentBalance);
        Assert.Equal((int)CategoryEnum.FOOD, transaction.CategoryId);
    }

    [Fact]
    public void EditTransaction_MultipleEditsToSameTransaction_ShouldTrackBalanceCorrectly()
    {
        var account = new Account(Guid.NewGuid(), AccountTypeEnum.CHECKING, "Test Account", 1000m);
        var transaction = account.AddTransaction(
            "Original Transaction",
            300m,
            CategoryEnum.OTHERS,
            TransactionTypeEnum.EXPENSE,
            DateTime.Now);

        account.EditTransaction(transaction.Id, 200m, "First Update", CategoryEnum.OTHERS, TransactionTypeEnum.EXPENSE);
        Assert.Equal(800m, account.CurrentBalance);

        account.EditTransaction(transaction.Id, 200m, "Second Update", CategoryEnum.OTHERS, TransactionTypeEnum.INCOME);
        Assert.Equal(1200m, account.CurrentBalance);

        account.EditTransaction(transaction.Id, 350m, "Third Update", CategoryEnum.OTHERS, TransactionTypeEnum.INCOME);
        Assert.Equal(1350m, account.CurrentBalance);
    }

    [Fact]
    public void EditTransaction_NonExistentTransactionId_ShouldThrowDomainException()
    {
        var account = new Account(Guid.NewGuid(), AccountTypeEnum.CHECKING, "Test Account", 1000m);
        var nonExistentId = Guid.NewGuid();

        var exception = Assert.Throws<DomainException>(() =>
            account.EditTransaction(
                nonExistentId,
                100m,
                "Test Transaction",
                CategoryEnum.OTHERS,
                TransactionTypeEnum.EXPENSE));

        Assert.Contains($"Transaction with id {nonExistentId} not found", exception.Message);
    }

    [Fact]
    public void EditTransaction_EmptyDescription_ShouldThrowDomainException()
    {
        var account = new Account(Guid.NewGuid(), AccountTypeEnum.CHECKING, "Test Account", 1000m);
        var transaction = account.AddTransaction(
            "Original Transaction",
            300m,
            CategoryEnum.OTHERS,
            TransactionTypeEnum.EXPENSE,
            DateTime.Now);

        var exception = Assert.Throws<DomainException>(() =>
            account.EditTransaction(
                transaction.Id,
                100m,
                "",
                CategoryEnum.OTHERS,
                TransactionTypeEnum.EXPENSE));

        Assert.Contains("Transaction description cannot be empty", exception.Message);
    }

    [Fact]
    public void EditTransaction_NegativeAmount_ShouldThrowDomainException()
    {
        var account = new Account(Guid.NewGuid(), AccountTypeEnum.CHECKING, "Test Account", 1000m);
        var transaction = account.AddTransaction(
            "Original Transaction",
            300m,
            CategoryEnum.OTHERS,
            TransactionTypeEnum.EXPENSE,
            DateTime.Now);

        var exception = Assert.Throws<DomainException>(() =>
            account.EditTransaction(
                transaction.Id,
                -100m,
                "Updated Transaction",
                CategoryEnum.OTHERS,
                TransactionTypeEnum.EXPENSE));

        Assert.Contains("Transaction amount cannot be negative", exception.Message);
    }

    [Fact]
    public void DeleteTransaction_ShouldUpdateBalanceCorrectly()
    {
        var account = new Account(Guid.NewGuid(), AccountTypeEnum.CHECKING, "Test Account", 1000m);
        var transaction = account.AddTransaction(
            "Groceries",
            300m,
            CategoryEnum.FOOD,
            TransactionTypeEnum.EXPENSE,
            DateTime.Now);

        Assert.Equal(700m, account.CurrentBalance);

        account.DeleteTransaction(transaction.Id);

        Assert.Equal(1000m, account.CurrentBalance);
    }

    [Fact]
    public void Constructor_WithEmptyName_ShouldThrowDomainException()
    {
        var exception = Assert.Throws<DomainException>(() =>
            new Account(Guid.NewGuid(), AccountTypeEnum.CHECKING, "", 1000m));

        Assert.Contains("Account name cannot be empty", exception.Message);
    }

    [Fact]
    public void DeleteAccount_ShouldRemoveAllTransactionsAndMarkAsDeleted()
    {
        var account = new Account(Guid.NewGuid(), AccountTypeEnum.CHECKING, "Test Account", 1000m);

        account.AddTransaction("Transaction 1", 200m, CategoryEnum.FOOD, TransactionTypeEnum.EXPENSE, DateTime.Now);
        account.AddTransaction("Transaction 2", 300m, CategoryEnum.SALARY, TransactionTypeEnum.INCOME, DateTime.Now);

        account.DeleteAccount();

        Assert.False(account.IsActive);
        Assert.Equal(1000m, account.CurrentBalance);
        Assert.All(account.Transactions, transaction => Assert.False(transaction.IsActive));
    }

    [Fact]
    public void DeleteTransaction_WithNonExistentId_ShouldThrowDomainException()
    {
        var account = new Account(Guid.NewGuid(), AccountTypeEnum.CHECKING, "Test Account", 1000m);
        var nonExistentId = Guid.NewGuid();

        var exception = Assert.Throws<DomainException>(() => account.DeleteTransaction(nonExistentId));

        Assert.Contains($"Transaction with id {nonExistentId} not found", exception.Message);
    }

    [Fact]
    public void DeleteTransaction_WithMultipleTransactions_ShouldOnlyDeleteSpecificTransaction()
    {
        var account = new Account(Guid.NewGuid(), AccountTypeEnum.CHECKING, "Test Account", 1000m);

        account.AddTransaction("Transaction 1", 200m, CategoryEnum.FOOD, TransactionTypeEnum.EXPENSE, DateTime.Now);
        var transaction2 = account.AddTransaction("Transaction 2", 300m, CategoryEnum.SALARY, TransactionTypeEnum.INCOME, DateTime.Now);
        account.AddTransaction("Transaction 3", 100m, CategoryEnum.OTHERS, TransactionTypeEnum.EXPENSE, DateTime.Now);

        Assert.Equal(1000m, account.CurrentBalance);

        account.DeleteTransaction(transaction2.Id);

        Assert.False(transaction2.IsActive);
        Assert.Equal(700m, account.CurrentBalance);
    }
}
