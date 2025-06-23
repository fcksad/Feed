using System;
using UnityEngine;

public class Health : IDamageable
{
    private int _maxHealth = 100;
    private int _currentHealth;
    private bool _isDead => _currentHealth <= 0;

    public event Action<int> OnDamagedEvent;
    public event Action<int> OnHealedEvent;
    public event Action OnDiedEvent;

    public Health()
    {
        _currentHealth = _maxHealth;
    }

    public Health(int currentHealth)
    {
        _currentHealth = currentHealth;
    }

    public void TakeDamage(int amount)
    {
        if (_isDead) return;

        _currentHealth -= amount;
        OnDamagedEvent?.Invoke(amount);

        if (_currentHealth <= 0)
        {
            _currentHealth = 0;
            OnDiedEvent?.Invoke();
        }
    }

    public void Heal(int amount)
    {
        if (_isDead) return;

        _currentHealth = Mathf.Min(_currentHealth + amount, _maxHealth);
        OnHealedEvent?.Invoke(amount);
    }
}
