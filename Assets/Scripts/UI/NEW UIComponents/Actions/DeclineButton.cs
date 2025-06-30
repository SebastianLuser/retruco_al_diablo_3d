using Services;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.UIComponents.Actions
{
    public class DeclineButton : BaseUIComponent
    {
        [SerializeField] private Button declineButton;
        [SerializeField] private TextMeshProUGUI buttonText;

        protected override void SetupComponent()
        {
            componentId = "DeclineButton";
            
            if (declineButton != null)
            {
                declineButton.onClick.AddListener(() => 
                {
                    EmitEvent("ResponseGiven", false);
                    Hide();
                });
            }

            if (buttonText != null)
                buttonText.text = "Decline";
        }

        public void SetText(string text)
        {
            if (buttonText != null)
                buttonText.text = text;
        }
    }
}