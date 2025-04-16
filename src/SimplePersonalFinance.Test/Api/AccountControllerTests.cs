using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SimplePersonalFinance.API.Controllers;
using SimplePersonalFinance.Application.Commands.CreateAccount;
using SimplePersonalFinance.Application.Commands.CreateTransaction;
using SimplePersonalFinance.Application.Commands.EditTransaction;
using SimplePersonalFinance.Application.Commands.RemoveTransaction;
using SimplePersonalFinance.Application.Queries.GetAccount;
using SimplePersonalFinance.Application.Queries.GetAccountsByUserId;
using SimplePersonalFinance.Application.Queries.GetAccountTransactions;
using SimplePersonalFinance.Application.ViewModels;
using SimplePersonalFinance.Application.ViewModels.Accounts;
using SimplePersonalFinance.Core.Domain.Enums;

namespace SimplePersonalFinance.Test.API.Controllers;

public class AccountControllerTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly AccountController _controller;

    public AccountControllerTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _controller = new AccountController(_mediatorMock.Object);
    }

    [Fact]
    public async Task GetById_WhenAccountExists_ShouldReturnOkResult()
    {
        // Arrange
        var accountId = Guid.NewGuid();
        var accountViewModel =
                new AccountViewModel(accountId, Guid.NewGuid(), (int)AccountTypeEnum.CHECKING, "Test Account", "Checking", 1000m, 1000m);

        _mediatorMock.Setup(m => m.Send(It.IsAny<GetAccountByIdQuery>(), default))
            .ReturnsAsync(ResultViewModel<AccountViewModel>.Success(accountViewModel));

        // Act
        var result = await _controller.GetById(accountId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<ResultViewModel<AccountViewModel>>(okResult.Value);
        Assert.True(returnValue.IsSuccess);
        Assert.Equal(accountViewModel, returnValue.Data);
    }

    [Fact]
    public async Task GetById_WhenAccountDoesNotExist_ShouldReturnBadRequest()
    {
        // Arrange
        var accountId = Guid.NewGuid();
        _mediatorMock.Setup(m => m.Send(It.IsAny<GetAccountByIdQuery>(), default))
            .ReturnsAsync(ResultViewModel<AccountViewModel>.Error("Account not found"));

        // Act
        var result = await _controller.GetById(accountId);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Account not found", badRequestResult.Value);
    }

    [Fact]
    public async Task Post_WhenCommandSucceeds_ShouldReturnCreatedAtAction()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var newAccountId = Guid.NewGuid();
        var command = new CreateAccountCommand(userId, AccountTypeEnum.CHECKING, "Test Account", 1000m);

        _mediatorMock.Setup(m => m.Send(command, default))
            .ReturnsAsync(ResultViewModel<Guid>.Success(newAccountId));

        // Act
        var result = await _controller.Post(command);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal("GetById", createdResult.ActionName);
        Assert.Equal(newAccountId, createdResult.RouteValues["id"]);
        Assert.Equal(command, createdResult.Value);
    }

    [Fact]
    public async Task Post_WhenCommandFails_ShouldReturnBadRequest()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var command = new CreateAccountCommand(userId, AccountTypeEnum.CHECKING, "Test Account", 1000m);

        _mediatorMock.Setup(m => m.Send(command, default))
            .ReturnsAsync(ResultViewModel<Guid>.Error("Failed to create account"));

        // Act
        var result = await _controller.Post(command);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Failed to create account", badRequestResult.Value);
    }

    [Fact]
    public async Task PutTransaction_WhenCommandSucceeds_ShouldReturnCreatedAtAction()
    {
        // Arrange
        var accountId = Guid.NewGuid();
        var transactionId = Guid.NewGuid();
        var command = new EditAccountTransactionCommand(
            transactionId,
            accountId,
            500m,
            "Updated Description",
            CategoryEnum.FOOD,
            TransactionTypeEnum.EXPENSE);

        _mediatorMock.Setup(m => m.Send(command, default))
            .ReturnsAsync(ResultViewModel<Guid>.Success(transactionId));

        // Act
        var result = await _controller.PutTransaction(command);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal("GetTransactions", createdResult.ActionName);
        Assert.Equal(accountId, createdResult.RouteValues["accountId"]);
        Assert.Equal(command, createdResult.Value);
    }

    [Fact]
    public async Task PutTransaction_WhenCommandFails_ShouldReturnBadRequest()
    {
        // Arrange
        var accountId = Guid.NewGuid();
        var transactionId = Guid.NewGuid();
        var command = new EditAccountTransactionCommand(
            transactionId,
            accountId,
            500m,
            "Updated Description",
            CategoryEnum.FOOD,
            TransactionTypeEnum.EXPENSE);

        _mediatorMock.Setup(m => m.Send(command, default))
            .ReturnsAsync(ResultViewModel<Guid>.Error("Transaction not found"));

        // Act
        var result = await _controller.PutTransaction(command);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Transaction not found", badRequestResult.Value);
    }

    [Fact]
    public async Task DeleteTransaction_WhenCommandSucceeds_ShouldReturnNoContent()
    {
        // Arrange
        var accountId = Guid.NewGuid();
        var transactionId = Guid.NewGuid();

        _mediatorMock.Setup(m => m.Send(It.IsAny<DeleteAccountTransactionCommand>(), default))
            .ReturnsAsync(ResultViewModel<Guid>.Success(transactionId));

        // Act
        var result = await _controller.DeleteTransaction(accountId, transactionId);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task DeleteTransaction_WhenCommandFails_ShouldReturnBadRequest()
    {
        // Arrange
        var accountId = Guid.NewGuid();
        var transactionId = Guid.NewGuid();

        _mediatorMock.Setup(m => m.Send(It.IsAny<DeleteAccountTransactionCommand>(), default))
            .ReturnsAsync(ResultViewModel<Guid>.Error("Failed to delete transaction"));

        // Act
        var result = await _controller.DeleteTransaction(accountId, transactionId);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Failed to delete transaction", badRequestResult.Value);
    }
}
