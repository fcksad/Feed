public interface IDamageable
{
    void TakeDamage(int amount);
    void Heal(int amount);
    int CurrentHealth { get; }
    int MaxHealth { get; }
    bool IsDead { get; }
}