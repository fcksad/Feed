using UnityEngine;
using UnityEngine.AI;

public class EntityBrain : MonoBehaviour
{
    [SerializeField] private EntityMoveController _moveController;
    [SerializeField] private float _visionDistance = 10f;
    [SerializeField] private LayerMask _visionMask;

    private Character _target;
    private Vector3 _nextPoint;
    private float _waitTimer;

    private enum State { Patrol, Watch, Attack, Flee, Idle }
    [SerializeField] private State _state = State.Patrol;

    private void Start()
    {
        _nextPoint = GetRandomNavMeshPoint(10f);
        _moveController.MoveTo(_nextPoint);
    }

    private void FixedUpdate()
    {
        switch (_state)
        {
            case State.Patrol:
                Patrol();
                break;
            case State.Watch:
                Watch();
                break;
            case State.Attack:
                Attack();
                break;
            case State.Flee:
                Flee();
                break;
            case State.Idle:
                Idle();
                break;
        }

        TryDetectPlayer();
        TryDetectObstacleAhead();
    }

    private void Patrol()
    {
/*        _moveController.Look(_nextPoint);*/

        if (_moveController.ReachedPoint(1f))
        {
            _waitTimer = Random.Range(2f, 4f);
            _state = State.Idle;

            _nextPoint = GetRandomNavMeshPoint(10f);
            _moveController.MoveTo(_nextPoint);
        }
    }

    private void Watch()
    {
        _moveController.Look(_target.transform.position);
    }

    private void Attack()
    {
        _moveController.Look(_target.transform.position);
        _moveController.MoveTo(_target.transform.position);
    }

    private void Flee()
    {
        Vector3 dir = (transform.position - _target.transform.position).normalized;
        Vector3 fleePoint = transform.position + dir * 10f;

        if (NavMesh.SamplePosition(fleePoint, out var hit, 5f, NavMesh.AllAreas))
        {
            _moveController.MoveTo(hit.position);
        }
    }

    private void Idle()
    {
        _waitTimer -= Time.deltaTime;
        if (_waitTimer <= 0)
        {
            _state = State.Patrol;
            _moveController.MoveTo(_nextPoint);
        }
    }

    private void TryDetectPlayer()
    {
        Vector3 origin = _moveController.Head.position;
        Vector3 direction = (_nextPoint - origin).normalized;

        Debug.DrawRay(origin, direction * _visionDistance, Color.red);

        Ray ray = new Ray(origin, direction);
        if (Physics.Raycast(ray, out var hit, _visionDistance, _visionMask))
        {
            if (hit.collider.TryGetComponent(out Door door))
                door.Open();

            if (hit.collider.TryGetComponent(out Character character))
            {
                _target = character;
                _state = State.Attack;
            }
        }
        else if (_state == State.Attack)
        {
            _waitTimer = 2f;
            _state = State.Watch;
        }
    }

    private void TryDetectObstacleAhead()
    {
        Vector3 origin = _moveController.Head.position;
        Vector3 direction = _moveController.Head.position;

        if (Physics.Raycast(origin, direction, out RaycastHit hit, 2f))
        {
            if (hit.collider.TryGetComponent(out Door door))
            {
                door.Open();
            }
        }
    }

    private Vector3 GetRandomNavMeshPoint(float radius)
    {
        for (int i = 0; i < 10; i++)
        {
            Vector3 randomDirection = Random.insideUnitSphere * radius;
            randomDirection += transform.position;

            if (NavMesh.SamplePosition(randomDirection, out var hit, 2f, NavMesh.AllAreas))
                return hit.position;
        }

        return transform.position;
    }
}
