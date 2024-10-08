using Chess.Shared.Constants;
using Chess.Shared.Extensions;
using Chess.Shared.Models.Movement;

namespace Chess.Shared.Models.Pieces;

public class Rook(ChessFile chessFile, int rank) : Piece(chessFile, rank)
{
    public const char Identifier = 'R';

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

            if (board.PieceColorOnSquare(targetSquare) != Color)
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

        this.Forward(canMoveToSquare, addToMoves);
        this.Backward(canMoveToSquare, addToMoves);
        this.Left(canMoveToSquare, addToMoves);
        this.Right(canMoveToSquare, addToMoves);

        return moves;
    }
}