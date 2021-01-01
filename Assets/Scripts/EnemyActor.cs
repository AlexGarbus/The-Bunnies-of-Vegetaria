using System.Collections;
using UnityEngine;

namespace TheBunniesOfVegetaria
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class EnemyActor : Actor<Enemy>
    {
        [SerializeField] private float deathTime;
        [SerializeField] private int deathFrames;

        protected override Vector2 StepDirection { get => Vector2.left; }

        private SpriteRenderer spriteRenderer;

        protected override void Initialize()
        {
            // Set sprite
            spriteRenderer.sprite = Resources.Load<Sprite>($"Sprites/Enemies/{Fighter.spriteFileName}");
            spriteRenderer.enabled = true;
        }

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        public override void Defeat()
        {
            base.Defeat();
            StartCoroutine(FadeOut());
        }

        /// <summary>
        /// Fade the enemy's sprite to transparent, and then hide the enemy.
        /// </summary>
        private IEnumerator FadeOut()
        {
            int framesComplete = 0;
            float waitTime = deathTime / deathFrames;

            // Handle fade
            while (framesComplete < deathFrames)
            {
                framesComplete++;

                spriteRenderer.color = new Color(1, 1, 1, 1f - (float)framesComplete / deathFrames);

                yield return new WaitForSeconds(waitTime);
            }

            // Hide enemy
            spriteRenderer.color = new Color(1, 1, 1, 1);
            spriteRenderer.enabled = false;
        }
    }
}