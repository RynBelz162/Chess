using System.Collections.Generic;
using Chess.Api.Constants;
using Chess.Api.Models.Pieces;
using FluentAssertions;
using Xunit;

namespace Chess.Api.Tests.Models.Pieces;

public class PawnTests
{
    [Fact]
    public void RecalculateAvailableMoves_EmptyAbove_CanMoveUp()
    {
        var pawn = new Pawn
        {
            Color = ChessColor.White,
        };

        var mockBoard = new ChessBoardBuilder()
            .PlacePieceAt(pawn, ChessFile.B, 2)
            .Build();

        var moves = pawn.RecalculateAvailableMoves(mockBoard);
        List<string> expectedMoves = new() { "B3" };

        moves.Should().BeEquivalentTo(expectedMoves);
    }
}