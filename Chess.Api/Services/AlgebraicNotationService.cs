using System.Text.RegularExpressions;
using Chess.Shared.Constants;
using Chess.Shared.Helpers;
using Chess.Shared.Models;
using Chess.Shared.Models.Movement;
using Chess.Shared.Models.Pieces;

namespace Chess.Api.Services;

public partial class AlgebraicNotationService : IAlgebraicNotationService
{
    private const int MinimumValidMoveLength = 2;
    private const int MaximumValidMoveLength = 6;

    private enum CastleSide { King, Queen }

    [GeneratedRegex("\\w\\d", RegexOptions.IgnoreCase | RegexOptions.Compiled, "en-US")]
    private static partial Regex MoveRegex();

    public MovementRequest GetRequest(string move, Board board, ChessColor color)
    {
        var invalidMove = IsInvalidMove(move);
        if (invalidMove is not null)
        {
            return invalidMove;
        }

        var castleMove = CastleMove(move, board, color);
        if (castleMove is not null)
        {
            return castleMove;
        }

        var pawnMove = PawnMoves(move, board, color);
        if (pawnMove is not null)
        {
            return pawnMove;
        }

        var nonPawnMove = NonPawnMoves(move, board, color);
        if (nonPawnMove is not null)
        {
            return nonPawnMove;
        }

        return MovementRequest.InvalidMove;
    }
    
    private static MovementRequest? IsInvalidMove(string move)
    {
        if (string.IsNullOrWhiteSpace(move))
        {
            return MovementRequest.NoMove;
        }

        if (move.Length < MinimumValidMoveLength || move.Length > MaximumValidMoveLength)
        {
            return MovementRequest.InvalidMove;
        }

        return null;
    }
    
    private static MovementRequest? CastleMove(string move, Board board, ChessColor color)
    {
        var side = GetCastleSide(move);
        if (side is null)
        {
            return null;
        }

        var rank = color == ChessColor.White ? 1 : 8;
        var kingSquare = $"E{rank}";
        var kingTarget = side == CastleSide.King ? $"G{rank}" : $"C{rank}";

        var king = board.PieceOnSquare(kingSquare);
        if (king is not King || !king.AvailableMoves.Contains(kingTarget))
        {
            return null;
        }

        return new MovementRequest
        {
            PieceType = typeof(King),
            PieceSquare = kingSquare,
            TargetSquare = kingTarget,
        };
    }

    private static CastleSide? GetCastleSide(string move)
    {
        move = move.ToUpperInvariant();

        if (move is "O-O-O" or "OOO")
        {
            return CastleSide.Queen;
        }

        if (move is "O-O" or "OO")
        {
            return CastleSide.King;
        }

        if (move.Length == 3 && move[0] == 'K')
        {
            return move[1] switch
            {
                'H' => CastleSide.King,
                'A' => CastleSide.Queen,
                _ => null
            };
        }

        return null;
    }

    private static MovementRequest? PawnMoves(string move, Board board, ChessColor color)
    {
        // first character is not a pawn move
        var firstChar = move[0];

        if (ChessPieceHelper.IsPieceIdentifier(firstChar) && firstChar != 'p' && firstChar != 'P')
        {
            return null;
        }

        var matches = MoveRegex().Matches(move);

        return matches.Count switch
        {
            1 => CreateResult(typeof(Pawn), board, color, matches[0].Value),
            2 => CreateResult(typeof(Pawn), board, color, matches[1].Value, matches[0].Value),
            _ => MovementRequest.InvalidMove
        };
    }

    private static MovementRequest? NonPawnMoves(string move, Board board, ChessColor color)
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
            1 => CreateResult(pieceType, board, color, matches[0].Value),
            2 => CreateResult(pieceType, board, color, matches[1].Value, matches[0].Value),
            _ => MovementRequest.InvalidMove
        };
    }

    private static MovementRequest CreateResult(Type pieceType, Board board, ChessColor color, string targetSquare, string? fromSquare = null)
    {
        if (fromSquare is null)
        {
            var piecesMoving = board.PieceWithAvailableMove(pieceType, targetSquare, color);
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