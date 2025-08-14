using UnityEngine;

public class FollowBaseState : IEntityState
{

    private readonly Transform _self;
    private readonly EntityMoveController _move;
    private readonly EntityPathFinderBrain _pf;
    private readonly Transform _target;
    private readonly float _speed;

    private const float WaypointEps = 0.2f;

    public FollowBaseState(Transform self, EntityMoveController move, EntityPathFinderBrain pf, Transform target, float speed)
    {
        _self = self;
        _move = move;
        _pf = pf;
        _target = target;
        _speed = speed;
    }

    public void Enter() { }
    public void Exit() { }

    public void Tick()
    {
        if (_target == null) return;

        // если видим цель напр€мую Ч идЄм сразу к ней
        Vector3 goal = _pf.HasDirectLOS() ? _target.position : _pf.GetWaypoint();

        // если уже почти на точке Ч подправим гол на насто€щую цель (сглаживает "залипание" у стены)
        if (Vector3.Distance(_self.position, goal) < WaypointEps && !_pf.HasDirectLOS())
            goal = _target.position;

        _move.MoveTowards(goal, _speed);
    }
}
