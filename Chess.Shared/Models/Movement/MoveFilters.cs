namespace Chess.Shared.Models.Movement;

// Ordered pipeline of move-legality filters. Running this after a raw
// recalculation leaves every piece holding only its legal moves.
public static class MoveFilters
{
    // Filters operate on disjoint piece sets (pins on non-kings, king safety on
    // kings) and read attacks from the live board, so the order is not
    // significant; the pipeline simply makes the full rule set explicit.
    private static readonly IReadOnlyList<IMoveFilter> Filters =
    [
        new PinFilter(),
        new KingSafetyFilter(),
        new CheckEvasionFilter(),
    ];

    public static void Apply(Board board)
    {
        foreach (var filter in Filters)
        {
            filter.Apply(board);
        }
    }
}
