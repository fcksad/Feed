using UnityEngine;

public interface IInteractable 
{

    public Outline Outline { get;}

    void Interact();
    void ReceiveInteractionFrom(IGrabbable item);

}
