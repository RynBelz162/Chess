using Chess.Api.Constants;
using Chess.Api.Helpers;

namespace Chess.Api.Models.Pieces;

public class Pawn : Piece
{
    // TODO: Fix class to account for moving black pieces
    public override List<string> RecalculateAvailableMoves(Board board)
    {
        var moves = new List<string>();

        var nextRank = this.CurrentRank + 1;
        var squareAbove = $"{this.CurrentFile}{nextRank}";
        if(!board.IsSquareOccupied(squareAbove))
        {
            moves.Add(squareAbove);
        }

        var (leftFile, rightFile) = ChessFileHelper.GetLeftAndRightFile(this.CurrentFile);
        
        AddDiagonalMove(moves, board, leftFile, nextRank);
        AddDiagonalMove(moves, board, rightFile, nextRank);

        return moves;
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
        if (IsOccupied && board.PieceColorOnSqaure(targetSquare) != this.Color)
        {
            moves.Add(targetSquare);
        }
    }

    private void CheckForEnPassant(ICollection<string> moves, Board board, ChessFile file, int rank)
    {
        var nextToPawn = $"{file}{rank - 1}";
        var IsOccupied = board.IsSquareOccupied(nextToPawn);

        if (!IsOccupied)
        {
            return;
        }

        if (board.PieceColorOnSqaure(nextToPawn) == this.Color)
        {
            return;
        }

        var pieceNextToPawn = board.Squares[nextToPawn].Piece;
        if (pieceNextToPawn?.NumberOfMoves == 0 && pieceNextToPawn is Pawn)
        {
            moves.Add($"{file}{rank}");
        }
    }
}