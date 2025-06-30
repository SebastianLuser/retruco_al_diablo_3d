using System;
using UnityEngine;

namespace Services
{
    public class RetrucoAlDiabloStarter : MonoBehaviour
    {
        private void Awake()
        {
            ServiceLocator.Register<IMatchesHistoryManager>(new MatchesHistoryManager());
        }
    }
}