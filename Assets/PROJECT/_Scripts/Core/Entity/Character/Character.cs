using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    [field: SerializeField] public UnityEngine.CharacterController CharacterController {  get; private set; }
    [field: SerializeField] public Transform CharacterModel { get; private set; }
    [field: SerializeField] public Transform HeadRoot { get; private set; }
    [field: SerializeField] public Transform CameraRoot { get; private set; }
    [field: SerializeField] public FootstepConfig Footstep { get; private set; }
    [field: SerializeField] public LayerMask FootstepMask { get; private set; }

    [field: SerializeField] public List<Transform> FootstepPositions { get; private set; }
    [field: SerializeField] public InteractController InteractController { get; private set; }
    [field: SerializeField] public GrabController GrabController { get; private set; }

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked; // todo
    }
}
