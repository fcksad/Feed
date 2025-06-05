public interface IInteractable 
{
    public string Name { get;}
    void Interact();
    void ReceiveInteractionFrom(IGrabbable item);

}
