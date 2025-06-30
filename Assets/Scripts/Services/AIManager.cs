using Components;
using UnityEngine;
using GameSystems.AI;
using GameSystems.Bids;

namespace Services
{
    public enum AIStrategyType
    {
        MinPower,
        MaxPower,
        Random
    }

    public class AIManager : MonoBehaviour
    {
        [Header("AI Configuration")] 
        [SerializeField] private AIStrategyType defaultStrategy = AIStrategyType.MinPower;

        [Header("AI Decision Strategies")] 
        [SerializeField] private bool alwaysAcceptEnvido = true;
        [SerializeField] private bool alwaysAcceptTruco = true;
        [SerializeField] private bool shouldCallEnvidoFirstRound = false;

        void Awake()
        {
            RegisterAIStrategy();
            RegisterDecisionStrategies();
        }

        private void RegisterAIStrategy()
        {
            IAIStrategy strategy = defaultStrategy switch
            {
                AIStrategyType.MinPower => new MinPowerStrategy(),
                AIStrategyType.MaxPower => new MaxPowerStrategy(),
                AIStrategyType.Random => new RandomPowerStrategy(),
                _ => new MinPowerStrategy()
            };

            ServiceLocator.Register<IAIStrategy>(strategy);
            Debug.Log($"AI Strategy registered: {strategy.GetType().Name}");
        }

        private void RegisterDecisionStrategies()
        {
            IEnvidoDecisionStrategy envidoStrategy;
            
            if (shouldCallEnvidoFirstRound && alwaysAcceptEnvido)
            {
                envidoStrategy = new AlwaysEnvidoStrategy();
            }
            else if (shouldCallEnvidoFirstRound && !alwaysAcceptEnvido)
            {
                envidoStrategy = new CallButDeclineEnvidoStrategy();
            }
            else if (!shouldCallEnvidoFirstRound && alwaysAcceptEnvido)
            {
                envidoStrategy = new AcceptOnlyEnvidoStrategy();
            }
            else
            {
                envidoStrategy = new NeverEnvidoStrategy();
            }

            ITrucoDecisionStrategy trucoStrategy = alwaysAcceptTruco
                ? new AlwaysAcceptTrucoStrategy()
                : new AlwaysDeclineTrucoStrategy();

            ServiceLocator.Register<IEnvidoDecisionStrategy>(envidoStrategy);
            ServiceLocator.Register<ITrucoDecisionStrategy>(trucoStrategy);

            Debug.Log($"Decision strategies registered - Envido: {envidoStrategy.GetType().Name}, Truco: {trucoStrategy.GetType().Name}");
        }
    }

    public class NeverEnvidoStrategy : IEnvidoDecisionStrategy
    {
        public bool ShouldCallEnvido(TurnManager mgr) => false;
        public bool ShouldAcceptEnvido(Player self, Player opponent) => false;
    }

    public class CallButDeclineEnvidoStrategy : IEnvidoDecisionStrategy
    {
        public bool ShouldCallEnvido(TurnManager mgr) => mgr.bazaCount == 0;
        public bool ShouldAcceptEnvido(Player self, Player opponent) => false;
    }

    public class AcceptOnlyEnvidoStrategy : IEnvidoDecisionStrategy
    {
        public bool ShouldCallEnvido(TurnManager mgr) => false;
        public bool ShouldAcceptEnvido(Player self, Player opponent) => true;
    }

    public class AlwaysDeclineTrucoStrategy : ITrucoDecisionStrategy
    {
        public bool ShouldAcceptTruco(IBid currentBid, Player self, Player opponent) => false;
        public bool ShouldRaiseTruco(IBid currentBid, Player self, Player opponent) => false;
    }
}