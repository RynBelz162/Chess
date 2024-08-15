using Chess.Shared.Constants;
using Chess.Shared.Extensions;
using Chess.Shared.Models.Movement;

namespace Chess.Shared.Models.Pieces;

public class Queen(ChessFile chessFile, int rank) : Piece(chessFile, rank)
{
    public const char Identifier = 'Q';

    public override List<string> RecalculateAvailableMoves(Board board)
    {
        var moves = new List<string>();
        MovementResult canMoveToSquare(ChessFile file, int rank)
        {
            var targetSquare = $"{file}{rank}";
            var isOccupied = board.IsSquareOccupied(targetSquare);

            if (!isOccupied)
            {
                return new MovementResult(false, file, rank);
            }

            if (board.PieceColorOnSquare(targetSquare) != Color)
            {
                return new MovementResult(true, file, rank);
            }

            return new MovementResult(true, null, null);
        }

        void addToMoves(MovementResult result)
        {
            if (!result.HasTargetSquare) return;
            moves.Add($"{result.TargetFile}{result.TargetRank}");
        }

        // left, right, up, down
        this.Forward(canMoveToSquare, addToMoves);
        this.Backward(canMoveToSquare, addToMoves);
        this.Left(canMoveToSquare, addToMoves);
        this.Right(canMoveToSquare, addToMoves);

        // diagonals
        this.DiagonalBottomLeft(canMoveToSquare, addToMoves);
        this.DiagonalBottomRight(canMoveToSquare, addToMoves);
        this.DiagonalTopLeft(canMoveToSquare, addToMoves);
        this.DiagonalTopRight(canMoveToSquare, addToMoves);

        return moves;
    }
}