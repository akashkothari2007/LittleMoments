using UnityEngine;

public class TurnOnFire : MonoBehaviour, IInteractable
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public StoryManager2 storyManager;
    // Start is called once 
    public void Interact(Inventory inventory)
    {
        storyManager.OnFireStoked();
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
