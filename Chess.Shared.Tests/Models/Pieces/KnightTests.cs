using Chess.Shared.Constants;
using Chess.Shared.Models.Pieces;

namespace Chess.Shared.Tests.Models.Pieces;

public class KnightTests
{
    [Fact]
    public void RecalculateAvailableMoves_CanMoveAllDirections()
    {
        var knight = new Knight { Color = ChessColor.White };
        var board = new ChessBoardBuilder()
            .PlacePieceAt(knight, ChessFile.C, 3)
            .Build();

        var availableMoves = knight.RecalculateAvailableMoves(board);

        var expectedMoves = new List<string>
        {
            "B5", "A4", "D5", "E4", "E2", "D1", "B1", "A2"
        };

        availableMoves.Should().BeEquivalentTo(expectedMoves);
    }

    // TODO: Finish adding more tests
}