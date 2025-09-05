using UnityEngine;

namespace AsteroidsModern.Interfaces
{
    public interface IVisualEffect
    {
        void PlayEffect(Vector2 position, float duration = 1f);
        void StopEffect();
    }
}