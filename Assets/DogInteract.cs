using UnityEngine;

public class DogInteract : MonoBehaviour, IInteractable
{
    public void Interact(Inventory inventory)
    {
        AudioSource aS = GetComponent<AudioSource>();
        if (aS.isPlaying) return;
        aS.Play();
    }
    
}
