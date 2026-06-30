using System.Text;
using Chess.Shared.Constants;
using Chess.Shared.Helpers;
using Chess.Shared.Models.Pieces;

namespace Chess.Shared.Models;

public class Board
{
    private const int NumberOfRanks = 8;

    // https://www.chessprogramming.org/Forsyth-Edwards_Notation
    private const string StartingFen = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR";
    public string CurrentFen { get; set; } = StartingFen;
    
    public Dictionary<string, Square> Squares { get; set; } = [];
    public HashSet<Piece> Pieces { get; set; } = [];

    public Board()
    {
        foreach (char file in Enum.GetValues<ChessFile>())
        {
            for (int i = 1; i <= NumberOfRanks; i++)
            {
                Squares.Add($"{file}{i}", new ());
            }
        }
    }

    public bool IsSquareOccupied(string targetSquare) =>
        Squares[targetSquare].IsOccupied;

    public ChessColor? PieceColorOnSquare(string targetSquare) =>
        Squares[targetSquare].Piece?.Color;

    public Piece? PieceOnSquare(string targetSquare) =>
        Squares[targetSquare].Piece;

    public void UpdateFen()
    {
        var sb = new StringBuilder();

        for (int rank = NumberOfRanks; rank >= 1; rank--)
        {
            var emptyCount = 0;
            foreach (char file in ChessFileHelper.OrderedFiles)
            {
                var piece = Squares[$"{file}{rank}"].Piece;
                if (piece is null)
                {
                    emptyCount++;
                    continue;
                }

                if (emptyCount > 0)
                {
                    sb.Append(emptyCount);
                    emptyCount = 0;
                }

                var identifier = ChessPieceHelper.IdentifierFromType(piece.GetType());
                sb.Append(piece.Color == ChessColor.White
                    ? char.ToUpperInvariant(identifier)
                    : char.ToLowerInvariant(identifier));
            }

            if (emptyCount > 0)
            {
                sb.Append(emptyCount);
            }

            if (rank > 1)
            {
                sb.Append('/');
            }
        }

        CurrentFen = sb.ToString();
    }

    public List<Piece> PieceWithAvailableMove(Type pieceType, string targetMove, ChessColor color) =>
        Pieces
            .Where(x => x.GetType() == pieceType)
            .Where(x => x.Color == color)
            .Where(x => x.AvailableMoves.Any(move => move.Equals(targetMove, StringComparison.OrdinalIgnoreCase)))
            .ToList();

    public bool KingIsInCheck(ChessColor color)
    {
        // A board may have no king of a colour in tests or partial positions; a
        // missing king cannot be in check.
        if (Pieces.FirstOrDefault(p => p is King && p.Color == color) is not King king)
        {
            return false;
        }

        return king.IsKingChecked(this, color);
    }

    // Move filters leave every piece holding only legal moves, so a side with no
    // available move anywhere has no legal move at all.
    public bool HasAnyLegalMove(ChessColor color) =>
        Pieces.Any(piece => piece.Color == color && !piece.IsCaptured && piece.CanMove());

    // Checkmate: the king is in check and the side cannot escape, capture, or
    // block to relieve it.
    public bool IsCheckmated(ChessColor color) =>
        KingIsInCheck(color) && !HasAnyLegalMove(color);

    // Stalemate: the side is not in check yet has no legal move to make.
    public bool IsStalemated(ChessColor color) =>
        !KingIsInCheck(color) && !HasAnyLegalMove(color);
    public bool IsSquareAttackedBy(string square, ChessColor attackingColor) =>
        Pieces
            .Where(piece => piece.Color == attackingColor && !piece.IsCaptured)
            .Any(piece => piece.RecalculateAvailableMoves(this).Contains(square));

    public bool MoveExposesOwnKing(Piece piece, string targetSquare)
    {
        var king = Pieces.FirstOrDefault(p => p is King && p.Color == piece.Color);
        if (king is null)
        {
            return false;
        }

        var originSquare = piece.CurrentSquare;

        // The captured square is usually the target, but en passant takes a pawn
        // that sits beside the origin, so ask the piece which square it clears.
        var capturedSquare = piece.CapturedSquare(targetSquare, this);
        var capturedPiece = Squares[capturedSquare].Piece;

        ApplySimulatedMove(piece, originSquare, targetSquare, capturedSquare, capturedPiece);

        var attackingColor = piece.Color == ChessColor.White ? ChessColor.Black : ChessColor.White;
        var kingExposed = IsSquareAttackedBy(king.CurrentSquare, attackingColor);

        RestoreSimulatedMove(piece, originSquare, targetSquare, capturedSquare, capturedPiece);

        return kingExposed;
    }

    private void ApplySimulatedMove(Piece piece, string originSquare, string targetSquare, string capturedSquare, Piece? capturedPiece)
    {
        if (capturedPiece is not null)
        {
            Pieces.Remove(capturedPiece);
            Squares[capturedSquare].Piece = null;
        }

        Squares[originSquare].Piece = null;
        Squares[targetSquare].Piece = piece;
        piece.SetSquare((ChessFile)targetSquare[0], targetSquare[1] - '0');
    }

    private void RestoreSimulatedMove(Piece piece, string originSquare, string targetSquare, string capturedSquare, Piece? capturedPiece)
    {
        piece.SetSquare((ChessFile)originSquare[0], originSquare[1] - '0');
        Squares[originSquare].Piece = piece;
        Squares[targetSquare].Piece = null;

        if (capturedPiece is not null)
        {
            Squares[capturedSquare].Piece = capturedPiece;
            Pieces.Add(capturedPiece);
        }
    }
}