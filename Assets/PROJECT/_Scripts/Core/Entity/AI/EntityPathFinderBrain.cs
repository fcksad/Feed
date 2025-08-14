using UnityEngine;

public class EntityPathFinderBrain : IEntityTickable
{
    private readonly Transform _agent;
    private readonly Transform _target;

    private readonly LayerMask _obstacleMask;
    private readonly float _agentRadius;
    private readonly float _stepDistance;
    private readonly int _samples;
    private readonly float _repathInterval;

    private float _lastRepathTime;
    private Vector3 _currentWaypoint;
    private bool _hasWaypoint;

    private const float Skin = 0.05f;

    private float _prevBestScore = float.PositiveInfinity;
    private float _holdUntil = 0f;

    [SerializeField] private float _minScoreGain = 1; 

    [SerializeField] private float _waypointHoldTime = 0.5f; 

    [SerializeField] private float _waypointReachedEps = 0.35f; 
    [SerializeField] private float _extraHoldTime = 0.4f;   
    [SerializeField] private int _maxNoProgressTicks = 6;

    [SerializeField] private float _minForwardDot = 0.0f; // >= 0 значит не позволяем идти назад (>90°)
    [SerializeField] private float _minGoalDot = 0.0f;

    private float _lastWpDist = float.PositiveInfinity;
    private int _noProgressTicks = 0;

    private Vector3 Flat(Vector3 v) => new Vector3(v.x, 0f, v.z);

    private bool _debug;
    private float _debugDuration = 0.25f;
    private bool _debugDepthTest = true;


    public void SetDebug(bool enabled, float duration = 0.25f, bool depthTest = true)
    {
        _debug = enabled;
        _debugDuration = Mathf.Max(0.02f, duration);
        _debugDepthTest = depthTest;
    }

    public EntityPathFinderBrain(
        Transform agent,
        Transform target,
        LayerMask obstacleMask,
        float agentRadius = 0.35f,
        float stepDistance = 2.5f,
        int samples = 20,
        float repathInterval = 0.2f)
    {
        _agent = agent;
        _target = target;

        _obstacleMask = obstacleMask;
        _agentRadius = Mathf.Max(0.05f, agentRadius);
        _stepDistance = Mathf.Max(0.5f, stepDistance);
        _samples = Mathf.Clamp(samples, 8, 64);
        _repathInterval = Mathf.Max(0.05f, repathInterval);
    }

    public void TickUpdate()
    {
        if (_agent == null || _target == null) return;
        if (Time.time - _lastRepathTime < _repathInterval) return;

        _lastRepathTime = Time.time;
        RecomputeWaypoint();
    }

    public Vector3 GetWaypoint() => _hasWaypoint ? _currentWaypoint : _target.position;

    public bool HasDirectLOS()
    {
        var from = AgentEye();
        var to = TargetEye();

        var dir = to - from;
        var dist = dir.magnitude;
        if (dist < 0.001f) return true;
        dir /= dist;

        if (Physics.CheckSphere(from, _agentRadius * 0.98f, _obstacleMask, QueryTriggerInteraction.Ignore))
        {
            if (_debug) { DCross(from, 0.15f, Color.magenta); }
            return false;
        }

        var start = from + dir * (_agentRadius * 0.1f);

        bool blocked = Physics.SphereCast(
            start, _agentRadius, dir, out var hit, dist,
            _obstacleMask, QueryTriggerInteraction.Ignore);

        if (_debug)
        {
            var col = blocked ? Color.red : Color.green;
            DLine(start, blocked ? hit.point : to, col);
            if (blocked) DCross(hit.point, 0.15f, Color.red);
        }

        return !blocked;
    }

    private void RecomputeWaypoint()
    {
        if (HasDirectLOS())
        {
            _currentWaypoint = _target.position;
            _hasWaypoint = true;

            if (_debug)
            {
                DLine(_agent.position, _currentWaypoint, new Color(0.2f, 0.6f, 1f));
                DCross(_currentWaypoint, 0.18f, new Color(0.2f, 0.6f, 1f));
            }
            return;
        }

        if (_hasWaypoint)
        {
            float dist = Vector3.Distance(Flat(_agent.position), Flat(_currentWaypoint));
            bool reached = dist <= _waypointReachedEps;

            if (!reached)
            {
                if (Time.time < _holdUntil + _extraHoldTime)
                {
                    if (dist < _lastWpDist - 0.01f) _noProgressTicks = 0;
                    else _noProgressTicks++;

                    _lastWpDist = dist;

                    if (_noProgressTicks <= _maxNoProgressTicks)
                    {
                        if (_debug)
                        {
                            DLine(_agent.position, _currentWaypoint, new Color(0.2f, 0.8f, 1f));
                        }
                        return;
                    }
                }
            }
            else
            {
                _holdUntil = Time.time;
                _lastWpDist = float.PositiveInfinity;
                _noProgressTicks = 0;
            }
        }

        var bestScore = float.PositiveInfinity;
        var bestPoint = _agent.position;

        var origin = AgentEye();
        var toTarget = (TargetEye() - origin);
        var flatToTarget = Vector3.ProjectOnPlane(toTarget, Vector3.up).normalized;

        TryEdgeFollow(origin, flatToTarget, ref bestPoint, ref bestScore);

        for (int i = 0; i < _samples; i++)
        {
            float t = (float)i / _samples;
            float angle = t * 360f;
            Vector3 dir = Quaternion.AngleAxis(angle, Vector3.up) * flatToTarget;

            var cand = CastStep(origin, dir, out bool collided, out Vector3 castEnd, out float free01);
            float score = Heuristic(cand, collided, free01);

            if (_debug)
            {
                var rayCol = new Color(1f, 0.9f, 0.2f);
                DLine(origin, castEnd, rayCol);
                if (collided) DCross(castEnd, 0.12f, Color.red);
                if ((cand - castEnd).sqrMagnitude > 0.0001f)
                    DLine(castEnd, cand, new Color(1f, 0.65f, 0.1f));
            }

            if (score < bestScore)
            {
                bestScore = score;
                bestPoint = cand;
            }


        }

        _currentWaypoint = bestPoint;
        _hasWaypoint = true;

        if (_hasWaypoint)
        {
            if (Time.time < _holdUntil)
            {
                bestPoint = _currentWaypoint;
                bestScore = _prevBestScore;
            }
            else
            {
                float gain = _prevBestScore - bestScore;
                if (gain < _minScoreGain && (bestPoint - _currentWaypoint).sqrMagnitude > 0.01f)
                {
                    bestPoint = _currentWaypoint;
                    bestScore = _prevBestScore;
                }
            }
        }

        _currentWaypoint = bestPoint;
        _prevBestScore = bestScore;
        _hasWaypoint = true;
        _holdUntil = Time.time + _waypointHoldTime;

        if (_debug)
        {
            var chosen = new Color(0f, 1f, 1f);
            DLine(_agent.position, _currentWaypoint, chosen);
            DCross(_currentWaypoint, 0.2f, chosen);
        }
    }

    private void TryEdgeFollow(Vector3 origin, Vector3 toward, ref Vector3 bestPoint, ref float bestScore)
    {
        if (Physics.SphereCast(origin, _agentRadius, toward, out var hit, _stepDistance, _obstacleMask, QueryTriggerInteraction.Ignore))
        {
            var n = hit.normal;
            var tangentL = Vector3.Normalize(Vector3.Cross(Vector3.up, n));
            var tangentR = -tangentL;

            var left = CastStep(origin, tangentL, out var colL, out var endL, out var freeL);
            var right = CastStep(origin, tangentR, out var colR, out var endR, out var freeR);

            var ls = Heuristic(left, colL, freeL, edgeFollowBonus: -0.5f);
            var rs = Heuristic(right, colR, freeR, edgeFollowBonus: -0.5f);

            if (_debug)
            {
                var purp = new Color(0.8f, 0.3f, 0.9f);
                DLine(origin, endL, purp);
                DLine(origin, endR, purp);
                if (colL) DCross(endL, 0.12f, purp);
                if (colR) DCross(endR, 0.12f, purp);
            }

            if (ls < bestScore) { bestScore = ls; bestPoint = left; }
            if (rs < bestScore) { bestScore = rs; bestPoint = right; }
        }
    }

    private Vector3 CastStep(Vector3 origin, Vector3 dir, out bool collided, out Vector3 castEnd, out float free01)
    {
        dir = dir.normalized;

        if (Physics.SphereCast(origin, _agentRadius, dir, out var hit, _stepDistance, _obstacleMask, QueryTriggerInteraction.Ignore))
        {
            collided = true;
            castEnd = hit.point;
            free01 = Mathf.Clamp01(hit.distance / _stepDistance);

            var safe = hit.point - dir * (_agentRadius + Skin + 0.05f);
            safe.y = _agent.position.y;
            return safe;
        }
        else
        {
            collided = false;
            castEnd = origin + dir * _stepDistance;
            free01 = 1f;

            var free = castEnd;
            free.y = _agent.position.y;
            return free;
        }
    }

    private float Heuristic(Vector3 candidate, bool collided, float free01, float edgeFollowBonus = 0f)
    {
        var toTarget = TargetFlat() - candidate;
        float dist = toTarget.magnitude;

        var toCand = (candidate - AgentFlat()).normalized;
        var toGoal = (TargetFlat() - AgentFlat()).normalized;
        float anglePenalty = (1f - Vector3.Dot(toCand, toGoal)) * 2.0f;

        float wallPenalty = Physics.CheckSphere(candidate + Vector3.up * 0.1f, _agentRadius * 0.9f, _obstacleMask, QueryTriggerInteraction.Ignore) ? 1.5f : 0f;

        float collisionPenalty = collided ? (1f + (1f - free01) * 2.0f) : 0f;
        float freeBonus = free01 * 0.75f;

        var forward = _agent.forward;
        var toCandFromAgent = (candidate - _agent.position).normalized;
        float headingPenalty = (1f - Mathf.Clamp01(Vector3.Dot(forward, toCandFromAgent))) * 0.5f;

        return dist + anglePenalty + wallPenalty + collisionPenalty + headingPenalty - freeBonus + edgeFollowBonus;
    }


    private Vector3 AgentEye() { var p = _agent.position; p.y += _agentRadius * 0.5f; return p; }
    private Vector3 TargetEye() { var p = _target.position; p.y += _agentRadius * 0.5f; return p; }
    private Vector3 AgentFlat() => new Vector3(_agent.position.x, 0f, _agent.position.z);
    private Vector3 TargetFlat() => new Vector3(_target.position.x, 0f, _target.position.z);

    private void DLine(Vector3 a, Vector3 b, Color c)
        => Debug.DrawLine(a, b, c, _debugDuration, _debugDepthTest);

    private void DRay(Vector3 o, Vector3 d, float len, Color c)
        => Debug.DrawRay(o, d.normalized * len, c, _debugDuration, _debugDepthTest);

    private void DCross(Vector3 p, float size, Color c)
    {
        var x = new Vector3(size, 0, 0);
        var z = new Vector3(0, 0, size);
        Debug.DrawLine(p - x, p + x, c, _debugDuration, _debugDepthTest);
        Debug.DrawLine(p - z, p + z, c, _debugDuration, _debugDepthTest);
    }

}
