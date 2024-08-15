using Chess.Shared.Constants;

namespace Chess.Shared.Helpers;

public static class ChessFileHelper 
{
    private static readonly ChessFile[] _allFiles = [];
    public static readonly char[] OrderedFiles;
    static ChessFileHelper()
    {
        if (_allFiles.Length == 0)
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
            .Select(x => (char)x)
            .ToArray();

        if (files is null || files.Length == 0)
        {
            throw new ApplicationException("Missing files constants.");
        }

        return files;
    }

    public static ChessFile ToChessFile(this int value) =>
        value switch 
        {
            1 => ChessFile.A,
            2 => ChessFile.B,
            3 => ChessFile.C,
            4 => ChessFile.D,
            5 => ChessFile.E,
            6 => ChessFile.F,
            7 => ChessFile.G,
            8 => ChessFile.H,
            _ => throw new ArgumentException("Invalid integer to ChessFile", nameof(value))
        };
}