using System.Collections;
using UnityEngine;

namespace TheBunniesOfVegetaria
{
    public class BattleBackground : MonoBehaviour
    {
        [Tooltip("The background sprites for each area. These should be in ascending order by area.")]
        [SerializeField] private Sprite[] backgrounds;

        public bool IsScrolling { get; private set; } = false;
        public float ScreenWidth => Mathf.Abs(maxPositionX - minPositionX);

        private float minPositionX, maxPositionX;
        private SpriteRenderer[] spriteRenderers;

        private void Awake()
        {
            spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
            minPositionX = spriteRenderers[0].transform.position.x - 4;
            maxPositionX = spriteRenderers[spriteRenderers.Length - 1].transform.position.x;
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
            int tilesScrolled = 0;

            do
            {
                // Move background tiles
                foreach (SpriteRenderer spriteRenderer in spriteRenderers)
                {
                    Transform spriteTransform = spriteRenderer.transform;
                    spriteTransform.Translate(Vector2.left * speed * Time.fixedDeltaTime);

                    if (spriteTransform.position.x <= minPositionX)
                    {
                        // Reposition offscreen tile
                        Vector2 newPosition = spriteTransform.position;
                        newPosition.x = maxPositionX;
                        spriteTransform.position = newPosition;
                        tilesScrolled++;
                    }
                }

                // Move objects
                foreach (Transform movingTransform in movingTransforms)
                { 
                    movingTransform.Translate(Vector2.left * speed * Time.fixedDeltaTime);
                }

                yield return null;
            } while (tilesScrolled < screens * spriteRenderers.Length);

            foreach (SpriteRenderer spriteRenderer in spriteRenderers)
            {
                spriteRenderer.transform.position = RoundX(spriteRenderer.transform.position);
            }

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
            foreach (SpriteRenderer spriteRenderer in spriteRenderers)
            {
                spriteRenderer.sprite = backgrounds[(int)area - 1];
            }
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