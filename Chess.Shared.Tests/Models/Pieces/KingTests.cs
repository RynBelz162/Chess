using Chess.Shared.Constants;
using Chess.Shared.Helpers;
using Chess.Shared.Models.Pieces;

namespace Chess.Shared.Tests.Models.Pieces;

public class KingTests
{
    private static King _defaultKingPiece =>
        new(ChessFile.D, 4)
        {
            Color = ChessColor.White
        };

    [Fact]
    public void RecalculateAvailableMoves_WhenInMiddleOfBoard_ShouldMoveAllDirections()
    {
        var board = new ChessBoardBuilder()
            .PlacePiece(_defaultKingPiece)
            .Build();

        var moves = _defaultKingPiece.RecalculateAvailableMoves(board);
        List<string> expectedMoves =
        [
            "D5", "D3", "C4", "E4", "C5", "C3", "E3", "E5"
        ];

        moves.Should().BeEquivalentTo(expectedMoves);
    }

    [Fact]
    public void RecalculateAvailableMoves_WhenInMiddleOfBoard_ShouldCaptureAllDirections()
    {
        var board = new ChessBoardBuilder()
            .PlacePiece(_defaultKingPiece)
            .CreatePieceAt('p', ChessFile.D, 5)
            .CreatePieceAt('q', ChessFile.E, 4)
            .CreatePieceAt('N', ChessFile.C, 5)
            .Build();

        var moves = _defaultKingPiece.RecalculateAvailableMoves(board);
        List<string> expectedMoves =
        [
            "D5", "D3", "C4", "E4", "C3", "E3", "E5"
        ];

        moves.Should().BeEquivalentTo(expectedMoves);
    }

    [Theory]
    [InlineData(ChessColor.White, 1, 'R')]
    [InlineData(ChessColor.Black, 8, 'r')]
    public void RecalculateAvailableMoves_WhenBothSidesClear_ShouldOfferBothCastles(
        ChessColor color, int rank, char rook)
    {
        var kingPiece = new King(ChessFile.E, rank) { Color = color };
        var board = new ChessBoardBuilder()
            .PlacePiece(kingPiece)
            .CreatePieceAt(rook, ChessFile.A, rank)
            .CreatePieceAt(rook, ChessFile.H, rank)
            .Build();

        var moves = kingPiece.RecalculateAvailableMoves(board);

        moves.Should().Contain($"G{rank}");
        moves.Should().Contain($"C{rank}");
    }

    [Fact]
    public void RecalculateAvailableMoves_WhenSquaresBetweenOccupied_ShouldNotOfferCastle()
    {
        var kingPiece = new King(ChessFile.E, 1) { Color = ChessColor.White };
        var board = new ChessBoardBuilder()
            .PlacePiece(kingPiece)
            .CreatePieceAt('R', ChessFile.A, 1)
            .CreatePieceAt('R', ChessFile.H, 1)
            .CreatePieceAt('B', ChessFile.F, 1) // blocks king side
            .CreatePieceAt('N', ChessFile.B, 1) // blocks queen side
            .Build();

        var moves = kingPiece.RecalculateAvailableMoves(board);

        moves.Should().NotContain("G1");
        moves.Should().NotContain("C1");
    }

    [Fact]
    public void RecalculateAvailableMoves_WhenRookHasMoved_ShouldNotOfferCastle()
    {
        var kingPiece = new King(ChessFile.E, 1) { Color = ChessColor.White };
        var board = new ChessBoardBuilder()
            .PlacePiece(kingPiece)
            .CreatePieceAt('R', ChessFile.H, 1, numOfMoves: 1)
            .Build();

        var moves = kingPiece.RecalculateAvailableMoves(board);

        moves.Should().NotContain("G1");
    }

    [Fact]
    public void RecalculateAvailableMoves_WhenKingHasMoved_ShouldNotOfferCastle()
    {
        var kingPiece = new King(ChessFile.E, 1) { Color = ChessColor.White, NumberOfMoves = 1 };
        var board = new ChessBoardBuilder()
            .PlacePiece(kingPiece)
            .CreatePieceAt('R', ChessFile.H, 1)
            .Build();

        var moves = kingPiece.RecalculateAvailableMoves(board);

        moves.Should().NotContain("G1");
    }

    [Fact]
    public void Move_WhenCastlingKingSide_ShouldRelocateRook()
    {
        var kingPiece = new King(ChessFile.E, 1) { Color = ChessColor.White };
        var board = new ChessBoardBuilder()
            .PlacePiece(kingPiece)
            .CreatePieceAt('R', ChessFile.H, 1)
            .Build();

        var result = kingPiece.Move("G1", board);

        result.IsSuccess.Should().BeTrue();
        kingPiece.CurrentSquare.Should().Be("G1");
        board.PieceOnSquare("F1").Should().BeOfType<Rook>();
        board.IsSquareOccupied("H1").Should().BeFalse();
    }

    [Fact]
    public void Move_WhenCastlingQueenSide_ShouldRelocateRook()
    {
        var kingPiece = new King(ChessFile.E, 1) { Color = ChessColor.White };
        var board = new ChessBoardBuilder()
            .PlacePiece(kingPiece)
            .CreatePieceAt('R', ChessFile.A, 1)
            .Build();

        var result = kingPiece.Move("C1", board);

        result.IsSuccess.Should().BeTrue();
        kingPiece.CurrentSquare.Should().Be("C1");
        board.PieceOnSquare("D1").Should().BeOfType<Rook>();
        board.IsSquareOccupied("A1").Should().BeFalse();
    }
}