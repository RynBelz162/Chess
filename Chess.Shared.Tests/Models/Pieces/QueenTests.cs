using Chess.Shared.Constants;
using Chess.Shared.Helpers;
using Chess.Shared.Models.Pieces;

namespace Chess.Shared.Tests.Models.Pieces;

public class QueenTests
{
    [Fact]
    public void RecalculateAvailableMoves_MiddleOfBoard_MovesAllDirections()
    {
        var queen = new Queen(ChessFile.D, 4)
        {
            Color = ChessColor.White
        };

        var board = new ChessBoardBuilder()
            .PlacePiece(queen)
            .Build();

        var moves = queen.RecalculateAvailableMoves(board);
        var expectedMoves = new List<string>
        {
            "A4", "B4", "C4", "E4", "F4", "G4", "H4", // left to right
            "D1", "D2", "D3", "D5", "D6", "D7", "D8", // top to bottom
            "C5", "B6", "A7", "E5", "F6", "G7", "H8", // top diagonals
            "C3", "B2", "A1", "E3", "F2", "G1" // bottom diagonals
        };

        moves.Should().BeEquivalentTo(expectedMoves);
    }

    [Fact]
    public void RecalculateAvailableMoves_MiddleOfBoard_CaptureAllDirections()
    {
        var queen = new Queen(ChessFile.E, 4)
        {
            Color = ChessColor.White
        };

        var board = new ChessBoardBuilder()
            .PlacePiece(queen)
            .CreatePieceAt('p', ChessFile.B, 4)
            .CreatePieceAt('q', ChessFile.E, 7)
            .CreatePieceAt('r', ChessFile.F, 4)
            .CreatePieceAt('n', ChessFile.E, 2)
            .CreatePieceAt('P', ChessFile.C, 6)
            .CreatePieceAt('p', ChessFile.H, 7)
            .CreatePieceAt('R', ChessFile.B, 1)
            .CreatePieceAt('N', ChessFile.F, 3)
            .Build();

        var moves = queen.RecalculateAvailableMoves(board);
        var expectedMoves = new List<string>
        {
            "B4", "C4", "D4", "F4", // left to right
            "E5", "E6", "E7", "E3", "E2", // top to bottom
            "D5", "F5", "G6", "H7", // top diagonals
            "D3", "C2", // bottom diagonal
        };

        moves.Should().BeEquivalentTo(expectedMoves);
    }
}