using Chess.Shared.Constants;
using Chess.Shared.Helpers;
using Chess.Shared.Models.Pieces;

namespace Chess.Shared.Tests.Models.Pieces;

public class PieceTests
{
    [Fact]
    public void Move_Capture_MarksCapturedAndRemovesFromBoard()
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

    [Fact]
    public void Move_Capture_PlacesMoverOnTargetAndVacatesSource()
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
    public void Move_Capture_RemovedPieceDroppedFromFen()
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
