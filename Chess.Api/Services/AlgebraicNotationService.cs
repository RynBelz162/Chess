using System.Text.RegularExpressions;
using Chess.Shared.Models;
using Chess.Shared.Models.Movement;
using Chess.Shared.Models.Pieces;

namespace Chess.Api.Services;

public class AlgebraicNotationService : IAlgebraicNotationService
{
    private const int MinimumValidMoveLength = 2;
    private const int MaximumValidMoveLength = 6;
    private Regex MoveRegex = new Regex("\\w\\d", RegexOptions.Compiled | RegexOptions.IgnoreCase);

    // TODO: Implement rest of request parsing
    public MovementRequest GetRequest(string move, Board board)
    {
        var invalidMove = IsInvalidMove(move);
        if (invalidMove is not null)
        {
            return invalidMove;
        }

        var pawnMove = PawnMoves(move, board);
        if (pawnMove is not null)
        {
            return pawnMove;
        }

        return MovementRequest.InvalidMove;
    }
    
    private MovementRequest? IsInvalidMove(string move)
    {
        if (string.IsNullOrWhiteSpace(move))
        {
            return MovementRequest.NoMove;
        }

        if (move.Length < 2 || move.Length > 6)
        {
            return MovementRequest.InvalidMove;
        }

        return null;
    }
    
    private MovementRequest? PawnMoves(string move, Board board)
    {
        // cant be a pawn move
        if (move.Length != 2 && move.Length != 3)
        {
            return null;
        }

        Match targetSquare;
        if (move.Length == 2)
        { 
            targetSquare = MoveRegex.Match(move);
        }
        else
        {
            targetSquare = MoveRegex.Match(move, 1);
        }

        var piecesMoving = board.PieceWithAvailableMove<Pawn>(targetSquare.Value);
        if (piecesMoving.Count != 1)
        {
            return MovementRequest.AmbiguousMove;
        }

        return new MovementRequest
        {
            TargetSquare = targetSquare.Value.ToUpper(),
            PieceType = typeof(Pawn),
            PieceSquare = piecesMoving[0].CurrentSqaure
        };
    }
}