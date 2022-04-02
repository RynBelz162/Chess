using Chess.Shared.Constants;
using Chess.Shared.Models;
using Chess.Shared.Models.Pieces;

namespace Chess.Shared.Helpers;

public static class BoardMovementHelper
{
    private static int MaxRank = 8;
    private static int MinRank = 1;

    public static void Forward(Piece piece, Func<ChessFile, int, MovementResult> check, Action<MovementResult>? action = null)
    {
        for (int i = piece.CurrentRank + 1; i <= MaxRank; i++)
        {
            var result = check.Invoke(piece.CurrentFile, i);
            action?.Invoke(result);

            if (result.StopMoving) break;
        }
    }

    public static void Backward(Piece piece, Func<ChessFile, int, MovementResult> check, Action<MovementResult>? action = null)
    {
        for (int i = piece.CurrentRank - 1; i >= MinRank; i--)
        {
            var result = check.Invoke(piece.CurrentFile, i);
            action?.Invoke(result);

            if (result.StopMoving) break;
        }
    }

    public static void Left(Piece piece, Func<ChessFile, int, MovementResult> check, Action<MovementResult>? action = null)
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

    public static void Right(Piece piece, Func<ChessFile, int, MovementResult> check, Action<MovementResult>? action = null)
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

    public static void DiagonalTopLeft(ChessFile file, int rank, Func<ChessFile, int, MovementResult> check)
    {
        ChessFile currentFile = file;
        for (int i = rank; i < MaxRank; i++)
        {
            var (left, _) = ChessFileHelper.GetLeftAndRightFile(file);
            if (left is null) break;

            currentFile = left.Value;
            var result = check.Invoke(currentFile, i);
            if (result.StopMoving) break;
        }
    }

    public static void DiagonalTopRight(ChessFile file, int rank, Func<ChessFile, int, MovementResult> check)
    {
        ChessFile currentFile = file;
        for (int i = rank; i < MaxRank; i++)
        {
            var (_, right) = ChessFileHelper.GetLeftAndRightFile(file);
            if (right is null) break;

            currentFile = right.Value;
            var result = check.Invoke(currentFile, i);
            if (result.StopMoving) break;
        }
    }

    public static void DiagonalBottomLeft(ChessFile file, int rank, Func<ChessFile, int, MovementResult> check)
    {
        ChessFile currentFile = file;
        for (int i = rank; i > MinRank; i--)
        {
            var (left, _) = ChessFileHelper.GetLeftAndRightFile(file);
            if (left is null) break;

            currentFile = left.Value;
            var result = check.Invoke(currentFile, i);
            if (result.StopMoving) break;
        }
    }


    public static void DiagonalBottomRight(ChessFile file, int rank, Func<ChessFile, int, MovementResult> check)
    {
        ChessFile currentFile = file;
        for (int i = rank; i > MinRank; i--)
        {
            var (_, right) = ChessFileHelper.GetLeftAndRightFile(file);
            if (right is null) break;

            currentFile = right.Value;
            var result = check.Invoke(currentFile, i);
            if (result.StopMoving) break;
        }
    }
}