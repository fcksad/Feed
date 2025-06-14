using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Model")]
    [field: SerializeField] public UnityEngine.CharacterController CharacterController { get; private set; }
    [field: SerializeField] public Transform CharacterModel { get; private set; }

    [Header("Health")]
    [field: SerializeField] public Health Health { get; private set; }

    [Header("Foot")]
    [field: SerializeField] public FootstepConfig Footstep { get; private set; }
    [field: SerializeField] public LayerMask FootstepMask { get; private set; }
    [field: SerializeField] public List<Transform> FootstepPositions { get; private set; }


    private void Start()
    {
        Health.OnDiedEvent += Died;
    }

    private void OnDestroy()
    {
        Health.OnDiedEvent -= Died;
    }

    private void Died()
    {
        Destroy(gameObject);
        Debug.LogError("Died");
    }
}
