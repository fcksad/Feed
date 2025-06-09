using System.Collections;
using UnityEngine;

public class CharacterController : IControllable
{
    [Header("Move")]
    private const float WALK_SPEED = 3f;
    private const float RUN_SPEED = 6f;

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
    private const float _mouseSensitivity = 0.4f;
    private const float _maxLookAngle = 85f;
    private float _verticalVelocity;
    private float _cameraPitch = 0f;

    [Header("Head Bobbing")]
    private Coroutine _headBobCoroutine;
    private float _bobAmount = 0.05f;
    private float _bobDuration = 0.2f;


    private readonly ICommandController _commandController;
    private readonly Character _character;

    private IAudioService _audioService;


    public CharacterController(ICommandController commandController, IAudioService audioService , Character character)
    {
        _commandController = commandController;
        _audioService = audioService;
        _character = character;
    }

    public void Move(Vector2 input, bool isRunning, bool jumpRequested, bool isCrouching)
    {
        ApplyGravity();

        if (!isCrouching && (jumpRequested && _character.CharacterController.isGrounded))
            Jump();

        float speed;
        if (isCrouching)
            speed = WALK_SPEED * 0.5f;
        else
            speed = isRunning ? RUN_SPEED : WALK_SPEED;

        var command = new MoveCommand(_character.CharacterController, _character.HeadRoot, input, speed, _verticalVelocity);
        _commandController.SetCommand(command);

        float currentStepCooldown;

        if (isCrouching)
            currentStepCooldown = WALK_STEP_COOLDOWN * 2;
        else
            currentStepCooldown = isRunning ? RUN_STEP_COOLDOWN : WALK_STEP_COOLDOWN;

        if (input.sqrMagnitude > 0.01f && _character.CharacterController.isGrounded)
        {
            if (Time.time - _lastStepTime >= currentStepCooldown)
            {
                var positions = _character.FootstepPositions;
                if (positions != null && positions.Count > 0)
                {
                    var pos = positions[_currentFootstepIndex];
                    _audioService.Play(_character.FootstepSound, position: pos.position);
                   // ApplyHeadBob();
                    _currentFootstepIndex = (_currentFootstepIndex + 1) % positions.Count;
                }

                _lastStepTime = Time.time;
            }
        }
    }


    private void ApplyGravity()
    {
        if (_character.CharacterController.isGrounded)
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
        if (_character.CharacterController.isGrounded)
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
            float radius = _character.CharacterController.radius * 0.9f;

            if (Physics.SphereCast(_character.HeadRoot.position, radius, Vector3.up, out RaycastHit hit, 1, ~0, QueryTriggerInteraction.Ignore))
            {
                return false;
            }
        }

        float newHeight = isCrouching ? _crouchHeight : _standHeight;
        float heightDiff = _standHeight - newHeight;

        _character.CharacterController.height = newHeight;
        _character.CharacterController.center = new Vector3(0f, -heightDiff / 2f, 0f);

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

    private void ApplyHeadBob()
    {
        if (_headBobCoroutine != null)
            _character.StopCoroutine(_headBobCoroutine);

        _headBobCoroutine = _character.StartCoroutine(HeadBobRoutine());
    }

    private IEnumerator HeadBobRoutine()
    {
        Transform cam = _character.CameraRoot;
        Vector3 originalPos = cam.localPosition;
        Vector3 downPos = new Vector3(originalPos.x, originalPos.y - _bobAmount, originalPos.z);

        float t = 0f;

        while (t < _bobDuration)
        {
            t += Time.deltaTime;
            cam.localPosition = Vector3.Lerp(originalPos, downPos, t / _bobDuration);
            yield return null;
        }

        t = 0f;
        while (t < _bobDuration)
        {
            t += Time.deltaTime;
            cam.localPosition = Vector3.Lerp(downPos, originalPos, t / _bobDuration);
            yield return null;
        }

        cam.localPosition = originalPos;
    }

}
