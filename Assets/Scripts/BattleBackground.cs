using System.Collections;
using UnityEngine;
using UnityEngine.U2D;

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
        private PixelPerfectCamera pixelPerfectCamera;
        private SpriteRenderer spriteRenderer;

        private void Awake()
        {
            pixelPerfectCamera = Camera.main.GetComponent<PixelPerfectCamera>();

            // Get sprite renderer values
            spriteRenderer = GetComponent<SpriteRenderer>();
            spriteWidth = spriteRenderer.bounds.size.x;

            // Set start position so that the background extends to the right
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
        /// <param name="movingTransforms">An array of transforms that should move with the background.</param>
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

                // Loop background when it passes the start position
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

            // Make sure background is pixel perfect
            transform.position = pixelPerfectCamera.RoundToPixel(transform.position);

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
    }
}