using UnityEngine;
using Zenject;

[RequireComponent(typeof(UnityEngine.CharacterController))]
public class EntityBrain : MonoBehaviour, IEntityTickable
{
    private enum State
    {
        Idle,      
        Patrol
    }

    [SerializeField] private EntityMoveController _moveController;
    [field: SerializeField] public Transform Player { get; private set; }
    [field: SerializeField] public float MoveSpeed { get; private set; } = 3f;

    [Header("Pathfinder")]
    [SerializeField] private LayerMask _obstacleMask;
    [SerializeField] private float _agentRadius = 0.35f;
    [SerializeField] private float _stepDistance = 2.5f;
    [SerializeField] private int _samples = 20;
    [SerializeField] private float _repathInterval = 0.2f;

    private EntityPathFinderBrain _pathFinder;


    public IEntityState _currentState;


    [Inject] private IEntityTickService _tickService;
    [SerializeField] private float _desiredTickRate = 0.1f;

    private void Awake()
    {
        if (_moveController == null) _moveController = GetComponent<EntityMoveController>();
    }

    private void Start()
    {
        _pathFinder = new EntityPathFinderBrain(
            transform,
            Player,
            _obstacleMask,
            _agentRadius,
            _stepDistance,
            _samples,
            _repathInterval
        );
        _pathFinder.SetDebug(true, duration: _repathInterval * 1.1f, depthTest: true);

        SwitchState(new FollowBaseState(transform, _moveController, _pathFinder, Player, MoveSpeed));

        _tickService.Register(this, _desiredTickRate); // «медленный» тик для pathfinder-а
    }

    public void TickUpdate()
    {
        _pathFinder?.TickUpdate();
    }

    public void Update()
    {
        _currentState?.Tick();
    }

    public void SwitchState(IEntityState newState)
    {
        _currentState?.Exit();
        _currentState = newState;
        _currentState.Enter();
    }

    private void OnDestroy()
    {
        _tickService.Unregister(this);
    }

}
