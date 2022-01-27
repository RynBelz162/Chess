using Chess.Api.Constants;
using Chess.Api.Models;
using Chess.Api.Models.Pieces;

namespace Chess.Api.Tests;

public class ChessBoardBuilder
{
    private Board _board = new Board();

    public Board Build() => _board;

    public ChessBoardBuilder PlacePieceAt(Piece piece, ChessFile file, int rank)
    {
        piece.CurrentFile = file;
        piece.CurrentRank = rank;
        var targetSquare = $"{file}{rank}";
        
        _board.Squares[targetSquare].Piece = piece;
        return this;
    }

    public ChessBoardBuilder CreatePieceAt(char pieceIdentifier, ChessFile file, int rank, int numOfMoves = 0)
    {
        var piece = CreatePiece(pieceIdentifier, numOfMoves);
        piece.CurrentFile = file;
        piece.CurrentRank = rank;
        var targetSquare = $"{file}{rank}";

        _board.Squares[targetSquare].Piece = piece;
        return this;
    }

    private static Piece CreatePiece(char identifier, int numOfMoves) => identifier switch
    {
        // Black
        'k' => new King { Color = ChessColor.Black, NumberOfMoves = numOfMoves },
        'q' => new Queen { Color = ChessColor.Black, NumberOfMoves = numOfMoves },
        'r' => new Rook { Color = ChessColor.Black, NumberOfMoves = numOfMoves },
        'n' => new Knight { Color = ChessColor.Black, NumberOfMoves = numOfMoves },
        'b' => new Bishop { Color = ChessColor.Black, NumberOfMoves = numOfMoves },
        'p' => new Pawn { Color = ChessColor.Black, NumberOfMoves = numOfMoves },
        // White
        'K' => new King { Color = ChessColor.White, NumberOfMoves = numOfMoves },
        'Q' => new Queen { Color = ChessColor.White, NumberOfMoves = numOfMoves },
        'R' => new Rook { Color = ChessColor.White, NumberOfMoves = numOfMoves },
        'N' => new Knight { Color = ChessColor.White, NumberOfMoves = numOfMoves },
        'B' => new Bishop { Color = ChessColor.White, NumberOfMoves = numOfMoves },
        'P' => new Pawn { Color = ChessColor.White, NumberOfMoves = numOfMoves },
        _ => new Pawn { Color = ChessColor.White, NumberOfMoves = numOfMoves } ,
    };
}