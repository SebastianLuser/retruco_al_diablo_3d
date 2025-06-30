using Services;
using UnityEngine;
using UnityEngine.UI;

namespace GameSystems.AI
{
    [RequireComponent(typeof(Dropdown))]
    public class AIStrategyDropdown : MonoBehaviour
    {
        private Dropdown _dropdown;

        void Start()
        {
            _dropdown = GetComponent<Dropdown>();
            _dropdown.ClearOptions();
            _dropdown.AddOptions(new System.Collections.Generic.List<string>
            {
                "MinPowerStrategy",
                "MaxPowerStrategy",
                "RandomPowerStrategy"
            });
            _dropdown.onValueChanged.AddListener(OnChanged);

            ApplyStrategy(0);
        }

        private void OnChanged(int idx)
        {
            ApplyStrategy(idx);
        }

        private void ApplyStrategy(int idx)
        {
            IAIStrategy strat = idx switch
            {
                0 => new MinPowerStrategy(),
                1 => new MaxPowerStrategy(),
                2 => new RandomPowerStrategy(),
                _ => new MinPowerStrategy()
            };

            TurnManager.Instance.SetAIStrategy(strat);
            Debug.Log($"🧠 Estrategia seleccionada vía Dropdown: {strat.GetType().Name}");
        }
    }
}