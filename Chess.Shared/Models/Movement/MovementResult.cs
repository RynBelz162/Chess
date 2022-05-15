using Chess.Shared.Constants;

namespace Chess.Shared.Models.Movement;

public class MovementResult
{
    public bool StopMoving { get; }
    public ChessFile? TargetFile { get; }
    public int? TargetRank { get; }

    public bool HasTargetSquare => TargetFile.HasValue && TargetRank.HasValue;

    public MovementResult(bool stopMoving, ChessFile? targetFile, int? targetRank)
    {
        StopMoving = stopMoving;
        TargetFile = targetFile;
        TargetRank = targetRank;
    }
}