namespace MobiusMemory.Cards;

public class Card : ICard
{
    public string Name { get; init; }

    public Card(string name)
    {
        if (string.IsNullOrEmpty(name))
            throw new ArgumentException(nameof(Name));

        Name = name;
    }

    public bool Equals(ICard? other) => other != null && Name == other.Name;
}