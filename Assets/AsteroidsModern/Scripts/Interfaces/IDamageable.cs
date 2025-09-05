namespace AsteroidsModern.Interfaces
{
    public interface IDamageable
    {
        void TakeDamage(int damage);
        bool IsDestroyed { get; }
        void InstantDestroy();
    }
}