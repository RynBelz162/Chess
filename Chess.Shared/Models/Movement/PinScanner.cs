using Chess.Shared.Constants;
using Chess.Shared.Models.Pieces;

namespace Chess.Shared.Models.Movement;

// Finds absolute pins by walking the eight rays out from the king. A friendly
// piece that is the sole blocker between its king and an enemy slider aligned
// with that ray is pinned: it may only move along the ray or capture the pinner.
//
// One scan replaces simulating every candidate move, so no piece's moves are
// recalculated to detect a pin.
public static class PinScanner
{
    private readonly record struct Direction(int File, int Rank, bool IsDiagonal);

    private static readonly Direction[] Rays =
    [
        new(0, 1, false), new(0, -1, false), new(1, 0, false), new(-1, 0, false),
        new(1, 1, true), new(1, -1, true), new(-1, 1, true), new(-1, -1, true),
    ];

    // Maps each pinned piece to the squares it is still allowed to move to.
    public static Dictionary<Piece, HashSet<string>> FindPins(Board board, ChessColor color)
    {
        var pins = new Dictionary<Piece, HashSet<string>>();

        var king = board.Pieces.FirstOrDefault(p => p is King && p.Color == color && !p.IsCaptured);
        if (king is null)
        {
            return pins;
        }

        foreach (var ray in Rays)
        {
            ScanRay(board, king, ray, pins);
        }

        return pins;
    }

    private static void ScanRay(Board board, Piece king, Direction ray, Dictionary<Piece, HashSet<string>> pins)
    {
        var fileIndex = (char)king.CurrentFile - 'A';
        var rank = king.CurrentRank;

        // Squares the pinned piece could move to: the empty squares along the
        // ray plus the pinner's square. The candidate's own square is omitted.
        var allowedSquares = new HashSet<string>();
        Piece? candidate = null;

        while (true)
        {
            fileIndex += ray.File;
            rank += ray.Rank;
            if (fileIndex is < 0 or > 7 || rank is < 1 or > 8)
            {
                return; // ran off the board without finding a pinner
            }

            var square = $"{(char)('A' + fileIndex)}{rank}";
            var piece = board.PieceOnSquare(square);

            if (piece is null)
            {
                allowedSquares.Add(square);
                continue;
            }

            if (candidate is null)
            {
                // The first piece on the ray can only be pinned if it is friendly.
                if (piece.Color != king.Color)
                {
                    return;
                }

                candidate = piece;
                continue;
            }

            // The second piece pins the candidate only if it is an enemy slider
            // that attacks along this ray; capturing it is then a legal escape.
            if (piece.Color != king.Color && AttacksAlong(piece, ray))
            {
                allowedSquares.Add(square);
                pins[candidate] = allowedSquares;
            }

            return;
        }
    }

    private static bool AttacksAlong(Piece piece, Direction ray) =>
        ray.IsDiagonal
            ? piece is Bishop or Queen
            : piece is Rook or Queen;
}
