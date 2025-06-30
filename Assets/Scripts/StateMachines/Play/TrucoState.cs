using System;
using System.Collections.Generic;
using GameSystems.Bids;
using Services;
using StateMachines.Main;
using UnityEngine;

namespace StateMachines.Play
{
    public class TrucoState : IState
    {
        readonly TurnManager   mgr;
        readonly IBid          currentBid;
        readonly int           returnToPlayer;
        readonly int           responder;

        IUIManager             ui;
        IBidFactory            bidFactory;
        BidValidator           bidValidator;

        public TrucoState(TurnManager mgr, IBid bid)
        {
            this.mgr           = mgr;
            currentBid         = bid;
            returnToPlayer     = mgr.activePlayer;
            responder          = 1 - mgr.activePlayer;
        }

        public void Enter()
        {
            mgr.bloqueadoPorCanto = true;
            ui         = ServiceLocator.Get<IUIManager>();
            bidFactory = ServiceLocator.Get<IBidFactory>();
            bidValidator = new BidValidator();
            bidValidator.AcceptBid(currentBid);

            if (responder == 1 && mgr.activePlayer == 0)
            {
                bool quiso = TurnManager.Instance.trucoStrategy?.ShouldAcceptTruco(currentBid, mgr.Player, mgr.Opponent) ?? true;
                Debug.Log($"🤖 IA Truco: {(quiso ? "QUIERO" : "NO QUIERO")}");
                OnResponse(quiso);
                return;
            }

            ui.OnAcceptDeclineResponse -= OnResponse;
            ui.OnAcceptDeclineResponse += OnResponse;
            ui.ShowAcceptDeclinePanel(OnResponse);

            ui.OnBidSelected -= HandleCounterBid;
            ui.OnBidSelected += HandleCounterBid;

            var tipos = new[] { BidType.Truco, BidType.ReTruco, BidType.ValeCuatro };
            var list  = new List<BidType>();
            foreach (var t in tipos)
                if (bidValidator.CanBid(bidFactory.CreateBid(t), mgr))
                    list.Add(t);

            ui.ShowTrucoOptions(list);
        }

        public void Update() { }

        public void Exit()
        {
            ui.HideAcceptDeclinePanel();
            ui.HideTrucoOptions();
            ui.ClearAllListeners();
        }

        void HandleCounterBid(BidType type)
        {
            var bid = bidFactory.CreateBid(type);
            if (!bidValidator.CanBid(bid, mgr)) return;

            Debug.Log($"🗣️ Se sube Truco: {bid.Name}");

            bidValidator.AcceptBid(bid);
            ui.HideTrucoOptions();
            ui.HideAcceptDeclinePanel();
            ui.ClearAllListeners();
            mgr.ChangeState(new TrucoState(mgr, bid));
        }

        void OnResponse(bool quiso)
        {
            ui.OnAcceptDeclineResponse -= OnResponse;

            if (quiso)
            {
                mgr.GameService.AcceptTruco(currentBid);
                mgr.bloqueadoPorCanto = false;
                mgr.ChangeState(new PlayState(mgr, returnToPlayer, true));
            }
            else
            {
                mgr.GameService.DeclineTruco(currentBid, returnToPlayer);
                mgr.bloqueadoPorCanto = false;
                mgr.ChangeState(new DealHandState(mgr));
            }
        }
    }
}
