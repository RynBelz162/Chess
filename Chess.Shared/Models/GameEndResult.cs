using Chess.Shared.Enums;

namespace Chess.Shared.Models;

public record GameEndResult
{
    public required GameEndOutcome Outcome { get; init; }
    public required GameEndReason Reason { get; init; }

    public static string FormatGameEnd(GameEndResult result)
    {
        var reason = result.Reason switch
        {
            GameEndReason.Checkmate => "checkmate",
            GameEndReason.Resignation => "resignation",
            GameEndReason.Stalemate => "stalemate",
            _ => result.Reason.ToString().ToLower()
        };

        return result.Outcome switch
        {
            GameEndOutcome.Winning => $"You win by {reason}!",
            GameEndOutcome.Losing => $"You lose by {reason}.",
            GameEndOutcome.Drawing => $"Draw by {reason}.",
            _ => "Game over."
        };
    }
};