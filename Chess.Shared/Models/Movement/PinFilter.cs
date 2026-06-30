using Chess.Shared.Constants;
using Chess.Shared.Models.Pieces;

namespace Chess.Shared.Models.Movement;

// A piece is "pinned" when moving it would expose its own king to check, so it
// is stuck on its line until the threat is resolved. Pins are located with a
// ray scan from the king (see PinScanner) rather than by simulating every move.
public sealed class PinFilter : IMoveFilter
{
    public void Apply(Board board)
    {
        RestrictPinnedPieces(board, ChessColor.White);
        RestrictPinnedPieces(board, ChessColor.Black);

        RemoveEnPassantDiscoveredChecks(board);
    }

    private static void RestrictPinnedPieces(Board board, ChessColor color)
    {
        foreach (var (piece, allowedSquares) in PinScanner.FindPins(board, color))
        {
            piece.AvailableMoves = piece.AvailableMoves
                .Where(allowedSquares.Contains)
                .ToList();
        }
    }

    // A ray scan cannot see the horizontal discovered check that en passant
    // creates by clearing two pawns from the rank at once, so only those rare
    // captures fall back to simulating the move.
    private static void RemoveEnPassantDiscoveredChecks(Board board)
    {
        var pawns = board.Pieces.Where(p => p is Pawn && !p.IsCaptured).ToList();

        foreach (var pawn in pawns)
        {
            pawn.AvailableMoves = pawn.AvailableMoves
                .Where(move => !IsEnPassant(pawn, move, board) || !board.MoveExposesOwnKing(pawn, move))
                .ToList();
        }
    }

    // En passant is the only move whose captured square differs from its target.
    private static bool IsEnPassant(Piece pawn, string move, Board board) =>
        pawn.CapturedSquare(move, board) != move;
}
