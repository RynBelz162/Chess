using Chess.Shared.Constants;
using Chess.Shared.Models;

namespace Chess.Api.Services;

public interface ISetupService
{
    ChessColor DeterminePlayerColor();
    ChessColor GetOppositeColor(ChessColor color);
    Board InitializeBoard();
}