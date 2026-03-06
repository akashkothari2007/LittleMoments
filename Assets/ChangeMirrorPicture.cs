using UnityEngine;

public class ChangeMirrorPicture : MonoBehaviour, IInteractable
{
    public SpriteRenderer[] pictures;
    private SpriteRenderer curImage;
    private SpriteRenderer prevImage;
    public SpriteRenderer firstPic;
    public float fadeSpeed = 1f;
    public bool open = false;
    public RectTransform rectTransform;
    public float finalScale = 1.5f;
    public float initialScale = 1f;
    public float duration = 1f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (open && rectTransform.localScale.x < finalScale)
        {
            rectTransform.localScale += Vector3.one * (finalScale - initialScale) / duration * Time.deltaTime;
        }
        if (prevImage && prevImage.color.a > 0)
        {
            Color prevColor = prevImage.color;
            prevImage.color = prevColor;
        }
        if (curImage && curImage.color.a < 1)       
        {
            Color curColor = curImage.color;
            curColor.a += fadeSpeed * Time.deltaTime;
            curImage.color = curColor;
        }
    }
    public void Interact(Inventory inventory)
    {
        if (!open) {
            transform.GetComponent<AudioSource>().Play();
            curImage = firstPic;
            open = true;
            return;
        }
        
        if (prevImage)
        {
            Color curColor = prevImage.color;
            curColor = new Color(curColor.r, curColor.g, curColor.b, 0);
            prevImage.color = curColor;
        }
        if (curImage)
        {
            prevImage = curImage;
            
        }
        curImage = pictures[Random.Range(0, pictures.Length)];
    }
}
