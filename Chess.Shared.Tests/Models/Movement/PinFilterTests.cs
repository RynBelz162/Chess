using Chess.Shared.Constants;
using Chess.Shared.Helpers;
using Chess.Shared.Models;
using Chess.Shared.Models.Pieces;

namespace Chess.Shared.Tests.Models.Movement;

public class PinFilterTests
{
    // Builds: white king on E1, white piece on E4, black rook on E8.
    // The white piece is pinned along the E file.
    private static Board BoardWithVerticalPin(char pinnedPiece) =>
        new ChessBoardBuilder()
            .CreatePieceAt('K', ChessFile.E, 1)
            .CreatePieceAt(pinnedPiece, ChessFile.E, 4)
            .CreatePieceAt('r', ChessFile.E, 8)
            .Build();

    [Fact]
    public void Apply_WhenBishopPinnedVertically_ShouldRemoveAllMoves()
    {
        var board = BoardWithVerticalPin('B');
        var bishop = board.PieceOnSquare("E4")!;

        // A bishop only moves diagonally, so every move leaves the file: fully pinned.
        bishop.AvailableMoves.Should().BeEmpty();
        bishop.CanMove().Should().BeFalse();
    }

    [Fact]
    public void Apply_WhenRookPinnedVertically_ShouldKeepOnlyMovesAlongPin()
    {
        var board = BoardWithVerticalPin('R');
        var rook = board.PieceOnSquare("E4")!;

        // The rook may slide along the E file (toward or onto the pinning rook)
        // but may never leave the file.
        rook.AvailableMoves.Should().OnlyContain(move => move.StartsWith('E'));
        rook.AvailableMoves.Should().Contain("E8"); // capturing the pinner is legal
        rook.AvailableMoves.Should().Contain("E2"); // sliding back toward the king
        rook.AvailableMoves.Should().NotContain("D4");
        rook.AvailableMoves.Should().NotContain("F4");
    }

    [Fact]
    public void Apply_WhenKnightPinned_ShouldRemoveAllMoves()
    {
        var board = BoardWithVerticalPin('N');
        var knight = board.PieceOnSquare("E4")!;

        // A knight can never stay on the pin line, so a pinned knight is frozen.
        knight.AvailableMoves.Should().BeEmpty();
    }

    [Fact]
    public void Apply_WhenPieceDiagonallyPinned_ShouldKeepOnlyMovesAlongDiagonal()
    {
        // White king on C1, white bishop on D2, black bishop on G5: the pin runs
        // along the C1-G5 diagonal.
        var board = new ChessBoardBuilder()
            .CreatePieceAt('K', ChessFile.C, 1)
            .CreatePieceAt('B', ChessFile.D, 2)
            .CreatePieceAt('b', ChessFile.G, 5)
            .Build();

        var bishop = board.PieceOnSquare("D2")!;

        bishop.AvailableMoves.Should().Contain("G5"); // capture the pinner
        bishop.AvailableMoves.Should().Contain("E3"); // slide along the diagonal
        bishop.AvailableMoves.Should().NotContain("D3"); // leaving the diagonal exposes the king
        bishop.AvailableMoves.Should().NotContain("C3");
    }

    [Fact]
    public void Apply_WhenNoEnemyBehindPiece_ShouldNotRestrictMoves()
    {
        // Same geometry but the rook is replaced with a harmless pawn, so there
        // is no pin and the bishop keeps its diagonal moves.
        var board = new ChessBoardBuilder()
            .CreatePieceAt('K', ChessFile.E, 1)
            .CreatePieceAt('B', ChessFile.E, 4)
            .CreatePieceAt('p', ChessFile.E, 8)
            .Build();

        var bishop = board.PieceOnSquare("E4")!;

        bishop.AvailableMoves.Should().Contain("D5");
        bishop.AvailableMoves.Should().Contain("F3");
    }

    [Fact]
    public void Apply_WhenFriendlyPieceBlocksAttacker_ShouldNotPin()
    {
        // A friendly pawn on E3 sits between the king and the candidate piece,
        // so the rook does not actually pin the bishop on E4.
        var board = new ChessBoardBuilder()
            .CreatePieceAt('K', ChessFile.E, 1)
            .CreatePieceAt('P', ChessFile.E, 3)
            .CreatePieceAt('B', ChessFile.E, 4)
            .CreatePieceAt('r', ChessFile.E, 8)
            .Build();

        var bishop = board.PieceOnSquare("E4")!;

        bishop.AvailableMoves.Should().Contain("D5");
        bishop.AvailableMoves.Should().Contain("F5");
    }

    [Fact]
    public void Apply_WhenEnPassantWouldDiscoverCheck_ShouldRemoveEnPassant()
    {
        // White king H5, white pawn E5, black pawn F5 (just advanced two), black
        // rook A5. Capturing en passant clears both pawns off the 5th rank, so
        // the rook would check the king — the capture is illegal.
        var board = new ChessBoardBuilder()
            .CreatePieceAt('K', ChessFile.H, 5)
            .CreatePieceAt('P', ChessFile.E, 5, numOfMoves: 1)
            .CreatePieceAt('p', ChessFile.F, 5, numOfMoves: 1)
            .CreatePieceAt('r', ChessFile.A, 5)
            .Build();

        var pawn = board.PieceOnSquare("E5")!;

        pawn.AvailableMoves.Should().NotContain("F6"); // en passant discovers check
        pawn.AvailableMoves.Should().Contain("E6"); // advancing keeps F5 shielding the king
    }

    [Fact]
    public void Apply_WhenEnPassantIsSafe_ShouldKeepEnPassant()
    {
        // Same position without the rook: nothing is discovered, so en passant
        // remains a legal capture.
        var board = new ChessBoardBuilder()
            .CreatePieceAt('K', ChessFile.H, 5)
            .CreatePieceAt('P', ChessFile.E, 5, numOfMoves: 1)
            .CreatePieceAt('p', ChessFile.F, 5, numOfMoves: 1)
            .Build();

        var pawn = board.PieceOnSquare("E5")!;

        pawn.AvailableMoves.Should().Contain("F6");
    }

    [Fact]
    public void Apply_WhenAlignedAttackerIsNotASlider_ShouldNotPin()
    {
        // A knight on the king's file does not pin along it, so the bishop in
        // between keeps its diagonal moves.
        var board = new ChessBoardBuilder()
            .CreatePieceAt('K', ChessFile.E, 1)
            .CreatePieceAt('B', ChessFile.E, 4)
            .CreatePieceAt('n', ChessFile.E, 8)
            .Build();

        var bishop = board.PieceOnSquare("E4")!;

        bishop.AvailableMoves.Should().Contain("D5");
        bishop.AvailableMoves.Should().Contain("F3");
    }

    [Fact]
    public void Apply_WhenSliderIsOnWrongAxis_ShouldNotPin()
    {
        // A rook attacks ranks and files, not diagonals, so it cannot pin the
        // bishop sitting on the king's diagonal.
        var board = new ChessBoardBuilder()
            .CreatePieceAt('K', ChessFile.C, 1)
            .CreatePieceAt('B', ChessFile.D, 2)
            .CreatePieceAt('r', ChessFile.G, 5)
            .Build();

        var bishop = board.PieceOnSquare("D2")!;

        bishop.AvailableMoves.Should().Contain("C3"); // free to leave the diagonal
    }

    [Fact]
    public void Apply_WhenPieceNotPinned_ShouldLeaveKingMovesUntouched()
    {
        // Regression guard: the king itself is never pin-filtered.
        var king = new King(ChessFile.D, 4) { Color = ChessColor.White };
        var board = new ChessBoardBuilder()
            .PlacePiece(king)
            .Build();

        king.AvailableMoves.Should().HaveCount(8);
    }
}
