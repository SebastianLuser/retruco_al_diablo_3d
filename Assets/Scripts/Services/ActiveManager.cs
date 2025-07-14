using System;
using Services;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class ActiveManager : MonoBehaviour
{
    public static ActiveManager Instance;

    [SerializeField] private TMP_Text _diceOne, _diceTwo;
    [SerializeField] private GameObject _diceSelectorParent;

    [SerializeField] private int firstDice, secondDice;

    [SerializeField] private TMP_Text _selectedDiceText;

    [SerializeField] private TMP_Text _firstDescription, _secondDescription;

    [SerializeField] private Image _diceImage;

    private int _diceSelected;
    public int DiceSelected => _diceSelected;

    private void Awake()
    {
        Instance = this;
    }

    public void RerollDice()
    {
        _selectedDiceText.transform.parent.gameObject.SetActive(false);

        _diceSelectorParent.SetActive(true);

        _diceOne.gameObject.SetActive(true);
        _diceTwo.gameObject.SetActive(true);
        firstDice = Random.Range(1, 4);
        secondDice = Random.Range(1, 4);

        secondDice = secondDice == firstDice ? secondDice == 3 ? 1 : secondDice + 1 : secondDice;

        _diceOne.text = firstDice.ToString();
        _diceTwo.text = secondDice.ToString();

        _firstDescription.text = GetDiceDescription(firstDice);
        _secondDescription.text = GetDiceDescription(secondDice);
    }

    public void OnSelectedDice(int id)
    {
        _selectedDiceText.transform.parent.gameObject.SetActive(true);
        _diceSelected = id == 0 ? firstDice : secondDice;
        _selectedDiceText.text = _diceSelected.ToString();

        TurnManager.Instance.isDiceSelected = true;
    }

    public void ActiveDiceMode()
    {
        TurnManager.Instance.isUsingActives = !TurnManager.Instance.isUsingActives;

        _diceImage.color = TurnManager.Instance.isUsingActives ? Color.green : Color.white;
    }

    public void OnUseDice()
    {
        _selectedDiceText.transform.parent.gameObject.SetActive(false);
    }

    private string GetDiceDescription(int dice)
    {
        switch (dice)
        {
            case 1:
                return "+1 on Card Rank";
            case 2:
                return "Redraw a card";
            case 3:
                return "Randomize the suit of a card";
            default:
                return "Not Info";
        }
    }

}
