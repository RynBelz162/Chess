using Chess.Console.Services;
using Chess.Shared.Constants;
using Chess.Shared.Models;
using Chess.Shared.Models.State;
using Spectre.Console.Testing;

namespace Chess.Console.Tests.Services;

public class BoardRendererServiceTests
{
    private const string StartingFen = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR";

    private static (BoardRendererService service, TestConsole console) CreateService()
    {
        var console = new TestConsole();
        return (new BoardRendererService(console), console);
    }

    private static GameStateSnapshot Snapshot(string fen, int whitePoints = 0, int blackPoints = 0) => new()
    {
        PlayerOne = new Player { Color = ChessColor.White, Points = whitePoints },
        PlayerTwo = new Player { Color = ChessColor.Black, Points = blackPoints },
        CurrentFen = fen,
    };

    private static string[] GetOutputLines(TestConsole console) =>
        console.Output.Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

    [Fact]
    public void Render_WhenWhitePerspective_ShouldShowFileLabelsLeftToRight()
    {
        var (service, console) = CreateService();

        service.Render(Snapshot(StartingFen), ChessColor.White);

        var lines = GetOutputLines(console);
        lines[17].Should().Be("a   b   c   d   e   f   g   h");
    }

    [Fact]
    public void Render_WhenWhitePerspective_ShouldShowRank8AsFirstRankRow()
    {
        var (service, console) = CreateService();

        service.Render(Snapshot(StartingFen), ChessColor.White);

        var lines = GetOutputLines(console);
        lines[1].Should().StartWith("8");
    }

    [Fact]
    public void Render_WhenWhitePerspective_ShouldShowRank1AsLastRankRow()
    {
        var (service, console) = CreateService();

        service.Render(Snapshot(StartingFen), ChessColor.White);

        var lines = GetOutputLines(console);
        lines[15].Should().StartWith("1");
    }

    [Fact]
    public void Render_WhenWhitePerspective_ShouldShowBlackPiecesOnRank8()
    {
        var (service, console) = CreateService();

        service.Render(Snapshot(StartingFen), ChessColor.White);

        var lines = GetOutputLines(console);
        lines[1].Should().Be("8 | r | n | b | q | k | b | n | r |");
    }

    [Fact]
    public void Render_WhenWhitePerspective_ShouldShowWhitePiecesOnRank1()
    {
        var (service, console) = CreateService();

        service.Render(Snapshot(StartingFen), ChessColor.White);

        var lines = GetOutputLines(console);
        lines[15].Should().Be("1 | R | N | B | Q | K | B | N | R |");
    }

    [Fact]
    public void Render_WhenWhitePerspective_ShouldShowDotsForEmptyRanks()
    {
        var (service, console) = CreateService();

        service.Render(Snapshot(StartingFen), ChessColor.White);

        var lines = GetOutputLines(console);
        // Ranks 6,5,4,3 are empty at lines 5,7,9,11
        foreach (int i in new[] { 5, 7, 9, 11 })
        {
            lines[i].Should().Contain("·");
        }
    }

    [Fact]
    public void Render_WhenBlackPerspective_ShouldShowFileLabelsRightToLeft()
    {
        var (service, console) = CreateService();

        service.Render(Snapshot(StartingFen), ChessColor.Black);

        var lines = GetOutputLines(console);
        lines[17].Should().Be("h   g   f   e   d   c   b   a");
    }

    [Fact]
    public void Render_WhenBlackPerspective_ShouldShowRank1AsFirstRankRow()
    {
        var (service, console) = CreateService();

        service.Render(Snapshot(StartingFen), ChessColor.Black);

        var lines = GetOutputLines(console);
        lines[1].Should().StartWith("1");
    }

    [Fact]
    public void Render_WhenBlackPerspective_ShouldShowRank8AsLastRankRow()
    {
        var (service, console) = CreateService();

        service.Render(Snapshot(StartingFen), ChessColor.Black);

        var lines = GetOutputLines(console);
        lines[15].Should().StartWith("8");
    }

    [Fact]
    public void Render_WhenBlackPerspective_ShouldShowBlackPiecesOnRank8WithFilesReversed()
    {
        var (service, console) = CreateService();

        service.Render(Snapshot(StartingFen), ChessColor.Black);

        var lines = GetOutputLines(console);
        lines[15].Should().Be("8 | r | n | b | k | q | b | n | r |");
    }

    [Fact]
    public void Render_WhenWhitePerspective_ShouldShowPlayerCapturesOnBottomRow()
    {
        var (service, console) = CreateService();
        // White is missing a knight and a pawn -> black captured them (shown top).
        // Black is missing a queen -> white captured it (shown bottom).
        var fen = "rnb1kbnr/pppppppp/8/8/8/8/PPPPPP1P/RNBQKB1R";

        service.Render(Snapshot(fen), ChessColor.White);

        var lines = GetOutputLines(console);
        lines[15].Should().Contain("q");   // white player's capture
        lines[1].Should().Contain("N").And.Contain("P"); // opponent's captures
    }

    [Fact]
    public void Render_WhenStartingPosition_ShouldShowNoCapturedPieces()
    {
        var (service, console) = CreateService();

        service.Render(Snapshot(StartingFen), ChessColor.White);

        var lines = GetOutputLines(console);
        lines[1].Should().Be("8 | r | n | b | q | k | b | n | r |");
        lines[15].Should().Be("1 | R | N | B | Q | K | B | N | R |");
    }

    [Fact]
    public void Render_WhenPlayerAhead_ShouldShowAdvantageOnBottomRow()
    {
        var (service, console) = CreateService();
        // White (player) up 5 points, shown next to white's captures (bottom).
        service.Render(Snapshot(StartingFen, whitePoints: 5, blackPoints: 0), ChessColor.White);

        var lines = GetOutputLines(console);
        lines[15].Should().Contain("+5");
        lines[1].Should().NotContain("+");
    }

    [Fact]
    public void Render_WhenOpponentAhead_ShouldShowAdvantageOnTopRow()
    {
        var (service, console) = CreateService();
        // Black (opponent) up 3 points, shown next to black's captures (top).
        service.Render(Snapshot(StartingFen, whitePoints: 0, blackPoints: 3), ChessColor.White);

        var lines = GetOutputLines(console);
        lines[1].Should().Contain("+3");
        lines[15].Should().NotContain("+");
    }

    [Fact]
    public void Render_WhenPointsEqual_ShouldShowNoAdvantage()
    {
        var (service, console) = CreateService();

        service.Render(Snapshot(StartingFen, whitePoints: 4, blackPoints: 4), ChessColor.White);

        var lines = GetOutputLines(console);
        lines[1].Should().NotContain("+");
        lines[15].Should().NotContain("+");
    }

    [Fact]
    public void Render_WhenCalled_ShouldOutput18Lines()
    {
        var (service, console) = CreateService();

        service.Render(Snapshot(StartingFen), ChessColor.White);

        // 1 top separator + 8 × (rank row + separator) + 1 file footer = 18
        GetOutputLines(console).Should().HaveCount(18);
    }

    [Fact]
    public void Render_WhenFenHasFullPosition_ShouldIgnoreNonPositionFields()
    {
        var (service, console) = CreateService();
        var fullFen = $"{StartingFen} w KQkq - 0 1";

        service.Render(Snapshot(fullFen), ChessColor.White);

        var lines = GetOutputLines(console);
        lines[1].Should().Be("8 | r | n | b | q | k | b | n | r |");
    }

    [Fact]
    public void Render_WhenBoardEmpty_ShouldShowAllSquaresAsDots()
    {
        var (service, console) = CreateService();
        var emptyFen = "8/8/8/8/8/8/8/8";

        service.Render(Snapshot(emptyFen), ChessColor.White);

        var lines = GetOutputLines(console);
        foreach (int i in new[] { 1, 3, 5, 7, 9, 11, 13, 15 })
        {
            // Only inspect the board cells, not the captured-piece tray to the right.
            var cells = lines[i].Split("   ")[0];
            cells.Should().Contain("·").And.NotContainAny("r", "n", "b", "q", "k", "p", "R", "N", "B", "Q", "K", "P");
        }
    }
}
