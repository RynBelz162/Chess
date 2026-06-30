namespace Chess.Shared.Models.Movement;

// A move filter prunes illegal moves from pieces' AvailableMoves after the
// board has recalculated raw movement. Each filter owns one legality rule.
public interface IMoveFilter
{
    void Apply(Board board);
}
