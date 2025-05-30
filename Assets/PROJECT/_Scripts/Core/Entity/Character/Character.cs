using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    [field: SerializeField] public UnityEngine.CharacterController CharacterController {  get; private set; }
    [field: SerializeField] public Transform CameraRoot { get; private set; }
    [field: SerializeField] public AudioConfig FootstepSound { get; private set; }
    [field: SerializeField] public List<Transform> FootstepPositions { get; private set; }
}
