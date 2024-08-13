using System.Collections.Generic;
using Chess.Api.Services;
using Chess.Shared.Constants;
using Chess.Shared.Helpers;
using Chess.Shared.Models;
using Chess.Shared.Models.Movement;
using Chess.Shared.Models.Pieces;

namespace Chess.Api.Tests;

public class AlgebraicNotationServiceTests
{
    private readonly AlgebraicNotationService _algebraicNotationService;

    public AlgebraicNotationServiceTests()
    {
        _algebraicNotationService = new AlgebraicNotationService();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void GetRequest_EmptyMove_ReturnsInvalid(string move)
    {
        var result = _algebraicNotationService.GetRequest(move, It.IsAny<Board>());
        result.IsValid.Should().BeFalse();
        result.ErrorMessage.Should().Be("No move was provided.");
    }

    [Theory]
    [InlineData("q")]
    [InlineData("qxe4=d6")]
    public void GetRequest_InvalidMove_ReturnsInvalid(string move)
    {
        var result = _algebraicNotationService.GetRequest(move, It.IsAny<Board>());
        result.IsValid.Should().BeFalse();
        result.ErrorMessage.Should().Be("Invalid move was provided.");
    }

    [Theory]
    [MemberData(nameof(MoveRequestData))]
    public void GetRequest_EmptyMove_ReturnsValid(string move, MovementRequest expected, Board board)
    {
        var result = _algebraicNotationService.GetRequest(move, board);
        result.Should().BeEquivalentTo(expected);
    }

    public static IEnumerable<object[]> MoveRequestData =>
        new List<object[]>
        {
            // pawn to e4 from e3
            new object[]
            { 
                "e4",
                new MovementRequest
                {
                    PieceType = typeof(Pawn),
                    TargetSquare = "E4",
                    PieceSquare = "E3",
                    IsValid = true,
                },
                new ChessBoardBuilder().CreatePieceAt('P', ChessFile.E, 3).Build()
            },
            // rook to h8
            new object[]
            { 
                "rh8",
                new MovementRequest
                {
                    PieceType = typeof(Rook),
                    TargetSquare = "H8",
                    PieceSquare = "E8",
                    IsValid = true,
                },
                new ChessBoardBuilder().CreatePieceAt('R', ChessFile.E, 8).Build()
            },
            // bishop takes b2
            new object[]
            { 
                "bxb2",
                new MovementRequest
                {
                    PieceType = typeof(Bishop),
                    TargetSquare = "B2",
                    PieceSquare = "C1",
                    IsValid = true,
                },
                new ChessBoardBuilder().CreatePieceAt('B', ChessFile.C, 1).Build()
            },
            // queen takes e4
            new object[]
            { 
                "qxe4",
                new MovementRequest
                {
                    PieceType = typeof(Queen),
                    TargetSquare = "E4",
                    PieceSquare = "E3",
                    IsValid = true,
                },
                new ChessBoardBuilder().CreatePieceAt('Q', ChessFile.E, 3).Build()
            },
            // pawn to e4 from e3 but weird
            new object[]
            { 
                "pe3xe4",
                new MovementRequest
                {
                    PieceType = typeof(Pawn),
                    TargetSquare = "E4",
                    PieceSquare = "E3",
                    IsValid = true,
                },
                new ChessBoardBuilder().CreatePieceAt('P', ChessFile.E, 3).Build()
            },
            // king takes A4 but specific
            new object[]
            { 
                "ka3xa4",
                new MovementRequest
                {
                    PieceType = typeof(King),
                    TargetSquare = "A4",
                    PieceSquare = "A3",
                    IsValid = true,
                },
                new ChessBoardBuilder().CreatePieceAt('K', ChessFile.A, 3).Build()
            },
        };
}