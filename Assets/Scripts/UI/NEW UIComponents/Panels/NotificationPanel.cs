using Services;
using TMPro;
using UnityEngine;

namespace UI.UIComponents.Panels
{
    public class NotificationPanel : BaseUIComponent
    {
        [SerializeField] private TextMeshProUGUI messageText;
        [SerializeField] private float autoHideDelay = 3f;

        protected override void SetupComponent()
        {
            componentId = "NotificationPanel";
        }

        public void ShowMessage(string message, float delay = -1)
        {
            if (messageText != null)
                messageText.text = message;
                
            Show();
            
            if (delay >= 0)
                Invoke(nameof(Hide), delay);
            else if (autoHideDelay > 0)
                Invoke(nameof(Hide), autoHideDelay);
        }
    }

}