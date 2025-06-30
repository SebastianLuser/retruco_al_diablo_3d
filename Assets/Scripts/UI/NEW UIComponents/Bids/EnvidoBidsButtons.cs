using GameSystems.Bids;
using UnityEngine;
using UnityEngine.UI;

namespace UI.UIComponents.Bids
{
    public class EnvidoBidButton : BaseUIComponent
    {
        [SerializeField] private Button envidoButton;

        protected override void SetupComponent()
        {
            componentId = "EnvidoButton";
            
            if (envidoButton != null)
            {
                envidoButton.onClick.AddListener(() => 
                {
                    EmitEvent("BidSelected", BidType.Envido);
                    Hide();
                });
            }
        }
    }
    
    public class RealEnvidoButton : BaseUIComponent
    {
        [SerializeField] private Button realEnvidoButton;

        protected override void SetupComponent()
        {
            componentId = "RealEnvidoButton";
            
            if (realEnvidoButton != null)
            {
                realEnvidoButton.onClick.AddListener(() => 
                {
                    EmitEvent("BidSelected", BidType.RealEnvido);
                    Hide();
                });
            }
        }
    }

    
    public class FaltaEnvidoButton : BaseUIComponent
    {
        [SerializeField] private Button faltaEnvidoButton;

        protected override void SetupComponent()
        {
            componentId = "FaltaEnvidoButton";
            
            if (faltaEnvidoButton != null)
            {
                faltaEnvidoButton.onClick.AddListener(() => 
                {
                    EmitEvent("BidSelected", BidType.FaltaEnvido);
                    Hide();
                });
            }
        }
    }
}