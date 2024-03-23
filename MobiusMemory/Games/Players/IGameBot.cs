using System.Drawing;
using MobiusMemory.Cards;
using MobiusMemory.Cards.Positioning;

namespace MobiusMemory.Games.Players;

public interface IGameBot
{
    public IPlayer Player { get; }

    public void MemoriseEmptyPlace(Position position);
    public void MemoriseCard(ICard card, IList<Position> positions);
    public Position SelectCard();
    public Position SelectCard(ICard card, Position position);
}