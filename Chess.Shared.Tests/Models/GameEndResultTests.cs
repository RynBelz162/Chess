using Chess.Shared.Enums;
using Chess.Shared.Models;

namespace Chess.Shared.Tests.Models;

public class GameEndResultTests
{
    [Theory]
    [InlineData(GameEndOutcome.Winning, GameEndReason.Checkmate, "You win by checkmate!")]
    [InlineData(GameEndOutcome.Winning, GameEndReason.Resignation, "You win by resignation!")]
    [InlineData(GameEndOutcome.Winning, GameEndReason.Stalemate, "You win by stalemate!")]
    [InlineData(GameEndOutcome.Losing, GameEndReason.Checkmate, "You lose by checkmate.")]
    [InlineData(GameEndOutcome.Losing, GameEndReason.Resignation, "You lose by resignation.")]
    [InlineData(GameEndOutcome.Losing, GameEndReason.Stalemate, "You lose by stalemate.")]
    [InlineData(GameEndOutcome.Drawing, GameEndReason.Checkmate, "Draw by checkmate.")]
    [InlineData(GameEndOutcome.Drawing, GameEndReason.Resignation, "Draw by resignation.")]
    [InlineData(GameEndOutcome.Drawing, GameEndReason.Stalemate, "Draw by stalemate.")]
    public void FormatGameEnd_WhenGivenOutcomeAndReason_ShouldReturnExpectedMessage(GameEndOutcome outcome, GameEndReason reason, string expected)
    {
        var result = new GameEndResult { Outcome = outcome, Reason = reason };

        var message = GameEndResult.FormatGameEnd(result);

        message.Should().Be(expected);
    }
}
