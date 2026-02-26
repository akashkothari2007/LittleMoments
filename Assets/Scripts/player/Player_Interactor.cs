using UnityEngine;

public class Player_Interactor : MonoBehaviour
{
    public Inventory inventory;
    public float distanceToInteract = 1f;
    private Camera cam;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // left click
        
        {
            Vector2 mouseWorldPos = cam.ScreenToWorldPoint(Input.mousePosition);

            RaycastHit2D hit = Physics2D.Raycast(mouseWorldPos, Vector2.zero);

            if (hit.collider != null)
            {
                // print distance from player to clicked object
                float distance = Vector2.Distance(transform.position, hit.collider.gameObject.transform.position);
                if (distance <= distanceToInteract)
                {
                    var interactable = hit.collider.gameObject.GetComponent<IInteractable>();
                    if (interactable != null)
                    {
                        interactable.Interact(inventory);
                    } 
                }
                
                
            }
        }
    }
}
