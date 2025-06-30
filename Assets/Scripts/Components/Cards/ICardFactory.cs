using UnityEngine;

namespace Components.Cards
{
    public interface ICardFactory
    {
        GameObject Create(Card card, Transform parent, int ownerID);
    }
}