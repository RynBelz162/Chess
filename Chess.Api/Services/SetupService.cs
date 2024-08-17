using Chess.Shared.Constants;
using Chess.Shared.Helpers;
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
        _ => throw new ArgumentException("Color is not a possible Chess color value", nameof(color)),
    };

    public Board InitializeBoard() 
    {
        var board = new Board()
        {
            Pieces = InitializePieces()
        };

        Parallel.ForEach(board.Pieces, piece =>
        {
            board.Squares[piece.CurrentSquare].Piece = piece;
            piece.AvailableMoves = piece.RecalculateAvailableMoves(board);
        });
        
        return board;
    }

    private static HashSet<Piece> InitializePieces()
    {
        var result = new HashSet<Piece>();
        AddWhite(result);
        AddBlack(result);
        return result;
    }

    private static void AddWhite(HashSet<Piece> pieceDict)
    {
        pieceDict.Add(new Queen(ChessFile.D, 1) { Color = ChessColor.White });
        pieceDict.Add(new King(ChessFile.E, 1) { Color = ChessColor.White });

        CreatePieces<Rook>(pieceDict, 2, ChessColor.White);
        CreatePieces<Knight>(pieceDict, 2, ChessColor.White);
        CreatePieces<Bishop>(pieceDict, 2, ChessColor.White);
        CreatePieces<Pawn>(pieceDict, 8, ChessColor.White);
    }

    private static void AddBlack(HashSet<Piece> pieceDict)
    {
        pieceDict.Add(new Queen(ChessFile.D, 8) { Color = ChessColor.Black });
        pieceDict.Add(new King(ChessFile.E, 8) { Color = ChessColor.Black });

        CreatePieces<Rook>(pieceDict, 2, ChessColor.Black);
        CreatePieces<Knight>(pieceDict, 2, ChessColor.Black);
        CreatePieces<Bishop>(pieceDict, 2, ChessColor.Black);
        CreatePieces<Pawn>(pieceDict, 8, ChessColor.Black);
    }

    private static void CreatePieces<T>(HashSet<Piece> pieceDict, int numberToCreate, ChessColor color) where T : Piece
    {
        for (int i = 0; i < numberToCreate; i++)
        {
            var piece = CreatePieceType<T>(color, i + 1);
            pieceDict.Add(piece);
        }
    }

    private static Piece CreatePieceType<T>(ChessColor color, int index) where T : Piece
    {
        var rank = color == ChessColor.White ? 1 : 8;
        var pawnRank = color == ChessColor.White ? 2: 7;

        return typeof(T) switch
        {
            Type t when t == typeof(Pawn) =>
                new Pawn(index.ToChessFile(), pawnRank) { Color = color },

            Type t when t == typeof(Knight) =>
                new Knight(index == 1 ? ChessFile.B : ChessFile.G, rank) { Color = color },

            Type t when t == typeof(Rook) =>
                new Rook(index == 1 ? ChessFile.A : ChessFile.H, rank) { Color = color },

            Type t when t == typeof(Bishop) =>
                new Bishop(index == 1 ? ChessFile.C : ChessFile.F, rank) { Color = color },

            _ => throw new NotImplementedException()
        };
    }
}