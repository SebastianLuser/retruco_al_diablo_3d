using System;
using System.Collections;
using System.Linq;
using Components;
using Components.Cards;
using GameSystems;
using GameSystems.AI;
using GameSystems.Bids;
using StateMachines;
using StateMachines.Main;
using StateMachines.Play;
using UnityEngine;

namespace Services
{
    public class TurnManager : MonoBehaviour
    {
        private IDeckService deckSvc;
        private IPlacementService placeSvc;
        private IGameService gameSvc;
        private IAIStrategy aiStrategy;
        private StateMachine fsm;
        public static TurnManager Instance { get; private set; }

        [Header("Game Settings")] public float opponentPlayDelay = 1.0f;
        public int totalBazas = 3;
        public bool isDiceSelected = false;

        [Header("Game State")] public int bazaCount = 0;
        public bool playerIsHand = true;
        public int playerBazaWins = 0;
        public int opponentBazaWins = 0;
        public int activePlayer = 0;
        public bool playerMoveDone = false;
        public int actualRound = 0;

        [Header("Bid State")] public bool EnvidoCantado { get; private set; } = false;
        public bool TrucoCantado { get; set; } = false;
        public bool bloqueadoPorCanto = false;

        [Header("Cards Played")] public Card lastPlayedCardPlayer = null;
        public Card lastPlayedCardOpponent = null;

        [Header("Envido System")] private EnvidoManager currentEnvidoManager;

        // AI Strategies
        public IEnvidoDecisionStrategy envidoStrategy { get; private set; }
        public ITrucoDecisionStrategy trucoStrategy { get; private set; }

        public IDeckService DeckService => deckSvc;
        public IPlacementService PlacementService => placeSvc;
        public IGameService GameService => gameSvc;

        public Player Player => deckSvc?.Player;
        public Player Opponent => deckSvc?.Opponent;

        public bool isUsingActives;

        #region Unity Lifecycle

        void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        void Start()
        {
            StartCoroutine(InitializeGameDelayed());
        }

        void Update()
        {
            fsm?.Update();
        }

        #endregion

        #region Initialization

        private IEnumerator InitializeGameDelayed()
        {
            while (!PassiveManager.Instance.IsPassiveSelected)
            {
                yield return null;
            }

            InitializeServices();
            InitializeAIStrategies();

            gameSvc = ServiceLocator.Get<IGameService>();
            fsm = new StateMachine();

            Debug.Log("🎮 TurnManager initialized - Starting game");
            ChangeState(new DealHandState(this));
        }

        private void InitializeServices()
        {
            try
            {
                ServiceLocator.Register<IBidFactory>(new BidFactory());
                deckSvc = ServiceLocator.Get<IDeckService>();
                placeSvc = ServiceLocator.Get<IPlacementService>();

                aiStrategy = ServiceLocator.Get<IAIStrategy>();
                Debug.Log("All services initialized successfully");
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"❌ Error inicializando servicios: {ex.Message}");
            }
        }

        private void InitializeAIStrategies()
        {
            try
            {
                envidoStrategy = ServiceLocator.Get<IEnvidoDecisionStrategy>();
                trucoStrategy = ServiceLocator.Get<ITrucoDecisionStrategy>();
                Debug.Log("Using AI strategies from AIManager");
            }
            catch
            {
                Debug.LogWarning("AIManager strategies not found, using fallback");
                SetEnvidoStrategy(new AlwaysEnvidoStrategy());
                SetTrucoStrategy(new AlwaysAcceptTrucoStrategy());
            }

            Debug.Log("AI strategies initialized");
        }

        #endregion

        #region Main State Transitions

        public void TransitionToEnvidoState(EnvidoManager envidoManager)
        {
            Debug.Log("🎯 TRANSICIÓN: PlayState → EnvidoState");

            currentEnvidoManager = envidoManager;
            bloqueadoPorCanto = true;

            ChangeState(new EnvidoState(this, envidoManager));
        }

        public void TransitionToPlayState()
        {
            Debug.Log("🎮 TRANSICIÓN: EnvidoState → PlayState");

            currentEnvidoManager?.Reset();
            bloqueadoPorCanto = false;

            EnvidoEvents.RequestCleanup();
            CardPositionEvents.RequestPositionReset();

            Debug.Log("🧹 Event-based cleanup completed");
            ChangeState(new PlayState(this, activePlayer, true));
        }

        public void TransitionToGameOver()
        {
            Debug.Log("💀 TRANSICIÓN: → GameOverState");

            ChangeState(new GameOverState(this));
        }

        #endregion

        #region Sub-State Management

        public void ChangeState(IState newState)
        {
            fsm?.ChangeState(newState);
        }

        #endregion

        #region Envido Management

        public EnvidoManager GetOrCreateEnvidoManager()
        {
            if (currentEnvidoManager == null)
                currentEnvidoManager = new EnvidoManager();

            return currentEnvidoManager;
        }

        public void MarcarEnvidoComoCantado()
        {
            EnvidoCantado = true;
            Debug.Log("🎯 Envido marcado como cantado para esta ronda");
        }

        public void ResetEnvidoFlags()
        {
            EnvidoCantado = false;
            TrucoCantado = false;
            bloqueadoPorCanto = false;

            currentEnvidoManager?.Reset();

            Debug.Log("🔄 Flags de Envido y Truco reseteados");
        }

        #endregion

        #region Card Playing

        public void OnPlayerCardPlayed(Card card, GameObject cardObject)
        {
            if (bloqueadoPorCanto)
            {
                Debug.Log("❌ Click bloqueado: hay un canto activo");
                return;
            }

            if (activePlayer != 0)
            {
                Debug.Log("❌ No es turno del jugador");
                return;
            }

            Debug.Log($"✅ Jugador juega: {card}");
            lastPlayedCardPlayer = card;
            Player.PlayCard(card);
            placeSvc.PlacePlayerCard(cardObject);
            playerMoveDone = true;
        }

        #endregion

        #region AI Management

        public void SetAIStrategy(IAIStrategy newStrategy)
        {
            aiStrategy = newStrategy;
            ServiceLocator.Register(newStrategy);
            Debug.Log($"🧠 Estrategia de IA: {newStrategy.GetType().Name}");
        }

        public void SetEnvidoStrategy(IEnvidoDecisionStrategy strat)
        {
            envidoStrategy = strat;
        }

        public void SetTrucoStrategy(ITrucoDecisionStrategy strat)
        {
            trucoStrategy = strat;
        }

        public void ExecuteAITurn()
        {
            if (bloqueadoPorCanto)
            {
                Debug.Log("❌ Turno de IA bloqueado por canto pendiente");
                return;
            }

            if (activePlayer != 1)
            {
                Debug.Log("❌ No es turno de la IA");
                return;
            }

            Card aiCard = aiStrategy.ChooseCard(Opponent);
            if (aiCard == null)
            {
                Debug.LogWarning("❌ La IA no tiene carta para jugar");
                TransitionToGameOver();
                return;
            }

            GameObject targetGO = FindAICardObject(aiCard);

            if (targetGO != null)
            {
                lastPlayedCardOpponent = aiCard;
                Opponent.PlayCard(aiCard);
                placeSvc.PlaceOpponentCard(targetGO);
                Debug.Log($"🤖 IA juega: {aiCard}");
                playerMoveDone = true;
            }
            else
            {
                Debug.LogWarning("❌ No se encontró el objeto visual de la carta de la IA");
            }
        }

        private GameObject FindAICardObject(Card targetCard)
        {
            foreach (Transform child in deckSvc.OpponentHandParent)
            {
                CardClick cc = child.GetComponent<CardClick>();
                if (cc != null && cc.card != null &&
                    cc.card.suit == targetCard.suit && cc.card.rank == targetCard.rank)
                {
                    return child.gameObject;
                }
            }

            return null;
        }

        public void AITurn()
        {
            StartCoroutine(DelayedAITurn());
        }

        private System.Collections.IEnumerator DelayedAITurn()
        {
            yield return new WaitForSeconds(opponentPlayDelay);
            ExecuteAITurn();
        }

        #endregion

        #region Input Management

        public void EnablePlayerInput(bool enable)
        {
            if (bloqueadoPorCanto)
            {
                Debug.Log("⛔ Input bloqueado por canto pendiente");
                CardClick.enableClicks = false;
                return;
            }

            Debug.Log($"📲 Input del jugador: {(enable ? "HABILITADO" : "DESHABILITADO")}");
            CardClick.enableClicks = enable;
        }

        #endregion
    }
}