using Chess.Api.Models.Pieces;

namespace Chess.Api.Models;

public class Board
{
    // https://www.chessprogramming.org/Forsyth-Edwards_Notation
    private const string StartingFen = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBKQKNR";
    public string CurrentFen { get; set; } = StartingFen;
    
    public Dictionary<string, Square> Squares { get; set; } = new Dictionary<string, Square>();
    public List<Piece> Pieces { get; set; } = new List<Piece>();
}