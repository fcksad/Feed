using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : MonoBehaviour ,IControllable
{
    [Header("Move")]
    private const float WALK_SPEED = 3f;
    private const float RUN_SPEED = 6f;
    [SerializeField] private UnityEngine.CharacterController _characterController;

    private float _lastStepTime;
    private const float WALK_STEP_COOLDOWN = 0.5f;
    private const float RUN_STEP_COOLDOWN = 0.3f;
    private int _currentFootstepIndex;

    [Header("Jump")]
    private float _jumpHeight = 0.5f;
    private const float FALL_MULTIPLIER = 2.5f;

    [Header("Crouch")]
    private Coroutine _crouchCamCoroutine;
    private float _camPos = 0.7f;
    private float _crouchCamPos = 0.1f;
    private float _standHeight = 1.65f;
    private float _crouchHeight = 1f;

    [Header("Look")]
    private float _mouseSensitivity = 0.4f;
    private const float _maxLookAngle = 85f;
    private float _verticalVelocity;
    private float _cameraPitch = 0f;

    [Header("Footstep")]
    private FootstepPlayer _footstepPlayer;
    [SerializeField] private FootstepConfig _footstep;
    [SerializeField] private LayerMask _footstepMask;
    [SerializeField] private List<Transform> _footstepPositions;

    private ICommandController _commandController;
    private Character _character;

    private IAudioService _audioService;

    public void Initialize(IAudioService audioService , Character character, float mouseSensitivity)
    {
        _audioService = audioService;
        _character = character;
        _mouseSensitivity = mouseSensitivity;

        _commandController = new CommandController();
        _footstepPlayer = new FootstepPlayer(audioService, _footstep, _footstepMask, _character.transform);
    }

    public void Move(Vector2 input, bool isRunning, bool jumpRequested, bool isCrouching)
    {
        ApplyGravity();

        if (!isCrouching && (jumpRequested && _characterController.isGrounded))
            Jump();

        float speed;
        if (isCrouching)
            speed = WALK_SPEED * 0.5f;
        else
            speed = isRunning ? RUN_SPEED : WALK_SPEED;

        var command = new MoveCommand(_characterController, _character.HeadRoot, input, speed, _verticalVelocity);
        _commandController.SetCommand(command);

        float currentStepCooldown;

        if (isCrouching)
            currentStepCooldown = WALK_STEP_COOLDOWN * 2;
        else
            currentStepCooldown = isRunning ? RUN_STEP_COOLDOWN : WALK_STEP_COOLDOWN;

        if (input.sqrMagnitude > 0.01f && _characterController.isGrounded)
        {
            if (Time.time - _lastStepTime >= currentStepCooldown)
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

    private void ApplyGravity()
    {
        if (_characterController.isGrounded)
        {
            if (_verticalVelocity < 0)
                _verticalVelocity = -1f;
        }
        else
        {
            bool falling = _verticalVelocity < 0;
            float gravityScale = falling ? FALL_MULTIPLIER : 1f;
            _verticalVelocity += Physics.gravity.y * gravityScale * Time.deltaTime;
        }
    }

    private void Jump()
    {
        if (_characterController.isGrounded)
        {
            _verticalVelocity = Mathf.Sqrt(2f * 9.81f * _jumpHeight); 
        }
    }

    public void Look(Vector2 delta)
    {
        delta *= _mouseSensitivity;

        _cameraPitch -= delta.y;
        _cameraPitch = Mathf.Clamp(_cameraPitch, -_maxLookAngle, _maxLookAngle);
        _character.HeadRoot.localEulerAngles = new Vector3(_cameraPitch, 0f, 0f);

        _character.CharacterModel.transform.Rotate(Vector3.up * delta.x);
    }

    public bool Crouch(bool isCrouching)
    {
        if (!isCrouching)
        {
            if (Physics.SphereCast(_character.HeadRoot.position, _characterController.radius, Vector3.up, out RaycastHit hit,_standHeight - _crouchHeight, ~0, QueryTriggerInteraction.Ignore))
            {
                return false;
            }
        }

        float newHeight = isCrouching ? _crouchHeight : _standHeight;
        float heightDiff = _standHeight - newHeight;

        _characterController.height = newHeight;
        _characterController.center = new Vector3(0f, -heightDiff / 2f, 0f);

        float targetY = isCrouching ? _crouchCamPos : _camPos;

        if (_crouchCamCoroutine != null)
            _character.StopCoroutine(_crouchCamCoroutine);

        _crouchCamCoroutine = _character.StartCoroutine(LerpCameraY(targetY));

        return true;
    }

    private IEnumerator LerpCameraY(float targetY)
    {
        Transform cam = _character.HeadRoot;
        float duration = 0.2f;
        float time = 0f;

        Vector3 startPos = cam.localPosition;
        Vector3 targetPos = new Vector3(startPos.x, targetY, startPos.z);

        while (time < duration)
        {
            time += Time.deltaTime;
            cam.localPosition = Vector3.Lerp(startPos, targetPos, time / duration);
            yield return null;
        }

        cam.localPosition = targetPos;
    }

    private void FixedUpdate()
    {
        _commandController?.Tick();
    }
}
