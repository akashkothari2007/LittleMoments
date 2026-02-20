using UnityEngine;

public class PickUp : MonoBehaviour, IInteractable
{
    public ItemType itemType = ItemType.RedFlower;
    public int totalAmount = 1;
    public int amountToGive = 1;
    public bool destroyOnInteract = true;
    public bool available = true;
    public void Interact(Inventory inventory)
    {
        Debug.Log($"Interacted with pickup of type {itemType}");
        if (!available) return;

        inventory.Add(itemType, amountToGive);
        totalAmount -= 1;
        if (totalAmount <= 0)
        {
            available = false;
            if (destroyOnInteract)
            {
                gameObject.SetActive(false);
            }
        }
        
    }

}
