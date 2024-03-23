using MobiusMemory.Cards;
using MobiusMemory.Cards.Positioning;

namespace MobiusMemory.Decks;

public interface IDeck
{
    public int AreaWidth { get; }
    public IReadOnlyList<IReadOnlyList<ICard?>> Cards { get; }
    
    public event Action? CardsAreOut;

    public bool OpenCards(CardsSelection selection);
}