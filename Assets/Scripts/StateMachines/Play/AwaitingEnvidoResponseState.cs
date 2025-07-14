using Components.Cards;
using UnityEngine;
using Services;
using GameSystems;
using GameSystems.Bids;
using GameSystems.Dialogs;
using StateMachines.Envido;
using StateMachines.Main;

namespace StateMachines.Play
{
    public class AwaitingEnvidoResponseState : IState
    {
        private TurnManager mgr;
        private BidType currentBidType;
        private EnvidoManager envidoManager;
        private IResponseService responseService;
        private int callerPlayerId;
        private int responderPlayerId;
        private bool isFromRaise;
        private bool isAITurn;

        public AwaitingEnvidoResponseState(TurnManager mgr, BidType bidType, EnvidoManager envidoManager)
        {
            this.mgr = mgr;
            this.currentBidType = bidType;
            this.envidoManager = envidoManager;
            this.callerPlayerId = mgr.activePlayer;
            this.responderPlayerId = 1 - mgr.activePlayer;
            this.isFromRaise = false;
        }

        public AwaitingEnvidoResponseState(TurnManager mgr, BidType bidType, EnvidoManager envidoManager,
            bool fromRaise)
        {
            this.mgr = mgr;
            this.currentBidType = bidType;
            this.envidoManager = envidoManager;
            this.callerPlayerId = mgr.activePlayer;
            this.responderPlayerId = 1 - mgr.activePlayer;
            this.isFromRaise = fromRaise;
        }

        public void Enter()
        {
            Debug.Log($"⏳ AWAITING: {(responderPlayerId == 0 ? "Player" : "AI")} must respond to {currentBidType}");
            Debug.Log($"💰 Points accumulated so far: {envidoManager.GetAccumulatedPoints()}");
            Debug.Log($"🔄 Is from raise: {isFromRaise}");

            CardClick.enableClicks = false;
            mgr.bloqueadoPorCanto = true;

            if (responderPlayerId == 1)
            {
                Debug.Log("Here?");
                isAITurn = true;
                HandleAIResponse();
            }
            else
            {
                Debug.Log("or Here?");
                isAITurn = false;
                ShowPlayerResponseUI();
            }
        }

        private void HandleAIResponse()
        {
            bool shouldAccept = mgr.envidoStrategy?.ShouldAcceptEnvido(mgr.Opponent, mgr.Player) ?? true;

            Debug.Log($"🤖 BID RESPONSE: AI decides {(shouldAccept ? "QUIERO" : "NO QUIERO")} for {currentBidType}");

            if (shouldAccept)
            {
                if (EnvidoCalculator.CalculateEnvido(mgr.Opponent.hand) > 30 && currentBidType != BidType.FaltaEnvido)
                {
                    OnRaiseEnvido();
                }

                OnAcceptEnvido();
            }
            else
            {
                OnDeclineEnvido();
            }
        }

        private void ShowPlayerResponseUI()
        {
            var actions = new ResponseActions(
                onAccept: OnAcceptEnvido,
                onDecline: OnDeclineEnvido,
                onRaise: CanRaise() ? OnRaiseEnvido : null
            );

            try
            {
                responseService = ServiceLocator.Get<IResponseService>();
                responseService.ShowBidResponse(actions, GetBidDisplayName());
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Error ResponseService: {ex.Message}");
            }
        }

        public void Update()
        {
        }

        public void Exit()
        {
            responseService?.HideBidResponse();
        }

        #region Response Logic

        private bool CanRaise()
        {
            return GetNextBidType() != null;
        }

        private string GetBidDisplayName()
        {
            string baseName = currentBidType.ToString();
            if (envidoManager.HasBids())
            {
                return $"{envidoManager.GetBidsDescription()} + {baseName}";
            }

            return baseName;
        }

        private void OnAcceptEnvido()
        {
            Debug.Log($"✅ BID ACCEPTED: {currentBidType} ACCEPTED");

            if (isFromRaise)
            {
                envidoManager.AddBid(currentBidType);
                Debug.Log($"⬆️ RAISE ACCEPTED: Added {currentBidType} to sequence");
            }
            else
            {
                Debug.Log($"✅ INITIAL BID ACCEPTED: {currentBidType} was already added when called");
            }

            Debug.Log($"💰 Total accumulated: {envidoManager.GetAccumulatedPoints()} points");

            if (isAITurn)
                DialogueManager.Instance.EnqueueDialogue(new DialogueEntry()
                {
                    autoPass = true,
                    dialogueText = "Quiero carajo!",
                    duration = 2f,
                    speaker = "Diablo"
                });

            mgr.bloqueadoPorCanto = false;
            mgr.TransitionToEnvidoState(envidoManager);
        }

        private void OnDeclineEnvido()
        {
            Debug.Log($"❌ BID DECLINED: {currentBidType} DECLINED");

            int accumulatedPoints = envidoManager.GetAccumulatedPoints();

            if (callerPlayerId == 0)
            {
                mgr.GameService.PlayerWinsEnvidoPoints(accumulatedPoints +
                                                       (PassiveManager.Instance.IsExtraEnvidoPointsPassive ? 1 : 0));
                Debug.Log($"🏆 ENVIDO POINTS: Player wins {accumulatedPoints} points (AI declined {currentBidType})");
            }
            else
            {
                mgr.GameService.OpponentWinsEnvidoPoints(accumulatedPoints);
                Debug.Log(
                    $"💀 ENVIDO POINTS: Opponent wins {accumulatedPoints} points (Player declined {currentBidType})");
            }

            Debug.Log($"📊 Points breakdown: {accumulatedPoints} (sequence: {envidoManager.GetBidsDescription()})");

            mgr.MarcarEnvidoComoCantado();
            mgr.bloqueadoPorCanto = false;
            mgr.ChangeState(new PlayState(mgr, mgr.activePlayer, true));

            if (isAITurn)
                DialogueManager.Instance.EnqueueDialogue(new DialogueEntry()
                {
                    autoPass = true,
                    dialogueText = "No quiero",
                    duration = 2f,
                    speaker = "Diablo"
                });
        }

        private void OnRaiseEnvido()
        {
            BidType? nextBid = GetNextBidType();

            if (nextBid == null)
            {
                Debug.LogWarning("Cannot raise further - accepting automatically");
                OnAcceptEnvido();
                return;
            }

            Debug.Log($"⬆️ BID RAISED: {currentBidType} → {nextBid}");

            envidoManager.AddBid(currentBidType);

            Debug.Log($"💰 Points accumulated: {envidoManager.GetAccumulatedPoints()}");
            Debug.Log($"📋 Sequence: {envidoManager.GetBidsDescription()} + {nextBid}");

            mgr.activePlayer = responderPlayerId;

            if (isAITurn)
                DialogueManager.Instance.EnqueueDialogue(new DialogueEntry()
                {
                    autoPass = true,
                    dialogueText = $"Quiero {nextBid}!",
                    duration = 2f,
                    speaker = "Diablo"
                });

            mgr.ChangeState(new AwaitingEnvidoResponseState(mgr, nextBid.Value, envidoManager, true));
        }

        #endregion

        #region Bid Logic

        private BidType? GetNextBidType()
        {
            if (currentBidType == BidType.Envido)
            {
                var existingBids = envidoManager.GetCalledBids();
                int envidoCount = 0;
                foreach (var bid in existingBids)
                {
                    if (bid == BidType.Envido) envidoCount++;
                }

                if (envidoCount < 2)
                {
                    return BidType.Envido;
                }
                else
                {
                    return BidType.RealEnvido;
                }
            }

            switch (currentBidType)
            {
                case BidType.RealEnvido:
                    return BidType.FaltaEnvido;

                case BidType.FaltaEnvido:
                    return null;

                default:
                    Debug.LogWarning($"Unknown BidType: {currentBidType}");
                    return null;
            }
        }

        #endregion
    }
}