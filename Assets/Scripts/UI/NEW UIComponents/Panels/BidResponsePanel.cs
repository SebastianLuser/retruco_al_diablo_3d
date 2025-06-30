using Services;
using TMPro;
using UnityEngine;

namespace UI.UIComponents.Panels
{
    public class BidResponsePanel : BaseUIComponent
    {
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private TextMeshProUGUI descriptionText;

        protected override void SetupComponent()
        {
            componentId = "BidResponsePanel";
        }

        public void SetBidInfo(string bidName, string description = null)
        {
            if (titleText != null)
                titleText.text = $"{bidName} Called!";
                
            if (descriptionText != null)
                descriptionText.text = description ?? $"Opponent called {bidName}. Choose your response:";
        }
    }
}