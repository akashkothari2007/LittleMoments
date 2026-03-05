using UnityEngine;

public class WaterPlant : MonoBehaviour, IInteractable
{
    private bool watered = false;
    public Color finalColor;
    private Color initialColor;
    public float duration;
    private float elapsed = 0f;
    private SpriteRenderer sr;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        sr = transform.GetComponent<SpriteRenderer>();
        initialColor = sr.color;
    }

    // Update is called once per frame
    void Update()
    {
        if (watered && sr.color != finalColor)
        {
            elapsed += Time.deltaTime;
            sr.color = Color.Lerp(initialColor, finalColor, elapsed / duration);
        }
    }
    public void Interact(Inventory inventory)
    {
        if (watered) return;
        transform.GetComponent<AudioSource>().Play();
        watered = true;
    }
}
