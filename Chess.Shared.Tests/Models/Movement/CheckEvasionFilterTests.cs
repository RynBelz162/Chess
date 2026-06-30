using Chess.Shared.Constants;
using Chess.Shared.Helpers;

namespace Chess.Shared.Tests.Models.Movement;

public class CheckEvasionFilterTests
{
    [Fact]
    public void Apply_WhenInCheck_ShouldRestrictDefenderToBlockingSquare()
    {
        // Re1 checks the king on e8. The rook on a7 has many moves, but the only
        // legal one is interposing on e7.
        var board = new ChessBoardBuilder()
            .CreatePieceAt('k', ChessFile.E, 8)
            .CreatePieceAt('r', ChessFile.A, 7)
            .CreatePieceAt('R', ChessFile.E, 1)
            .Build();

        var defender = board.PieceOnSquare("A7")!;

        defender.AvailableMoves.Should().Equal("E7");
    }

    [Fact]
    public void Apply_WhenInCheck_ShouldRestrictDefenderToCapturingChecker()
    {
        // Re7 checks the king on e8. The rook on a7 can only resolve it by
        // capturing the checker on e7.
        var board = new ChessBoardBuilder()
            .CreatePieceAt('k', ChessFile.E, 8)
            .CreatePieceAt('r', ChessFile.A, 7)
            .CreatePieceAt('R', ChessFile.E, 7)
            .Build();

        var defender = board.PieceOnSquare("A7")!;

        defender.AvailableMoves.Should().Equal("E7");
    }

    [Fact]
    public void Apply_WhenInDoubleCheck_ShouldLeaveDefenderNoMoves()
    {
        // Both Re8-file and Bb... no single interposition or capture answers two
        // checkers at once, so only the king may move (handled elsewhere).
        // Rook on e1 and bishop on h5 both check the king on e8.
        var board = new ChessBoardBuilder()
            .CreatePieceAt('k', ChessFile.E, 8)
            .CreatePieceAt('r', ChessFile.A, 7)
            .CreatePieceAt('R', ChessFile.E, 1)
            .CreatePieceAt('B', ChessFile.H, 5)
            .Build();

        var defender = board.PieceOnSquare("A7")!;

        defender.AvailableMoves.Should().BeEmpty();
    }

    [Fact]
    public void Apply_WhenNotInCheck_ShouldNotRestrictDefenders()
    {
        // No check: the rook keeps its full set of legal moves.
        var board = new ChessBoardBuilder()
            .CreatePieceAt('k', ChessFile.E, 8)
            .CreatePieceAt('r', ChessFile.A, 7)
            .Build();

        var defender = board.PieceOnSquare("A7")!;

        defender.AvailableMoves.Should().Contain("A1");
        defender.AvailableMoves.Should().Contain("H7");
    }
}
