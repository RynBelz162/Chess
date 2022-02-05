using Chess.Api.Constants;

namespace Chess.Api.Helpers;

public static class ChessFileHelper 
{
    private static readonly ChessFile[] _allFiles = Array.Empty<ChessFile>();
    public static readonly char[] OrderedFiles;
    static ChessFileHelper()
    {
        if (!_allFiles.Any())
        {
            _allFiles = Enum.GetValues<ChessFile>();
        }

        OrderedFiles = GetOrderedFiles();
    }

    public static (ChessFile? leftFile, ChessFile? rightFile) GetLeftAndRightFile(ChessFile currentFile)
    {
        var currIndex = Array.IndexOf(_allFiles, currentFile);

        ChessFile? leftFile = currIndex == 0 ? null : _allFiles[currIndex - 1];
        ChessFile? rightFile = (currIndex == _allFiles.Length - 1) ? null : _allFiles[currIndex + 1];

        return (leftFile, rightFile);
    }

    private static char[] GetOrderedFiles()
    {
        var files = _allFiles
            .Select(x => ((char)x))
            .ToArray();

        if (files is null || !files.Any())
        {
            throw new ApplicationException("Missing files constants.");
        }

        return files;
    }
}