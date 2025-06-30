using Services;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.UIComponents.Actions
{
    public class AcceptButton : BaseUIComponent
    {
        [SerializeField] private Button acceptButton;
        [SerializeField] private TextMeshProUGUI buttonText;

        protected override void SetupComponent()
        {
            componentId = "AcceptButton";
            
            if (acceptButton != null)
            {
                acceptButton.onClick.AddListener(() => 
                {
                    EmitEvent("ResponseGiven", true);
                    Hide();
                });
            }

            if (buttonText != null)
                buttonText.text = "Accept";
        }

        public void SetText(string text)
        {
            if (buttonText != null)
                buttonText.text = text;
        }
    }

}