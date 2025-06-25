namespace AI
{
    public interface IAIStrategy
    {
        Card ChooseCard(Player opponent);
    }
}