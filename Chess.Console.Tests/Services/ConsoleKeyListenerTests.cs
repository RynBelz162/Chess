using System.Threading;
using System.Threading.Tasks;
using Chess.Console.Services;

namespace Chess.Console.Tests.Services;

public class ConsoleKeyListenerTests
{
    [Fact]
    public async Task ListenUntil_TokenAlreadyCancelled_NeverInvokesHandler()
    {
        var invoked = false;
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        var task = ConsoleKeyListener.ListenUntil(_ =>
        {
            invoked = true;
            return true;
        }, cts.Token);

        var act = async () => await task.WaitAsync(TimeSpan.FromSeconds(1));

        // Cancelled token short-circuits before the poll body runs, so the handler
        // is never called and the console (which throws on redirected input) is never touched.
        await act.Should().ThrowAsync<TaskCanceledException>();
        invoked.Should().BeFalse();
    }

    [Fact]
    public async Task ListenUntil_TokenAlreadyCancelled_EndsInCanceledState()
    {
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        var task = ConsoleKeyListener.ListenUntil(_ => false, cts.Token);

        try
        {
            await task.WaitAsync(TimeSpan.FromSeconds(1));
        }
        catch (TaskCanceledException)
        {
            // expected
        }

        task.IsCanceled.Should().BeTrue();
    }
}
