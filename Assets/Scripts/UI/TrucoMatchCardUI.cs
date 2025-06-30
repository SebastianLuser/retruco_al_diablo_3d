using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TrucoMatchCardUI : MonoBehaviour
{
    [SerializeField] private TMP_Text dateText;
    [SerializeField] private TMP_Text playerText;
    [SerializeField] private TMP_Text resultText;
    [SerializeField] private Image panel;
    public void SetMatch(Match match)
    {
        dateText.text = match.Date.ToString("MM/dd/yyyy hh:mm");
        playerText.text = $"{match.Player1} vs Diablo";
        resultText.text = $"Ganó: {match.Result}";
        panel.color = match.Result == "Diablo" ? Color.red : Color.green;
    } 
}