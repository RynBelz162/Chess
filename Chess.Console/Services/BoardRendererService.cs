using Chess.Shared.Constants;
using Chess.Shared.Models.State;
using Spectre.Console;

namespace Chess.Console.Services;

public class BoardRendererService(IAnsiConsole console) : IBoardRendererService
{
    private const string BoardLineSeparator = "  [dim]+---+---+---+---+---+---+---+---+[/]";

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

    public void Render(GameStateSnapshot snapshot, ChessColor perspective)
    {
        var isPlayingWhite = perspective == ChessColor.White;

        var positionPart = snapshot.CurrentFen.Split(' ')[0];
        var ranks = positionPart.Split('/');

        var board = ParseRanks(ranks);

        var files = "abcdefgh";

        var rankOrder = isPlayingWhite
            ? [.. Enumerable.Range(1, 8).Reverse()]
            : Enumerable.Range(1, 8).ToList();

        var fileOrder = isPlayingWhite
            ? [.. Enumerable.Range(0, 8)]
            : Enumerable.Range(0, 8).Reverse().ToList();

        var (opponentsCapturedPieces, playersCapturedPieces) = GetCapturedPieces(board, snapshot, isPlayingWhite);

        console.MarkupLine(BoardLineSeparator);
        for (int rankIdx = 0; rankIdx < rankOrder.Count; rankIdx++)
        {
            PrintRow(rankOrder, fileOrder, board, rankIdx, opponentsCapturedPieces, playersCapturedPieces);
        }

        var fileLabels = string.Join("   ", fileOrder.Select(i => files[i]));
        console.MarkupLine($"    [dim]{fileLabels}[/]");
        console.WriteLine();
    }

    private static char[][] ParseRanks(string[] ranks)
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

    // Full complement of each piece type in a standard set.
    private static readonly (char Type, int Count)[] FullSet =
    [
        ('Q', 1), ('R', 2), ('B', 2), ('N', 2), ('P', 8),
    ];

    private (string? opponentsCapturedPieces, string? playersCapturedPieces) GetCapturedPieces(char[][] board, GameStateSnapshot snapshot, bool isPlayingWhite)
    {
        var player = snapshot.GetPlayerByColor(isPlayingWhite ? ChessColor.White : ChessColor.Black);
        var opponent = snapshot.GetPlayerByColor(isPlayingWhite ? ChessColor.Black : ChessColor.White);

        var opponentsCapturedPieces = CapturedMarkup(board, capturedColorWhite: isPlayingWhite);
        var playersCapturedPieces = CapturedMarkup(board, capturedColorWhite: !isPlayingWhite);

        // Whoever is ahead on points shows the advantage next to their captured pieces.
        var advantage = player.Points - opponent.Points;
        if (advantage > 0)
        {
            playersCapturedPieces = AppendAdvantage(playersCapturedPieces, advantage);
        }
        else if (advantage < 0)
        {
            opponentsCapturedPieces = AppendAdvantage(opponentsCapturedPieces, -advantage);
        }

        return (opponentsCapturedPieces, playersCapturedPieces);
    }

    private static string CapturedMarkup(char[][] board, bool capturedColorWhite)
    {
        var onBoard = new Dictionary<char, int>();
        foreach (var row in board)
        {
            foreach (var piece in row)
            {
                if (piece == '\0')
                    continue;
                bool isWhite = char.IsUpper(piece);
                if (isWhite != capturedColorWhite)
                    continue;
                var type = char.ToUpper(piece);
                onBoard[type] = onBoard.GetValueOrDefault(type) + 1;
            }
        }

        var cells = new List<string>();
        foreach (var (type, count) in FullSet)
        {
            var missing = count - onBoard.GetValueOrDefault(type);
            for (int i = 0; i < missing; i++)
            {
                var piece = capturedColorWhite ? type : char.ToLower(type);
                cells.Add(CellMarkup(piece));
            }
        }

        return string.Join(" ", cells);
    }

    private static string AppendAdvantage(string capturedMarkup, int advantage)
    {
        var badge = $"[bold yellow]+{advantage}[/]";
        return string.IsNullOrEmpty(capturedMarkup) ? badge : $"{capturedMarkup} {badge}";
    }

    private static string CellMarkup(char piece) =>
        piece == '\0' ? "[dim]·[/]" : PieceDisplay.GetValueOrDefault(piece, piece.ToString());

    private void PrintRow(
        List<int> rankOrder, 
        List<int> fileOrder, 
        char[][] board, 
        int rankIdx, 
        string? opponentsCapturedPieces, 
        string? playersCapturedPieces)
    {
        var rank = rankOrder[rankIdx];
        var row = board[rank - 1];
        var cells = string.Join("[dim]|[/]", fileOrder.Select(f => $" {CellMarkup(row[f])} "));

        string captured = "";

        // if the first rank, show the opponent's captured pieces;
        if (rankIdx == 0)
        {
            captured += $"   {opponentsCapturedPieces}";
        }
        // if the last rank, show the player's captured pieces
        else if (rankIdx == rankOrder.Count - 1)
        {
            captured = $"   {playersCapturedPieces}";
        }

        console.MarkupLine($"[dim]{rank}[/] [dim]|[/]{cells}[dim]|[/]{captured}");
        console.MarkupLine(BoardLineSeparator);
    }
}
