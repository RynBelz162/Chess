using Chess.Shared.Constants;
using Chess.Shared.Models.Pieces;

namespace Chess.Shared.Models.Movement;

// The king may never move onto a square the enemy attacks. Castling adds two
// more restrictions: the king cannot castle out of, through, or into check.
public sealed class KingSafetyFilter : IMoveFilter
{
    public void Apply(Board board)
    {
        var kings = board.Pieces
            .Where(piece => piece is King && !piece.IsCaptured)
            .ToList();

        foreach (var king in kings)
        {
            king.AvailableMoves = king.AvailableMoves
                .Where(move => IsSafe(board, king, move))
                .ToList();
        }
    }

    private static bool IsSafe(Board board, Piece king, string move)
    {
        return IsCastleMove(king, move)
            ? CanCastleSafely(board, king, move)
            : !board.MoveExposesOwnKing(king, move);
    }

    // A castle is the only king move that spans two files.
    private static bool IsCastleMove(Piece king, string move) =>
        Math.Abs(move[0] - (char)king.CurrentFile) == 2;

    private static bool CanCastleSafely(Board board, Piece king, string target)
    {
        var attackingColor = king.Color == ChessColor.White ? ChessColor.Black : ChessColor.White;

        // Cannot castle while currently in check.
        if (board.IsSquareAttackedBy(king.CurrentSquare, attackingColor))
        {
            return false;
        }

        // The king cannot pass through, nor land on, an attacked square.
        return !board.MoveExposesOwnKing(king, TransitSquare(king, target))
            && !board.MoveExposesOwnKing(king, target);
    }

    // The square the king steps over while castling sits between its origin and
    // target on the same rank.
    private static string TransitSquare(Piece king, string target)
    {
        var transitFile = (char)(((char)king.CurrentFile + target[0]) / 2);
        return $"{transitFile}{king.CurrentRank}";
    }
}
