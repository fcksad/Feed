using Service;
using System.Collections.Generic;
using UnityEngine;

public class EntityMoveController : MonoBehaviour
{
    [SerializeField] private Transform _model;
    [field: SerializeField] public Transform Head { get; private set; }

    [Header("Footstep")]
    [SerializeField] private SourceType _footstep = SourceType.Footstep;
    [SerializeField] private LayerMask _footstepMask;
    [SerializeField] private List<Transform> _footstepPositions;

    private ISurfaceAudioService _surfaceAudioService;
    private IAudioService _audioService;
    protected FootstepPlayer _footstepPlayer;
    private int _currentFootstepIndex;
    private float _lastStepTime;

    private const float STEP_COOLDOWN = 0.4f;


    [SerializeField] private float _rotationSpeed = 540f; // deg/sec
    [SerializeField] private float _gravity = -18f;
    [SerializeField] private UnityEngine.CharacterController _characterController;
    private float _verticalVel;

    public void Initialize(IAudioService audioService, ISurfaceAudioService surfaceAudioService)
    {
        _audioService = audioService;
        _surfaceAudioService = surfaceAudioService;
        _footstepPlayer = new FootstepPlayer(audioService, _surfaceAudioService, _footstepMask, transform);
    }

    public void MoveTowards(Vector3 worldPoint, float speed)
    {
        Vector3 pos = transform.position;
        Vector3 to = (worldPoint - pos);
        to.y = 0f;

        Vector3 dir = to.sqrMagnitude > 0.0001f ? to.normalized : Vector3.zero;

        // поворот лицом к движению
        if (dir.sqrMagnitude > 0.0001f)
        {
            var targetRot = Quaternion.LookRotation(dir, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, _rotationSpeed * Time.deltaTime);
        }

        // гравитация (если нужно)
        if (_characterController.isGrounded && _verticalVel < 0f) _verticalVel = -2f;
        _verticalVel += _gravity * Time.deltaTime;

        Vector3 velocity = dir * speed + Vector3.up * _verticalVel;
        _characterController.Move(velocity * Time.deltaTime);
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
       /* if (_agent.velocity.sqrMagnitude > 0.01f)
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
        }*/
    }
}
