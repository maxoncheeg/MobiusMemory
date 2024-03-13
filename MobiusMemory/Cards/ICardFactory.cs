namespace MobiusMemory.Cards;

public interface ICardFactory
{
    public string GetCard(int index = -1);
}