namespace MobiusMemory.Games;

public class GameEventArgs : EventArgs
{
    public bool IsPlayerWin { get; set; } = false;
    public string PlayerName { get; set; } = string.Empty;
}