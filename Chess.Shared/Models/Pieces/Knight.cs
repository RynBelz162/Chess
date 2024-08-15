using Chess.Shared.Constants;
using Chess.Shared.Helpers;

namespace Chess.Shared.Models.Pieces;

public class Knight(ChessFile chessFile, int rank) : Piece(chessFile, rank)
{
    public const char Identifier = 'N';

    public override List<string> RecalculateAvailableMoves(Board board)
    {
        var moves = new List<string>();
        
        ForwardLefts(board, moves);
        ForwardRights(board, moves);
        BackwardsLeft(board, moves);
        BackwardsRight(board, moves);

        return moves;
    }

    private void ForwardLefts(Board board, ICollection<string> moves)
    {
        // Horsey moves two ranks forward one file over 🐴
        if (CurrentRank <= 6 && CurrentFile != ChessFile.A)
        {
            var (left, _) = ChessFileHelper.GetLeftAndRightFile(CurrentFile);
            if (left is not null)
            {
                var bigLTarget = $"{left}{CurrentRank + 2}";
                AddToMovesListIfValid(board, moves, bigLTarget);
            }
        }

        // Horsey moves one rank forward two files left 🐴
        if (CurrentRank != 8 && CurrentFile != ChessFile.B && CurrentFile != ChessFile.A)
        {
            var (left, _) = ChessFileHelper.GetLeftAndRightFile(CurrentFile);
            if (left is null)
            {
                return;
            }

            var (twoLeft, _) = ChessFileHelper.GetLeftAndRightFile(left.Value);
            if (twoLeft is null)
            {
                return;
            }

            var littleLTarget = $"{twoLeft}{CurrentRank + 1}";
            AddToMovesListIfValid(board, moves, littleLTarget);
        }
    }

    public void ForwardRights(Board board, ICollection<string> moves)
    {
        // Horsey moves two ranks forward one file over 🐴
        if (CurrentRank <= 6 && CurrentFile != ChessFile.H)
        {
            var (_, right) = ChessFileHelper.GetLeftAndRightFile(CurrentFile);
            if (right is not null)
            {
                var bigLTarget = $"{right}{CurrentRank + 2}";
                AddToMovesListIfValid(board, moves, bigLTarget);
            }
        }

        // Horsey moves one rank forward two files right 🐴
        if (CurrentRank != 8 && CurrentFile != ChessFile.G && CurrentFile != ChessFile.H)
        {
            var (_, right) = ChessFileHelper.GetLeftAndRightFile(CurrentFile);
            if (right is null)
            {
                return;
            }

            var (_, twoRight) = ChessFileHelper.GetLeftAndRightFile(right.Value);
            if (twoRight is null)
            {
                return;
            }

            var littleLTarget = $"{twoRight}{CurrentRank + 1}";
            AddToMovesListIfValid(board, moves, littleLTarget);
        }
    }

    public void BackwardsLeft(Board board, ICollection<string> moves)
    {
        // Horsey moves two ranks backwards one file over 🐴
        if (CurrentRank >= 3 && CurrentFile != ChessFile.A)
        {
            var (left, _) = ChessFileHelper.GetLeftAndRightFile(CurrentFile);
            if (left is not null)
            {
                var bigLTarget = $"{left}{CurrentRank - 2}";
                AddToMovesListIfValid(board, moves, bigLTarget);
            }
        }

        // Horsey moves one rank backwards two files left 🐴
        if (CurrentRank != 1 && CurrentFile != ChessFile.B && CurrentFile != ChessFile.A)
        {
            var (left, _) = ChessFileHelper.GetLeftAndRightFile(CurrentFile);
            if (left is null)
            {
                return;
            }

            var (twoLeft, _) = ChessFileHelper.GetLeftAndRightFile(left.Value);
            if (twoLeft is null)
            {
                return;
            }

            var littleLTarget = $"{twoLeft}{CurrentRank - 1}";
            AddToMovesListIfValid(board, moves, littleLTarget);
        }
    }

    public void BackwardsRight(Board board, ICollection<string> moves)
    {
        // Horsey moves two ranks backwards one file over 🐴
        if (CurrentRank >= 3 && CurrentFile != ChessFile.H)
        {
            var (_, right) = ChessFileHelper.GetLeftAndRightFile(CurrentFile);
            if (right is not null)
            {
                var bigLTarget = $"{right}{CurrentRank - 2}";
                AddToMovesListIfValid(board, moves, bigLTarget);
            }
        }

        // Horsey moves one rank backwards two files right 🐴
        if (CurrentRank != 1 && CurrentFile != ChessFile.G && CurrentFile != ChessFile.H)
        {
            var (_, right) = ChessFileHelper.GetLeftAndRightFile(CurrentFile);
            if (right is null)
            {
                return;
            }

            var (_, twoRight) = ChessFileHelper.GetLeftAndRightFile(right.Value);
            if (twoRight is null)
            {
                return;
            }

            var littleLTarget = $"{twoRight}{CurrentRank - 1}";
            AddToMovesListIfValid(board, moves, littleLTarget);
        }
    }

    private void AddToMovesListIfValid(Board board, ICollection<string> moves, string targetSqaure)
    {
        if (!CanMoveToSqaure(board, targetSqaure))
        {
            return;
        }

        moves.Add(targetSqaure);
    }

    private bool CanMoveToSqaure(Board board, string targetSqaure)
    {
        if (!board.IsSquareOccupied(targetSqaure))
        {
            return true;
        }

        if (board.PieceColorOnSquare(targetSqaure) == Color)
        {
            return false;
        }

        return true;
    }
}