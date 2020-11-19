namespace ReadyGamerOne.Rougelike.Person
{
    public interface IRankablePerson
    {
        int Exp { get; }
        int MaxExp { get; }
        string Rank { get; }
        bool TryLevelUp(int extraExp);
    }
}