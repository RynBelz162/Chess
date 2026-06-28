using Chess.Shared.Constants;
using Chess.Shared.Helpers;

namespace Chess.Shared.Models.Pieces;

public class King(ChessFile chessFile, int rank) : Piece(chessFile, rank)
{
    public const char Identifier = 'K';

    // King cannot be captured, so it carries no point value.
    public override int Value => 0;

    public override List<string> RecalculateAvailableMoves(Board board)
    {
        var moves = new List<string>();

        Forwards(moves, board);
        Backwards(moves, board);
        Left(moves, board);
        Right(moves, board);
        DiagonalTopLeft(moves, board);
        DiagonalTopRight(moves, board);
        DiagonalBottomLeft(moves, board);
        DiagonalBottomRight(moves, board);

        CheckCastles(moves, board);

        return moves;
    }

    public override Result<int> Move(string square, Board board)
    {
        if (NumberOfMoves > 0)
        {
            return base.Move(square, board);
        }

        var castle = CastleOptions().FirstOrDefault(o => o.KingTarget == square);
        var result = base.Move(square, board);
        
        if (!result.IsSuccess || castle is null)
        {
            return result;
        }

        // The king moved; bring the rook to the other side of it and refresh moves.
        if (board.PieceOnSquare(castle.RookSquare) is Rook rook)
        {
            rook.Relocate(castle.RookTarget, board);
            RecalculateAllMoves(board);
        }

        return result;
    }

    public bool IsKingChecked(Board board, ChessColor color)
    {
        var attackingColor = color == ChessColor.White ? ChessColor.Black : ChessColor.White;

        return board.Pieces
            .Any(p => p.Color == attackingColor && p.AvailableMoves.Contains(CurrentSquare));
    }

    private void Forwards(ICollection<string> moves, Board board)
    {
        if (CurrentRank == 8)
        {
            return;
        }

        var squareAbove = $"{CurrentFile}{CurrentRank + 1}";

        AddToMovesListIfValid(board, moves, squareAbove);
    }

    private void Backwards(ICollection<string> moves, Board board)
    {
        if (CurrentRank == 1)
        {
            return;
        }

        var squareBelow = $"{CurrentFile}{CurrentRank - 1}";

        AddToMovesListIfValid(board, moves, squareBelow);
    }

    private void Left(ICollection<string> moves, Board board)
    {
        if (CurrentFile == ChessFile.A)
        {
            return;
        }

        var (left, _) = ChessFileHelper.GetLeftAndRightFile(CurrentFile);
        var leftSquare = $"{left}{CurrentRank}";

        AddToMovesListIfValid(board, moves, leftSquare);
    }

    public void Right(ICollection<string> moves, Board board)
    {
        if (CurrentFile == ChessFile.H)
        {
            return;
        }

        var (_, right) = ChessFileHelper.GetLeftAndRightFile(CurrentFile);
        var rightSquare = $"{right}{CurrentRank}";
        
        AddToMovesListIfValid(board, moves, rightSquare);
    }


    public void DiagonalTopLeft(ICollection<string> moves, Board board)
    {
        if (CurrentFile == ChessFile.A || CurrentRank == 8)
        {
            return;
        }

        var (left, _) = ChessFileHelper.GetLeftAndRightFile(CurrentFile);
        var targetSquare = $"{left}{CurrentRank + 1}";
        
        AddToMovesListIfValid(board, moves, targetSquare);
    }


    public void DiagonalTopRight(ICollection<string> moves, Board board)
    {
        if (CurrentFile == ChessFile.H || CurrentRank == 8)
        {
            return;
        }

        var (_, right) = ChessFileHelper.GetLeftAndRightFile(CurrentFile);
        var targetSquare = $"{right}{CurrentRank + 1}";
        
        AddToMovesListIfValid(board, moves, targetSquare);
    }

    public void DiagonalBottomLeft(ICollection<string> moves, Board board)
    {
        if (CurrentFile == ChessFile.A || CurrentRank == 1)
        {
            return;
        }

        var (left, _) = ChessFileHelper.GetLeftAndRightFile(CurrentFile);
        var targetSquare = $"{left}{CurrentRank - 1}";
        
        AddToMovesListIfValid(board, moves, targetSquare);
    }


    public void DiagonalBottomRight(ICollection<string> moves, Board board)
    {
        if (CurrentFile == ChessFile.H || CurrentRank == 1)
        {
            return;
        }

        var (_, right) = ChessFileHelper.GetLeftAndRightFile(CurrentFile);
        var targetSquare = $"{right}{CurrentRank - 1}";
        
        AddToMovesListIfValid(board, moves, targetSquare);
    }

    private void AddToMovesListIfValid(Board board, ICollection<string> moves, string targetSquare)
    {
        if (!CanMoveToSquare(board, targetSquare))
        {
            return;
        }

        moves.Add(targetSquare);
    }

    private bool CanMoveToSquare(Board board, string targetSquare)
    {
        if (!board.IsSquareOccupied(targetSquare))
        {
            return true;
        }

        if (board.PieceColorOnSquare(targetSquare) == Color)
        {
            return false;
        }

        return true;
    }

    // King's target square, the rook involved, where the rook lands,
    // and the squares that must be empty between them.
    private sealed record CastleOption(string KingTarget, string RookSquare, string RookTarget, string[] EmptySquares);

    private int HomeRank => Color == ChessColor.White ? 1 : 8;

    private IEnumerable<CastleOption> CastleOptions()
    {
        var rank = HomeRank;

        // King side (H-rook): king to G, rook to F.
        yield return new CastleOption($"G{rank}", $"H{rank}", $"F{rank}", [$"F{rank}", $"G{rank}"]);

        // Queen side (A-rook): king to C, rook to D.
        yield return new CastleOption($"C{rank}", $"A{rank}", $"D{rank}", [$"B{rank}", $"C{rank}", $"D{rank}"]);
    }

    private void CheckCastles(ICollection<string> moves, Board board)
    {
        if (NumberOfMoves > 0)
        {
            return;
        }

        foreach (var option in CastleOptions())
        {
            if (CanCastle(board, option))
            {
                moves.Add(option.KingTarget);
            }
        }
    }

    private static bool CanCastle(Board board, CastleOption option)
    {
        if (board.PieceOnSquare(option.RookSquare) is not Rook rook || rook.NumberOfMoves > 0)
        {
            return false;
        }

        return option.EmptySquares.All(square => !board.IsSquareOccupied(square));
    }
}