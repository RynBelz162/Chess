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
    public void RecalculateAvailableMoves_MiddleOfBoard_MovesAllDirections()
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
    public void RecalculateAvailableMoves_MiddleOfBoard_CaptureAllDirections()
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
}