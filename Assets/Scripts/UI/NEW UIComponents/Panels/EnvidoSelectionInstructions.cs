using Services;
using TMPro;
using UnityEngine;

namespace UI.UIComponents.Panels
{
    public class EnvidoSelectionInstructions : BaseUIComponent
    {
        [SerializeField] private TextMeshProUGUI instructionText;

        protected override void SetupComponent()
        {
            componentId = "EnvidoInstructions";
            
            if (instructionText != null)
                instructionText.text = "Select 1-2 cards for your Envido calculation";
        }

        public void SetInstructions(string text)
        {
            if (instructionText != null)
                instructionText.text = text;
        }
    }
}