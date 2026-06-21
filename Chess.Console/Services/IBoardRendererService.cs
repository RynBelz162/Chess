using Chess.Shared.Constants;

namespace Chess.Console.Services;

public interface IBoardRendererService
{
    void Render(string fen, ChessColor perspective);
}
