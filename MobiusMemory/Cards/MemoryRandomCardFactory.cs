using System.Collections;

namespace MobiusMemory.Cards;

public class MemoryRandomCardFactory : ICardFactory
{
    private readonly Random _random = new();
    private readonly ArrayList _indexMemory = [];

    private readonly ArrayList _cards =
    [
        "one", "two", "three", "four",
        "apple", "orange", "melon", "watermelon", "sugar",
        "car"
    ];

    public string GetCard(int index = -1)
    {
        if (index != -1 && (index < 0 || index > _cards.Count))
            throw new IndexOutOfRangeException(nameof(index));
        if(index == -1)
            do
                index = _random.Next(_cards.Count);
            while (_indexMemory.IndexOf(index) != -1);
        if (_cards.Count <= index)
            throw new ApplicationException("Incorrect card factory index");
        if (_cards[index] is not string value || string.IsNullOrEmpty(value))
            throw new ApplicationException("Incorrect card factory configuration ");

        _indexMemory.Add(index);
        return value;
    }
}