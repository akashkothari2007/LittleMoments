using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Flicker : MonoBehaviour
{
    [Header("Light Reference")]
    public Light2D light2D;   // for URP 2D lights
    public Light normalLight; // optional (if using regular Light)

    [Header("Base Settings")]
    public float baseIntensity = 1f;

    [Header("Flicker Range")]
    public float minIntensity = 0.7f;
    public float maxIntensity = 1.2f;

    [Header("Timing")]
    public float minTimeBetweenFlickers = 0.05f;
    public float maxTimeBetweenFlickers = 0.25f;

    public float flickerSpeed = 20f; // how fast it snaps

    private float targetIntensity;
    private float timer;

    void Start()
    {
        if (light2D == null)
            light2D = GetComponent<Light2D>();

        if (normalLight == null)
            normalLight = GetComponent<Light>();

        targetIntensity = baseIntensity;
        timer = Random.Range(minTimeBetweenFlickers, maxTimeBetweenFlickers);
    }

    void Update()
    {
        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            targetIntensity = Random.Range(minIntensity, maxIntensity);
            timer = Random.Range(minTimeBetweenFlickers, maxTimeBetweenFlickers);
        }

        float current = GetCurrentIntensity();
        float newIntensity = Mathf.Lerp(current, targetIntensity, Time.deltaTime * flickerSpeed);

        SetIntensity(newIntensity);
    }

    float GetCurrentIntensity()
    {
        if (light2D != null)
            return light2D.intensity;

        if (normalLight != null)
            return normalLight.intensity;

        return 0f;
    }

    void SetIntensity(float value)
    {
        if (light2D != null)
            light2D.intensity = value;

        if (normalLight != null)
            normalLight.intensity = value;
    }
}