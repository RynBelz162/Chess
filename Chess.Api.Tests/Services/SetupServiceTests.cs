using System;
using Chess.Api.Services;
using Chess.Shared.Constants;

namespace Chess.Api.Tests.Services;

public class SetupServiceTests
{
    private readonly SetupService _setupService = new();

    [Theory]
    [InlineData(ChessColor.White, ChessColor.Black)]
    [InlineData(ChessColor.Black, ChessColor.White)]
    public void GetOppositeColor_WhenPassValidColor_ShouldReturnOpposite(ChessColor color, ChessColor opposite)
    {
        var oppositeColor = _setupService.GetOppositeColor(color);
        oppositeColor.Should().Be(opposite);
    }

    [Fact]
    public void GetOppositeColor_WhenInvalidColor_ThrowsInvalidException()
    {
        var act = () => _setupService.GetOppositeColor((ChessColor)2);
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void InitializeBoard_ShouldReturnFreshBoard()
    {
        var board = _setupService.InitializeBoard();
        board.Pieces.Count.Should().Be(32);
    }
}