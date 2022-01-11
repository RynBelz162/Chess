using Chess.Api.Constants;
using Chess.Api.Helpers;
using FluentAssertions;
using Xunit;

namespace Chess.Api.Tests.Helpers;

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
}