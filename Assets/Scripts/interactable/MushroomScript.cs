using UnityEngine;
using System.Collections;

public class MushroomScript : MonoBehaviour, IInteractable
{
    public SpecialEffects specialEffects;
    public float effectDuration = 10f; // duration of drug effects in seconds
    public void Start()
    {
        
    }
    public void Interact(Inventory inventory)
    {
        if (specialEffects.drugged) return; // already drugged
        specialEffects.drugged = true;
        transform.GetComponent<AudioSource>().Play();
        transform.GetComponent<SpriteRenderer>().enabled = false; // play mushroom sfx
        //start a coroutine to end drug effects after duration\
        StartCoroutine(EndDrugEffectsAfterDelay());
    }

    private IEnumerator EndDrugEffectsAfterDelay()
    {
        yield return new WaitForSeconds(effectDuration);
        transform.GetComponent<SpriteRenderer>().enabled = true; // reset mushroom sprite
        specialEffects.drugged = false;
    }
}
