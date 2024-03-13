using MobiusMemory.Cards;

namespace MobiusMemory.Decks;

public interface IDeck
{
    public event Action? CardsAreOut;
    public IReadOnlyCollection<IReadOnlyCollection<ICard?>> Cards { get; }

    public bool CheckCards(System.Drawing.Point first, System.Drawing.Point second);
}