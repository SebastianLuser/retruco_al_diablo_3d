using UnityEngine;
using UnityEngine.UI;
using TMPro;
using GameSystems;

namespace Services
{
    public interface IResponseService
    {
        void ShowBidResponse(ResponseActions actions, string bidName = "");
        void HideBidResponse();
    }

    public class ResponseService : MonoBehaviour, IResponseService
    {
        [SerializeField] private GameObject responsePanel;
        [SerializeField] private Button acceptButton;
        [SerializeField] private Button declineButton;
        [SerializeField] private Button raiseButton;
        
        [SerializeField] private TextMeshProUGUI titleText;

        private ResponseActions currentActions;

        void Awake()
        {
            ServiceLocator.Register<IResponseService>(this);
            
            if (responsePanel != null)
                responsePanel.SetActive(false);
                
            Debug.Log("Response service registered");
        }

        void Start()
        {
            acceptButton?.onClick.AddListener(OnAcceptPressed);
            declineButton?.onClick.AddListener(OnDeclinePressed);
            raiseButton?.onClick.AddListener(OnRaisePressed);
        }

        public void ShowBidResponse(ResponseActions actions, string bidName = "")
        {
            if (responsePanel == null || actions == null) return;

            currentActions = actions;
            responsePanel.SetActive(true);
            
            if (titleText != null)
                titleText.text = $"Te cantaron {bidName}";
            
            if (raiseButton != null)
                raiseButton.gameObject.SetActive(actions.HasRaiseAction);
            
            Debug.Log($"📱 Mostrando respuesta para {bidName}");
        }

        public void HideBidResponse()
        {
            responsePanel?.SetActive(false);
            currentActions = null;
        }

        private void OnAcceptPressed()
        {
            Debug.Log("✅ QUIERO presionado");
            currentActions?.ExecuteAccept();
            HideBidResponse();
        }

        private void OnDeclinePressed()
        {
            Debug.Log("❌ NO QUIERO presionado");
            currentActions?.ExecuteDecline();
            HideBidResponse();
        }

        private void OnRaisePressed()
        {
            Debug.Log("⬆️ SUBIR presionado");
            currentActions?.ExecuteRaise();
            HideBidResponse();
        }
    }
}