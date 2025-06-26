using Match.Bids;
using UnityEngine;


namespace MCTS
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;

        public TrucoGameState gameState;

        public int[] aiHand;
        public int[] playerHand;
        
        private void Awake()
        {
            Instance = this;
            gameState = new TrucoGameState(aiHand, new BidFactory());
        }

        public void MakeAIAction()
        {
            
        }

        public void MakePlayerAction()
        {
            
        }
    }
}