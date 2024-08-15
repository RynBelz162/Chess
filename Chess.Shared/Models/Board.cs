using Chess.Shared.Constants;
using Chess.Shared.Models.Pieces;

namespace Chess.Shared.Models;

public class Board
{
    private const int NumberOfRanks = 8;

    // https://www.chessprogramming.org/Forsyth-Edwards_Notation
    private const string StartingFen = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBKQKNR";
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

    public List<Piece> PieceWithAvailableMove(Type pieceType, string targetMove) =>
        Pieces
            .Where(x => x.GetType() == pieceType)
            .Where(x => x.AvailableMoves.Any(move => move.Equals(targetMove, StringComparison.OrdinalIgnoreCase)))
            .ToList();
}