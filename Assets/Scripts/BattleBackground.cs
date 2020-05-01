using System.Collections;
using UnityEngine;

namespace TheBunniesOfVegetaria
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class BattleBackground : MonoBehaviour
    {
        [Tooltip("The background sprites for each area. These should be in ascending order by area.")]
        [SerializeField] private Sprite[] backgrounds;

        public bool IsScrolling { get; private set; } = false;
        public float ScreenWidth => spriteWidth / 2f;

        private float spriteWidth;
        private Vector2 startPosition;
        private SpriteRenderer spriteRenderer;

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            spriteWidth = spriteRenderer.bounds.size.x;
            startPosition = Vector2.right * spriteWidth / 4f;
            transform.position = startPosition;
        }

        private void Start()
        {
            SetBackground(GameManager.Instance.BattleArea);
        }

        /// <summary>
        /// Scroll the background horizontally to the left by a certain number of screens.
        /// </summary>
        /// <param name="screens">The number of screens to scroll left.</param>
        /// <param name="speed">The speed at which to scroll the background.</param>
        /// <param name="movingTransforms">An array of the transform components for objects that should move with the background.</param>
        public IEnumerator ScrollBackground(int screens, float speed, Transform[] movingTransforms)
        {
            if (screens <= 0)
                yield break;

            IsScrolling = true;
            int screensScrolled = 0;

            do
            {
                // Move background
                transform.Translate(Vector2.left * speed * Time.deltaTime);
                if (transform.position.x < -startPosition.x)
                {
                    Vector2 newPosition = startPosition;
                    newPosition.x -= (-startPosition.x - transform.position.x);
                    transform.position = newPosition;
                    screensScrolled++;
                }

                // Move objects with background
                foreach (Transform movingTransform in movingTransforms)
                { 
                    movingTransform.Translate(Vector2.left * speed * Time.deltaTime);
                }

                yield return null;
            } while (screensScrolled < screens);

            transform.position = RoundX(spriteRenderer.transform.position);

            foreach (Transform movingTransform in movingTransforms)
            {
                movingTransform.position = RoundX(movingTransform.position);
            }

            IsScrolling = false;
        }

        /// <summary>
        /// Set the battle background.
        /// </summary>
        /// <param name="area">The area to set as the background.</param>
        private void SetBackground(Globals.Area area)
        {
            spriteRenderer.sprite = backgrounds[(int)area - 1];
        }

        /// <summary>
        /// Round a 2D vector's X component to the nearest integer.
        /// </summary>
        /// <param name="vector">The 2D vector to round.</param>
        /// <returns>A 2D vector with the X component rounded to an integer.</returns>
        private Vector2 RoundX(Vector2 vector)
        {
            vector.x = Mathf.Round(vector.x);
            return vector;
        }
    }
}