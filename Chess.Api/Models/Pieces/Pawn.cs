using Chess.Api.Constants;
using Chess.Api.Helpers;

namespace Chess.Api.Models.Pieces;

public class Pawn : Piece
{
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

    private void AddDiagonalMove(List<string> moves, Board board, ChessFile? file, int rank)
    {
        if (!file.HasValue)
        {
            return;
        }

        var targetSquare = $"{file.Value}{rank}";
        var IsOccupied = board.IsSquareOccupied(targetSquare);

        // TODO: EN PASSANT 🇫🇷 🥖 (https://www.youtube.com/watch?v=MUxAdu5hvKk)
        if (!IsOccupied)
        {
            return;
        }

        // Can capture piece
        if (IsOccupied && board.PieceColorOnSqaure(targetSquare) != this.Color)
        {
            moves.Add(targetSquare);
        }
    }
}