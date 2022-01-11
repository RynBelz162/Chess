using Chess.Api.Constants;
using Chess.Api.Models.Pieces;

namespace Chess.Api.Models;

public class Board
{
    private const int NumberOfRanks = 8;

    // https://www.chessprogramming.org/Forsyth-Edwards_Notation
    private const string StartingFen = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBKQKNR";
    public string CurrentFen { get; set; } = StartingFen;
    
    public Dictionary<string, Square> Squares { get; set; } = new Dictionary<string, Square>();
    public Dictionary<char, Piece> Pieces { get; set; } = new Dictionary<char, Piece>();

    public bool IsSquareOccupied(string targetSquare) =>
        Squares[targetSquare].IsOccupied;

    public ChessColor? PieceColorOnSqaure(string targetSquare) =>
        Squares[targetSquare].Piece?.Color;

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
}