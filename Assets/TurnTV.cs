using UnityEngine;

public class TurnTV : MonoBehaviour, IInteractable
{
    public StoryManager2 storyManager;
    // Start is called once 
    public void Interact(Inventory inventory)
    {
        storyManager.OnTVTurnedOn();
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
