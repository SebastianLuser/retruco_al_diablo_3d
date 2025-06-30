using Components;
using Components.Cards;

namespace GameSystems.AI
{
    public interface IAIStrategy
    {
        Card ChooseCard(Player opponent);
    }
}