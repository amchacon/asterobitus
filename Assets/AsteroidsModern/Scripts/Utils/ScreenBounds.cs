using AsteroidsModern.Core;
using UnityEngine;

namespace AsteroidsModern.Extensions
{
    public class ScreenBounds : MonoBehaviour
    {
        [SerializeField] private bool destroyOutside = false;
        
        private Rigidbody2D rb;
        private Camera cam;
        private float halfWidth;
        private float halfHeight;
        private float pad;

        void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            cam = Camera.main;

            halfHeight = cam.orthographicSize;
            halfWidth = halfHeight * cam.aspect;

            var sr = GetComponent<SpriteRenderer>();
            if (sr != null)
                pad = sr.bounds.extents.magnitude * 0.5f;
            else
                pad = 0.2f;
        }

        void FixedUpdate()
        {
            if (destroyOutside)
            {
                CheckDestroyOutside();
            }
            else
            {
                WrapPosition();
            }
        }

        private void CheckDestroyOutside()
        {
            Vector2 pos = rb.position;

            if (pos.x > halfWidth + pad || pos.x < -halfWidth - pad ||
                pos.y > halfHeight + pad || pos.y < -halfHeight - pad)
            {
                PoolManager.Instance.ReturnToPool(gameObject);
            }
        }

        private void WrapPosition()
        {
            Vector2 pos = rb.position;

            if (pos.x > halfWidth + pad)
                pos.x = -halfWidth - pad;
            else if (pos.x < -halfWidth - pad)
                pos.x = halfWidth + pad;

            if (pos.y > halfHeight + pad)
                pos.y = -halfHeight - pad;
            else if (pos.y < -halfHeight - pad)
                pos.y = halfHeight + pad;

            
            rb.position = pos;
        }
    }
}