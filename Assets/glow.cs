using UnityEngine;
using UnityEngine.Rendering.Universal;

public class glow : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public float maxIntensity = 2f;
    public float minIntensity = 0f;
    public float pulseSpeed = 1f;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //smoothly pulse glow from min to max to min so on
        float intensity = (Mathf.Sin(Time.time * pulseSpeed) + 1f) / 2f * (maxIntensity - minIntensity) + minIntensity;
        transform.GetComponent<Light2D>().intensity = intensity;
    }
}
