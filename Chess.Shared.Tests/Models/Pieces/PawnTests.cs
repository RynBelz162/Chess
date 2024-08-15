using Chess.Shared.Constants;
using Chess.Shared.Helpers;
using Chess.Shared.Models.Pieces;

namespace Chess.Shared.Tests.Models.Pieces;

public class PawnTests
{
    [Theory]
    [MemberData(nameof(ForwardData))]
    public void RecalculateAvailableMoves_EmptyForward_CanMoveForward(ChessColor color, int numberOfMoves, string[] expectedMoves)
    {
        var pawn = new Pawn(ChessFile.B, 5)
        {
            Color = color,
            NumberOfMoves = numberOfMoves,
        };

        var mockBoard = new ChessBoardBuilder()
            .PlacePiece(pawn)
            .Build();

        var moves = pawn.RecalculateAvailableMoves(mockBoard);
        moves.Should().BeEquivalentTo(expectedMoves);
    }

    public static TheoryData<ChessColor, int,  string[]> ForwardData =>
        new()
        {
            { ChessColor.White, 0,  [ "B6", "B7" ] },
            { ChessColor.White, 1, [ "B6" ] },
            { ChessColor.Black, 0, [ "B4", "B3" ] },
            { ChessColor.Black, 3, [ "B4" ] },
        };

    [Theory]
    [InlineData('p')]
    [InlineData('P')]
    [InlineData('r')]
    [InlineData('B')]
    [InlineData('q')]
    public void RecalculateAvailableMoves_White_SpaceTakenAbove_CantMoveUp(char pieceIdentifier)
    {
        var pawn = new Pawn(ChessFile.B, 2)
        {
            Color = ChessColor.White,
        };

        var mockBoard = new ChessBoardBuilder()
            .CreatePieceAt(pieceIdentifier, ChessFile.B, 3)
            .PlacePiece(pawn)
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
        var pawn = new Pawn(ChessFile.B, 2)
        {
            Color = ChessColor.White,
        };

        var mockBoard = new ChessBoardBuilder()
            .CreatePieceAt(pieceIdentifier, ChessFile.A, 3)
            .PlacePiece(pawn)
            .Build();

        var moves = pawn.RecalculateAvailableMoves(mockBoard);
        List<string> expectedMoves = ["B3", "B4", "A3"];

        moves.Should().BeEquivalentTo(expectedMoves);
    }

    [Fact]
    public void RecalculateAvailableMoves_White_CanEnPassant()
    {
        var pawn = new Pawn(ChessFile.B, 2)
        {
            Color = ChessColor.White,
        };

        var mockBoard = new ChessBoardBuilder()
            .CreatePieceAt('p', ChessFile.A, 2, 0)
            .CreatePieceAt('p', ChessFile.C, 2, 0)
            .PlacePiece(pawn)
            .Build();

        var moves = pawn.RecalculateAvailableMoves(mockBoard);
        List<string> expectedMoves = ["B3", "B4", "C3", "A3"];

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
        var pawn = new Pawn(ChessFile.B, 2)
        {
            Color = ChessColor.White,
        };

        var mockBoard = new ChessBoardBuilder()
            .CreatePieceAt(pieceIdentifier, ChessFile.A, 2, 0)
            .CreatePieceAt(pieceIdentifier, ChessFile.C, 2, 0)
            .PlacePiece(pawn)
            .Build();

        var moves = pawn.RecalculateAvailableMoves(mockBoard);
        List<string> expectedMoves = ["B3", "B4"];

        moves.Should().BeEquivalentTo(expectedMoves);
    }

    [Fact]
    public void RecalculateAvailableMoves_White_AlreadyMovedFromStart_CantEnPassant()
    {
        var pawn = new Pawn(ChessFile.B, 2)
        {
            Color = ChessColor.White,
        };

        var mockBoard = new ChessBoardBuilder()
            .CreatePieceAt('p', ChessFile.A, 2, 1)
            .CreatePieceAt('p', ChessFile.C, 2, 1)
            .PlacePiece(pawn)
            .Build();

        var moves = pawn.RecalculateAvailableMoves(mockBoard);
        List<string> expectedMoves = ["B3", "B4"];

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
        var pawn = new Pawn(ChessFile.B, 3)
        {
            Color = ChessColor.Black,
        };

        var mockBoard = new ChessBoardBuilder()
            .CreatePieceAt(pieceIdentifier, ChessFile.B, 2)
            .PlacePiece(pawn)
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
        var pawn = new Pawn(ChessFile.B, 6)
        {
            Color = ChessColor.Black,
            NumberOfMoves = 2,
        };

        var mockBoard = new ChessBoardBuilder()
            .CreatePieceAt(pieceIdentifier, ChessFile.A, 5)
            .PlacePiece(pawn)
            .Build();

        var moves = pawn.RecalculateAvailableMoves(mockBoard);
        List<string> expectedMoves = ["B5", "A5"];

        moves.Should().BeEquivalentTo(expectedMoves);
    }

    [Fact]
    public void RecalculateAvailableMoves_Black_CanEnPassant()
    {
        var pawn = new Pawn(ChessFile.B, 6)
        {
            Color = ChessColor.Black,
        };

        var mockBoard = new ChessBoardBuilder()
            .CreatePieceAt('P', ChessFile.A, 6, 0)
            .CreatePieceAt('P', ChessFile.C, 6, 0)
            .PlacePiece(pawn)
            .Build();

        var moves = pawn.RecalculateAvailableMoves(mockBoard);
        List<string> expectedMoves = ["B5", "B4", "C5", "A5"];

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
        var pawn = new Pawn(ChessFile.B, 5)
        {
            Color = ChessColor.Black,
            NumberOfMoves = 3,
        };

        var mockBoard = new ChessBoardBuilder()
            .CreatePieceAt(pieceIdentifier, ChessFile.A, 5, 0)
            .CreatePieceAt(pieceIdentifier, ChessFile.C, 5, 0)
            .PlacePiece(pawn)
            .Build();

        var moves = pawn.RecalculateAvailableMoves(mockBoard);
        List<string> expectedMoves = ["B4"];

        moves.Should().BeEquivalentTo(expectedMoves);
    }

    [Fact]
    public void RecalculateAvailableMoves_Black_AlreadyMovedFromStart_CantEnPassant()
    {
        var pawn = new Pawn(ChessFile.B, 7)
        {
            Color = ChessColor.Black,
        };

        var mockBoard = new ChessBoardBuilder()
            .CreatePieceAt('P', ChessFile.A, 7, 1)
            .CreatePieceAt('P', ChessFile.C, 7, 1)
            .PlacePiece(pawn)
            .Build();

        var moves = pawn.RecalculateAvailableMoves(mockBoard);
        List<string> expectedMoves = ["B6", "B5"];

        moves.Should().BeEquivalentTo(expectedMoves);
    }
}