using UnityEngine;

public class TitleFloat : MonoBehaviour
{
    public float wantedY = 161f;
    public float startY = 600f;     // off screen start
    public float moveSpeed = 500f;  // pixels per second

    public float bobbingAmplitude = 5f;   // how much it bobs
    public float bobbingFrequency = 2f;   // speed of bob

    private RectTransform rect;
    private bool reachedTarget = false;
    private float baseY;

    void Start()
    {
        rect = GetComponent<RectTransform>();

        // Start high up
        rect.anchoredPosition = new Vector2(rect.anchoredPosition.x, startY);

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
        Vector2 target = new Vector2(current.x, wantedY);

        rect.anchoredPosition =
            Vector2.MoveTowards(current, target, moveSpeed * Time.deltaTime);

        if (Mathf.Abs(rect.anchoredPosition.y - wantedY) < 0.1f)
        {
            reachedTarget = true;
        }
    }

    void Bob()
    {
        float offset = Mathf.Sin(Time.time * bobbingFrequency) * bobbingAmplitude;
        rect.anchoredPosition =
            new Vector2(rect.anchoredPosition.x, baseY + offset);
    }
}