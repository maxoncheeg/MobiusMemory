namespace MobiusMemory.Cards.Positioning;

public readonly struct CardsSelection
{
    public CardsSelection(Position first, Position second)
    {
        if (first.X == second.X && second.Y == first.Y)
            throw new ArgumentException("same positions");
        First = first;
        Second = second;
    }

    public Position First { get; init; }
    public Position Second { get; init; }

    public void Deconstruct(out Position first, out Position second)
    {
        first = First;
        second = Second;
    }
}