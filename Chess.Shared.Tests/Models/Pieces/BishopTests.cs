using Chess.Shared.Constants;
using Chess.Shared.Helpers;
using Chess.Shared.Models.Pieces;

namespace Chess.Shared.Tests.Models.Pieces;

public class BishopTests
{
    [Theory]
    [InlineData(ChessColor.White)]
    [InlineData(ChessColor.Black)]
    public void RecalculateAvailableMoves_NoBlockers(ChessColor color)
    {
        var bishop = new Bishop(ChessFile.E, 4)
        {
            Color = color
        };

        var board = new ChessBoardBuilder()
            .PlacePiece(bishop)
            .Build();

        var availableMoves = bishop.RecalculateAvailableMoves(board);
        var expectedMoves = new List<string>
        {
            "D3", "C2", "B1", "F3", "G2", "H1", "D5", "C6", "B7", "A8", "F5", "G6", "H7"
        };

        availableMoves.Should().BeEquivalentTo(expectedMoves);
    }

    [Fact]
    public void RecalculateAvailableMoves_White_Blockers_NoMoves()
    {
        var bishop = new Bishop(ChessFile.E, 4)
        {
            Color = ChessColor.White
        };

        var board = new ChessBoardBuilder()
            .PlacePiece(bishop)
            .CreatePieceAt('P', ChessFile.D, 5)
            .CreatePieceAt('Q', ChessFile.D, 3)
            .CreatePieceAt('K', ChessFile.F, 5)
            .CreatePieceAt('R', ChessFile.F, 3)
            .Build();

        var availableMoves = bishop.RecalculateAvailableMoves(board);

        availableMoves.Should().BeEmpty();
    }

    [Fact]
    public void RecalculateAvailableMoves_Black_Blockers_NoMoves()
    {
        var bishop = new Bishop(ChessFile.E, 4)
        {
            Color = ChessColor.Black
        };

        var board = new ChessBoardBuilder()
            .PlacePiece(bishop)
            .CreatePieceAt('p', ChessFile.D, 5)
            .CreatePieceAt('q', ChessFile.D, 3)
            .CreatePieceAt('k', ChessFile.F, 5)
            .CreatePieceAt('r', ChessFile.F, 3)
            .Build();

        var availableMoves = bishop.RecalculateAvailableMoves(board);

        availableMoves.Should().BeEmpty();
    }

    [Fact]
    public void RecalculateAvailableMoves_White_Captures_HasMoves()
    {
        var bishop = new Bishop(ChessFile.E, 4)
        {
            Color = ChessColor.White
        };

        var board = new ChessBoardBuilder()
            .PlacePiece(bishop)
            .CreatePieceAt('p', ChessFile.D, 5)
            .CreatePieceAt('q', ChessFile.D, 3)
            .CreatePieceAt('k', ChessFile.F, 5)
            .CreatePieceAt('r', ChessFile.F, 3)
            .Build();

        var availableMoves = bishop.RecalculateAvailableMoves(board);
        var expectedMoves = new List<string>
        {
            "D5", "D3", "F5", "F3"
        };

        availableMoves.Should().BeEquivalentTo(expectedMoves);
    }

    [Fact]
    public void RecalculateAvailableMoves_Black_Captures_HasMoves()
    {
        var bishop = new Bishop(ChessFile.E, 4)
        {
            Color = ChessColor.Black
        };

        var board = new ChessBoardBuilder()
            .PlacePiece(bishop)
            .CreatePieceAt('P', ChessFile.D, 5)
            .CreatePieceAt('Q', ChessFile.D, 3)
            .CreatePieceAt('K', ChessFile.F, 5)
            .CreatePieceAt('R', ChessFile.F, 3)
            .Build();

        var availableMoves = bishop.RecalculateAvailableMoves(board);
        var expectedMoves = new List<string>
        {
            "D5", "D3", "F5", "F3"
        };

        availableMoves.Should().BeEquivalentTo(expectedMoves);
    }
}