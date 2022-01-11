using Chess.Api.Constants;

namespace Chess.Api.Helpers;

public static class ChessFileHelper 
{
    public static (ChessFile? leftFile, ChessFile? rightFile) GetLeftAndRightFile(ChessFile currentFile)
    {
        var files = Enum.GetValues<ChessFile>();
        var currIndex = Array.IndexOf(files, currentFile);

        ChessFile? leftFile = currIndex == 0 ? null : files[currIndex - 1];
        ChessFile? rightFile = (currIndex == files.Length - 1) ? null : files[currIndex + 1];

        return (leftFile, rightFile);
    }
}