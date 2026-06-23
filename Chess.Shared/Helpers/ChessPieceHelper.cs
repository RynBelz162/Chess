using Chess.Shared.Models.Pieces;

namespace Chess.Shared.Helpers;

public static class ChessPieceHelper
{
    public static Type TypeFromIdentifier(char identifier) =>
        char.ToUpper(identifier) switch
        {
            Pawn.Identifier => typeof(Pawn),
            Rook.Identifier => typeof(Rook),
            Bishop.Identifier => typeof(Bishop),
            Knight.Identifier => typeof(Knight),
            Queen.Identifier => typeof(Queen),
            King.Identifier => typeof(King),
            _ => throw new ArgumentException($"{identifier} is not a valid piece identifier.")
        };

    public static char IdentifierFromType(Type pieceType) => pieceType switch
    {
        Type t when t == typeof(Pawn) => Pawn.Identifier,
        Type t when t == typeof(Rook) => Rook.Identifier,
        Type t when t == typeof(Bishop) => Bishop.Identifier,
        Type t when t == typeof(Knight) => Knight.Identifier,
        Type t when t == typeof(Queen) => Queen.Identifier,
        Type t when t == typeof(King) => King.Identifier,
        _ => throw new ArgumentException($"{pieceType.Name} is not a valid piece type.")
    };

    public static bool IsPieceIdentifier(char character) =>
        char.ToUpper(character) switch
        {
            Pawn.Identifier => true,
            Rook.Identifier => true,
            Bishop.Identifier => true,
            Knight.Identifier => true,
            Queen.Identifier => true,
            King.Identifier => true,
            _ => false
        };
}