using Chess.Shared.Constants;
using Chess.Shared.Models;
using Chess.Shared.Models.Pieces;

namespace Chess.Shared.Helpers;

public class ChessBoardBuilder
{
    private readonly Board _board = new();

    public Board Build()
    {
        Parallel.ForEach(_board.Pieces, piece =>
        {
            piece.AvailableMoves = piece.RecalculateAvailableMoves(_board);
        });

        return _board;
    }

    public ChessBoardBuilder PlacePiece(Piece piece)
    {
        _board.Pieces.Add(piece);
        _board.Squares[piece.CurrentSquare].Piece = piece;
        return this;
    }

    public ChessBoardBuilder CreatePieceAt(char pieceIdentifier, ChessFile file, int rank, int numOfMoves = 0)
    {
        var piece = CreatePiece(pieceIdentifier, numOfMoves, file, rank);
        var targetSquare = $"{file}{rank}";

        _board.Pieces.Add(piece);
        _board.Squares[targetSquare].Piece = piece;
        return this;
    }

    private static Piece CreatePiece(char identifier, int numOfMoves, ChessFile file, int rank) => identifier switch
    {
        // Black
        'k' => new King(file, rank) { Color = ChessColor.Black, NumberOfMoves = numOfMoves },
        'q' => new Queen(file, rank) { Color = ChessColor.Black, NumberOfMoves = numOfMoves },
        'r' => new Rook(file, rank) { Color = ChessColor.Black, NumberOfMoves = numOfMoves },
        'n' => new Knight(file, rank) { Color = ChessColor.Black, NumberOfMoves = numOfMoves },
        'b' => new Bishop(file, rank) { Color = ChessColor.Black, NumberOfMoves = numOfMoves },
        'p' => new Pawn(file, rank) { Color = ChessColor.Black, NumberOfMoves = numOfMoves },
        // White
        'K' => new King(file, rank) { Color = ChessColor.White, NumberOfMoves = numOfMoves },
        'Q' => new Queen(file, rank) { Color = ChessColor.White, NumberOfMoves = numOfMoves },
        'R' => new Rook(file, rank) { Color = ChessColor.White, NumberOfMoves = numOfMoves },
        'N' => new Knight(file, rank) { Color = ChessColor.White, NumberOfMoves = numOfMoves },
        'B' => new Bishop(file, rank) { Color = ChessColor.White, NumberOfMoves = numOfMoves },
        'P' => new Pawn(file, rank) { Color = ChessColor.White, NumberOfMoves = numOfMoves },
        _ => new Pawn(file, rank) { Color = ChessColor.White, NumberOfMoves = numOfMoves } ,
    };
}