using Chess.Shared.Constants;
using Chess.Shared.Helpers;
using Chess.Shared.Models.Pieces;

namespace Chess.Shared.Tests.Models.Pieces;

public class PieceTests
{
    [Fact]
    public void Move_WhenCapturing_ShouldMarkCapturedAndRemoveFromBoard()
    {
        var rook = new Rook(ChessFile.E, 4)
        {
            Color = ChessColor.White
        };

        var board = new ChessBoardBuilder()
            .PlacePiece(rook)
            .CreatePieceAt('q', ChessFile.E, 7)
            .Build();

        var captured = board.PieceOnSquare("E7")!;

        rook.Move("E7", board);

        captured.IsCaptured.Should().BeTrue();
        board.Pieces.Should().NotContain(captured);
    }

    [Theory]
    [InlineData('q', 9)]
    [InlineData('r', 5)]
    [InlineData('b', 3)]
    [InlineData('n', 3)]
    [InlineData('p', 1)]
    public void Move_WhenCapturing_ShouldReturnCapturedPieceValue(char capturedPiece, int expectedValue)
    {
        var rook = new Rook(ChessFile.E, 4)
        {
            Color = ChessColor.White
        };

        var board = new ChessBoardBuilder()
            .PlacePiece(rook)
            .CreatePieceAt(capturedPiece, ChessFile.E, 7)
            .Build();

        var result = rook.Move("E7", board);

        result.Value.Should().Be(expectedValue);
    }

    [Fact]
    public void Move_WhenNotCapturing_ShouldReturnZero()
    {
        var rook = new Rook(ChessFile.E, 4)
        {
            Color = ChessColor.White
        };

        var board = new ChessBoardBuilder()
            .PlacePiece(rook)
            .Build();

        var result = rook.Move("E7", board);

        result.Value.Should().Be(0);
    }

    [Fact]
    public void Move_WhenCapturing_ShouldPlaceMoverOnTargetAndVacateSource()
    {
        var rook = new Rook(ChessFile.E, 4)
        {
            Color = ChessColor.White
        };

        var board = new ChessBoardBuilder()
            .PlacePiece(rook)
            .CreatePieceAt('q', ChessFile.E, 7)
            .Build();

        rook.Move("E7", board);

        board.PieceOnSquare("E7").Should().Be(rook);
        board.PieceOnSquare("E4").Should().BeNull();
        rook.CurrentSquare.Should().Be("E7");
        board.Pieces.Should().Contain(rook);
    }

    [Fact]
    public void Move_WhenCapturing_ShouldDropRemovedPieceFromFen()
    {
        var rook = new Rook(ChessFile.E, 4)
        {
            Color = ChessColor.White
        };

        var board = new ChessBoardBuilder()
            .PlacePiece(rook)
            .CreatePieceAt('q', ChessFile.E, 7)
            .Build();

        rook.Move("E7", board);
        board.UpdateFen();

        // Black queen ('q') is gone, only the white rook ('R') remains.
        board.CurrentFen.Should().NotContain("q");
        board.CurrentFen.Should().Contain("R");
    }
}
