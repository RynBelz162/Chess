using Chess.Shared.Constants;
using Chess.Shared.Helpers;
using Chess.Shared.Models.Movement;
using Chess.Shared.Models.Pieces;

namespace Chess.Shared.Extensions;

public static class PieceExtensions
{
    private static int MaxRank = 8;
    private static int MinRank = 1;

    public static void Forward(this Piece piece, Func<ChessFile, int, MovementResult> check, Action<MovementResult>? action = null)
    {
        for (int i = piece.CurrentRank + 1; i <= MaxRank; i++)
        {
            var result = check.Invoke(piece.CurrentFile, i);
            action?.Invoke(result);

            if (result.StopMoving) break;
        }
    }

    public static void Backward(this Piece piece, Func<ChessFile, int, MovementResult> check, Action<MovementResult>? action = null)
    {
        for (int i = piece.CurrentRank - 1; i >= MinRank; i--)
        {
            var result = check.Invoke(piece.CurrentFile, i);
            action?.Invoke(result);

            if (result.StopMoving) break;
        }
    }

    public static void Left(this Piece piece, Func<ChessFile, int, MovementResult> check, Action<MovementResult>? action = null)
    {
        var allFiles = ChessFileHelper.OrderedFiles;
        var index = Array.IndexOf(allFiles, ((char)piece.CurrentFile));

        for (int i = index - 1; i >= 0; i--)
        {
            var result = check.Invoke((ChessFile)allFiles[i], piece.CurrentRank);
            action?.Invoke(result);

            if (result.StopMoving) break;
        }
    }

    public static void Right(this Piece piece, Func<ChessFile, int, MovementResult> check, Action<MovementResult>? action = null)
    {
        var allFiles = ChessFileHelper.OrderedFiles;
        var index = Array.IndexOf(allFiles, ((char)piece.CurrentFile));

        for (int i = index + 1; i <= allFiles.Length - 1; i++)
        {
            var result = check.Invoke((ChessFile)allFiles[i], piece.CurrentRank);
            action?.Invoke(result);

            if (result.StopMoving) break;
        }
    }

    public static void DiagonalTopLeft(this Piece piece, Func<ChessFile, int, MovementResult> check, Action<MovementResult>? action = null)
    {
        var file = piece.CurrentFile;
        for (int i = (piece.CurrentRank + 1); i <= 8; i++)
        {
            var (left, _) = ChessFileHelper.GetLeftAndRightFile(file);
            if (left is null) break;

            file = left.Value;
            var result = check.Invoke(file, i);
            action?.Invoke(result);

            if (result.StopMoving) break;
        }
    }

    public static void DiagonalTopRight(this Piece piece, Func<ChessFile, int, MovementResult> check, Action<MovementResult>? action = null)
    {
        var file = piece.CurrentFile;
        for (int i = (piece.CurrentRank + 1); i <= 8; i++)
        {
            var (_, right) = ChessFileHelper.GetLeftAndRightFile(file);
            if (right is null) break;

            file = right.Value;
            var result = check.Invoke(file, i);
            action?.Invoke(result);

            if (result.StopMoving) break;
        }
    }

    public static void DiagonalBottomLeft(this Piece piece, Func<ChessFile, int, MovementResult> check, Action<MovementResult>? action = null)
    {
        var file = piece.CurrentFile;
        for (int i = (piece.CurrentRank - 1); i >= 1; i--)
        {
            var (left, _) = ChessFileHelper.GetLeftAndRightFile(file);
            if (left is null) break;

            file = left.Value;
            var result = check.Invoke(file, i);
            action?.Invoke(result);

            if (result.StopMoving) break;
        }
    }


    public static void DiagonalBottomRight(this Piece piece, Func<ChessFile, int, MovementResult> check, Action<MovementResult>? action = null)
    {
        var file = piece.CurrentFile;
        for (int i = (piece.CurrentRank - 1); i >= 1; i--)
        {
            var (_, right) = ChessFileHelper.GetLeftAndRightFile(file);
            if (right is null) break;

            file = right.Value;
            var result = check.Invoke(file, i);
            action?.Invoke(result);
            
            if (result.StopMoving) break;
        }
    }
}