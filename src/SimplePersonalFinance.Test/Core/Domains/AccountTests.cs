using SimplePersonalFinance.Core.Domain.Entities;
using SimplePersonalFinance.Core.Domain.Enums;

namespace SimplePersonalFinance.Test.Core.Domain.Entities;

public class AccountTests
{
    [Fact]
    public void Constructor_ShouldInitializeCorrectly()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var name = "Test Account";
        var initialBalance = 1000m;

        // Act
        var account = new Account(userId, AccountTypeEnum.CHECKING, name, initialBalance);

        // Assert
        Assert.Equal(userId, account.UserId);
        Assert.Equal((int)AccountTypeEnum.CHECKING, account.AccountTypeId);
        Assert.Equal(name, account.Name);
        Assert.Equal(initialBalance, account.InitialBalance);
        Assert.Equal(initialBalance, account.CurrentBalance);
        Assert.Empty(account.Transactions);
    }

    [Fact]
    public void AddTransaction_IncomeShouldIncreaseBalance()
    {
        // Arrange
        var account = new Account(Guid.NewGuid(), AccountTypeEnum.CHECKING, "Test Account", 1000m);
        var initialBalance = account.CurrentBalance;
        var transactionAmount = 500m;

        // Act
        var transaction = account.AddTransaction(
            "Salary",
            transactionAmount,
            CategoryEnum.SALARY,
            TransactionTypeEnum.INCOME,
            DateTime.Now);

        // Assert
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
        // Arrange
        var account = new Account(Guid.NewGuid(), AccountTypeEnum.CHECKING, "Test Account", 1000m);
        var initialBalance = account.CurrentBalance;
        var transactionAmount = 300m;

        // Act
        var transaction = account.AddTransaction(
            "Groceries",
            transactionAmount,
            CategoryEnum.FOOD,
            TransactionTypeEnum.EXPENSE,
            DateTime.Now);

        // Assert
        Assert.Equal(initialBalance - transactionAmount, account.CurrentBalance);
        Assert.Contains(transaction, account.Transactions);
        Assert.Equal("Groceries", transaction.Description);
        Assert.Equal(transactionAmount, transaction.Amount);
        Assert.Equal((int)CategoryEnum.FOOD, transaction.CategoryId);
        Assert.Equal((int)TransactionTypeEnum.EXPENSE, transaction.TransactionTypeId);
    }

    [Fact]
    public void EditTransaction_ShouldUpdateBalanceCorrectly()
    {
        // Arrange
        var account = new Account(Guid.NewGuid(), AccountTypeEnum.CHECKING, "Test Account", 1000m);
        var transaction = account.AddTransaction(
            "Groceries",
            300m,
            CategoryEnum.FOOD,
            TransactionTypeEnum.EXPENSE,
            DateTime.Now);

        // Balance should be 700 after expense
        Assert.Equal(700m, account.CurrentBalance);

        // Act - Change expense amount to 200
        account.EditTransaction(
            transaction.Id,
            200m,
            "Updated Groceries",
            CategoryEnum.FOOD,
            TransactionTypeEnum.EXPENSE);

        // Assert - Balance should now be 800 (original 1000 - new expense 200)
        Assert.Equal(800m, account.CurrentBalance);
        Assert.Equal("Updated Groceries", transaction.Description);
        Assert.Equal(200m, transaction.Amount);
    }

    [Fact]
    public void EditTransaction_ChangingTypeFromExpenseToIncome_ShouldUpdateBalanceCorrectly()
    {
        // Arrange
        var account = new Account(Guid.NewGuid(), AccountTypeEnum.CHECKING, "Test Account", 1000m);
        var transaction = account.AddTransaction(
            "Refund",
            300m,
            CategoryEnum.OTHERS,
            TransactionTypeEnum.EXPENSE,
            DateTime.Now);

        // Balance should be 700 after expense
        Assert.Equal(700m, account.CurrentBalance);

        // Act - Change transaction type from expense to income
        account.EditTransaction(
            transaction.Id,
            300m,
            "Corrected Refund",
            CategoryEnum.OTHERS,
            TransactionTypeEnum.INCOME);

        // Assert - Balance should now be 1300 (original 1000 + 300 instead of - 300)
        Assert.Equal(1300m, account.CurrentBalance);
        Assert.Equal((int)TransactionTypeEnum.INCOME, transaction.TransactionTypeId);
    }

    [Fact]
    public void EditTransaction_ChangingTypeFromIncomeToExpense_ShouldUpdateBalanceCorrectly()
    {
        // Arrange
        var account = new Account(Guid.NewGuid(), AccountTypeEnum.CHECKING, "Test Account", 1000m);
        var transaction = account.AddTransaction(
            "COMPRA",
            300m,
            CategoryEnum.OTHERS,
            TransactionTypeEnum.INCOME,
            DateTime.Now);


        Assert.Equal(1300m, account.CurrentBalance);

        // Act
        account.EditTransaction(
            transaction.Id,
            300m,
            "Corrected COMPRA",
            CategoryEnum.OTHERS,
            TransactionTypeEnum.EXPENSE);


        Assert.Equal(700m, account.CurrentBalance);
        Assert.Equal((int)TransactionTypeEnum.EXPENSE, transaction.TransactionTypeId);
    }

    [Fact]
    public void EditTransaction_IncreaseAmount_ShouldUpdateBalanceCorrectly()
    {
        // Arrange
        var account = new Account(Guid.NewGuid(), AccountTypeEnum.CHECKING, "Test Account", 1000m);
        var transaction = account.AddTransaction(
            "Teste",
            300m,
            CategoryEnum.OTHERS,
            TransactionTypeEnum.INCOME,
            DateTime.Now);

        // Balance should be 700 after expense
        Assert.Equal(1300m, account.CurrentBalance);

        // Act - Change transaction type from expense to income
        account.EditTransaction(
            transaction.Id,
            500m,
            "Corrected Teste",
            CategoryEnum.OTHERS,
            TransactionTypeEnum.INCOME);

        // Assert - Balance should now be 1300 (original 1000 + 300 instead of - 300)
        Assert.Equal(1500m, account.CurrentBalance);
        Assert.Equal((int)TransactionTypeEnum.INCOME, transaction.TransactionTypeId);
    }

    [Fact]
    public void EditTransaction_DecreaseAmount_ShouldUpdateBalanceCorrectly()
    {
        // Arrange
        var account = new Account(Guid.NewGuid(), AccountTypeEnum.CHECKING, "Test Account", 1000m);
        var transaction = account.AddTransaction(
            "Teste",
            300m,
            CategoryEnum.OTHERS,
            TransactionTypeEnum.EXPENSE,
            DateTime.Now);

        // Balance should be 700 after expense
        Assert.Equal(700m, account.CurrentBalance);

        // Act - Change transaction type from expense to income
        account.EditTransaction(
            transaction.Id,
            500m,
            "Corrected Teste",
            CategoryEnum.OTHERS,
            TransactionTypeEnum.EXPENSE);

        // Assert - Balance should now be 1300 (original 1000 + 300 instead of - 300)
        Assert.Equal(500m, account.CurrentBalance);
        Assert.Equal((int)TransactionTypeEnum.EXPENSE, transaction.TransactionTypeId);
    }

    [Fact]
    public void DeleteTransaction_ShouldUpdateBalanceCorrectly()
    {
        // Arrange
        var account = new Account(Guid.NewGuid(), AccountTypeEnum.CHECKING, "Test Account", 1000m);
        var transaction = account.AddTransaction(
            "Groceries",
            300m,
            CategoryEnum.FOOD,
            TransactionTypeEnum.EXPENSE,
            DateTime.Now);

        // Balance should be 700 after expense
        Assert.Equal(700m, account.CurrentBalance);

        // Act
        account.DeleteTransaction(transaction.Id);

        // Assert
        Assert.Equal(1000m, account.CurrentBalance); // Balance should be back to original
    }
}
