using Chess.Shared.Constants;
using Chess.Shared.Helpers;
using FluentAssertions;
using Xunit;

namespace Chess.Shared.Tests.Helpers;

public class ChessFileHelperTests
{
    [Theory]
    [InlineData(ChessFile.D, ChessFile.C, ChessFile.E)]
    [InlineData(ChessFile.G, ChessFile.F, ChessFile.H)]
    [InlineData(ChessFile.B, ChessFile.A, ChessFile.C)]
    [InlineData(ChessFile.A, null, ChessFile.B)]
    [InlineData(ChessFile.H, ChessFile.G, null)]
    public void GetLeftAndRightFile_ReturnsCorrectFiles(ChessFile currentFile, ChessFile? expectedLeft, ChessFile? expectedRight)
    {
        var (leftFile, rightFile) = ChessFileHelper.GetLeftAndRightFile(currentFile);
        leftFile.Should().Be(expectedLeft);
        rightFile.Should().Be(expectedRight);
    }

    [Fact]
    public void GetOrderedFiles_ReturnsOrderedFiles()
    {
        var files = ChessFileHelper.OrderedFiles;
        var expected = new List<char>
        {
            'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H'
        };

        files.Should().BeInAscendingOrder();
        files.Should().BeEquivalentTo(expected);
    }
}