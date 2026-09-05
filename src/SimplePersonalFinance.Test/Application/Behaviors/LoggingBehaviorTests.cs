using Microsoft.Extensions.Logging;
using SimplePersonalFinance.Application.Behaviors;

namespace SimplePersonalFinance.Test.Application.Behaviors;

public class LoggingBehaviorTests
{
    [Fact]
    public async Task Handle_ShouldLogOperationNameWithoutRequestPropertyValues()
    {
        var logger = new CapturingLogger<LoggingBehavior<SensitiveRequest, string>>();
        var behavior = new LoggingBehavior<SensitiveRequest, string>(logger);
        var request = new SensitiveRequest(
            "private.user@example.test",
            "test-password-value",
            "Private medical payment description",
            "987654.32");

        var result = await behavior.Handle(
            request,
            _ => Task.FromResult("ok"),
            CancellationToken.None);

        var logs = string.Join(Environment.NewLine, logger.Messages);

        Assert.Equal("ok", result);
        Assert.Contains(nameof(SensitiveRequest), logs, StringComparison.Ordinal);
        Assert.DoesNotContain(request.Email, logs, StringComparison.Ordinal);
        Assert.DoesNotContain(request.Password, logs, StringComparison.Ordinal);
        Assert.DoesNotContain(request.Description, logs, StringComparison.Ordinal);
        Assert.DoesNotContain(request.Amount, logs, StringComparison.Ordinal);
    }

    private sealed record SensitiveRequest(
        string Email,
        string Password,
        string Description,
        string Amount);

    private sealed class CapturingLogger<T> : ILogger<T>
    {
        public List<string> Messages { get; } = [];

        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(
            LogLevel logLevel,
            EventId eventId,
            TState state,
            Exception? exception,
            Func<TState, Exception?, string> formatter)
        {
            Messages.Add(formatter(state, exception));
        }
    }
}
