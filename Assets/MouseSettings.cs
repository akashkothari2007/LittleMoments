using UnityEngine;

public class MouseSettings : MonoBehaviour
{
  

    [Header("Trail Follow")]
    public float distanceFromCamera = 10f; // how far in front of the camera
    public float trailScale = 1f;          // scales the whole trail object

    private Camera cam;

    void Start()
    {
        DontDestroyOnLoad(gameObject);

        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = false;

        cam = Camera.main;

        ApplyScale();
    }

    void Update()
    {
        if (cam == null) cam = Camera.main;
        if (cam == null) return;
        

        Vector3 m = Input.mousePosition;
        m.z = distanceFromCamera; 
        transform.position = cam.ScreenToWorldPoint(m);
    }

    public void ApplyScale()
    {
        transform.localScale = Vector3.one * trailScale;
    }
}