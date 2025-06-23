using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EntityMoveController : MonoBehaviour
{
    [SerializeField] private Transform _model;
    [field: SerializeField] public Transform Head { get; private set; }
    [SerializeField] private NavMeshAgent _agent;

    [Header("Footstep")]
    [SerializeField] private FootstepConfig _footstep;
    [SerializeField] private LayerMask _footstepMask;
    [SerializeField] private List<Transform> _footstepPositions;

    private FootstepPlayer _footstepPlayer;
    private IAudioService _audioService;
    private int _currentFootstepIndex;
    private float _lastStepTime;

    private const float STEP_COOLDOWN = 0.4f;

    public void Initialize(IAudioService audioService)
    {
        _audioService = audioService;
        _footstepPlayer = new FootstepPlayer(audioService, _footstep, _footstepMask, transform);
    }

    public void MoveTo(Vector3 point)
    {
        if (_agent.isOnNavMesh)
            _agent.SetDestination(point);
    }

    public bool ReachedPoint(float tolerance = 1f)
    {
        return !_agent.pathPending && _agent.remainingDistance <= tolerance && _agent.velocity.sqrMagnitude < 0.1f;
    }

    public void Look(Vector3 targetPos)
    {
       /* Vector3 direction = (targetPos - _model.position).normalized;
        direction.y = 0f;

        if (direction != Vector3.zero)
            _model.rotation = Quaternion.LookRotation(direction);*/
    }

    private void Update()
    {
        TryPlayFootstep();
    }

    private void TryPlayFootstep()
    {
        if (_agent.velocity.sqrMagnitude > 0.01f)
        {
            if (Time.time - _lastStepTime >= STEP_COOLDOWN)
            {
                if (_footstepPositions != null && _footstepPositions.Count > 0)
                {
                    var pos = _footstepPositions[_currentFootstepIndex].position;
                    _footstepPlayer.TryPlayFootstep(pos);
                    _currentFootstepIndex = (_currentFootstepIndex + 1) % _footstepPositions.Count;
                }

                _lastStepTime = Time.time;
            }
        }
    }
}
