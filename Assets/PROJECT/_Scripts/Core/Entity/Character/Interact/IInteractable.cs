using UnityEngine;

public interface IInteractable 
{
    public Outline Outline { get; set; }

    void Interact();
    
}
