using Chess.Shared.Constants;
using Chess.Shared.Models;
using Chess.Shared.Models.Pieces;
using ChessFile = Chess.Shared.Constants.ChessFile;

namespace Chess.Api.Services;

public class SetupService : ISetupService
{
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

    public Board InitializeBoard() => new Board()
    {
        Pieces = InitializePieces()
    };
    

    private Dictionary<string, Piece> InitializePieces()
    {
        var result = new Dictionary<string, Piece>();
        AddWhite(result);
        AddBlack(result);
        return result;
    }

    private void AddWhite(Dictionary<string, Piece> pieceDict)
    {
        pieceDict.Add("Q", new Queen());
        pieceDict.Add("K", new King());

        CreatePieces<Rook>(pieceDict, 'R', 2);
        CreatePieces<Knight>(pieceDict, 'N', 2);
        CreatePieces<Bishop>(pieceDict, 'B', 2);
        CreatePieces<Pawn>(pieceDict, 'P', 8);
    }

    private void AddBlack(Dictionary<string, Piece> pieceDict)
    {
        pieceDict.Add("q", new Queen());
        pieceDict.Add("k", new King());

        CreatePieces<Rook>(pieceDict, 'r', 2);
        CreatePieces<Knight>(pieceDict, 'n', 2);
        CreatePieces<Bishop>(pieceDict, 'b', 2);
        CreatePieces<Pawn>(pieceDict, 'p', 8);
    }

    private void CreatePieces<T>(Dictionary<string, Piece> pieceDict, char identifier, int numberToCreate) where T : Piece
    {
        for (int i = 0; i < numberToCreate; i++)
        {
            var piece = Activator.CreateInstance<T>();
            pieceDict.Add($"{identifier}{i+1}", piece);
        }
    }
}