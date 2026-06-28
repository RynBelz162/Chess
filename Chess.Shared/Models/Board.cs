using System.Text;
using Chess.Shared.Constants;
using Chess.Shared.Helpers;
using Chess.Shared.Models.Pieces;

namespace Chess.Shared.Models;

public class Board
{
    private const int NumberOfRanks = 8;

    // https://www.chessprogramming.org/Forsyth-Edwards_Notation
    private const string StartingFen = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR";
    public string CurrentFen { get; set; } = StartingFen;
    
    public Dictionary<string, Square> Squares { get; set; } = [];
    public HashSet<Piece> Pieces { get; set; } = [];

    public Board()
    {
        foreach (char file in Enum.GetValues<ChessFile>())
        {
            for (int i = 1; i <= NumberOfRanks; i++)
            {
                Squares.Add($"{file}{i}", new ());
            }
        }
    }

    public bool IsSquareOccupied(string targetSquare) =>
        Squares[targetSquare].IsOccupied;

    public ChessColor? PieceColorOnSquare(string targetSquare) =>
        Squares[targetSquare].Piece?.Color;

    public Piece? PieceOnSquare(string targetSquare) =>
        Squares[targetSquare].Piece;

    public void UpdateFen()
    {
        var sb = new StringBuilder();

        for (int rank = NumberOfRanks; rank >= 1; rank--)
        {
            var emptyCount = 0;
            foreach (char file in ChessFileHelper.OrderedFiles)
            {
                var piece = Squares[$"{file}{rank}"].Piece;
                if (piece is null)
                {
                    emptyCount++;
                    continue;
                }

                if (emptyCount > 0)
                {
                    sb.Append(emptyCount);
                    emptyCount = 0;
                }

                var identifier = ChessPieceHelper.IdentifierFromType(piece.GetType());
                sb.Append(piece.Color == ChessColor.White
                    ? char.ToUpperInvariant(identifier)
                    : char.ToLowerInvariant(identifier));
            }

            if (emptyCount > 0)
            {
                sb.Append(emptyCount);
            }

            if (rank > 1)
            {
                sb.Append('/');
            }
        }

        CurrentFen = sb.ToString();
    }

    public List<Piece> PieceWithAvailableMove(Type pieceType, string targetMove, ChessColor color) =>
        Pieces
            .Where(x => x.GetType() == pieceType)
            .Where(x => x.Color == color)
            .Where(x => x.AvailableMoves.Any(move => move.Equals(targetMove, StringComparison.OrdinalIgnoreCase)))
            .ToList();

    public bool KingIsInCheck(ChessColor color)
    {
        var king = Pieces.First(p => p is King && p.Color == color);
        return ((King)king).IsKingChecked(this, color);
    }
}