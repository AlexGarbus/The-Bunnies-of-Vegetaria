using UnityEngine;

namespace TheBunniesOfVegetaria
{
    [RequireComponent(typeof(Animator))]
    public class BattleEffect : MonoBehaviour
    {
        private Animator animator;

        private void Start()
        {
            animator = GetComponent<Animator>();
        }

        public void PlaySlash()
        {
            animator.SetTrigger("Slash");
        }

        public void PlayHeal()
        {
            animator.SetTrigger("Heal");
        }
    }
}