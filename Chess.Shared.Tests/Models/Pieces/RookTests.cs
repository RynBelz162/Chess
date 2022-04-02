using Chess.Shared.Constants;
using Chess.Shared.Models.Pieces;

namespace Chess.Shared.Tests.Models.Pieces;

public class RookTests
{
    [Fact]
    public void RecalculateAvailableMoves_MiddleOfBoard_MovesLeftRightUpDown()
    {
        var rook = new Rook { Color = ChessColor.White };
        var board = new ChessBoardBuilder()
            .PlacePieceAt(rook, ChessFile.D, 4)
            .Build();

        var moves = rook.RecalculateAvailableMoves(board);
        var expectedMoves = new List<string>
        {
            "A4", "B4", "C4", "E4", "F4", "G4", "H4", "D3", "D2", "D1", "D5", "D6", "D7", "D8"
        };

        moves.Should().BeEquivalentTo(expectedMoves);
    }

    [Fact]
    public void RecalculateAvailableMoves_MiddleOfBoard_CaptureAllDirections()
    {
        var rook = new Rook { Color = ChessColor.White };
        var board = new ChessBoardBuilder()
            .PlacePieceAt(rook, ChessFile.E, 4)
            .CreatePieceAt('p', ChessFile.B, 4)
            .CreatePieceAt('q', ChessFile.E, 7)
            .CreatePieceAt('r', ChessFile.F, 4)
            .CreatePieceAt('n', ChessFile.E, 2)
            .Build();

        var moves = rook.RecalculateAvailableMoves(board);
        var expectedMoves = new List<string>
        {
            "B4", "C4", "D4", "F4", "E7", "E6", "E5", "E3", "E2"
        };

        moves.Should().BeEquivalentTo(expectedMoves);
    }
}