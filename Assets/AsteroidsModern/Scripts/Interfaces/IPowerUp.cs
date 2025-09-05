using AsteroidsModern.Enums;
using UnityEngine;

namespace AsteroidsModern.Interfaces
{
    //TODO: Implement PowerUp system
    public interface IPowerUp
    {
        PowerUpType Type { get; }
        float Duration { get; }
        void ApplyEffect(GameObject target);
        void RemoveEffect(GameObject target);
    }
}