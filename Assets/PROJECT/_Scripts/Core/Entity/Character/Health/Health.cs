using System;
using UnityEngine;

public class Health : MonoBehaviour, IDamageable
{
    [SerializeField] private int _maxHealth = 100;

    [field: SerializeField] public int CurrentHealth { get; private set; }
    public int MaxHealth => _maxHealth;
    public bool IsDead => CurrentHealth <= 0;

    public event Action<int> OnDamagedEvent;
    public event Action<int> OnHealedEvent;
    public event Action OnDiedEvent;

    private void Awake()
    {
        CurrentHealth = _maxHealth;
    }

    public void TakeDamage(int amount)
    {
        if (IsDead) return;

        CurrentHealth -= amount;
        OnDamagedEvent?.Invoke(amount);

        if (CurrentHealth <= 0)
        {
            CurrentHealth = 0;
            OnDiedEvent?.Invoke();
        }
    }

    public void Heal(int amount)
    {
        if (IsDead) return;

        CurrentHealth = Mathf.Min(CurrentHealth + amount, _maxHealth);
        OnHealedEvent?.Invoke(amount);
    }
}
