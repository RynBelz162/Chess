using Chess.Shared.Constants;
using Chess.Shared.Helpers;

namespace Chess.Shared.Tests.Models;

public class CheckmateStalemateTests
{
    [Fact]
    public void IsCheckmated_WhenTwoRookLadderTrapsKing_ShouldBeTrue()
    {
        // Black king A8 is checked on the 8th rank by Rh8 while Rg7 seals the
        // 7th rank, so every escape square is covered: ladder mate.
        var board = new ChessBoardBuilder()
            .CreatePieceAt('k', ChessFile.A, 8)
            .CreatePieceAt('R', ChessFile.H, 8)
            .CreatePieceAt('R', ChessFile.G, 7)
            .Build();

        board.KingIsInCheck(ChessColor.Black).Should().BeTrue();
        board.IsCheckmated(ChessColor.Black).Should().BeTrue();
        board.IsStalemated(ChessColor.Black).Should().BeFalse();
    }

    [Fact]
    public void IsCheckmated_WhenProtectedQueenDeliversMate_ShouldBeTrue()
    {
        // Qg7 checks the king on h8 and cannot be captured because the bishop on
        // b2 defends it along the long diagonal.
        var board = new ChessBoardBuilder()
            .CreatePieceAt('k', ChessFile.H, 8)
            .CreatePieceAt('Q', ChessFile.G, 7)
            .CreatePieceAt('B', ChessFile.B, 2)
            .Build();

        board.IsCheckmated(ChessColor.Black).Should().BeTrue();
    }

    [Fact]
    public void IsCheckmated_WhenKingCanStepOffTheLine_ShouldBeFalse()
    {
        // The rook checks down the e file, but the king is in the open and simply
        // walks off it — check, not mate.
        var board = new ChessBoardBuilder()
            .CreatePieceAt('k', ChessFile.E, 5)
            .CreatePieceAt('R', ChessFile.E, 1)
            .Build();

        board.KingIsInCheck(ChessColor.Black).Should().BeTrue();
        board.IsCheckmated(ChessColor.Black).Should().BeFalse();
    }

    [Fact]
    public void IsCheckmated_WhenCheckCanBeBlocked_ShouldBeFalse()
    {
        // Re1 checks the king on e8, but the rook on a7 can interpose on e7.
        var board = new ChessBoardBuilder()
            .CreatePieceAt('k', ChessFile.E, 8)
            .CreatePieceAt('r', ChessFile.A, 7)
            .CreatePieceAt('R', ChessFile.E, 1)
            .Build();

        board.KingIsInCheck(ChessColor.Black).Should().BeTrue();
        board.IsCheckmated(ChessColor.Black).Should().BeFalse();
    }

    [Fact]
    public void IsStalemated_WhenKingHasNoMoveAndIsNotInCheck_ShouldBeTrue()
    {
        // Classic corner stalemate: the king on a8 is not in check, but Qc7
        // removes every square it could move to.
        var board = new ChessBoardBuilder()
            .CreatePieceAt('k', ChessFile.A, 8)
            .CreatePieceAt('Q', ChessFile.C, 7)
            .CreatePieceAt('K', ChessFile.C, 1)
            .Build();

        board.KingIsInCheck(ChessColor.Black).Should().BeFalse();
        board.IsStalemated(ChessColor.Black).Should().BeTrue();
        board.IsCheckmated(ChessColor.Black).Should().BeFalse();
    }

    [Fact]
    public void IsStalemated_WhenAnotherPieceCanStillMove_ShouldBeFalse()
    {
        // Same king trap, but a free pawn on h7 still has a legal push, so the
        // side is not stalemated.
        var board = new ChessBoardBuilder()
            .CreatePieceAt('k', ChessFile.A, 8)
            .CreatePieceAt('p', ChessFile.H, 7)
            .CreatePieceAt('Q', ChessFile.C, 7)
            .CreatePieceAt('K', ChessFile.C, 1)
            .Build();

        board.IsStalemated(ChessColor.Black).Should().BeFalse();
    }

    [Fact]
    public void StartingPosition_ShouldBeNeitherCheckmateNorStalemate()
    {
        var board = new ChessBoardBuilder()
            .CreatePieceAt('k', ChessFile.E, 8)
            .CreatePieceAt('K', ChessFile.E, 1)
            .CreatePieceAt('P', ChessFile.E, 2)
            .Build();

        board.IsCheckmated(ChessColor.White).Should().BeFalse();
        board.IsStalemated(ChessColor.White).Should().BeFalse();
    }
}
