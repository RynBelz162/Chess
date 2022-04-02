using Chess.Shared.Constants;
using Chess.Shared.Helpers;

namespace Chess.Shared.Models.Pieces;

public class Bishop : Piece
{
    public override List<string> RecalculateAvailableMoves(Board board)
    {
        var moves = new List<string>();
        moves.AddRange(AddTopLeft(board));
        moves.AddRange(AddTopRight(board));
        moves.AddRange(AddBottomLeft(board));
        moves.AddRange(AddBottomRight(board));
        return moves;
    }

    private List<string> AddTopLeft(Board board)
    {
        var moves = new List<string>();

        ChessFile file = CurrentFile;
        for (int i = (CurrentRank + 1); i <= 8; i++)
        {
            var (leftFile, _) = ChessFileHelper.GetLeftAndRightFile(file);
            if (leftFile is null)
            {
                break;
            }

            file = leftFile.Value;
            var targetSquare = $"{(char)file}{i}";
            if (!board.IsSquareOccupied(targetSquare))
            {
                moves.Add(targetSquare);
                continue;
            }

            if (board.PieceColorOnSqaure(targetSquare) != Color)
            {
                moves.Add(targetSquare);
            }

            break;
        }

        return moves;
    }

    private List<string> AddTopRight(Board board)
    {
        var moves = new List<string>();

        ChessFile file = CurrentFile;
        for (int i = (CurrentRank + 1); i <= 8; i++)
        {
            var (_, rightFile) = ChessFileHelper.GetLeftAndRightFile(file);
            if (rightFile is null)
            {
                break;
            }

            file = rightFile.Value;
            var targetSquare = $"{(char)file}{i}";
            if (!board.IsSquareOccupied(targetSquare))
            {
                moves.Add(targetSquare);
                continue;
            }

            if (board.PieceColorOnSqaure(targetSquare) != Color)
            {
                moves.Add(targetSquare);
            }

            break;
        }

        return moves;
    }

    private List<string> AddBottomRight(Board board)
    {
        var moves = new List<string>();

        ChessFile file = CurrentFile;
        for (int i = (CurrentRank - 1); i >= 1; i--)
        {
            var (_, rightFile) = ChessFileHelper.GetLeftAndRightFile(file);
            if (rightFile is null)
            {
                break;
            }

            file = rightFile.Value;
            var targetSquare = $"{(char)rightFile}{i}";
            if (!board.IsSquareOccupied(targetSquare))
            {
                moves.Add(targetSquare);
                continue;
            }

            if (board.PieceColorOnSqaure(targetSquare) != Color)
            {
                moves.Add(targetSquare);
            }

            break;
        }

        return moves;
    }

    private List<string> AddBottomLeft(Board board)
    {
        var moves = new List<string>();

        ChessFile file = CurrentFile;
        for (int i = (CurrentRank - 1); i >= 1; i--)
        {
            var (leftFile, _) = ChessFileHelper.GetLeftAndRightFile(file);
            if (leftFile is null)
            {
                break;
            }

            file = leftFile.Value;
            var targetSquare = $"{(char)leftFile}{i}";
            if (!board.IsSquareOccupied(targetSquare))
            {
                moves.Add(targetSquare);
                continue;
            }

            if (board.PieceColorOnSqaure(targetSquare) != Color)
            {
                moves.Add(targetSquare);
            }

            break;
        }

        return moves;
    }
}