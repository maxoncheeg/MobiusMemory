using System.Drawing;
using MobiusMemory.Cards;

namespace MobiusMemory.Decks;

public class Deck : IDeck
{
    private readonly List<List<ICard?>> _cards = [];
    
    public event Action? CardsAreOut;

    public IReadOnlyCollection<IReadOnlyCollection<ICard?>> Cards => _cards;

    public Deck(int width, ICardFactory factory)
    {
        if (width < 2)
            throw new ArgumentException("cardsAmount must be greater than 1", nameof(width));
        ArgumentNullException.ThrowIfNull(factory);

        string cardName = factory.GetCard();
        for (int i = 0, cardCounter = 0; i < width; i++)
        {
            List<ICard?> cardLine = [];
            for (int j = 0; j < width; j++)
            {
                cardLine.Add(new Card(cardName));
                if (++cardCounter == 2)
                {
                    cardCounter = 0;
                    cardName = factory.GetCard();
                }
            }
            _cards.Add(cardLine);
        }
    }

    public bool CheckCards(Point first, Point second)
    {
        ICard? card = _cards[first.Y][first.X];
        bool result = false;

        if (card == null && !(result = card!.Equals(_cards[second.Y][second.X]))) return result;
        
        _cards[first.Y][first.X] = _cards[second.Y][second.X] = null;
        CheckNullableCards();
        
        return result;
    }

    private void CheckNullableCards()
    {
        if (_cards.SelectMany(line => line).OfType<ICard>().Any())
            return;
        
        CardsAreOut?.Invoke();
    }
}