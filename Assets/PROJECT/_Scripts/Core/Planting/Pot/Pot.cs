using UnityEngine;

public class Pot : MonoBehaviour , IInteractable
{
    [field:SerializeField] public Outline Outline { get; set; }

    public void Interact()
    {

    }
}
