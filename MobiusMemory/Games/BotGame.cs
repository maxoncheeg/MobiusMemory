using System.Diagnostics;
using MobiusMemory.Cards;
using MobiusMemory.Cards.Positioning;
using MobiusMemory.Decks;
using MobiusMemory.Games.Players;

namespace MobiusMemory.Games;

public enum BotDifficulty
{
    Easy,
    Medium,
    Hard,
    Impossible
}

public class BotGame : IGame
{
    private readonly Random _random = new();
    private readonly IDeck _deck;
    private readonly Dictionary<IGameBot, BotDifficulty>? _bots;
    private readonly List<IPlayer> _players;
    private readonly IPlayer _player;

    public IPlayer CurrentTurnPlayer { get; private set; }

    public IReadOnlyCollection<IPlayer> Players => _players;
    public IDictionary<IGameBot, BotDifficulty>? Bots => _bots;

    public event EventHandler<GameEventArgs>? GameFinished;
    public event Action<string>? MessageReceived;

    public BotGame(IDeck deck, IPlayer? player, Dictionary<string, BotDifficulty>? botDifficulties = null)
    {
        //ArgumentNullException.ThrowIfNull(player);
        ArgumentNullException.ThrowIfNull(deck);

        _deck = deck;
        _deck.CardsAreOut += DeckOnCardsAreOut;
        _players = [];
        if(player != null)
            _players.Add(player);
        
        _player = player;

        if (botDifficulties != null && botDifficulties.Count != 0)
        {
            _bots = new Dictionary<IGameBot, BotDifficulty>();
            foreach (var pair in botDifficulties)
                _bots.Add(new GameBot(_deck.AreaWidth, pair.Key), pair.Value);
            _players.AddRange(_bots.Select(bot => bot.Key.Player));
        }

        _players.Shuffle(_random);
        CurrentTurnPlayer = _players.First();
    }

    private void DeckOnCardsAreOut()
    {
        IPlayer player = (from p in _players
            select p).MaxBy(p => p.Points);
        GameFinished?.Invoke(this, new GameEventArgs() {PlayerName = player.Name});
    }

    public OpenStatus OpenCard(IPlayer player, CardsSelection selection)
    {
        if (selection.First.Equals(selection.Second)) return OpenStatus.Failure;
        if (!_players.Contains(player))
        {
            MessageReceived?.Invoke("playerDoesntExist");
            return OpenStatus.WrongTurn;
        }

        if (player != CurrentTurnPlayer)
        {
            MessageReceived?.Invoke("wrongTurn");
            return OpenStatus.WrongTurn;
        }

        bool openSuccess = _deck.OpenCards(selection);
        Debug.WriteLine(player.Name + Environment.NewLine + openSuccess);
        MemoriseSelectionForBots(selection, openSuccess);

        if (!openSuccess)
            CurrentTurnPlayer = _players[(_players.IndexOf(CurrentTurnPlayer) + 1) % _players.Count];

        return openSuccess ? OpenStatus.Success : OpenStatus.Failure;
    }

    public IReadOnlyList<IReadOnlyList<ICard?>> GetCards() => _deck.Cards;

    private void MemoriseSelectionForBots(CardsSelection selection, bool openSuccess)
    {
        if (_bots == null) return;
        var (first, second) = selection;

        foreach (var bot in _bots)
        {
            if (openSuccess)
            {
                bot.Key.MemoriseEmptyPlace(first);
                bot.Key.MemoriseEmptyPlace(second);
                continue;
            }

            List<Position> firstPositions = [], secondPositions = [];

            if (bot.Value > BotDifficulty.Easy)
            {
                if (_deck.Cards[first.Y][first.X] != null)
                    firstPositions.Add(first);
                if (_deck.Cards[first.Y][first.X] != null)
                    secondPositions.Add(second);
            }

            switch (bot.Value)
            {
                case BotDifficulty.Hard:
                    if (_deck.Cards[first.Y][first.X] != null)
                        MemoriseAroundCards(firstPositions, first);
                    if (_deck.Cards[second.Y][second.X] != null)
                        MemoriseAroundCards(secondPositions, second);
                    break;
                case BotDifficulty.Medium:
                    if (_deck.Cards[first.Y][first.X] != null)
                        MemoriseAllAroundCards(firstPositions, first);
                    if (_deck.Cards[second.Y][second.X] != null)
                        MemoriseAllAroundCards(secondPositions, second);
                    break;
                case BotDifficulty.Impossible:
                case BotDifficulty.Easy:
                default:
                    break;
            }

            if (firstPositions.Count > 0 && _deck.Cards[first.Y][first.X] is { } firstCard)
                bot.Key.MemoriseCard(firstCard, firstPositions);
            if (secondPositions.Count > 0 && _deck.Cards[second.Y][second.X] is { } secondCard)
                bot.Key.MemoriseCard(secondCard, secondPositions);
        }
    }

    private void MemoriseAllAroundCards(ICollection<Position> positions, Position cardPosition)
    {
        MemoriseAroundCards(positions, cardPosition);
        if (_random.Next(3) >= 1
            && cardPosition.Y - 1 >= 0
            && cardPosition.X - 1 >= 0 && _deck.Cards[cardPosition.Y - 1][cardPosition.X - 1] != null)
            positions.Add(new Position(cardPosition.X - 1, cardPosition.Y - 1));
        if (_random.Next(3) >= 1
            && cardPosition.Y + 1 < _deck.AreaWidth
            && cardPosition.X + 1 < _deck.AreaWidth && _deck.Cards[cardPosition.Y + 1][cardPosition.X + 1] != null)
            positions.Add(new Position(cardPosition.X + 1, cardPosition.Y + 1));
        if (_random.Next(3) >= 1
            && cardPosition.Y + 1 < _deck.AreaWidth
            && cardPosition.X - 1 >= 0 && _deck.Cards[cardPosition.Y + 1][cardPosition.X - 1] != null)
            positions.Add(new Position(cardPosition.X - 1, cardPosition.Y + 1));
        if (_random.Next(3) >= 1
            && cardPosition.Y - 1 >= 0
            && cardPosition.X + 1 < _deck.AreaWidth && _deck.Cards[cardPosition.Y - 1][cardPosition.X + 1] != null)
            positions.Add(new Position(cardPosition.X + 1, cardPosition.Y - 1));
    }

    private void MemoriseAroundCards(ICollection<Position> positions, Position cardPosition)
    {
        if (_random.Next(3) >= 1 && cardPosition.Y - 1 >= 0 && _deck.Cards[cardPosition.Y - 1][cardPosition.X] != null)
            positions.Add(new Position(cardPosition.X, cardPosition.Y - 1));
        if (_random.Next(3) >= 1 && cardPosition.Y + 1 < _deck.AreaWidth &&
            _deck.Cards[cardPosition.Y + 1][cardPosition.X] != null)
            positions.Add(new Position(cardPosition.X, cardPosition.Y + 1));
        if (_random.Next(3) >= 1 && cardPosition.X - 1 >= 0 && _deck.Cards[cardPosition.Y][cardPosition.X - 1] != null)
            positions.Add(new Position(cardPosition.X, cardPosition.Y - 1));
        if (_random.Next(3) >= 1 && cardPosition.X + 1 < _deck.AreaWidth &&
            _deck.Cards[cardPosition.Y][cardPosition.X + 1] != null)
            positions.Add(new Position(cardPosition.X, cardPosition.Y + 1));
    }
}