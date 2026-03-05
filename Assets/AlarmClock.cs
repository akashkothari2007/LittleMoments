using UnityEngine;

public class AlarmClock : MonoBehaviour, IInteractable
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void Interact(Inventory inventory)
    {
        if (transform.GetComponent<AudioSource>().isPlaying) return;
        transform.GetComponent<AudioSource>().Play();
    }
}
