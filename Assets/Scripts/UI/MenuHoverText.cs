using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(Collider))]
public class MenuHoverUIPanel : MonoBehaviour
{
    [Header("Prefab del tooltip")]
    [Tooltip("Prefab del Panel que contiene un Text o TMP_Text")]
    public GameObject tooltipPrefab;

    [Header("Padre UI")]
    [Tooltip("Transform del Canvas (Screen Space) donde instanciar el panel")]
    public Transform uiParent;

    [Header("Contenido")]
    [Tooltip("Texto que saldrá en el tooltip")]
    [TextArea]
    public string message = "Descripción de la carta";

    GameObject tooltipInstance;

    void OnMouseEnter()
    {
        // Si no hay prefab, padre o ya está activo, salimos
        if (tooltipPrefab == null || uiParent == null || tooltipInstance != null)
            return;

        // Instanciamos bajo el Canvas
        tooltipInstance = Instantiate(tooltipPrefab, uiParent);
        
        // Buscamos un Text clásico
        var uiText = tooltipInstance.GetComponentInChildren<Text>();
        if (uiText != null)
        {
            uiText.text = message;
            return;
        }
        // Si no, probamos TMP_Text
        var tmpText = tooltipInstance.GetComponentInChildren<TMP_Text>();
        if (tmpText != null)
        {
            tmpText.text = message;
        }
    }

    void OnMouseExit()
    {
        if (tooltipInstance != null)
            Destroy(tooltipInstance);
        tooltipInstance = null;
    }
}