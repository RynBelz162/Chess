using Chess.Shared.Models.Movement;
using Chess.Shared.Models.Pieces;

namespace Chess.Api.Services;

public class AlgebraicNotationService : IAlgebraicNotationService
{
    private const int MinimumValidMoveLength = 2;
    private const int MaximumValidMoveLength = 6;


    // TODO: Implement rest of request parsing
    public MovementRequest GetRequest(string move)
    {
        if (string.IsNullOrWhiteSpace(move))
        {
            return new MovementRequest 
            { 
                IsValid = false, 
                ErrorMessage = "No move was provided."
            };
        }

        if (move.Length < 2 || move.Length > 6)
        {
            return new MovementRequest 
            { 
                IsValid = false, 
                ErrorMessage = "Invalid move was provided."
            };
        }

        return new MovementRequest
        {
            PieceType = typeof(Pawn),
            PieceSquare = "B2",
            TargetSquare = "B3"
        };
    }
}