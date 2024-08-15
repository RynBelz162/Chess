using Chess.Shared.Constants;
using Chess.Shared.Helpers;

namespace Chess.Shared.Models.Pieces;

public class King(ChessFile chessFile, int rank) : Piece(chessFile, rank)
{
    public const char Identifier = 'K';

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

        // TODO: Fix castle logic for spaces in between rook and king 🙃
        CheckWhiteCastles(moves, board);
        CheckBlackCastles(moves, board);

        return moves;
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
        var targetSqaure = $"{left}{CurrentRank + 1}";
        
        AddToMovesListIfValid(board, moves, targetSqaure);
    }


    public void DiagonalTopRight(ICollection<string> moves, Board board)
    {
        if (CurrentFile == ChessFile.H || CurrentRank == 8)
        {
            return;
        }

        var (_, right) = ChessFileHelper.GetLeftAndRightFile(CurrentFile);
        var targetSqaure = $"{right}{CurrentRank + 1}";
        
        AddToMovesListIfValid(board, moves, targetSqaure);
    }

    public void DiagonalBottomLeft(ICollection<string> moves, Board board)
    {
        if (CurrentFile == ChessFile.A || CurrentRank == 1)
        {
            return;
        }

        var (left, _) = ChessFileHelper.GetLeftAndRightFile(CurrentFile);
        var targetSqaure = $"{left}{CurrentRank - 1}";
        
        AddToMovesListIfValid(board, moves, targetSqaure);
    }


    public void DiagonalBottomRight(ICollection<string> moves, Board board)
    {
        if (CurrentFile == ChessFile.H || CurrentRank == 1)
        {
            return;
        }

        var (_, right) = ChessFileHelper.GetLeftAndRightFile(CurrentFile);
        var targetSqaure = $"{right}{CurrentRank - 1}";
        
        AddToMovesListIfValid(board, moves, targetSqaure);
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

    private void CheckWhiteCastles(ICollection<string> moves, Board board)
    {
        if (Color != ChessColor.White || NumberOfMoves > 0)
        {
            return;
        }

        const string leftWhiteCastle = "A1";
        const string leftWhiteSpace = "C1";

        const string rightWhiteCastle = "H1";
        const string rightWhiteSpace = "G1";

        AddMoveIfCanCastle(moves, board, leftWhiteCastle, leftWhiteSpace);
        AddMoveIfCanCastle(moves, board, rightWhiteCastle, rightWhiteSpace);
    }

    private void CheckBlackCastles(ICollection<string> moves, Board board)
    {
        if (Color != ChessColor.Black || NumberOfMoves > 0)
        {
            return;
        }

        const string leftBlackCastle = "A8";
        const string leftBlackSpace = "G8";

        const string rightBlackCastle = "H8";
        const string rightBlackSpace = "C8";

        AddMoveIfCanCastle(moves, board, leftBlackCastle, leftBlackSpace);
        AddMoveIfCanCastle(moves, board, rightBlackCastle, rightBlackSpace);
    }

    private void AddMoveIfCanCastle(ICollection<string> moves, Board board, string rookSquare, string targetSqaure)
    {
        var targetPiece = board.PieceOnSquare(rookSquare);
        if (targetPiece is Rook rook && rook.NumberOfMoves == 0)
        {
            moves.Add(targetSqaure);
        }
    }
}