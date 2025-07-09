using System;
using UI;
using UnityEngine;

namespace Components.Cards
{
    public class CardView : MonoBehaviour
    {
        [SerializeField] private CardTextureDictionary textureDict;

        [SerializeField] private string frontQuadName = "Quad";

        public CardTextureDictionary TextureDict
        {
            get => textureDict;
            set => textureDict = value;
        }

        public int Owner { get; set; }
        
        [SerializeField] private CardClick _cardClick;
        [SerializeField] private HoverTooltipCard _hoverTooltipCard;

        public void Setup(Card card)
        {
            string key = card.ToString();
            Material tex = textureDict.GetCardTexture(key);

            Transform quad = transform.Find(frontQuadName);
            if (quad == null)
            {
                Debug.LogError($"[CardView] No child found '{frontQuadName}'");
                return;
            }

            var rend = quad.GetComponent<Renderer>();
            if (rend == null)
            {
                Debug.LogError("[CardView] Frontquad no render");
                return;
            }

            CardClick clickComponent = GetComponent<CardClick>();
            if (clickComponent != null)
            {
                clickComponent.card = card;
                clickComponent.ownerID = Owner;
            }

            rend.material = tex;
        }
    }
}