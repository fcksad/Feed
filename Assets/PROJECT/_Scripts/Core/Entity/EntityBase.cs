using System.Collections.Generic;
using UnityEngine;

public abstract class EntityBase : MonoBehaviour, IDamageable
{
    [Header("Model")]
    [field: SerializeField] public Transform CharacterModel { get; private set; }

    [Header("Health")]
    //health config
    protected Health Health;


    protected IAudioService _audioService;

    protected virtual void Awake()
    {
        Health = new Health();
    }

    protected virtual void Start()
    {
        Health.OnDiedEvent += Died;
    }

    protected virtual void OnDestroy()
    {
        Health.OnDiedEvent -= Died;
    }


    public virtual void Heal(int amount) => Health.Heal(amount);
    public virtual void TakeDamage(int amount) => Health.TakeDamage(amount);
    protected virtual void Died()
    {
        Destroy(gameObject);
        Debug.LogError("Died");
    }

}
