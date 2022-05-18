using Chess.Shared.Constants;
using Chess.Shared.Models;
using Chess.Shared.Models.Pieces;

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

    public Board InitializeBoard() 
    {
        var board = new Board()
        {
            Pieces = InitializePieces()
        };

        Parallel.ForEach(board.Pieces, piece =>
        {
            piece.AvailableMoves = piece.RecalculateAvailableMoves(board);
        });
        
        return board;
    }

    private HashSet<Piece> InitializePieces()
    {
        var result = new HashSet<Piece>();
        AddWhite(result);
        AddBlack(result);
        return result;
    }

    private void AddWhite(HashSet<Piece> pieceDict)
    {
        pieceDict.Add(new Queen() { Color = ChessColor.White });
        pieceDict.Add(new King() { Color = ChessColor.White });

        CreatePieces<Rook>(pieceDict, 2, ChessColor.White);
        CreatePieces<Knight>(pieceDict, 2, ChessColor.White);
        CreatePieces<Bishop>(pieceDict, 2, ChessColor.White);
        CreatePieces<Pawn>(pieceDict, 8, ChessColor.White);
    }

    private void AddBlack(HashSet<Piece> pieceDict)
    {
        pieceDict.Add(new Queen() { Color = ChessColor.Black });
        pieceDict.Add(new King() { Color = ChessColor.Black });

        CreatePieces<Rook>(pieceDict, 2, ChessColor.Black);
        CreatePieces<Knight>(pieceDict, 2, ChessColor.Black);
        CreatePieces<Bishop>(pieceDict, 2, ChessColor.Black);
        CreatePieces<Pawn>(pieceDict, 8, ChessColor.Black);
    }

    private void CreatePieces<T>(HashSet<Piece> pieceDict, int numberToCreate, ChessColor color) where T : Piece
    {
        for (int i = 0; i < numberToCreate; i++)
        {
            var piece = CreatePieceType<T>(color);
            pieceDict.Add(piece);
        }
    }

    private Piece CreatePieceType<T>(ChessColor color) where T : Piece =>
        typeof(T) switch
        {
            Type t when t == typeof(Pawn) => new Pawn() { Color = color },
            Type t when t == typeof(Knight) => new Knight() { Color = color },
            Type t when t == typeof(Rook) => new Rook() { Color = color },
            Type t when t == typeof(Bishop) => new Bishop() { Color = color },
            _ => throw new NotImplementedException()
        };
}