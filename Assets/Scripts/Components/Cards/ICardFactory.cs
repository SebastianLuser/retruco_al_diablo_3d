using UnityEngine;

namespace Cards
{
    public interface ICardFactory
    {
        GameObject Create(Card card, Transform parent, int ownerID);
    }
}