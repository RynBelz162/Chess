using Chess.Shared.Constants;
using Chess.Shared.Models.Pieces;

namespace Chess.Shared.Tests.Models.Pieces;

public class PawnTests
{
    [Theory]
    [MemberData(nameof(ForwardData))]
    public void RecalculateAvailableMoves_EmptyForward_CanMoveForward(ChessColor color, int numberOfMoves, List<string> expectedMoves)
    {
        var pawn = new Pawn
        {
            Color = color,
            NumberOfMoves = numberOfMoves,
        };

        var mockBoard = new ChessBoardBuilder()
            .PlacePieceAt(pawn, ChessFile.B, 5)
            .Build();

        var moves = pawn.RecalculateAvailableMoves(mockBoard);
        moves.Should().BeEquivalentTo(expectedMoves);
    }

    public static IEnumerable<object[]> ForwardData =>
        new List<object[]>
        {
            new object[] { ChessColor.White, 0, new List<string> { "B6", "B7" } },
            new object[] { ChessColor.White, 1, new List<string> { "B6" } },
            new object[] { ChessColor.Black, 0, new List<string> { "B4", "B3" } },
            new object[] { ChessColor.Black, 3, new List<string> { "B4" } },
        };

    [Theory]
    [InlineData('p')]
    [InlineData('P')]
    [InlineData('r')]
    [InlineData('B')]
    [InlineData('q')]
    public void RecalculateAvailableMoves_White_SpaceTakenAbove_CantMoveUp(char pieceIdentifier)
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
    public void RecalculateAvailableMoves_White_LeftCapture_DiagonalLeft(char pieceIdentifier)
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
        List<string> expectedMoves = new() { "B3", "B4", "A3" };

        moves.Should().BeEquivalentTo(expectedMoves);
    }

    [Fact]
    public void RecalculateAvailableMoves_White_CanEnPassant()
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
        List<string> expectedMoves = new() { "B3", "B4", "C3", "A3" };

        moves.Should().BeEquivalentTo(expectedMoves);
    }

    [Theory]
    [InlineData('n')]
    [InlineData('r')]
    [InlineData('b')]
    [InlineData('q')]
    [InlineData('k')]
    public void RecalculateAvailableMoves_White_IsNotPawn_CantEnPassant(char pieceIdentifier)
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
        List<string> expectedMoves = new() { "B3", "B4" };

        moves.Should().BeEquivalentTo(expectedMoves);
    }

    [Fact]
    public void RecalculateAvailableMoves_White_AlreadyMovedFromStart_CantEnPassant()
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
        List<string> expectedMoves = new() { "B3", "B4" };

        moves.Should().BeEquivalentTo(expectedMoves);
    }

    [Theory]
    [InlineData('P')]
    [InlineData('p')]
    [InlineData('R')]
    [InlineData('b')]
    [InlineData('Q')]
    public void RecalculateAvailableMoves_Black_SpaceTakenBelow_CantMoveBelow(char pieceIdentifier)
    {
        var pawn = new Pawn
        {
            Color = ChessColor.Black,
        };

        var mockBoard = new ChessBoardBuilder()
            .CreatePieceAt(pieceIdentifier, ChessFile.B, 2)
            .PlacePieceAt(pawn, ChessFile.B, 3)
            .Build();

        var moves = pawn.RecalculateAvailableMoves(mockBoard);
        moves.Should().BeEmpty();
    }

    [Theory]
    [InlineData('P')]
    [InlineData('N')]
    [InlineData('R')]
    [InlineData('B')]
    [InlineData('Q')]
    public void RecalculateAvailableMoves_Black_LeftCapture_DiagonalLeft(char pieceIdentifier)
    {
        var pawn = new Pawn
        {
            Color = ChessColor.Black,
            NumberOfMoves = 2,
        };

        var mockBoard = new ChessBoardBuilder()
            .CreatePieceAt(pieceIdentifier, ChessFile.A, 5)
            .PlacePieceAt(pawn, ChessFile.B, 6)
            .Build();

        var moves = pawn.RecalculateAvailableMoves(mockBoard);
        List<string> expectedMoves = new() { "B5", "A5" };

        moves.Should().BeEquivalentTo(expectedMoves);
    }

    [Fact]
    public void RecalculateAvailableMoves_Black_CanEnPassant()
    {
        var pawn = new Pawn
        {
            Color = ChessColor.Black,
        };

        var mockBoard = new ChessBoardBuilder()
            .CreatePieceAt('P', ChessFile.A, 6, 0)
            .CreatePieceAt('P', ChessFile.C, 6, 0)
            .PlacePieceAt(pawn, ChessFile.B, 6)
            .Build();

        var moves = pawn.RecalculateAvailableMoves(mockBoard);
        List<string> expectedMoves = new() { "B5", "B4", "C5", "A5" };

        moves.Should().BeEquivalentTo(expectedMoves);
    }

    [Theory]
    [InlineData('N')]
    [InlineData('R')]
    [InlineData('B')]
    [InlineData('Q')]
    [InlineData('K')]
    public void RecalculateAvailableMoves_Black_IsNotPawn_CantEnPassant(char pieceIdentifier)
    {
        var pawn = new Pawn
        {
            Color = ChessColor.Black,
            NumberOfMoves = 3,
        };

        var mockBoard = new ChessBoardBuilder()
            .CreatePieceAt(pieceIdentifier, ChessFile.A, 5, 0)
            .CreatePieceAt(pieceIdentifier, ChessFile.C, 5, 0)
            .PlacePieceAt(pawn, ChessFile.B, 5)
            .Build();

        var moves = pawn.RecalculateAvailableMoves(mockBoard);
        List<string> expectedMoves = new() { "B4" };

        moves.Should().BeEquivalentTo(expectedMoves);
    }

    [Fact]
    public void RecalculateAvailableMoves_Black_AlreadyMovedFromStart_CantEnPassant()
    {
        var pawn = new Pawn
        {
            Color = ChessColor.Black,
        };

        var mockBoard = new ChessBoardBuilder()
            .CreatePieceAt('P', ChessFile.A, 7, 1)
            .CreatePieceAt('P', ChessFile.C, 7, 1)
            .PlacePieceAt(pawn, ChessFile.B, 7)
            .Build();

        var moves = pawn.RecalculateAvailableMoves(mockBoard);
        List<string> expectedMoves = new() { "B6", "B5" };

        moves.Should().BeEquivalentTo(expectedMoves);
    }
}