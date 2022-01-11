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

    public ChessBoardBuilder CreatePieceAt(char pieceIdentifier, ChessFile file, int rank)
    {
        var piece = CreatePiece(pieceIdentifier);
        piece.CurrentFile = file;
        piece.CurrentRank = rank;
        var targetSquare = $"{file}{rank}";

        _board.Squares[targetSquare].Piece = piece;
        return this;
    }

    private static Piece CreatePiece(char identifier) => identifier switch
    {
        // Black
        'k' => new King { Color = ChessColor.Black },
        'q' => new Queen { Color = ChessColor.Black },
        'r' => new Rook { Color = ChessColor.Black },
        'n' => new Knight { Color = ChessColor.Black },
        'b' => new Bishop { Color = ChessColor.Black },
        'p' => new Pawn { Color = ChessColor.Black },
        // White
        'K' => new King { Color = ChessColor.White },
        'Q' => new Queen { Color = ChessColor.White },
        'E' => new Rook { Color = ChessColor.White },
        'N' => new Knight { Color = ChessColor.White },
        'B' => new Bishop { Color = ChessColor.White },
        'P' => new Pawn { Color = ChessColor.White },
        _ => new Pawn { Color = ChessColor.White } ,
    };
}