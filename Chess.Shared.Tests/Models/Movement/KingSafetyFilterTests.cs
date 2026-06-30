using Chess.Shared.Constants;
using Chess.Shared.Helpers;
using Chess.Shared.Models.Pieces;

namespace Chess.Shared.Tests.Models.Movement;

public class KingSafetyFilterTests
{
    [Fact]
    public void Apply_WhenSquareAttacked_ShouldRemoveThatKingMove()
    {
        // White king on E1; black rook on D8 attacks the whole D file, so D1/D2
        // are off-limits while E1/E2/F1/F2 stay open.
        var board = new ChessBoardBuilder()
            .CreatePieceAt('K', ChessFile.E, 1)
            .CreatePieceAt('r', ChessFile.D, 8)
            .Build();

        var king = board.PieceOnSquare("E1")!;

        king.AvailableMoves.Should().NotContain("D1");
        king.AvailableMoves.Should().NotContain("D2");
        king.AvailableMoves.Should().Contain("E2");
        king.AvailableMoves.Should().Contain("F2");
    }

    [Fact]
    public void Apply_WhenFleeingAlongCheckingLine_ShouldStillRemoveMove()
    {
        // Black rook checks down the E file. Retreating from E4 to E3 stays on
        // the file, so the king is still in check and E3 must be removed.
        var board = new ChessBoardBuilder()
            .CreatePieceAt('K', ChessFile.E, 4)
            .CreatePieceAt('r', ChessFile.E, 8)
            .Build();

        var king = board.PieceOnSquare("E4")!;

        king.AvailableMoves.Should().NotContain("E3");
        king.AvailableMoves.Should().NotContain("E5");
        king.AvailableMoves.Should().Contain("D4"); // stepping off the file escapes
        king.AvailableMoves.Should().Contain("F4");
    }

    [Fact]
    public void Apply_WhenCapturingUndefendedAttacker_ShouldAllowMove()
    {
        // King can take an adjacent, undefended attacker.
        var board = new ChessBoardBuilder()
            .CreatePieceAt('K', ChessFile.E, 1)
            .CreatePieceAt('r', ChessFile.E, 2)
            .Build();

        var king = board.PieceOnSquare("E1")!;

        king.AvailableMoves.Should().Contain("E2");
    }

    [Fact]
    public void Apply_WhenCapturingDefendedAttacker_ShouldRemoveMove()
    {
        // The attacking rook on E2 is defended by another rook on E8, so the
        // king cannot capture into check.
        var board = new ChessBoardBuilder()
            .CreatePieceAt('K', ChessFile.E, 1)
            .CreatePieceAt('r', ChessFile.E, 2)
            .CreatePieceAt('r', ChessFile.E, 8)
            .Build();

        var king = board.PieceOnSquare("E1")!;

        king.AvailableMoves.Should().NotContain("E2");
    }

    [Fact]
    public void Apply_WhenCastleTargetSafe_ShouldKeepCastle()
    {
        var board = new ChessBoardBuilder()
            .CreatePieceAt('K', ChessFile.E, 1)
            .CreatePieceAt('R', ChessFile.H, 1)
            .Build();

        var king = board.PieceOnSquare("E1")!;

        king.AvailableMoves.Should().Contain("G1");
    }

    [Fact]
    public void Apply_WhenKingInCheck_ShouldRemoveCastle()
    {
        // Black rook on E8 checks the king, which cannot castle out of check.
        var board = new ChessBoardBuilder()
            .CreatePieceAt('K', ChessFile.E, 1)
            .CreatePieceAt('R', ChessFile.H, 1)
            .CreatePieceAt('r', ChessFile.E, 8)
            .Build();

        var king = board.PieceOnSquare("E1")!;

        king.AvailableMoves.Should().NotContain("G1");
    }

    [Fact]
    public void Apply_WhenKingPassesThroughAttackedSquare_ShouldRemoveCastle()
    {
        // Black rook on F8 attacks F1, the square the king must step over while
        // castling king side.
        var board = new ChessBoardBuilder()
            .CreatePieceAt('K', ChessFile.E, 1)
            .CreatePieceAt('R', ChessFile.H, 1)
            .CreatePieceAt('r', ChessFile.F, 8)
            .Build();

        var king = board.PieceOnSquare("E1")!;

        king.AvailableMoves.Should().NotContain("G1");
    }

    [Fact]
    public void Apply_WhenCastleLandingSquareAttacked_ShouldRemoveCastle()
    {
        // Black rook on G8 attacks G1, the king's castling destination.
        var board = new ChessBoardBuilder()
            .CreatePieceAt('K', ChessFile.E, 1)
            .CreatePieceAt('R', ChessFile.H, 1)
            .CreatePieceAt('r', ChessFile.G, 8)
            .Build();

        var king = board.PieceOnSquare("E1")!;

        king.AvailableMoves.Should().NotContain("G1");
    }

    [Fact]
    public void Apply_WhenQueenSideTransitAttacked_ShouldRemoveCastle()
    {
        // Black rook on D8 attacks D1, the queen-side transit square.
        var board = new ChessBoardBuilder()
            .CreatePieceAt('K', ChessFile.E, 1)
            .CreatePieceAt('R', ChessFile.A, 1)
            .CreatePieceAt('r', ChessFile.D, 8)
            .Build();

        var king = board.PieceOnSquare("E1")!;

        king.AvailableMoves.Should().NotContain("C1");
    }
}
