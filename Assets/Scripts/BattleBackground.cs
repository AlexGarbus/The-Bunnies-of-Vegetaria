using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TheBunniesOfVegetaria
{
    public class BattleBackground : MonoBehaviour
    {
        private float minPositionX, maxPositionX;
        private SpriteRenderer[] spriteRenderers;

        private void Awake()
        {
            spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
            minPositionX = spriteRenderers[0].transform.position.x - 4;
            maxPositionX = spriteRenderers[spriteRenderers.Length - 1].transform.position.x;
        }

        /// <summary>
        /// Set the battle background.
        /// </summary>
        /// <param name="backgroundTile">The tile to use for the battle background. Should be 64 pixels (4 world units) long.</param>
        public void SetBackground(Sprite backgroundTile)
        {
            foreach (SpriteRenderer spriteRenderer in spriteRenderers)
                spriteRenderer.sprite = backgroundTile;
        }

        /// <summary>
        /// Scroll the background horizontally by a certain number of screens.
        /// </summary>
        /// <param name="screens">The number of screens to scroll.</param>
        /// <param name="speed">The speed at which to scroll the background.</param>
        public IEnumerator ScrollBackground(int screens, float speed)
        {
            int tilesScrolled = 0;

            do
            {
                foreach (SpriteRenderer spriteRenderer in spriteRenderers)
                {
                    Transform spriteTransform = spriteRenderer.transform;
                    spriteTransform.Translate(Vector2.left * speed * Time.fixedDeltaTime);

                    if (spriteTransform.position.x <= minPositionX)
                    {
                        Vector2 newPosition = spriteTransform.position;
                        newPosition.x = maxPositionX;
                        spriteTransform.position = newPosition;
                        tilesScrolled++;
                    }
                }

                yield return null;
            } while (tilesScrolled < screens * spriteRenderers.Length);
        }
    }
}