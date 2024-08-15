using Chess.Shared.Constants;
using Chess.Shared.Helpers;

namespace Chess.Shared.Models.Pieces;

public class Pawn(ChessFile chessFile, int rank) : Piece(chessFile, rank)
{
    public const char Identifier = 'P';

    private int NextRankForColor => Color == ChessColor.White ? CurrentRank + 1 : CurrentRank - 1;

    public override List<string> RecalculateAvailableMoves(Board board)
    {
        var moves = new List<string>();

        if (ShouldPromote())
        {
            // TODO: Implement pawn promotion.
            return moves;
        }

        AddForwardMoves(moves, board);

        var (leftFile, rightFile) = ChessFileHelper.GetLeftAndRightFile(CurrentFile);
        AddDiagonalMove(moves, board, leftFile, NextRankForColor);
        AddDiagonalMove(moves, board, rightFile, NextRankForColor);

        return moves;
    }

    private void AddForwardMoves(ICollection<string> moves, Board board)
    {
        var targetSquare = $"{CurrentFile}{NextRankForColor}";
        if(!board.IsSquareOccupied(targetSquare))
        {
            moves.Add(targetSquare);
        }

        if (NumberOfMoves > 0 && (NextRankForColor != 1 || NextRankForColor != 8))
        {
            return;
        }

        var targetRank = Color == ChessColor.White ? NextRankForColor + 1 : NextRankForColor - 1;
        var targetSecondSquare = $"{CurrentFile}{targetRank}";
        if(!board.IsSquareOccupied(targetSquare))
        {
            moves.Add(targetSecondSquare);
        }
    }

    private void AddDiagonalMove(ICollection<string> moves, Board board, ChessFile? file, int rank)
    {
        if (!file.HasValue)
        {
            return;
        }

        var targetSquare = $"{file.Value}{rank}";
        var IsOccupied = board.IsSquareOccupied(targetSquare);

        // EN PASSANT 🇫🇷 🥖 (https://www.youtube.com/watch?v=MUxAdu5hvKk)
        if (!IsOccupied)
        {
            CheckForEnPassant(moves, board, file.Value, rank);
            return;
        }

        // Can capture piece
        if (IsOccupied && board.PieceColorOnSquare(targetSquare) != this.Color)
        {
            moves.Add(targetSquare);
        }
    }

    private void CheckForEnPassant(ICollection<string> moves, Board board, ChessFile file, int rank)
    {
        var nextToPawn = $"{file}{CurrentRank}";
        var IsOccupied = board.IsSquareOccupied(nextToPawn);

        if (!IsOccupied)
        {
            return;
        }

        if (board.PieceColorOnSquare(nextToPawn) == this.Color)
        {
            return;
        }

        var pieceNextToPawn = board.PieceOnSquare(nextToPawn);
        if (pieceNextToPawn?.NumberOfMoves == 0 && pieceNextToPawn is Pawn)
        {
            moves.Add($"{file}{rank}");
        }
    }

    private bool ShouldPromote()
    {
        if (Color == ChessColor.White && CurrentRank == 7)
        {
            return true;
        }

        if (Color == ChessColor.Black && CurrentRank == 2)
        {
            return true;
        }

        return false;
    }
}