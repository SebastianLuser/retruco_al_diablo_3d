using UnityEngine;
using UnityEngine.UI;

namespace UI
{


    [RequireComponent(typeof(Collider))]
    public class HoverTooltipCard : MonoBehaviour
    {
        public GameObject tooltipPrefab;
        public Vector3 tooltipOffset = new Vector3(0, 1.5f, 0);
        public string tooltipText = "Información del objeto";

        public string outlineLayerName = "Outlin";

        private int originalLayer;
        private Renderer objectRenderer;
        private GameObject tooltipInstance;

        void Start()
        {
            originalLayer = gameObject.layer;

            objectRenderer = GetComponent<Renderer>();
        }

        void OnMouseEnter()
        {
            if (tooltipPrefab && tooltipInstance == null)
            {
                tooltipInstance = Instantiate(tooltipPrefab, transform.position + tooltipOffset, Quaternion.identity);
                Text textComponent = tooltipInstance.GetComponentInChildren<Text>();
                if (textComponent != null)
                {
                    textComponent.text = tooltipText;
                }
            }

            int outlineLayer = LayerMask.NameToLayer(outlineLayerName);
            if (outlineLayer != -1)
            {
                gameObject.layer = outlineLayer;
            }
            else
            {
                Debug.LogWarning("La layer '" + outlineLayerName + "' no existe.");
            }
        }

        void OnMouseExit()
        {
            if (tooltipInstance != null)
            {
                Destroy(tooltipInstance);
                tooltipInstance = null;
            }

            gameObject.layer = originalLayer;
        }

        void Update()
        {
            if (tooltipInstance != null)
            {
                tooltipInstance.transform.position = transform.position + tooltipOffset;
                tooltipInstance.transform.LookAt(Camera.main.transform);
            }
        }
    }

}