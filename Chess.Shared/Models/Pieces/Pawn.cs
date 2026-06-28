using Chess.Shared.Constants;
using Chess.Shared.Helpers;

namespace Chess.Shared.Models.Pieces;

public class Pawn(ChessFile chessFile, int rank) : Piece(chessFile, rank)
{
    public const char Identifier = 'P';

    public override int Value => 1;

    private int NextRankForColor => Color == ChessColor.White ? CurrentRank + 1 : CurrentRank - 1;

    public override Result<int> Move(string square, Board board)
    {
        var victimSquare = GetEnPassantVictimSquare(square, board);

        var result = base.Move(square, board);

        if (!result.IsSuccess || victimSquare is null)
        {
            return result;
        }

        // En passant: the captured pawn sits beside the origin, not on the target
        // square, so base.Move did not remove it.
        if (board.PieceOnSquare(victimSquare) is not { } victim)
        {
            return result;
        }

        victim.IsCaptured = true;
        board.Pieces.Remove(victim);
        board.Squares[victimSquare].Piece = null;
        RecalculateAllMoves(board);

        return Result.Ok(victim.Value);
    }

    private string? GetEnPassantVictimSquare(string square, Board board)
    {
        if (square.Length < 2 || square[0] == (char)CurrentFile || board.IsSquareOccupied(square))
        {
            return null;
        }

        var victimSquare = $"{square[0]}{CurrentRank}";
        if (board.PieceOnSquare(victimSquare) is Pawn victim && victim.Color != Color)
        {
            return victimSquare;
        }

        return null;
    }

    public override List<string> RecalculateAvailableMoves(Board board)
    {
        var moves = new List<string>();

        if (ShouldPromote())
        {
            // TODO: Implement pawn promotion.
            return moves;
        }

        AddForwardMoves(moves, board);

        var (leftFile, rightFile) = ChessFileHelper.GetLeftAndRightFile(CurrentFile);
        AddDiagonalMove(moves, board, leftFile, NextRankForColor);
        AddDiagonalMove(moves, board, rightFile, NextRankForColor);

        return moves;
    }

    private void AddForwardMoves(ICollection<string> moves, Board board)
    {
        var oneSquare = $"{CurrentFile}{NextRankForColor}";
        if (board.IsSquareOccupied(oneSquare))
        {
            // Blocked directly ahead: cannot advance one or two squares.
            return;
        }

        moves.Add(oneSquare);

        // The two-square advance is only available from the pawn's starting rank.
        if (NumberOfMoves > 0)
        {
            return;
        }

        var twoRank = Color == ChessColor.White ? NextRankForColor + 1 : NextRankForColor - 1;
        var twoSquare = $"{CurrentFile}{twoRank}";
        if (!board.IsSquareOccupied(twoSquare))
        {
            moves.Add(twoSquare);
        }
    }

    private void AddDiagonalMove(ICollection<string> moves, Board board, ChessFile? file, int rank)
    {
        if (!file.HasValue)
        {
            return;
        }

        var targetSquare = $"{file.Value}{rank}";
        var IsOccupied = board.IsSquareOccupied(targetSquare);

        // EN PASSANT 🇫🇷 🥖 (https://www.youtube.com/watch?v=MUxAdu5hvKk)
        if (!IsOccupied)
        {
            CheckForEnPassant(moves, board, file.Value, rank);
            return;
        }

        // Can capture piece
        if (IsOccupied && board.PieceColorOnSquare(targetSquare) != this.Color)
        {
            moves.Add(targetSquare);
        }
    }

    private void CheckForEnPassant(ICollection<string> moves, Board board, ChessFile file, int rank)
    {
        // En passant is only possible from the 5th rank (White) or the 4th rank (Black).
        var enPassantRank = Color == ChessColor.White ? 5 : 4;
        if (CurrentRank != enPassantRank)
        {
            return;
        }

        var nextToPawn = $"{file}{CurrentRank}";
        if (!board.IsSquareOccupied(nextToPawn))
        {
            return;
        }

        if (board.PieceColorOnSquare(nextToPawn) == this.Color)
        {
            return;
        }

        // The victim must be a pawn that just advanced two squares off its home
        // rank, so it has moved exactly once.
        if (board.PieceOnSquare(nextToPawn) is Pawn { NumberOfMoves: 1 })
        {
            moves.Add($"{file}{rank}");
        }
    }

    private bool ShouldPromote()
    {
        if (Color == ChessColor.White && CurrentRank == 7)
        {
            return true;
        }

        if (Color == ChessColor.Black && CurrentRank == 2)
        {
            return true;
        }

        return false;
    }
}