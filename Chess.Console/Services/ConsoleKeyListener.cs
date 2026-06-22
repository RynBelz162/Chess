namespace Chess.Console.Services;

public static class ConsoleKeyListener
{
    /// <summary>
    /// Polls the console for key presses on a background task until cancelled.
    /// The handler returns true to stop listening, false to keep going.
    /// </summary>
    public static Task ListenUntil(Func<ConsoleKey, bool> onKey, CancellationToken token)
    {
        return Task.Run(() =>
        {
            while (!token.IsCancellationRequested)
            {
                if (System.Console.KeyAvailable)
                {
                    var key = System.Console.ReadKey(intercept: true).Key;
                    if (onKey(key))
                    {
                        break;
                    }
                }

                Thread.Sleep(50);
            }
        }, token);
    }
}
