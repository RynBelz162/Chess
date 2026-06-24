using Chess.Console.Services;
using Chess.Shared.Constants;
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

    private static string[] GetOutputLines(TestConsole console) =>
        console.Output.Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

    [Fact]
    public void Render_WhitePerspective_FileLabelsAreLeftToRight()
    {
        var (service, console) = CreateService();

        service.Render(StartingFen, ChessColor.White);

        var lines = GetOutputLines(console);
        lines[17].Should().Be("a   b   c   d   e   f   g   h");
    }

    [Fact]
    public void Render_WhitePerspective_Rank8IsFirstRankRow()
    {
        var (service, console) = CreateService();

        service.Render(StartingFen, ChessColor.White);

        var lines = GetOutputLines(console);
        lines[1].Should().StartWith("8");
    }

    [Fact]
    public void Render_WhitePerspective_Rank1IsLastRankRow()
    {
        var (service, console) = CreateService();

        service.Render(StartingFen, ChessColor.White);

        var lines = GetOutputLines(console);
        lines[15].Should().StartWith("1");
    }

    [Fact]
    public void Render_WhitePerspective_BlackPiecesOnRank8()
    {
        var (service, console) = CreateService();

        service.Render(StartingFen, ChessColor.White);

        var lines = GetOutputLines(console);
        lines[1].Should().Be("8 | r | n | b | q | k | b | n | r |");
    }

    [Fact]
    public void Render_WhitePerspective_WhitePiecesOnRank1()
    {
        var (service, console) = CreateService();

        service.Render(StartingFen, ChessColor.White);

        var lines = GetOutputLines(console);
        lines[15].Should().Be("1 | R | N | B | Q | K | B | N | R |");
    }

    [Fact]
    public void Render_WhitePerspective_EmptyRanksShowDots()
    {
        var (service, console) = CreateService();

        service.Render(StartingFen, ChessColor.White);

        var lines = GetOutputLines(console);
        // Ranks 6,5,4,3 are empty at lines 5,7,9,11
        foreach (int i in new[] { 5, 7, 9, 11 })
        {
            lines[i].Should().Contain("·");
        }
    }

    [Fact]
    public void Render_BlackPerspective_FileLabelsAreRightToLeft()
    {
        var (service, console) = CreateService();

        service.Render(StartingFen, ChessColor.Black);

        var lines = GetOutputLines(console);
        lines[17].Should().Be("h   g   f   e   d   c   b   a");
    }

    [Fact]
    public void Render_BlackPerspective_Rank1IsFirstRankRow()
    {
        var (service, console) = CreateService();

        service.Render(StartingFen, ChessColor.Black);

        var lines = GetOutputLines(console);
        lines[1].Should().StartWith("1");
    }

    [Fact]
    public void Render_BlackPerspective_Rank8IsLastRankRow()
    {
        var (service, console) = CreateService();

        service.Render(StartingFen, ChessColor.Black);

        var lines = GetOutputLines(console);
        lines[15].Should().StartWith("8");
    }

    [Fact]
    public void Render_BlackPerspective_BlackPiecesOnRank8_FilesReversed()
    {
        var (service, console) = CreateService();

        service.Render(StartingFen, ChessColor.Black);

        var lines = GetOutputLines(console);
        lines[15].Should().Be("8 | r | n | b | k | q | b | n | r |");
    }

    [Fact]
    public void Render_WhitePerspective_PlayerCapturesOnBottomRow()
    {
        var (service, console) = CreateService();
        // White is missing a knight and a pawn -> black captured them (shown top).
        // Black is missing a queen -> white captured it (shown bottom).
        var fen = "rnb1kbnr/pppppppp/8/8/8/8/PPPPPP1P/RNBQKB1R";

        service.Render(fen, ChessColor.White);

        var lines = GetOutputLines(console);
        lines[15].Should().Contain("q");   // white player's capture
        lines[1].Should().Contain("N").And.Contain("P"); // opponent's captures
    }

    [Fact]
    public void Render_StartingPosition_NoCapturedPieces()
    {
        var (service, console) = CreateService();

        service.Render(StartingFen, ChessColor.White);

        var lines = GetOutputLines(console);
        lines[1].Should().Be("8 | r | n | b | q | k | b | n | r |");
        lines[15].Should().Be("1 | R | N | B | Q | K | B | N | R |");
    }

    [Fact]
    public void Render_OutputHas18Lines()
    {
        var (service, console) = CreateService();

        service.Render(StartingFen, ChessColor.White);

        // 1 top separator + 8 × (rank row + separator) + 1 file footer = 18
        GetOutputLines(console).Should().HaveCount(18);
    }

    [Fact]
    public void Render_FenWithFullPosition_IgnoresNonPositionFields()
    {
        var (service, console) = CreateService();
        var fullFen = $"{StartingFen} w KQkq - 0 1";

        service.Render(fullFen, ChessColor.White);

        var lines = GetOutputLines(console);
        lines[1].Should().Be("8 | r | n | b | q | k | b | n | r |");
    }

    [Fact]
    public void Render_EmptyBoard_AllSquaresAreDots()
    {
        var (service, console) = CreateService();
        var emptyFen = "8/8/8/8/8/8/8/8";

        service.Render(emptyFen, ChessColor.White);

        var lines = GetOutputLines(console);
        foreach (int i in new[] { 1, 3, 5, 7, 9, 11, 13, 15 })
        {
            // Only inspect the board cells, not the captured-piece tray to the right.
            var cells = lines[i].Split("   ")[0];
            cells.Should().Contain("·").And.NotContainAny("r", "n", "b", "q", "k", "p", "R", "N", "B", "Q", "K", "P");
        }
    }
}
