using UnityEngine;

public class SlidingRock : MonoBehaviour
{


    public bool locked = false;
    public StoryManager storyManager;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (storyManager.currentStoryState != StoryManager.StoryState.PathBuilding)
        {
            Debug.Log("Not the right time to place the rock");
            return;
        }
        if (locked) return;

        if (other.CompareTag("RockTarget"))
        {
            SnapTo(other.transform);
            other.gameObject.SetActive(false); 
        }
    }

    public void SnapTo(Transform target)
    {
        if (locked) return;
        storyManager.OnStonePlaced();
        locked = true;

        // snap position/rotation
        transform.position = target.position;
        transform.rotation = target.rotation;
        transform.GetComponent<AudioSource>().Play();
        //unactivate rigidbody
        transform.GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;      // stop any existing movement
        transform.GetComponent<Rigidbody2D>().angularVelocity = 0f;              // stop any existing rotation
        transform.GetComponent<Collider2D>().enabled = false;
    }
}
