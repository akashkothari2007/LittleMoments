using UnityEngine;

public class ScaleInteract : MonoBehaviour
{
    public bool open = false;
    public float openScale = 1.5f;
    public float closeScale = 1f;
    public float scaleDuration = 1f;
    public RectTransform rectTransform;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (open && rectTransform.localScale.x < openScale)
        {
            rectTransform.localScale += Vector3.one * (openScale - closeScale) / scaleDuration * Time.deltaTime;
        }
        else if (!open && rectTransform.localScale.x > closeScale)
        {
            rectTransform.localScale -= Vector3.one * (openScale - closeScale) / scaleDuration * Time.deltaTime;
        }
    }
    public void OnTriggerEnter2D(Collider2D other)
    {
        open = true;
        if (transform.GetComponent<AudioSource>().isPlaying) return;
        transform.GetComponent<AudioSource>().Play();
    } 
    public void OnTriggerExit2D(Collider2D other)
    {
        open = false;
    }
}
