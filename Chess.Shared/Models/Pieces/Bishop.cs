using Chess.Shared.Constants;
using Chess.Shared.Extensions;

namespace Chess.Shared.Models.Pieces;

public class Bishop : Piece
{
    public override List<string> RecalculateAvailableMoves(Board board)
    {
        var moves = new List<string>();

        Func<ChessFile, int, MovementResult> canMoveToSquare = (file, rank) =>
        {
            var targetSquare = $"{file}{rank}";
            var isOccupied = board.IsSquareOccupied(targetSquare);
            
            if (!isOccupied)
            {
                return new MovementResult(false, file, rank);
            }

            if (board.PieceColorOnSqaure(targetSquare) != Color)
            {
                return new MovementResult(true, file, rank);
            }

            return new MovementResult(true, null, null);
        };

        Action<MovementResult> addToMoves = result =>
        {
            if (!result.HasTargetSquare) return;
            moves.Add($"{result.TargetFile}{result.TargetRank}");
        };


        this.DiagonalTopLeft(canMoveToSquare, addToMoves);
        this.DiagonalTopRight(canMoveToSquare, addToMoves);
        this.DiagonalBottomLeft(canMoveToSquare, addToMoves);
        this.DiagonalBottomRight(canMoveToSquare, addToMoves);

        return moves;
    }
}