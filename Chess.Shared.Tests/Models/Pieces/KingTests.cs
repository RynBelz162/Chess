using Chess.Shared.Constants;
using Chess.Shared.Models.Pieces;

namespace Chess.Shared.Tests.Models.Pieces;

public class KingTests
{
    [Fact]
    public void RecalculateAvailableMoves_MiddleOfBoard_MovesAllDirections()
    {
        var king = new King { Color = ChessColor.White };

        var board = new ChessBoardBuilder()
            .PlacePieceAt(king, ChessFile.D, 4)
            .Build();

        var moves = king.RecalculateAvailableMoves(board);
        var expectedMoves = new List<string>
        {
            "D5", "D3", "C4", "E4", "C5", "C3", "E3", "E5"
        };

        moves.Should().BeEquivalentTo(expectedMoves);
    }

    [Fact]
    public void RecalculateAvailableMoves_MiddleOfBoard_CaptureAllDirections()
    {
        var king = new King { Color = ChessColor.White };
        var board = new ChessBoardBuilder()
            .PlacePieceAt(king, ChessFile.D, 4)
            .CreatePieceAt('p', ChessFile.D, 5)
            .CreatePieceAt('q', ChessFile.E, 4)
            .CreatePieceAt('N', ChessFile.C, 5)
            .Build();

        var moves = king.RecalculateAvailableMoves(board);
        var expectedMoves = new List<string>
        {
            "D5", "D3", "C4", "E4", "C3", "E3", "E5"
        };

        moves.Should().BeEquivalentTo(expectedMoves);
    }
}