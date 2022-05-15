namespace Chess.Shared.Models.Movement;

public class MovementRequest
{
    public bool IsValid { get; init; } = true;
    public string ErrorMessage { get; init; } = string.Empty;
    public Type PieceType { get; init; } = null!;
    public string PieceSquare { get; init; } = string.Empty;
    public string TargetSquare { get; init; } = string.Empty;
    public bool IsTaking { get; init; } = false;
    public bool IsPromoting { get; init; } = false;
    public Type? PromotionType { get; init; }
}