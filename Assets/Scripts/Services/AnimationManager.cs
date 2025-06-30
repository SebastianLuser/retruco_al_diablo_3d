using System.Collections;
using UnityEngine;

namespace Services
{
    public class AnimationManager : MonoBehaviour, IAnimationService
    {
        [SerializeField] private Animator player;
        [SerializeField] private Animator diablo;
        [SerializeField] private Animator door;
        
        private static readonly int Death = Animator.StringToHash("Death");
        private static readonly int Win = Animator.StringToHash("Win");
        private static readonly int Open = Animator.StringToHash("Open");
        
        private void Awake()
        {
            ServiceLocator.Register<IAnimationService>(this);
        }

        public void PlayDeathAnimation()
        {
            player.enabled = true;
            player.SetBool(Death, true);
            StartCoroutine(TriggerDiabloDeathAfterDelay());
        }

        public void PlayWinAnimation()
        {
            player.enabled = true;
            player.SetBool(Win, true);

            StartCoroutine(OpenDoorAfterDelay());
        }

        private IEnumerator OpenDoorAfterDelay()
        {
            yield return new WaitForSeconds(4f);
            door.SetBool(Open, true);
        }

        private IEnumerator TriggerDiabloDeathAfterDelay()
        {
            yield return new WaitForSeconds(4.3f);
            diablo.SetBool(Death, true);
        }
    }

    public interface IAnimationService
    {
        void PlayDeathAnimation();
        void PlayWinAnimation();
    }
}