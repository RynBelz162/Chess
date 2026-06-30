using Chess.Shared.Constants;
using Chess.Shared.Models.Pieces;

namespace Chess.Shared.Models.Movement;

// When a king is in check, every non-king move is legal only if it ends the
// check by capturing the attacker or blocking its line. The pin ray scan cannot
// express this, so each candidate move is verified by simulation — but only
// while the king is actually in check, which is rare.
public sealed class CheckEvasionFilter : IMoveFilter
{
    public void Apply(Board board)
    {
        RestrictWhenInCheck(board, ChessColor.White);
        RestrictWhenInCheck(board, ChessColor.Black);
    }

    private static void RestrictWhenInCheck(Board board, ChessColor color)
    {
        if (!board.KingIsInCheck(color))
        {
            return;
        }

        var defenders = board.Pieces
            .Where(piece => piece is not King && piece.Color == color && !piece.IsCaptured)
            .ToList();

        foreach (var piece in defenders)
        {
            piece.AvailableMoves = piece.AvailableMoves
                .Where(move => !board.MoveExposesOwnKing(piece, move))
                .ToList();
        }
    }
}
