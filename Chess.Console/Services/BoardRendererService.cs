using Chess.Shared.Constants;
using Spectre.Console;

namespace Chess.Console.Services;

public class BoardRendererService(IAnsiConsole console) : IBoardRendererService
{
    private static readonly Dictionary<char, string> PieceDisplay = new()
    {
        { 'K', "[bold white]K[/]" },
        { 'Q', "[bold white]Q[/]" },
        { 'R', "[bold white]R[/]" },
        { 'B', "[bold white]B[/]" },
        { 'N', "[bold white]N[/]" },
        { 'P', "[bold white]P[/]" },
        { 'k', "[bold grey]k[/]" },
        { 'q', "[bold grey]q[/]" },
        { 'r', "[bold grey]r[/]" },
        { 'b', "[bold grey]b[/]" },
        { 'n', "[bold grey]n[/]" },
        { 'p', "[bold grey]p[/]" },
    };

    public void Render(string fen, ChessColor perspective)
    {
        var positionPart = fen.Split(' ')[0];
        var ranks = positionPart.Split('/');

        var board = ParseRanks(ranks);

        var files = "abcdefgh";
        var rankNumbers = Enumerable.Range(1, 8).ToArray();

        IEnumerable<int> rankOrder = perspective == ChessColor.White
            ? rankNumbers.Reverse()
            : rankNumbers;

        IEnumerable<int> fileOrder = perspective == ChessColor.White
            ? Enumerable.Range(0, 8)
            : Enumerable.Range(0, 8).Reverse();

        var separator = "  [dim]+---+---+---+---+---+---+---+---+[/]";

        console.MarkupLine(separator);
        foreach (var rank in rankOrder)
        {
            var row = board[rank - 1];
            var cells = string.Join("[dim]|[/]", fileOrder.Select(f => $" {CellMarkup(row[f])} "));
            console.MarkupLine($"[dim]{rank}[/] [dim]|[/]{cells}[dim]|[/]");
            console.MarkupLine(separator);
        }

        var fileLabels = string.Join("   ", fileOrder.Select(i => files[i]));
        console.MarkupLine($"    [dim]{fileLabels}[/]");
        console.WriteLine();
    }

    internal static char[][] ParseRanks(string[] ranks)
    {
        var board = new char[8][];
        for (int rankIdx = 0; rankIdx < 8; rankIdx++)
        {
            int rankNumber = 8 - rankIdx;
            board[rankNumber - 1] = new char[8];
            int fileIdx = 0;
            foreach (char c in ranks[rankIdx])
            {
                if (char.IsDigit(c))
                    fileIdx += c - '0';
                else
                    board[rankNumber - 1][fileIdx++] = c;
            }
        }
        return board;
    }

    private static string CellMarkup(char piece) =>
        piece == '\0' ? "[dim]·[/]" : PieceDisplay.GetValueOrDefault(piece, piece.ToString());
}
