using UnityEngine;

public class EntityBrain : MonoBehaviour
{
    [SerializeField] private EntityMoveController _moveController;
    [SerializeField] private LayerMask _visionMask;

    private enum State { Patrol, Watch, Attack, Flee, Idle }


    private void Start()
    {

    }

  
}
