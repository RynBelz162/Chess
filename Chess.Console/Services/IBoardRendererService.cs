using Chess.Shared.Constants;
using Chess.Shared.Models.State;

namespace Chess.Console.Services;

public interface IBoardRendererService
{
    void Render(GameStateSnapshot snapshot, ChessColor perspective);
}
