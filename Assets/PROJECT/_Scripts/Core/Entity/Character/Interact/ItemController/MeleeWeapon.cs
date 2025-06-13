using UnityEngine;

public class MeleeWeapon : InteractableObject, IUsable
{

    public void Use()
    {
        Debug.LogError("Attack");
    }
}
