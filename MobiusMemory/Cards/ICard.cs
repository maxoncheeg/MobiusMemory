namespace MobiusMemory.Cards
{
    public interface ICard : IEquatable<ICard>
    {
        public string Name { get; }
    }
}
