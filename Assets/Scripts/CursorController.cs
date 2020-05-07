using UnityEngine;

namespace TheBunniesOfVegetaria
{ 
    [RequireComponent(typeof(SpriteRenderer))]
    public class CursorController : MonoBehaviour
    {
        private Vector2 spriteOffset;
        private Camera mainCamera;

        private void Awake()
        {
            // Set up custom cursor graphic
            Cursor.visible = false;
            Vector2 spriteExtents = GetComponent<SpriteRenderer>().bounds.extents;
            spriteOffset = new Vector2(spriteExtents.x, -spriteExtents.y);
        }

        private void Start()
        {
            mainCamera = Camera.main;
        }

        private void Update()
        {
            // Move with mouse
            transform.position = (Vector2)mainCamera.ScreenToWorldPoint(Input.mousePosition) + spriteOffset;
        }
    }
}
