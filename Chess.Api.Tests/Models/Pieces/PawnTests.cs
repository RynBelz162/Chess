using Chess.Api.Constants;
using Chess.Api.Models.Pieces;

namespace Chess.Api.Tests.Models.Pieces;

public class PawnTests
{
    [Fact]
    public void RecalculateAvailableMoves_EmptyAbove_CanMoveUp()
    {
        var pawn = new Pawn
        {
            Color = ChessColor.White,
        };

        var mockBoard = new ChessBoardBuilder()
            .PlacePieceAt(pawn, ChessFile.B, 2)
            .Build();

        var moves = pawn.RecalculateAvailableMoves(mockBoard);
        List<string> expectedMoves = new() { "B3" };

        moves.Should().BeEquivalentTo(expectedMoves);
    }

    [Theory]
    [InlineData('p')]
    [InlineData('P')]
    [InlineData('r')]
    [InlineData('B')]
    [InlineData('q')]
    public void RecalculateAvailableMoves_SpaceTakenAbove_CantMoveUp(char pieceIdentifier)
    {
        var pawn = new Pawn
        {
            Color = ChessColor.White,
        };

        var mockBoard = new ChessBoardBuilder()
            .CreatePieceAt(pieceIdentifier, ChessFile.B, 3)
            .PlacePieceAt(pawn, ChessFile.B, 2)
            .Build();

        var moves = pawn.RecalculateAvailableMoves(mockBoard);
        moves.Should().BeEmpty();
    }

    [Theory]
    [InlineData('p')]
    [InlineData('n')]
    [InlineData('r')]
    [InlineData('b')]
    [InlineData('q')]
    public void RecalculateAvailableMoves_LeftCapture_DiagonalLeft(char pieceIdentifier)
    {
        var pawn = new Pawn
        {
            Color = ChessColor.White,
        };

        var mockBoard = new ChessBoardBuilder()
            .CreatePieceAt(pieceIdentifier, ChessFile.A, 3)
            .PlacePieceAt(pawn, ChessFile.B, 2)
            .Build();

        var moves = pawn.RecalculateAvailableMoves(mockBoard);
        List<string> expectedMoves = new() { "B3", "A3" };

        moves.Should().BeEquivalentTo(expectedMoves);
    }

    [Fact]
    public void RecalculateAvailableMoves_CanEnPassant()
    {
        var pawn = new Pawn
        {
            Color = ChessColor.White,
        };

        var mockBoard = new ChessBoardBuilder()
            .CreatePieceAt('p', ChessFile.A, 2, 0)
            .CreatePieceAt('p', ChessFile.C, 2, 0)
            .PlacePieceAt(pawn, ChessFile.B, 2)
            .Build();

        var moves = pawn.RecalculateAvailableMoves(mockBoard);
        List<string> expectedMoves = new() { "B3", "C3", "A3" };

        moves.Should().BeEquivalentTo(expectedMoves);
    }

    [Theory]
    [InlineData('n')]
    [InlineData('r')]
    [InlineData('b')]
    [InlineData('q')]
    [InlineData('k')]
    public void RecalculateAvailableMoves_IsNotPawn_CantEnPassant(char pieceIdentifier)
    {
        var pawn = new Pawn
        {
            Color = ChessColor.White,
        };

        var mockBoard = new ChessBoardBuilder()
            .CreatePieceAt(pieceIdentifier, ChessFile.A, 2, 0)
            .CreatePieceAt(pieceIdentifier, ChessFile.C, 2, 0)
            .PlacePieceAt(pawn, ChessFile.B, 2)
            .Build();

        var moves = pawn.RecalculateAvailableMoves(mockBoard);
        List<string> expectedMoves = new() { "B3" };

        moves.Should().BeEquivalentTo(expectedMoves);
    }

    [Fact]
    public void RecalculateAvailableMoves_AlreadyMovedFromStart_CantEnPassant()
    {
        var pawn = new Pawn
        {
            Color = ChessColor.White,
        };

        var mockBoard = new ChessBoardBuilder()
            .CreatePieceAt('p', ChessFile.A, 2, 1)
            .CreatePieceAt('p', ChessFile.C, 2, 1)
            .PlacePieceAt(pawn, ChessFile.B, 2)
            .Build();

        var moves = pawn.RecalculateAvailableMoves(mockBoard);
        List<string> expectedMoves = new() { "B3" };

        moves.Should().BeEquivalentTo(expectedMoves);
    }
}