public class Pot : GrabbableObject
{
    public override void Interact()
    {

    }

    public override void InteractWith(IInteractable target)
    {
        
    }

    public override void ReceiveInteractionFrom(IGrabbable item)
    {
       /* if (item is SeedBag)
        {
            Debug.Log("—емена посе€ны в горшок!");
            // тут можно создать растение и убрать предмет из рук
        }
        else
        {
            Debug.Log("Ётот предмет нельз€ использовать на горшке.");
        }*/
    }
}
