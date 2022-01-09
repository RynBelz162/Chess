using Chess.Api.Constants;
using Chess.Api.Models;
using Chess.Api.Models.Pieces;
using File = Chess.Api.Constants.File;

namespace Chess.Api.Services;

public class SetupService : ISetupService
{
    private const int NumberOfRanks = 8;

    public ChessColor DeterminePlayerColor()
    {
        var random = new Random(new Guid().GetHashCode());
        return (ChessColor)random.Next(0, 2);
    }

    public ChessColor GetOppositeColor(ChessColor color) => color switch
    {
        ChessColor.White => ChessColor.Black,
        ChessColor.Black => ChessColor.White,
        _ => throw new ArgumentException(nameof(color)),
    };

    public Board InitializeBoard()
    {
        var board = new Board();

        foreach (int file in Enum.GetValues(typeof(File)))
        {
            for (int i = 1; i <= NumberOfRanks; i++)
            {
                board.Squares.Add($"{file}{i}", new ());
            }
        }

        board.Pieces = InitializePieces();

        return board;
    }

    private List<Piece> InitializePieces()
    {
        // TODO: implement
        return new List<Piece>();
    }
}