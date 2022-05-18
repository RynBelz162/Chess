using Chess.Shared.Models;
using Chess.Shared.Models.Movement;

namespace Chess.Api.Services;

public interface IAlgebraicNotationService
{
    MovementRequest GetRequest(string move, Board board);
}