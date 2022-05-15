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

    [Fact]
    public void RecaclulateAvailableMoves_EdgeOfBoard_CantMoveLeft()
    {
        var knight = new Knight { Color = ChessColor.White };

        var board = new ChessBoardBuilder()
            .PlacePieceAt(knight, ChessFile.A, 3)
            .CreatePieceAt('P', ChessFile.C, 4)
            .CreatePieceAt('k', ChessFile.B, 5)
            .Build();

        var availableMoves = knight.RecalculateAvailableMoves(board);

        var expectedMoves = new List<string>
        {
            "B5", "C2", "B1"
        };

        availableMoves.Should().BeEquivalentTo(expectedMoves);
    }

    [Fact]
    public void RecaclulateAvailableMoves_EdgeOfBoard_CantMoveRight()
    {
        var knight = new Knight { Color = ChessColor.White };

        var board = new ChessBoardBuilder()
            .PlacePieceAt(knight, ChessFile.H, 3)
            .CreatePieceAt('p', ChessFile.F, 4)
            .CreatePieceAt('k', ChessFile.G, 5)
            .Build();

        var availableMoves = knight.RecalculateAvailableMoves(board);

        var expectedMoves = new List<string>
        {
            "G5", "F4", "F2", "G1"
        };

        availableMoves.Should().BeEquivalentTo(expectedMoves);
    }
}