using System.Collections;
using UnityEngine;

public class MeleeWeapon : UsableObject
{
    [SerializeField] private int _damage = 10;
    [SerializeField] private float _attackRange = 1.4f;
    [SerializeField] private float _attackRadius = 0.5f;
    [SerializeField] private float _attackDelay = 0.8f;
    [SerializeField] private float _attackCooldown = 1.2f;
    [SerializeField] private LayerMask _hitMask;
    private bool _isAttacking;  

    public override void Use()
    {
        if (_isAttacking) return;
        base.Use();
        StartCoroutine(AttackCoroutine());

    }

    private IEnumerator AttackCoroutine()
    {
        _isAttacking = true;

        yield return new WaitForSeconds(_attackDelay);

        Vector3 origin = _camera.transform.position;
        Vector3 direction = _camera.transform.forward;
        Vector3 sphereCenter = origin + direction * _attackRange * 0.5f;

        Collider[] hits = Physics.OverlapSphere(sphereCenter, _attackRadius, _hitMask);

        foreach (var hit in hits)
        {
            if (hit.TryGetComponent<IDamageable>(out var target))
            {
                target.TakeDamage(_damage);
                Debug.Log($"Hit {hit.name} for {_damage} damage.");
                break;
            }
        }

        Debug.DrawRay(origin, direction * _attackRange, Color.red, 1f);

        yield return new WaitForSeconds(_attackCooldown);

        _isAttacking = false;
    }
}
