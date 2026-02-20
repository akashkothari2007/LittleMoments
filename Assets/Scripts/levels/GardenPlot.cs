using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class GardenPlot : MonoBehaviour, IInteractable
{
    public SpriteRenderer spriteRenderer;
    public Sprite redFlowerSprite;
    public Sprite blueFlowerSprite;
    public StoryManager storyManager;

    void Start()
{
    spriteRenderer = GetComponent<SpriteRenderer>();
}
    public void Interact(Inventory inventory)
    {
        if (storyManager.currentStoryState != StoryManager.StoryState.FlowerPlanting)
        {
            Debug.Log("Not the right time to plant flowers");
            return;
        }
        if (spriteRenderer.enabled)
        {
            Debug.Log("Garden plot already has a flower");
            return;
        }
        Debug.Log("Interacted with garden plot");
        if (inventory.TrySpend(ItemType.RedFlower, 1))
        {
            spriteRenderer.sprite = redFlowerSprite;
            spriteRenderer.enabled = true;
            storyManager.OnFlowerPlanted();
            Debug.Log("Planted red flower");
            transform.GetComponent<AudioSource>().Play();

        }
        else if (inventory.TrySpend(ItemType.BlueFlower, 1))
        {
            spriteRenderer.sprite = blueFlowerSprite;
            spriteRenderer.enabled = true;
            storyManager.OnFlowerPlanted();
            Debug.Log("Planted blue flower");
            transform.GetComponent<AudioSource>().Play();
        }
    }
}
