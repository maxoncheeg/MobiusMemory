namespace MobiusMemory.Cards.Positioning;

public struct Position
{
    public int X { get; init; }
    public int Y { get; init; }

    public Position(int x, int y)
    {
        if (x < 0)
            throw new ArgumentException($"{nameof(x)} must be greater or equals zero.");
        if (y < 0)
            throw new ArgumentException($"{nameof(y)} must be greater or equals zero.");

        X = x;
        Y = y;
    }
}