using UnityEngine;

public class TitleSlideAndBob : MonoBehaviour
{
    [Header("Slide Settings")]
    public float wantedX = 0f;
    public float wantedY = 161f;

    public float startX = -1000f;  // off-screen start
    public float startY = 161f;

    public float moveSpeed = 800f;

    [Header("Bobbing Settings")]
    public float bobbingAmplitude = 5f;
    public float bobbingFrequency = 2f;

    private RectTransform rect;
    private bool reachedTarget = false;

    private float baseX;
    private float baseY;

    void Start()
    {
        rect = GetComponent<RectTransform>();

        // Start position
        rect.anchoredPosition = new Vector2(startX, startY);

        baseX = wantedX;
        baseY = wantedY;
    }

    void Update()
    {
        if (!reachedTarget)
        {
            MoveTowardsTarget();
        }
        else
        {
            Bob();
        }
    }

    void MoveTowardsTarget()
    {
        Vector2 current = rect.anchoredPosition;
        Vector2 target = new Vector2(wantedX, wantedY);

        rect.anchoredPosition =
            Vector2.MoveTowards(current, target, moveSpeed * Time.deltaTime);

        if (Vector2.Distance(rect.anchoredPosition, target) < 0.1f)
        {
            reachedTarget = true;
        }
    }

    void Bob()
    {
        float offset = Mathf.Sin(Time.time * bobbingFrequency) * bobbingAmplitude;

        rect.anchoredPosition =
            new Vector2(baseX, baseY + offset);
    }
}