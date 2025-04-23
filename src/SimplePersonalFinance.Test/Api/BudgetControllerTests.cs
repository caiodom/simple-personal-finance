using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SimplePersonalFinance.API.Controllers;
using SimplePersonalFinance.API.Services.Interfaces;
using SimplePersonalFinance.Application.Commands.BudgetCommands.CreateBudget;
using SimplePersonalFinance.Application.Commands.BudgetCommands.EditBudget;
using SimplePersonalFinance.Application.Commands.BudgetCommands.RemoveBudget;
using SimplePersonalFinance.Application.Queries.BudgetQueries.GetBudgetById;
using SimplePersonalFinance.Application.ViewModels;
using SimplePersonalFinance.Application.ViewModels.Budgets;
using SimplePersonalFinance.Core.Domain.Enums;

namespace SimplePersonalFinance.Tests.API.Controllers;

public class BudgetControllerTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly BudgetController _controller;
    private readonly Mock<IAuthUserHandler> _authUserHandlerMock;
    public BudgetControllerTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _authUserHandlerMock = new Mock<IAuthUserHandler>();
        _controller = new BudgetController(_mediatorMock.Object, _authUserHandlerMock.Object);
    }

    [Fact]
    public async Task GetById_WhenBudgetExists_ReturnsOkResult()
    {
        // Arrange
        var budgetId = Guid.NewGuid();
        var budgetViewModel = new BudgetViewModel { Id = budgetId };
        _mediatorMock.Setup(m => m.Send(It.IsAny<GetBudgetByIdQuery>(), default))
            .ReturnsAsync(ResultViewModel<BudgetViewModel>.Success(budgetViewModel));

        // Act
        var result = await _controller.GetById(budgetId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(budgetViewModel, okResult.Value);
    }

    [Fact]
    public async Task GetById_WhenBudgetDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        var budgetId = Guid.NewGuid();
        _mediatorMock.Setup(m => m.Send(It.IsAny<GetBudgetByIdQuery>(), default))
            .ReturnsAsync(ResultViewModel<BudgetViewModel>.Error("Budget not found"));

        // Act
        var result = await _controller.GetById(budgetId);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal("Budget not found", notFoundResult.Value);
    }

    [Fact]
    public async Task CreateBudget_WhenValidCommand_ReturnsCreatedAtActionResult()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var command = new CreateBudgetCommand(
            CategoryEnum.ENTERTAINMENT,
            100m,
            1,
            2023);

        var budgetId = Guid.NewGuid();
        _authUserHandlerMock.Setup(m => m.GetUserId()).Returns(userId);

        _mediatorMock.Setup(m => m.Send(command, default))
            .ReturnsAsync(ResultViewModel<Guid>.Success(budgetId));

        // Act
        var result = await _controller.CreateBudget(command);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal(nameof(BudgetController.GetById), createdResult.ActionName);
        Assert.Equal(budgetId, createdResult.RouteValues["Id"]);
        Assert.Equal(command, createdResult.Value);
    }

    [Fact]
    public async Task CreateBudget_WhenInvalidCommand_ReturnsBadRequest()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var command = new CreateBudgetCommand(
            CategoryEnum.ENTERTAINMENT,
            100m,
            1,
            2023);

        _authUserHandlerMock.Setup(m => m.GetUserId())
                            .Returns(userId);

        _mediatorMock.Setup(m => m.Send(command, default))
            .ReturnsAsync(ResultViewModel<Guid>.Error("Budget already exists"));

        // Act
        var result = await _controller.CreateBudget(command);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Budget already exists", badRequestResult.Value);
    }

    [Fact]
    public async Task EditBudget_WhenValidCommand_ReturnsCreatedAtActionResult()
    {
        // Arrange
        var budgetId = Guid.NewGuid();
        var command = new EditBudgetCommand(budgetId, 200m, 2, 2023);

        _mediatorMock.Setup(m => m.Send(command, default))
            .ReturnsAsync(ResultViewModel<Guid>.Success(budgetId));

        // Act
        var result = await _controller.EditBudget(command);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal(nameof(BudgetController.GetById), createdResult.ActionName);
        Assert.Equal(budgetId, createdResult.RouteValues["Id"]);
        Assert.Equal(command, createdResult.Value);
    }

    [Fact]
    public async Task EditBudget_WhenInvalidCommand_ReturnsBadRequest()
    {
        // Arrange
        var budgetId = Guid.NewGuid();
        var command = new EditBudgetCommand(budgetId, 200m, 2, 2023);

        _mediatorMock.Setup(m => m.Send(command, default))
            .ReturnsAsync(ResultViewModel<Guid>.Error("Budget not found"));

        // Act
        var result = await _controller.EditBudget(command);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Budget not found", badRequestResult.Value);
    }

    [Fact]
    public async Task DeleteBudget_WhenBudgetExists_ReturnsNoContent()
    {
        // Arrange
        var budgetId = Guid.NewGuid();

        _mediatorMock.Setup(m => m.Send(It.IsAny<RemoveBudgetCommand>(), default))
            .ReturnsAsync(ResultViewModel<bool>.Success(true));

        // Act
        var result = await _controller.DeleteBudget(budgetId);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task DeleteBudget_WhenBudgetDoesNotExist_ReturnsBadRequest()
    {
        // Arrange
        var budgetId = Guid.NewGuid();

        _mediatorMock.Setup(m => m.Send(It.IsAny<RemoveBudgetCommand>(), default))
            .ReturnsAsync(ResultViewModel<bool>.Error("Budget not found"));

        // Act
        var result = await _controller.DeleteBudget(budgetId);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Budget not found", badRequestResult.Value);
    }
}
