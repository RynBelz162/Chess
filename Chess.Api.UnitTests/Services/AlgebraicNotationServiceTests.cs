using Chess.Api.Services;
using Chess.Shared.Constants;
using Chess.Shared.Helpers;
using Chess.Shared.Models;
using Chess.Shared.Models.Movement;
using Chess.Shared.Models.Pieces;

namespace Chess.Api.UnitTests.Services;

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
    public void GetRequest_WhenEmptyMove_ShouldReturnInvalid(string move)
    {
        var result = _algebraicNotationService.GetRequest(move, It.IsAny<Board>(), It.IsAny<ChessColor>());
        result.IsValid.Should().BeFalse();
        result.ErrorMessage.Should().Be("No move was provided.");
    }

    [Theory]
    [InlineData("q")]
    [InlineData("qxe4=d6")]
    public void GetRequest_WhenInvalidMove_ShouldReturnInvalid(string move)
    {
        var result = _algebraicNotationService.GetRequest(move, It.IsAny<Board>(), It.IsAny<ChessColor>());
        result.IsValid.Should().BeFalse();
        result.ErrorMessage.Should().Be("Invalid move was provided.");
    }

    [Theory]
    [MemberData(nameof(MoveRequestData))]
    public void GetRequest_WhenValidMove_ShouldReturnValid(string move, ChessColor color, MovementRequest expected, Board board)
    {
        var result = _algebraicNotationService.GetRequest(move, board, color);
        result.Should().BeEquivalentTo(expected);
    }

    private static Board WhiteCastleReadyBoard() =>
        new ChessBoardBuilder()
            .CreatePieceAt('K', ChessFile.E, 1)
            .CreatePieceAt('R', ChessFile.A, 1)
            .CreatePieceAt('R', ChessFile.H, 1)
            .Build();

    private static Board BlackCastleReadyBoard() =>
        new ChessBoardBuilder()
            .CreatePieceAt('k', ChessFile.E, 8)
            .CreatePieceAt('r', ChessFile.A, 8)
            .CreatePieceAt('r', ChessFile.H, 8)
            .Build();

    public static IEnumerable<object[]> MoveRequestData =>
        new List<object[]>
        {
            // kingside castle via symbol
            new object[]
            {
                "O-O",
                ChessColor.White,
                new MovementRequest
                {
                    PieceType = typeof(King),
                    TargetSquare = "G1",
                    PieceSquare = "E1",
                    IsValid = true,
                },
                WhiteCastleReadyBoard()
            },
            // queenside castle via symbol
            new object[]
            {
                "O-O-O",
                ChessColor.White,
                new MovementRequest
                {
                    PieceType = typeof(King),
                    TargetSquare = "C1",
                    PieceSquare = "E1",
                    IsValid = true,
                },
                WhiteCastleReadyBoard()
            },
            // kingside castle for black via O-O
            new object[]
            {
                "O-O",
                ChessColor.Black,
                new MovementRequest
                {
                    PieceType = typeof(King),
                    TargetSquare = "G8",
                    PieceSquare = "E8",
                    IsValid = true,
                },
                BlackCastleReadyBoard()
            },
            // king onto kingside rook
            new object[]
            {
                "KH1",
                ChessColor.White,
                new MovementRequest
                {
                    PieceType = typeof(King),
                    TargetSquare = "G1",
                    PieceSquare = "E1",
                    IsValid = true,
                },
                WhiteCastleReadyBoard()
            },
            // king onto queenside rook
            new object[]
            {
                "KA1",
                ChessColor.White,
                new MovementRequest
                {
                    PieceType = typeof(King),
                    TargetSquare = "C1",
                    PieceSquare = "E1",
                    IsValid = true,
                },
                WhiteCastleReadyBoard()
            },
            // black king onto kingside rook
            new object[]
            {
                "kh8",
                ChessColor.Black,
                new MovementRequest
                {
                    PieceType = typeof(King),
                    TargetSquare = "G8",
                    PieceSquare = "E8",
                    IsValid = true,
                },
                BlackCastleReadyBoard()
            },
            // black king onto queenside rook
            new object[]
            {
                "kA8",
                ChessColor.Black,
                new MovementRequest
                {
                    PieceType = typeof(King),
                    TargetSquare = "C8",
                    PieceSquare = "E8",
                    IsValid = true,
                },
                BlackCastleReadyBoard()
            },
            // king to castle target square directly
            new object[]
            {
                "Kg1",
                ChessColor.White,
                new MovementRequest
                {
                    PieceType = typeof(King),
                    TargetSquare = "G1",
                    PieceSquare = "E1",
                    IsValid = true,
                },
                WhiteCastleReadyBoard()
            },
            // pawn to e4 from e3
            new object[]
            {
                "e4",
                ChessColor.White,
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
                ChessColor.White,
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
                ChessColor.White,
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
                ChessColor.White,
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
                ChessColor.White,
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
                ChessColor.White,
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