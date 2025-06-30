using UnityEngine;
using Components.Cards;
using GameSystems.Bids;
using TMPro;

namespace Services
{
    public class GameManager : MonoBehaviour, IGameService
    {
        [SerializeField] private TMP_Text opponentPointTxt;
        [SerializeField] private TMP_Text playerPointTxt;
        [SerializeField] private GameObject hud;
        [SerializeField] private GameObject playerHand;
        [SerializeField] private GameObject opponentHand;

        public int gameTarget = 15;
        private int currentTrucoPoints = 1;

        private IPointSystem pointSystem;

        void Awake()
        {
            ServiceLocator.Register<IGameService>(this);
        }

        void Start()
        {
            pointSystem = ServiceLocator.Get<IPointSystem>();
            pointSystem.InitializePoints();
            pointSystem.OnPointsUpdated += UpdatePointUI;
        }

        public void PlayerWinsEnvidoPoints(int pts)
        {
            pointSystem.PlayerWinsPoints(pts);
        }

        public void OpponentWinsEnvidoPoints(int pts)
        {
            pointSystem.OpponentWinsPoints(pts);
        }

        public int EvaluateRound(Card playerCard, Card opponentCard, bool playerIsHand)
        {
            Debug.Log($"Evaluando: Jugador={playerCard}, IA={opponentCard}");

            int power1 = playerCard.GetPower();
            int power2 = opponentCard.GetPower();

            if (power1 == power2)
            {
                Debug.Log($"Empate - Gana {(playerIsHand ? "Jugador (es mano)" : "IA (es mano)")}");
                return playerIsHand ? 0 : 1;
            }

            if (power1 > power2)
            {
                Debug.Log("Gana Jugador");
                return 0;
            }

            Debug.Log("Gana IA");
            return 1;
        }

        public void ResolveHand(int handWinner)
        {
            if (pointSystem == null)
            {
                Debug.LogError("[GameManager] PointSystem is null in ResolveHand");
                return;
            }

            int trucoPoints = GetCurrentTrucoPoints();

            if (handWinner == 0)
            {
                pointSystem.PlayerWinsPoints(trucoPoints);
                Debug.Log($"✌️ El Jugador gana la mano. +{trucoPoints} punto(s).");
            }
            else
            {
                pointSystem.OpponentWinsPoints(trucoPoints);
                Debug.Log($"🤖 La IA gana la mano. +{trucoPoints} punto(s).");
            }

            ResetTrucoPoints();
        }

        private void UpdatePointUI(int playerPoints, int opponentPoints)
        {
            playerPointTxt.text = playerPoints.ToString();
            opponentPointTxt.text = opponentPoints.ToString();
        }

        public void AcceptTruco(IBid bid)
        {
            currentTrucoPoints = bid.PointValue;
            Debug.Log($"✅ Truco aceptado. Valor actual: {currentTrucoPoints} puntos.");
        }

        public void DeclineTruco(IBid bid, int callerPlayerId)
        {
            Debug.Log($"❌ Truco rechazado. {callerPlayerId} gana 1 punto.");

            if (callerPlayerId == 0)
                pointSystem.PlayerWinsPoints(1);
            else
                pointSystem.OpponentWinsPoints(1);

            currentTrucoPoints = 1;
        }

        public int GetCurrentTrucoPoints() => currentTrucoPoints;

        public void ResetTrucoPoints()
        {
            currentTrucoPoints = 1;
        }
    }

    public interface IGameService
    {
        int EvaluateRound(Card p, Card o, bool hand);
        void ResolveHand(int handWinner);
        void PlayerWinsEnvidoPoints(int pts);
        void OpponentWinsEnvidoPoints(int pts);
        void AcceptTruco(IBid bid);
        void DeclineTruco(IBid bid, int callerPlayerId);
    }
}