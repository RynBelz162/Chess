using System;
using Chess.Shared.Helpers;
using Chess.Shared.Models.Pieces;

namespace Chess.Shared.Tests.Helpers;

public class ChessPieceHelperTests
{
    [Theory]
    [InlineData('p', typeof(Pawn))]
    [InlineData('P', typeof(Pawn))]
    [InlineData('n', typeof(Knight))]
    [InlineData('N', typeof(Knight))]
    [InlineData('r', typeof(Rook))]
    [InlineData('R', typeof(Rook))]
    [InlineData('b', typeof(Bishop))]
    [InlineData('B', typeof(Bishop))]
    [InlineData('q', typeof(Queen))]
    [InlineData('Q', typeof(Queen))]
    [InlineData('k', typeof(King))]
    [InlineData('K', typeof(King))]
    public void TypeFromIdentifier_ValidTypes_ReturnsType(char identifier, Type expectedType)
    {
        var result = ChessPieceHelper.TypeFromIdentifier(identifier);
        result.Should().Be(expectedType);
    }

    [Fact]
    public void TypeFromIdentifier_InvalidType_ThrowsException()
    {
        Action action = () => ChessPieceHelper.TypeFromIdentifier('X');
        action.Should().Throw<ArgumentException>().WithMessage("X is not a valid piece identifier.");
    }

    [Theory]
    [InlineData('p', true)]
    [InlineData('P', true)]
    [InlineData('n', true)]
    [InlineData('N', true)]
    [InlineData('r', true)]
    [InlineData('R', true)]
    [InlineData('b', true)]
    [InlineData('B', true)]
    [InlineData('q', true)]
    [InlineData('Q', true)]
    [InlineData('k', true)]
    [InlineData('K', true)]
    [InlineData('x', false)]
    [InlineData('A', false)]
    [InlineData('I', false)]
    [InlineData('y', false)]
    public void IsPieceIdentifier_ValidCharacter_ReturnsExpecetd(char identifier, bool expected)
    {
        var result = ChessPieceHelper.IsPieceIdentifier(identifier);
        result.Should().Be(expected);
    }
}