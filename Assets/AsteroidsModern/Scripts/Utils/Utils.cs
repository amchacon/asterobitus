using UnityEngine;
using Random = UnityEngine.Random;

namespace AsteroidsModern.Extensions
{
    public static class Utils
    {
        public static bool IsObjectInScreen(this Transform obj, Camera camera, float margin = 0.1f)
        {
            if (camera == null) return false;

            Vector3 viewportPos = camera.WorldToViewportPoint(obj.position);
            return viewportPos.x >= -margin && viewportPos.x <= 1f + margin &&
                   viewportPos.y >= -margin && viewportPos.y <= 1f + margin;
        }

        public static Vector2 GetRandomScreenPosition(Camera camera)
        {
            if (camera == null) return Vector2.zero;

            Vector3 randomViewport = new Vector3(Random.Range(0f, 1f), Random.Range(0f, 1f), 0f);
            return camera.ViewportToWorldPoint(randomViewport);
        }

        public static Vector2 GetRandomEdgePosition(Camera camera)
        {
            if (camera == null) return Vector2.zero;

            int edge = Random.Range(0, 4);

            return edge switch
            {
                0 => camera.ViewportToWorldPoint(new Vector3(Random.Range(0f, 1f), 1.1f, 0)), // Top
                1 => camera.ViewportToWorldPoint(new Vector3(1.1f, Random.Range(0f, 1f), 0)), // Right
                2 => camera.ViewportToWorldPoint(new Vector3(Random.Range(0f, 1f), -0.1f, 0)), // Bottom
                3 => camera.ViewportToWorldPoint(new Vector3(-0.1f, Random.Range(0f, 1f), 0)), // Left
                _ => Vector2.zero
            };
        }

        public static bool HasLineOfSight(Vector2 from, Vector2 to, LayerMask obstacles)
        {
            Vector2 direction = to - from;
            float distance = direction.magnitude;

            RaycastHit2D hit = Physics2D.Raycast(from, direction.normalized, distance, obstacles);
            return hit.collider == null;
        }
    }
}