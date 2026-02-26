using UnityEngine;

public class FuseBox : MonoBehaviour, IInteractable
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public StoryManager2 storyManager;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void Interact(Inventory inventory)
    {
        storyManager.OnFuseBoxFix();
    }
}
