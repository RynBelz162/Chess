using Chess.Api.Helpers;

namespace Chess.Api.Models.Pieces;

public class Rook : Piece
{
    public override List<string> RecalculateAvailableMoves(Board board)
    {
        var moves = new List<string>();
        moves.AddRange(CalculateLeft(board));
        moves.AddRange(CalculateRight(board));
        moves.AddRange(CalculateTop(board));
        moves.AddRange(CalculateBottom(board));
        return moves;
    }

    private List<string> CalculateLeft(Board board)
    {
        var allFiles = ChessFileHelper.OrderedFiles;
        var index = Array.IndexOf(allFiles, ((char)CurrentFile));

        // if far left no moves available to left
        if (index == 0)
        {
            return new List<string>();
        }

        var moves = new List<string>();
        for (var i = index - 1; i >= 0; i--)
        {
            var targetSquare = $"{allFiles[i]}{CurrentRank}";
            var isOccupied = board.IsSquareOccupied(targetSquare);
            
            if (!isOccupied)
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

    private List<string> CalculateRight(Board board)
    {
        var allFiles = ChessFileHelper.OrderedFiles;
        var index = Array.IndexOf(allFiles, ((char)CurrentFile));

        // if far right no moves available to right
        if (index == (allFiles.Length - 1))
        {
            return new List<string>();
        }

        var moves = new List<string>();
        for (var i = index + 1; i <= allFiles.Length - 1; i++)
        {
            var targetSquare = $"{allFiles[i]}{CurrentRank}";
            var isOccupied = board.IsSquareOccupied(targetSquare);
            
            if (!isOccupied)
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

    private List<string> CalculateTop(Board board)
    {
        var moves = new List<string>();

        // if at top of board
        if (CurrentRank == 8)
        {
            return moves;
        }

        for (int i = CurrentRank + 1; i <= 8; i++)
        {
            var targetSquare = $"{CurrentFile}{i}";
            var isOccupied = board.IsSquareOccupied(targetSquare);
            
            if (!isOccupied)
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

    private List<string> CalculateBottom(Board board)
    {
        var moves = new List<string>();

        // if at bottom of board
        if (CurrentRank == 1)
        {
            return moves;
        }

        for (int i = CurrentRank - 1; i >= 1; i--)
        {
            var targetSquare = $"{CurrentFile}{i}";
            var isOccupied = board.IsSquareOccupied(targetSquare);
            
            if (!isOccupied)
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