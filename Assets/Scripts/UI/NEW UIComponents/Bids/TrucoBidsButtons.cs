using GameSystems.Bids;
using UnityEngine;
using UnityEngine.UI;

namespace UI.UIComponents.Bids
{
    public class TrucoButton : BaseUIComponent
    {
        [SerializeField] private Button trucoButton;

        protected override void SetupComponent()
        {
            componentId = "TrucoButton";
            
            if (trucoButton != null)
            {
                trucoButton.onClick.AddListener(() => 
                {
                    EmitEvent("BidSelected", BidType.Truco);
                    Hide();
                });
            }
        }
    }
    
    public class ReTrucoButton : BaseUIComponent
    {
        [SerializeField] private Button reTrucoButton;

        protected override void SetupComponent()
        {
            componentId = "TrucoButton";
            
            if (reTrucoButton != null)
            {
                reTrucoButton.onClick.AddListener(() => 
                {
                    EmitEvent("BidSelected", BidType.ReTruco);
                    Hide();
                });
            }
        }
    }
    
    public class ValeCuatroButton : BaseUIComponent
    {
        [SerializeField] private Button valeCuatroButton;

        protected override void SetupComponent()
        {
            componentId = "TrucoButton";
            
            if (valeCuatroButton != null)
            {
                valeCuatroButton.onClick.AddListener(() => 
                {
                    EmitEvent("BidSelected", BidType.ValeCuatro);
                    Hide();
                });
            }
        }
    }
}