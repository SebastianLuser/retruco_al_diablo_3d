using System;
using UnityEngine;

public class PassiveManager : MonoBehaviour
{
    public static PassiveManager Instance;

    public bool IsHiddenHandPassive { get; private set; } = false;
    public bool IsExtraEnvidoPointsPassive { get; private set; } = false;
    public bool IsSwapCardPassive { get; private set; } = false;
    
    public bool IsPassiveSelected { get; private set; } = false;

    private void Awake()
    {
        Instance= this;
    }

    public void SelectPassive(int index)
    {
        switch (index)
        {
            case 0:
                IsHiddenHandPassive = true;
                break;
            case 1:
                IsExtraEnvidoPointsPassive = true;
                break;
            case 2:
                IsSwapCardPassive = true;
                break;
        }

        IsPassiveSelected = true;
    }
}
