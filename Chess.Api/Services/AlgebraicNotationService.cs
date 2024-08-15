using System.Text.RegularExpressions;
using Chess.Shared.Helpers;
using Chess.Shared.Models;
using Chess.Shared.Models.Movement;
using Chess.Shared.Models.Pieces;

namespace Chess.Api.Services;

public partial class AlgebraicNotationService : IAlgebraicNotationService
{
    private const int MinimumValidMoveLength = 2;
    private const int MaximumValidMoveLength = 6;

    [GeneratedRegex("\\w\\d", RegexOptions.IgnoreCase | RegexOptions.Compiled, "en-US")]
    private static partial Regex MoveRegex();

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

        var nonPawnMove = NonPawnMoves(move, board);
        if (nonPawnMove is not null)
        {
            return nonPawnMove;
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
    
    private static MovementRequest? PawnMoves(string move, Board board)
    {
        // first character is not a pawn move
        var firstChar = move[0];
        var isPieceIdentifier = ChessPieceHelper.IsPieceIdentifier(firstChar);

        if (ChessPieceHelper.IsPieceIdentifier(firstChar) && firstChar != 'p' && firstChar != 'P')
        {
            return null;
        }

        var matches = MoveRegex().Matches(move);

        return matches.Count switch
        {
            1 => CreateResult(typeof(Pawn), board, matches[0].Value),
            2 => CreateResult(typeof(Pawn), board, matches[1].Value, matches[0].Value),
            _ => MovementRequest.InvalidMove
        };
    }

    private static MovementRequest? NonPawnMoves(string move, Board board)
    {
        var piece = move[0];
        var pieceType = ChessPieceHelper.TypeFromIdentifier(piece);

        if (pieceType == typeof(Pawn))
        {
            return MovementRequest.InvalidMove;
        }

        var matches = MoveRegex().Matches(move);

        return matches.Count switch
        {
            1 => CreateResult(pieceType, board, matches[0].Value),
            2 => CreateResult(pieceType, board, matches[1].Value, matches[0].Value),
            _ => MovementRequest.InvalidMove
        };
    }

    private static MovementRequest CreateResult(Type pieceType, Board board, string targetSquare, string? fromSquare = null)
    {
        if (fromSquare is null)
        {
            var piecesMoving = board.PieceWithAvailableMove(pieceType, targetSquare);
            if (piecesMoving.Count != 1)
            {
                return MovementRequest.AmbiguousMove;
            }

            fromSquare = piecesMoving[0].CurrentSquare;
        }

        return new MovementRequest
        {
            TargetSquare = targetSquare.ToUpper(),
            PieceType = pieceType,
            PieceSquare = fromSquare.ToUpper()
        };
    }
}