using System;
using System.Collections.Generic;
using Chess.Api.Services;
using Chess.Shared.Constants;
using Chess.Shared.Models;
using Chess.Shared.Models.Pieces;

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

        board.Should().BeEquivalentTo(ExpectedDefaultBoard, opt =>
            // Ignoring the available moves because there are already unit tests
            // see Chess.Shared.Tests.Models.Pieces test files.
            opt.Excluding(x => x.DeclaringType == typeof(Piece) && x.Name.Equals(nameof(Piece.AvailableMoves)))
        );
    }

    private static Board ExpectedDefaultBoard =>
        new()
        {
            CurrentFen = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBKQKNR",
            Squares = ExpectedSquares,
            Pieces = ExpectedPieces
        };

    private static Dictionary<string, Square> ExpectedSquares =>
        new()
        {
            { "A1", new Square() { Piece = new Rook(ChessFile.A, 1) { Color = ChessColor.White }} },
            { "A2", new Square() { Piece = new Pawn(ChessFile.A, 2) { Color = ChessColor.White} } },
            { "A3", new Square() },
            { "A4", new Square() },
            { "A5", new Square() },
            { "A6", new Square() },
            { "A7", new Square() { Piece = new Pawn(ChessFile.A, 7) { Color = ChessColor.Black} } },
            { "A8", new Square() { Piece = new Rook(ChessFile.A, 8) { Color = ChessColor.Black} } },
            { "B1", new Square() { Piece = new Knight(ChessFile.B, 1) { Color = ChessColor.White} }},
            { "B2", new Square() { Piece = new Pawn(ChessFile.B, 2) { Color = ChessColor.White} }},
            { "B3", new Square() },
            { "B4", new Square() },
            { "B5", new Square() },
            { "B6", new Square() },
            { "B7", new Square() { Piece = new Pawn(ChessFile.B, 7) { Color = ChessColor.Black} }},
            { "B8", new Square() { Piece = new Knight(ChessFile.B, 8) { Color = ChessColor.Black} }},
            { "C1", new Square() { Piece = new Bishop(ChessFile.C, 1) { Color = ChessColor.White} }},
            { "C2", new Square() { Piece = new Pawn(ChessFile.C, 2) { Color = ChessColor.White} }},
            { "C3", new Square() },
            { "C4", new Square() },
            { "C5", new Square() },
            { "C6", new Square() },
            { "C7", new Square() { Piece = new Pawn(ChessFile.C, 7) { Color = ChessColor.Black} } },
            { "C8", new Square() { Piece = new Bishop(ChessFile.C, 8) { Color = ChessColor.Black} }},
            { "D1", new Square() { Piece = new Queen(ChessFile.D, 1) { Color = ChessColor.White} }},
            { "D2", new Square() { Piece = new Pawn(ChessFile.D, 2) { Color = ChessColor.White} }},
            { "D3", new Square() },
            { "D4", new Square() },
            { "D5", new Square() },
            { "D6", new Square() },
            { "D7", new Square() { Piece = new Queen(ChessFile.D, 7) { Color = ChessColor.Black} }},
            { "D8", new Square() { Piece = new Pawn(ChessFile.D, 8) { Color = ChessColor.Black} }},
            { "E1", new Square() { Piece = new King(ChessFile.E, 1) { Color = ChessColor.White} }},
            { "E2", new Square() { Piece = new Pawn(ChessFile.E, 2) { Color = ChessColor.White} }},
            { "E3", new Square() },
            { "E4", new Square() },
            { "E5", new Square() },
            { "E6", new Square() },
            { "E7", new Square() { Piece = new Pawn(ChessFile.E, 7) { Color = ChessColor.Black} }},
            { "E8", new Square() { Piece = new King(ChessFile.E, 8) { Color = ChessColor.Black} }},
            { "F1", new Square() { Piece = new Bishop(ChessFile.F, 1) { Color = ChessColor.White} }},
            { "F2", new Square() { Piece = new Pawn(ChessFile.F, 2) { Color = ChessColor.White} }},
            { "F3", new Square() },
            { "F4", new Square() },
            { "F5", new Square() },
            { "F6", new Square() },
            { "F7", new Square() { Piece = new Pawn(ChessFile.F, 7) { Color = ChessColor.Black} }},
            { "F8", new Square() { Piece = new Bishop(ChessFile.F, 8) { Color = ChessColor.Black} }},
            { "G1", new Square() { Piece = new Knight(ChessFile.G, 1) { Color = ChessColor.White} }},
            { "G2", new Square() { Piece = new Pawn(ChessFile.G, 2) { Color = ChessColor.White} }},
            { "G3", new Square() },
            { "G4", new Square() },
            { "G5", new Square() },
            { "G6", new Square() },
            { "G7", new Square() { Piece = new Pawn(ChessFile.G, 7) { Color = ChessColor.Black} }},
            { "G8", new Square() { Piece = new Bishop(ChessFile.G, 8) { Color = ChessColor.Black} }},
            { "H1", new Square() { Piece = new Rook(ChessFile.H, 1) { Color = ChessColor.White} }},
            { "H2", new Square() { Piece = new Pawn(ChessFile.H, 2) { Color = ChessColor.White} }},
            { "H3", new Square() },
            { "H4", new Square() },
            { "H5", new Square() },
            { "H6", new Square() },
            { "H7", new Square() { Piece = new Pawn(ChessFile.H, 7) { Color = ChessColor.Black} }},
            { "H8", new Square() { Piece = new Rook(ChessFile.H, 8) { Color = ChessColor.Black} }},
        };

    private static HashSet<Piece> ExpectedPieces =>
    [
        new Rook(ChessFile.A, 1) { Color = ChessColor.White },
        new Rook(ChessFile.H, 1) { Color = ChessColor.White },
        new Knight(ChessFile.B, 1) { Color = ChessColor.White },
        new Knight(ChessFile.G, 1) { Color = ChessColor.White },
        new Bishop(ChessFile.C, 1) { Color = ChessColor.White },
        new Bishop(ChessFile.F, 1) { Color = ChessColor.White },
        new Queen(ChessFile.D, 1) { Color = ChessColor.White },
        new King(ChessFile.E, 1) { Color = ChessColor.White },

        new Pawn(ChessFile.A, 2) { Color = ChessColor.White },
        new Pawn(ChessFile.H, 2) { Color = ChessColor.White },
        new Pawn(ChessFile.B, 2) { Color = ChessColor.White },
        new Pawn(ChessFile.G, 2) { Color = ChessColor.White },
        new Pawn(ChessFile.C, 2) { Color = ChessColor.White },
        new Pawn(ChessFile.F, 2) { Color = ChessColor.White },
        new Pawn(ChessFile.D, 2) { Color = ChessColor.White },
        new Pawn(ChessFile.E, 2) { Color = ChessColor.White },

        new Rook(ChessFile.A, 8) { Color = ChessColor.Black },
        new Rook(ChessFile.H, 8) { Color = ChessColor.Black },
        new Knight(ChessFile.B, 8) { Color = ChessColor.Black },
        new Knight(ChessFile.G, 8) { Color = ChessColor.Black },
        new Bishop(ChessFile.C, 8) { Color = ChessColor.Black },
        new Bishop(ChessFile.F, 8) { Color = ChessColor.Black },
        new Queen(ChessFile.D, 8) { Color = ChessColor.Black },
        new King(ChessFile.E, 8) { Color = ChessColor.Black },

        new Pawn(ChessFile.A, 7) { Color = ChessColor.Black },
        new Pawn(ChessFile.H, 7) { Color = ChessColor.Black },
        new Pawn(ChessFile.B, 7) { Color = ChessColor.Black },
        new Pawn(ChessFile.G, 7) { Color = ChessColor.Black },
        new Pawn(ChessFile.C, 7) { Color = ChessColor.Black },
        new Pawn(ChessFile.F, 7) { Color = ChessColor.Black },
        new Pawn(ChessFile.D, 7) { Color = ChessColor.Black },
        new Pawn(ChessFile.E, 7) { Color = ChessColor.Black },
    ];
}