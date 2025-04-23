using SimplePersonalFinance.Core.Domain.Entities;
using SimplePersonalFinance.Core.Domain.Enums;
using SimplePersonalFinance.Core.Domain.Events;
using SimplePersonalFinance.Core.Domain.Exceptions;
using SimplePersonalFinance.Core.Domain.ValueObjects;

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
        Assert.Equal(initialBalance, account.InitialBalance.Amount);
        Assert.Equal(initialBalance, account.CurrentBalance.Amount);
        Assert.Empty(account.Transactions);
    }

    [Fact]
    public void AddTransaction_IncomeShouldIncreaseBalance()
    {
        // Arrange
        var account = new Account(Guid.NewGuid(), AccountTypeEnum.CHECKING, "Test Account", 1000m);
        var initialBalance = account.CurrentBalance.Amount;
        var transactionAmount = 500m;

        // Act
        var transaction = account.AddTransaction(
            "Salary",
            transactionAmount,
            CategoryEnum.SALARY,
            TransactionTypeEnum.INCOME,
            DateTime.Now);

        // Assert
        Assert.Equal(initialBalance + transactionAmount, account.CurrentBalance.Amount);
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
        Assert.Equal(initialBalance.Amount - transactionAmount, account.CurrentBalance.Amount);
        Assert.Contains(transaction, account.Transactions);
        Assert.Equal("Groceries", transaction.Description);
        Assert.Equal(transactionAmount, transaction.Amount);
        Assert.Equal((int)CategoryEnum.FOOD, transaction.CategoryId);
        Assert.Equal((int)TransactionTypeEnum.EXPENSE, transaction.TransactionTypeId);
    }

    [Fact]
    public void AddTransaction_WithZeroAmount_ShouldNotChangeBalance()
    {
        // Arrange
        var account = new Account(Guid.NewGuid(), AccountTypeEnum.CHECKING, "Test Account", 1000m);
        var initialBalance = account.CurrentBalance.Amount;

        // Act
        var transaction = account.AddTransaction(
            "Zero Transaction",
            0m, // Zero amount
            CategoryEnum.OTHERS,
            TransactionTypeEnum.EXPENSE,
            DateTime.Now);

        // Assert
        Assert.Equal(initialBalance, account.CurrentBalance.Amount); // Balance shouldn't change
        Assert.Contains(transaction, account.Transactions);
        Assert.Equal(0m, transaction.Amount);
        Assert.Equal((int)TransactionTypeEnum.EXPENSE, transaction.TransactionTypeId);
    }

    [Fact]
    public void AddTransaction_WithEmptyDescription_ShouldThrowDomainException()
    {
        // Arrange
        var account = new Account(Guid.NewGuid(), AccountTypeEnum.CHECKING, "Test Account", 1000m);

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() =>
            account.AddTransaction(
                "", // Empty description
                100m,
                CategoryEnum.FOOD,
                TransactionTypeEnum.EXPENSE,
                DateTime.Now));

        Assert.Contains("description cannot be empty", exception.Message);
    }

    [Fact]
    public void AddTransaction_WithNegativeAmount_ShouldThrowDomainException()
    {
        // Arrange
        var account = new Account(Guid.NewGuid(), AccountTypeEnum.CHECKING, "Test Account", 1000m);

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() =>
            account.AddTransaction(
                "Test Transaction",
                -100m, // Negative amount
                CategoryEnum.FOOD,
                TransactionTypeEnum.EXPENSE,
                DateTime.Now));

        Assert.Contains("Transaction amount cannot be negative", exception.Message);
    }

    [Fact]
    public void AddTransaction_WithLargeAmount_ShouldUpdateBalanceCorrectly()
    {
        // Arrange
        var account = new Account(Guid.NewGuid(), AccountTypeEnum.CHECKING, "Test Account", 1000m);
        var initialBalance = account.CurrentBalance.Amount;
        var largeAmount = 999999999.99m; // Very large amount to test decimal precision

        // Act
        var transaction = account.AddTransaction(
            "Large Transaction",
            largeAmount,
            CategoryEnum.OTHERS,
            TransactionTypeEnum.INCOME,
            DateTime.Now);

        // Assert
        Assert.Equal(initialBalance + largeAmount, account.CurrentBalance.Amount);
        Assert.Equal(largeAmount, transaction.Amount);
    }

    [Fact]
    public void AddTransaction_MultipleTransactions_ShouldUpdateBalanceCorrectly()
    {
        // Arrange
        var account = new Account(Guid.NewGuid(), AccountTypeEnum.CHECKING, "Test Account", 1000m);
        var initialBalance = account.CurrentBalance.Amount;

        // Act - Add multiple transactions of different types
        var transaction1 = account.AddTransaction("Income 1", 200m, CategoryEnum.SALARY, TransactionTypeEnum.INCOME, DateTime.Now);
        var transaction2 = account.AddTransaction("Expense 1", 50m, CategoryEnum.FOOD, TransactionTypeEnum.EXPENSE, DateTime.Now);
        var transaction3 = account.AddTransaction("Income 2", 300m, CategoryEnum.OTHERS, TransactionTypeEnum.INCOME, DateTime.Now);
        var transaction4 = account.AddTransaction("Expense 2", 150m, CategoryEnum.ENTERTAINMENT, TransactionTypeEnum.EXPENSE, DateTime.Now);

        // Calculate expected balance: 1000 + 200 - 50 + 300 - 150 = 1300
        var expectedBalance = initialBalance + 200m - 50m + 300m - 150m;

        // Assert
        Assert.Equal(expectedBalance, account.CurrentBalance.Amount);
        Assert.Equal(4, account.Transactions.Count);
    }

    [Fact]
    public void AddTransaction_WithFutureDate_ShouldStillAddTransaction()
    {
        // Arrange
        var account = new Account(Guid.NewGuid(), AccountTypeEnum.CHECKING, "Test Account", 1000m);
        var futureDate = DateTime.Now.AddMonths(1); // One month in the future

        // Act
        var transaction = account.AddTransaction(
            "Future Transaction",
            100m,
            CategoryEnum.OTHERS,
            TransactionTypeEnum.INCOME,
            futureDate);

        // Assert
        Assert.Equal(1100m, account.CurrentBalance.Amount);
        Assert.Equal(futureDate, transaction.Date);
    }

    [Fact]
    public void AddTransaction_WithPastDate_ShouldAddTransaction()
    {
        // Arrange
        var account = new Account(Guid.NewGuid(), AccountTypeEnum.CHECKING, "Test Account", 1000m);
        var pastDate = DateTime.Now.AddYears(-1); // One year in the past

        // Act
        var transaction = account.AddTransaction(
            "Past Transaction",
            100m,
            CategoryEnum.OTHERS,
            TransactionTypeEnum.EXPENSE,
            pastDate);

        // Assert
        Assert.Equal(900m, account.CurrentBalance.Amount);
        Assert.Equal(pastDate, transaction.Date);
    }

    [Fact]
    public void AddTransaction_ShouldAddDomainEvent()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var account = new Account(userId, AccountTypeEnum.CHECKING, "Test Account", 1000m);

        // Act
        account.AddTransaction(
            "Test Transaction",
            100m,
            CategoryEnum.FOOD,
            TransactionTypeEnum.EXPENSE,
            DateTime.Now);

        // Assert
        Assert.Single(account.DomainEvents);
        var domainEvent = account.DomainEvents.First();
        Assert.IsType<BudgetEvaluationRequestedDomainEvent>(domainEvent);

        var budgetEvent = (BudgetEvaluationRequestedDomainEvent)domainEvent;
        Assert.Equal(account.Id, budgetEvent.AccountId);
        Assert.Equal(userId, budgetEvent.UserId);
        Assert.Equal(CategoryEnum.FOOD, budgetEvent.Category);
    }

    [Fact]
    public void AddTransaction_MaxTransactionAmount_ShouldUpdateBalanceCorrectly()
    {
        // Arrange
        var account = new Account(Guid.NewGuid(), AccountTypeEnum.CHECKING, "Test Account", 1000m);
        var maxAmount = decimal.MaxValue / 1000000; // Avoid overflow but still test large values

        // Act
        var transaction = account.AddTransaction(
            "Max Transaction",
            maxAmount,
            CategoryEnum.OTHERS,
            TransactionTypeEnum.INCOME,
            DateTime.Now);

        // Assert
        Assert.Equal(1000m + maxAmount, account.CurrentBalance.Amount);
        Assert.Equal(maxAmount, transaction.Amount);
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
        Assert.Equal(700m, account.CurrentBalance.Amount);

        // Act - Change expense amount to 200
        account.EditTransaction(
            transaction.Id,
            200m,
            "Updated Groceries",
            CategoryEnum.FOOD,
            TransactionTypeEnum.EXPENSE);

        // Assert - Balance should now be 800 (original 1000 - new expense 200)
        Assert.Equal(800m, account.CurrentBalance.Amount);
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
        Assert.Equal(700m, account.CurrentBalance.Amount);

        // Act - Change transaction type from expense to income
        account.EditTransaction(
            transaction.Id,
            300m,
            "Corrected Refund",
            CategoryEnum.OTHERS,
            TransactionTypeEnum.INCOME);

        // Assert - Balance should now be 1300 (original 1000 + 300 instead of - 300)
        Assert.Equal(1300m, account.CurrentBalance.Amount);
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


        Assert.Equal(1300m, account.CurrentBalance.Amount);

        // Act
        account.EditTransaction(
            transaction.Id,
            300m,
            "Corrected COMPRA",
            CategoryEnum.OTHERS,
            TransactionTypeEnum.EXPENSE);


        Assert.Equal(700m, account.CurrentBalance.Amount);
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
        Assert.Equal(1300m, account.CurrentBalance.Amount);

        // Act - Change transaction type from expense to income
        account.EditTransaction(
            transaction.Id,
            500m,
            "Corrected Teste",
            CategoryEnum.OTHERS,
            TransactionTypeEnum.INCOME);

        // Assert - Balance should now be 1300 (original 1000 + 300 instead of - 300)
        Assert.Equal(1500m, account.CurrentBalance.Amount);
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
        Assert.Equal(700m, account.CurrentBalance.Amount);

        // Act - Change transaction type from expense to income
        account.EditTransaction(
            transaction.Id,
            500m,
            "Corrected Teste",
            CategoryEnum.OTHERS,
            TransactionTypeEnum.EXPENSE);

        // Assert - Balance should now be 1300 (original 1000 + 300 instead of - 300)
        Assert.Equal(500m, account.CurrentBalance.Amount);
        Assert.Equal((int)TransactionTypeEnum.EXPENSE, transaction.TransactionTypeId);
    }

    [Fact]
    public void EditTransaction_SameTypeButDifferentAmount_ShouldUpdateBalanceCorrectly()
    {
        // Arrange
        var account = new Account(Guid.NewGuid(), AccountTypeEnum.CHECKING, "Test Account", 1000m);
        var transaction = account.AddTransaction(
            "Original Transaction",
            300m,
            CategoryEnum.OTHERS,
            TransactionTypeEnum.INCOME,
            DateTime.Now);

        // Balance should be 1300 after income
        Assert.Equal(1300m, account.CurrentBalance.Amount);

        // Act - Change amount but keep same type
        account.EditTransaction(
            transaction.Id,
            350m, // Increased by 50
            "Updated Transaction",
            CategoryEnum.OTHERS,
            TransactionTypeEnum.INCOME);

        // Assert - Balance should now be 1350
        Assert.Equal(1350m, account.CurrentBalance.Amount);
        Assert.Equal("Updated Transaction", transaction.Description);
        Assert.Equal(350m, transaction.Amount);
    }

    [Fact]
    public void EditTransaction_ChangingBothTypeAndAmount_ShouldUpdateBalanceCorrectly()
    {
        // Arrange
        var account = new Account(Guid.NewGuid(), AccountTypeEnum.CHECKING, "Test Account", 1000m);
        var transaction = account.AddTransaction(
            "Original Transaction",
            300m,
            CategoryEnum.OTHERS,
            TransactionTypeEnum.EXPENSE,
            DateTime.Now);

        // Balance should be 700 after expense
        Assert.Equal(700m, account.CurrentBalance.Amount);

        // Act - Change both type (expense to income) and amount
        account.EditTransaction(
            transaction.Id,
            500m, // Different amount
            "Updated Transaction",
            CategoryEnum.SALARY,
            TransactionTypeEnum.INCOME);

        // Assert - Balance should now be 1500 (original 1000 + 500)
        Assert.Equal(1500m, account.CurrentBalance.Amount);
        Assert.Equal("Updated Transaction", transaction.Description);
        Assert.Equal(500m, transaction.Amount);
        Assert.Equal((int)TransactionTypeEnum.INCOME, transaction.TransactionTypeId);
        Assert.Equal((int)CategoryEnum.SALARY, transaction.CategoryId);
    }

    [Fact]
    public void EditTransaction_ToZeroAmount_ShouldUpdateBalanceCorrectly()
    {
        // Arrange
        var account = new Account(Guid.NewGuid(), AccountTypeEnum.CHECKING, "Test Account", 1000m);
        var transaction = account.AddTransaction(
            "Original Transaction",
            300m,
            CategoryEnum.OTHERS,
            TransactionTypeEnum.EXPENSE,
            DateTime.Now);

        // Balance should be 700 after expense
        Assert.Equal(700m, account.CurrentBalance.Amount);

        // Act - Change amount to zero
        account.EditTransaction(
            transaction.Id,
            0m, // Zero amount
            "Updated Transaction",
            CategoryEnum.OTHERS,
            TransactionTypeEnum.EXPENSE);

        // Assert - Balance should now be 1000 (original balance)
        Assert.Equal(1000m, account.CurrentBalance.Amount);
        Assert.Equal("Updated Transaction", transaction.Description);
        Assert.Equal(0m, transaction.Amount);
    }

    [Fact]
    public void EditTransaction_FromZeroToNonZero_ShouldUpdateBalanceCorrectly()
    {
        // Arrange
        var account = new Account(Guid.NewGuid(), AccountTypeEnum.CHECKING, "Test Account", 1000m);
        var transaction = account.AddTransaction(
            "Original Transaction",
            0m, // Zero amount initially
            CategoryEnum.OTHERS,
            TransactionTypeEnum.EXPENSE,
            DateTime.Now);

        // Balance should still be 1000 after zero expense
        Assert.Equal(1000m, account.CurrentBalance.Amount);

        // Act - Change amount from zero to non-zero
        account.EditTransaction(
            transaction.Id,
            250m,
            "Updated Transaction",
            CategoryEnum.OTHERS,
            TransactionTypeEnum.EXPENSE);

        // Assert - Balance should now be 750
        Assert.Equal(750m, account.CurrentBalance.Amount);
        Assert.Equal("Updated Transaction", transaction.Description);
        Assert.Equal(250m, transaction.Amount);
    }

    [Fact]
    public void EditTransaction_SameAmountButDifferentCategory_ShouldNotAffectBalance()
    {
        // Arrange
        var account = new Account(Guid.NewGuid(), AccountTypeEnum.CHECKING, "Test Account", 1000m);
        var transaction = account.AddTransaction(
            "Original Transaction",
            300m,
            CategoryEnum.ENTERTAINMENT,
            TransactionTypeEnum.EXPENSE,
            DateTime.Now);

        // Balance should be 700 after expense
        Assert.Equal(700m, account.CurrentBalance.Amount);

        // Act - Change only category
        account.EditTransaction(
            transaction.Id,
            300m, // Same amount
            "Updated Transaction",
            CategoryEnum.FOOD, // Different category
            TransactionTypeEnum.EXPENSE); // Same type

        // Assert - Balance should remain 700
        Assert.Equal(700m, account.CurrentBalance.Amount);
        Assert.Equal("Updated Transaction", transaction.Description);
        Assert.Equal((int)CategoryEnum.FOOD, transaction.CategoryId);
    }

    [Fact]
    public void EditTransaction_MultipleEditsToSameTransaction_ShouldTrackBalanceCorrectly()
    {
        // Arrange
        var account = new Account(Guid.NewGuid(), AccountTypeEnum.CHECKING, "Test Account", 1000m);
        var transaction = account.AddTransaction(
            "Original Transaction",
            300m,
            CategoryEnum.OTHERS,
            TransactionTypeEnum.EXPENSE,
            DateTime.Now);

        // Balance should be 700 after expense
        Assert.Equal(700m, account.CurrentBalance.Amount);

        // Act 1 - First edit: change amount
        account.EditTransaction(
            transaction.Id,
            200m, // Reduce expense
            "First Update",
            CategoryEnum.OTHERS,
            TransactionTypeEnum.EXPENSE);

        // Assert 1
        Assert.Equal(800m, account.CurrentBalance.Amount);

        // Act 2 - Second edit: change type
        account.EditTransaction(
            transaction.Id,
            200m, // Same amount
            "Second Update",
            CategoryEnum.OTHERS,
            TransactionTypeEnum.INCOME);

        // Assert 2
        Assert.Equal(1200m, account.CurrentBalance.Amount);

        // Act 3 - Third edit: change amount again
        account.EditTransaction(
            transaction.Id,
            350m, // Increase amount
            "Third Update",
            CategoryEnum.OTHERS,
            TransactionTypeEnum.INCOME);

        // Assert 3
        Assert.Equal(1350m, account.CurrentBalance.Amount);
    }

    [Fact]
    public void EditTransaction_NonExistentTransactionId_ShouldThrowDomainException()
    {
        // Arrange
        var account = new Account(Guid.NewGuid(), AccountTypeEnum.CHECKING, "Test Account", 1000m);
        var nonExistentId = Guid.NewGuid();

        // Act & Assert
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
        // Arrange
        var account = new Account(Guid.NewGuid(), AccountTypeEnum.CHECKING, "Test Account", 1000m);
        var transaction = account.AddTransaction(
            "Original Transaction",
            300m,
            CategoryEnum.OTHERS,
            TransactionTypeEnum.EXPENSE,
            DateTime.Now);

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() =>
            account.EditTransaction(
                transaction.Id,
                100m,
                "", // Empty description
                CategoryEnum.OTHERS,
                TransactionTypeEnum.EXPENSE));

        Assert.Contains("Transaction description cannot be empty", exception.Message);
    }

    [Fact]
    public void EditTransaction_NegativeAmount_ShouldThrowDomainException()
    {
        // Arrange
        var account = new Account(Guid.NewGuid(), AccountTypeEnum.CHECKING, "Test Account", 1000m);
        var transaction = account.AddTransaction(
            "Original Transaction",
            300m,
            CategoryEnum.OTHERS,
            TransactionTypeEnum.EXPENSE,
            DateTime.Now);

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() =>
            account.EditTransaction(
                transaction.Id,
                -100m, // Negative amount
                "Updated Transaction",
                CategoryEnum.OTHERS,
                TransactionTypeEnum.EXPENSE));

        Assert.Contains("Transaction amount cannot be negative", exception.Message);
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
        Assert.Equal(700m, account.CurrentBalance.Amount);

        // Act
        account.DeleteTransaction(transaction.Id);

        // Assert
        Assert.Equal(1000m, account.CurrentBalance.Amount); // Balance should be back to original
    }

    [Fact]
    public void Constructor_WithEmptyName_ShouldThrowDomainException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var emptyName = "";
        var initialBalance = 1000m;

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() =>
            new Account(userId, AccountTypeEnum.CHECKING, emptyName, initialBalance));

        Assert.Contains("Account name cannot be empty", exception.Message);
    }

    [Fact]
    public void UpdateCurrentBalance_WithNullAmount_ShouldThrowArgumentNullException()
    {
        // Arrange
        var account = new Account(Guid.NewGuid(), AccountTypeEnum.CHECKING, "Test Account", 1000m);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => account.UpdateCurrentBalance(null));
    }

    [Fact]
    public void DeleteAccount_ShouldRemoveAllTransactionsAndMarkAsDeleted()
    {
        // Arrange
        var account = new Account(Guid.NewGuid(), AccountTypeEnum.CHECKING, "Test Account", 1000m);

        // Add multiple transactions
        account.AddTransaction("Transaction 1", 200m, CategoryEnum.FOOD, TransactionTypeEnum.EXPENSE, DateTime.Now);
        account.AddTransaction("Transaction 2", 300m, CategoryEnum.SALARY, TransactionTypeEnum.INCOME, DateTime.Now);

        Assert.Equal(2, account.Transactions.Count);
        Assert.True(account.IsActive);

        // Act
        account.DeleteAccount();

        // Assert
        Assert.False(account.IsActive); // Account should be marked as deleted
        Assert.Equal(1000m, account.CurrentBalance.Amount); // Balance should be restored to initial balance

        // Verify transactions are marked as deleted by checking if they still affect the balance
        foreach (var transaction in account.Transactions)
        {
            Assert.False(transaction.IsActive);
        }
    }

    [Fact]
    public void DeleteTransaction_WithNonExistentId_ShouldThrowDomainException()
    {
        // Arrange
        var account = new Account(Guid.NewGuid(), AccountTypeEnum.CHECKING, "Test Account", 1000m);
        var nonExistentId = Guid.NewGuid();

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => account.DeleteTransaction(nonExistentId));

        Assert.Contains($"Transaction with id {nonExistentId} not found", exception.Message);
    }

    [Fact]
    public void DeleteTransaction_WithMultipleTransactions_ShouldOnlyDeleteSpecificTransaction()
    {
        // Arrange
        var account = new Account(Guid.NewGuid(), AccountTypeEnum.CHECKING, "Test Account", 1000m);

        // Add multiple transactions
        var transaction1 = account.AddTransaction("Transaction 1", 200m, CategoryEnum.FOOD, TransactionTypeEnum.EXPENSE, DateTime.Now);
        var transaction2 = account.AddTransaction("Transaction 2", 300m, CategoryEnum.SALARY, TransactionTypeEnum.INCOME, DateTime.Now);
        var transaction3 = account.AddTransaction("Transaction 3", 100m, CategoryEnum.OTHERS, TransactionTypeEnum.EXPENSE, DateTime.Now);

        Assert.Equal(3, account.Transactions.Count);
        Assert.Equal(1000m - 200m + 300m - 100m, account.CurrentBalance.Amount); // 1000

        // Act
        account.DeleteTransaction(transaction2.Id); // Delete the income transaction

        // Assert
        // Transaction should be marked as deleted
        Assert.False(transaction2.IsActive);

        // Balance should be updated correctly (income reversed)
        Assert.Equal(1000m - 200m - 100m, account.CurrentBalance.Amount); // 700
    }
}
