using Chess.Shared.Constants;
using Chess.Shared.Helpers;
using Chess.Shared.Models.Pieces;

namespace Chess.Shared.Tests.Models.Pieces;

public class PawnTests
{
    [Theory]
    [MemberData(nameof(ForwardData))]
    public void RecalculateAvailableMoves_WhenForwardEmpty_ShouldMoveForward(ChessColor color, int numberOfMoves, string[] expectedMoves)
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
    public void RecalculateAvailableMoves_WhenWhiteAndSpaceTakenAbove_ShouldNotMoveUp(char pieceIdentifier)
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
    public void RecalculateAvailableMoves_WhenWhiteAndLeftCaptureAvailable_ShouldMoveDiagonalLeft(char pieceIdentifier)
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
    public void RecalculateAvailableMoves_WhenWhiteAndEnPassantAvailable_ShouldAllowEnPassant()
    {
        var pawn = new Pawn(ChessFile.B, 5)
        {
            Color = ChessColor.White,
            NumberOfMoves = 1,
        };

        var mockBoard = new ChessBoardBuilder()
            .CreatePieceAt('p', ChessFile.A, 5, 1)
            .CreatePieceAt('p', ChessFile.C, 5, 1)
            .PlacePiece(pawn)
            .Build();

        var moves = pawn.RecalculateAvailableMoves(mockBoard);
        List<string> expectedMoves = ["B6", "C6", "A6"];

        moves.Should().BeEquivalentTo(expectedMoves);
    }

    [Theory]
    [InlineData('n')]
    [InlineData('r')]
    [InlineData('b')]
    [InlineData('q')]
    [InlineData('k')]
    public void RecalculateAvailableMoves_WhenWhiteAndTargetIsNotPawn_ShouldNotAllowEnPassant(char pieceIdentifier)
    {
        var pawn = new Pawn(ChessFile.B, 5)
        {
            Color = ChessColor.White,
            NumberOfMoves = 1,
        };

        var mockBoard = new ChessBoardBuilder()
            .CreatePieceAt(pieceIdentifier, ChessFile.A, 5, 1)
            .CreatePieceAt(pieceIdentifier, ChessFile.C, 5, 1)
            .PlacePiece(pawn)
            .Build();

        var moves = pawn.RecalculateAvailableMoves(mockBoard);
        List<string> expectedMoves = ["B6"];

        moves.Should().BeEquivalentTo(expectedMoves);
    }

    [Fact]
    public void RecalculateAvailableMoves_WhenWhiteAndVictimMovedMoreThanOnce_ShouldNotAllowEnPassant()
    {
        // A pawn that has moved more than once did not just double-move, so it is
        // no longer a valid en passant victim.
        var pawn = new Pawn(ChessFile.B, 5)
        {
            Color = ChessColor.White,
            NumberOfMoves = 1,
        };

        var mockBoard = new ChessBoardBuilder()
            .CreatePieceAt('p', ChessFile.A, 5, 2)
            .CreatePieceAt('p', ChessFile.C, 5, 2)
            .PlacePiece(pawn)
            .Build();

        var moves = pawn.RecalculateAvailableMoves(mockBoard);
        List<string> expectedMoves = ["B6"];

        moves.Should().BeEquivalentTo(expectedMoves);
    }

    [Theory]
    [InlineData('P')]
    [InlineData('p')]
    [InlineData('R')]
    [InlineData('b')]
    [InlineData('Q')]
    public void RecalculateAvailableMoves_WhenBlackAndSpaceTakenBelow_ShouldNotMoveDown(char pieceIdentifier)
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
    public void RecalculateAvailableMoves_WhenBlackAndLeftCaptureAvailable_ShouldMoveDiagonalLeft(char pieceIdentifier)
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
    public void RecalculateAvailableMoves_WhenBlackAndEnPassantAvailable_ShouldAllowEnPassant()
    {
        // Black pawn on its 4th rank; white pawns either side just advanced two
        // squares off their home rank (NumberOfMoves == 1).
        var pawn = new Pawn(ChessFile.B, 4)
        {
            Color = ChessColor.Black,
            NumberOfMoves = 1,
        };

        var mockBoard = new ChessBoardBuilder()
            .CreatePieceAt('P', ChessFile.A, 4, 1)
            .CreatePieceAt('P', ChessFile.C, 4, 1)
            .PlacePiece(pawn)
            .Build();

        var moves = pawn.RecalculateAvailableMoves(mockBoard);
        List<string> expectedMoves = ["B3", "C3", "A3"];

        moves.Should().BeEquivalentTo(expectedMoves);
    }

    [Theory]
    [InlineData('N')]
    [InlineData('R')]
    [InlineData('B')]
    [InlineData('Q')]
    [InlineData('K')]
    public void RecalculateAvailableMoves_WhenBlackAndTargetIsNotPawn_ShouldNotAllowEnPassant(char pieceIdentifier)
    {
        var pawn = new Pawn(ChessFile.B, 4)
        {
            Color = ChessColor.Black,
            NumberOfMoves = 1,
        };

        var mockBoard = new ChessBoardBuilder()
            .CreatePieceAt(pieceIdentifier, ChessFile.A, 4, 1)
            .CreatePieceAt(pieceIdentifier, ChessFile.C, 4, 1)
            .PlacePiece(pawn)
            .Build();

        var moves = pawn.RecalculateAvailableMoves(mockBoard);
        List<string> expectedMoves = ["B3"];

        moves.Should().BeEquivalentTo(expectedMoves);
    }

    [Fact]
    public void RecalculateAvailableMoves_WhenBlackAndVictimMovedMoreThanOnce_ShouldNotAllowEnPassant()
    {
        var pawn = new Pawn(ChessFile.B, 4)
        {
            Color = ChessColor.Black,
            NumberOfMoves = 1,
        };

        var mockBoard = new ChessBoardBuilder()
            .CreatePieceAt('P', ChessFile.A, 4, 2)
            .CreatePieceAt('P', ChessFile.C, 4, 2)
            .PlacePiece(pawn)
            .Build();

        var moves = pawn.RecalculateAvailableMoves(mockBoard);
        List<string> expectedMoves = ["B3"];

        moves.Should().BeEquivalentTo(expectedMoves);
    }

    [Fact]
    public void Move_WhenWhiteCapturesEnPassant_ShouldRemoveVictimPawn()
    {
        // White pawn on its 5th rank; black pawn beside it just double-moved
        // (NumberOfMoves == 1). Capturing to C6 removes the victim on C5.
        var pawn = new Pawn(ChessFile.B, 5)
        {
            Color = ChessColor.White,
            NumberOfMoves = 1,
        };

        var board = new ChessBoardBuilder()
            .CreatePieceAt('p', ChessFile.C, 5, 1)
            .PlacePiece(pawn)
            .Build();

        var result = pawn.Move("C6", board);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(1);
        pawn.CurrentSquare.Should().Be("C6");
        board.IsSquareOccupied("C5").Should().BeFalse();
        board.Pieces.Should().NotContain(p => p is Pawn && p.Color == ChessColor.Black);
    }

    [Fact]
    public void Move_WhenBlackCapturesEnPassant_ShouldRemoveVictimPawn()
    {
        // Black pawn on its 4th rank; white pawn beside it just double-moved
        // (NumberOfMoves == 1). Capturing to A3 removes the victim on A4.
        var pawn = new Pawn(ChessFile.B, 4)
        {
            Color = ChessColor.Black,
            NumberOfMoves = 1,
        };

        var board = new ChessBoardBuilder()
            .CreatePieceAt('P', ChessFile.A, 4, 1)
            .PlacePiece(pawn)
            .Build();

        var result = pawn.Move("A3", board);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(1);
        pawn.CurrentSquare.Should().Be("A3");
        board.IsSquareOccupied("A4").Should().BeFalse();
        board.Pieces.Should().NotContain(p => p is Pawn && p.Color == ChessColor.White);
    }

    [Fact]
    public void Move_WhenNormalDiagonalCapture_ShouldNotRemoveAdjacentPawn()
    {
        var pawn = new Pawn(ChessFile.B, 2)
        {
            Color = ChessColor.White,
        };

        var board = new ChessBoardBuilder()
            .CreatePieceAt('p', ChessFile.A, 3, 1)
            .CreatePieceAt('p', ChessFile.A, 2, 1)
            .PlacePiece(pawn)
            .Build();

        var result = pawn.Move("A3", board);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(1);
        pawn.CurrentSquare.Should().Be("A3");
        board.IsSquareOccupied("A2").Should().BeTrue();
    }

    [Fact]
    public void Move_WhenForwardMove_ShouldNotTriggerEnPassant()
    {
        var pawn = new Pawn(ChessFile.B, 2)
        {
            Color = ChessColor.White,
        };

        var board = new ChessBoardBuilder()
            .CreatePieceAt('p', ChessFile.C, 2, 0)
            .PlacePiece(pawn)
            .Build();

        var result = pawn.Move("B3", board);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(0); // nothing captured
        board.IsSquareOccupied("C2").Should().BeTrue(); // neighbor untouched
    }

    [Fact]
    public void Move_WhenSamePawnMovesTwice_ShouldAdvanceBothTimes()
    {
        // Regression: a pawn must keep its single forward move after it has
        // already moved once.
        var pawn = new Pawn(ChessFile.B, 2)
        {
            Color = ChessColor.White,
        };

        var board = new ChessBoardBuilder()
            .PlacePiece(pawn)
            .Build();

        pawn.Move("B3", board).IsSuccess.Should().BeTrue();
        pawn.NumberOfMoves.Should().Be(1);
        pawn.AvailableMoves.Should().Contain("B4");

        var second = pawn.Move("B4", board);

        second.IsSuccess.Should().BeTrue();
        pawn.CurrentSquare.Should().Be("B4");
        pawn.NumberOfMoves.Should().Be(2);
    }

    [Fact]
    public void Move_WhenCapturingUnmovedPawn_ShouldCapture()
    {
        // Regression: a normal diagonal capture must work regardless of how many
        // times the victim has moved (here, never).
        var pawn = new Pawn(ChessFile.B, 4)
        {
            Color = ChessColor.White,
        };

        var board = new ChessBoardBuilder()
            .CreatePieceAt('p', ChessFile.C, 5, 0)
            .PlacePiece(pawn)
            .Build();

        pawn.AvailableMoves.Should().Contain("C5");

        var result = pawn.Move("C5", board);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(1);
        pawn.CurrentSquare.Should().Be("C5");
        board.Pieces.Should().NotContain(p => p is Pawn && p.Color == ChessColor.Black);
    }
}