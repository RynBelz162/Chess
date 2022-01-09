using Chess.Api.Constants;
using Chess.Api.Models;

namespace Chess.Api.Services;

public interface ISetupService
{
    ChessColor DeterminePlayerColor();
    ChessColor GetOppositeColor(ChessColor color);
    Board InitializeBoard();
}