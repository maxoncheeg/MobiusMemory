using MobiusMemory.Cards.Positioning;
using MobiusMemory.Cards;
using MobiusMemory.Decks;
using MobiusMemory.Games.Players;

namespace MobiusMemory.Games;

public enum OpenStatus
{
    Success,
    Failure,
    WrongTurn
}
public interface IGame
{
    public IPlayer CurrentTurnPlayer { get; }
    public IReadOnlyCollection<IPlayer> Players { get; }
    public IDictionary<IGameBot, BotDifficulty>? Bots { get; }
 
    public event EventHandler<GameEventArgs>? GameFinished;
    public event Action<string>? MessageReceived;
    
    public OpenStatus OpenCard(IPlayer player, CardsSelection selection);
    
    public IReadOnlyList<IReadOnlyList<ICard?>> GetCards();
}