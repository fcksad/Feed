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


    [Header("Look")]
    private const float _mouseSensitivity = 0.4f;
    private const float _maxLookAngle = 85f;
    private float _verticalVelocity;
    private float _cameraPitch = 0f;

    private readonly ICommandController _commandController;
    private readonly Character _character;

    private IAudioService _audioService;


    public CharacterController(ICommandController commandController, IAudioService audioService , Character character)
    {
        _commandController = commandController;
        _audioService = audioService;
        _character = character;
    }

    public void Move(Vector2 input, bool isRunning, bool jumpRequested)
    {
        ApplyGravity();

        if (jumpRequested && _character.CharacterController.isGrounded)
            Jump();

        var speed = isRunning ? RUN_SPEED : WALK_SPEED;
        var command = new MoveCommand(_character.CharacterController, _character.CameraRoot, input, speed, _verticalVelocity);
        _commandController.SetCommand(command);

        float currentStepCooldown = isRunning ? RUN_STEP_COOLDOWN : WALK_STEP_COOLDOWN;

        if (input.sqrMagnitude > 0.01f && _character.CharacterController.isGrounded)
        {
            if (Time.time - _lastStepTime >= currentStepCooldown)
            {
                var positions = _character.FootstepPositions;
                if (positions != null && positions.Count > 0)
                {
                    var pos = positions[_currentFootstepIndex];
                    _audioService.Play(_character.FootstepSound, position: pos.position);

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
        _character.CameraRoot.localEulerAngles = new Vector3(_cameraPitch, 0f, 0f);

        _character.transform.Rotate(Vector3.up * delta.x);
    }

}
