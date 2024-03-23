using System.Drawing;
using MobiusMemory.Cards;
using MobiusMemory.Cards.Positioning;

namespace MobiusMemory.Games.Players;

public class GameBot : IGameBot
{
    private readonly int _areaWidth;
    private readonly Random _random = new();
    private readonly List<Position> _remainCards = [];
    private readonly Dictionary<string, List<Position>> _memory = [];

    public IPlayer Player { get; }

    public GameBot(int cardsAreaWidth, string botName)
    {
        _areaWidth = cardsAreaWidth;
        Player = new Player(Guid.NewGuid(), $"Bot {botName}");
        

        for (int i = 0; i < _areaWidth; i++)
            for (int j = 0; j < _areaWidth; j++)
                _remainCards.Add(new(j, i));
    }

    public void MemoriseEmptyPlace(Position position)
    {
        _remainCards.Remove(position);

        var keys = _memory.Where(pair => pair.Value.Contains(position)).Select(pair => pair.Key);
        foreach (var key in keys)
            _memory[key].Remove(position);
    }

    public void MemoriseCard(ICard card, IList<Position> positions)
    {
        if (!_memory.ContainsKey(card.Name))
            _memory.Add(card.Name, []);

        foreach (var position in positions)
            if (!_memory[card.Name].Contains(position) && _remainCards.Contains(position))
                _memory[card.Name].Add(position);
    }

    // todo: make normal method
    public Position SelectCard()
    {
        List<string> keys = _memory.Where(pair => pair.Value.Count >= 2).Select(pair => pair.Key).ToList();
        if (keys.Count != 0)
        {
            var positions = _memory[keys.ElementAt(_random.Next(keys.Count()))];
            return positions[_random.Next(positions.Count)];
        }

        return _remainCards[_random.Next(_remainCards.Count)];
    }

    public Position SelectCard(ICard card, Position position)
    {
        if (!_memory.TryGetValue(card.Name, out List<Position>? value) || value.Count <= 1)
        {
            Position? pos = _remainCards.FirstOrDefault(pos => !pos.Equals(position));
            if (!pos.HasValue) throw new ApplicationException("not enough cards");
            return pos.Value;
        }

        return value.LastOrDefault(pos => !pos.Equals(position));
    }
}