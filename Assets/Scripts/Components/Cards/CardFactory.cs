namespace Cards
{
    using UnityEngine;

    public class CardFactory : MonoBehaviour, ICardFactory
    {
        [SerializeField] private GameObject cardPrefab;
        void Awake()
        {
            ServiceLocator.Register<ICardFactory>(this);
        }

        public GameObject Create(Card card, Transform parent, int ownerID)
        {
            var go = Instantiate(cardPrefab, parent);
            go.transform.localPosition = Vector3.zero;
            go.transform.localRotation = Quaternion.identity;
            
            var view = go.GetComponent<CardView>();
            view.TextureDict = ServiceLocator.Get<CardTextureDictionary>();
            view.Owner = ownerID;
            view.Setup(card);
            
            var click = go.GetComponent<CardClick>();
            if (click != null) click.ownerID = ownerID;

            return go;
        }
    }

}