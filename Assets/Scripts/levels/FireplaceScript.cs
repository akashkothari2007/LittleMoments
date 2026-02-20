using UnityEngine;
using System.Collections;

public class FireplaceScript : MonoBehaviour, IInteractable
{

    public GameObject fire;
    public GameObject log1;
    public GameObject log2;
    public GameObject log3;
    public float logsPlaced = 0;
    

    public AudioSource startSfxSource;   // Source A
    public AudioSource loopSource;       // Source B

    public AudioClip igniteClip;
    public AudioClip logClip;

    public float loopTargetVolume = 0.6f;
    public float fadeInTime = 1.0f;
    public float startDelay = 0.2f; 
    public bool turnOffVolume = false;

    public StoryManager storyManager;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Interact(Inventory inventory)
    {
        if (storyManager.currentStoryState != StoryManager.StoryState.LightCampfire)
        {
            Debug.Log("Not the right time to light the campfire");
            return;
        }

        if (logsPlaced < 3)
        {
            if (inventory.TrySpend(ItemType.Log, 1))
            {
                logsPlaced++;
                startSfxSource.PlayOneShot(logClip);
                if (logsPlaced == 1)
                {
                    log1.SetActive(true);
                }
                else if (logsPlaced == 2)
                {
                    log2.SetActive(true);
                }
                else if (logsPlaced == 3)
                {
                    log3.SetActive(true);
                }
            }
        } else
        {
            if (inventory.TrySpend(ItemType.FlintAndSteel, 1))
            {
                LightFire();
                storyManager.OnFireLit();
                fire.SetActive(true);
                log1.SetActive(false);
                log2.SetActive(false);
                log3.SetActive(false);
            }
        }
    }

         // tiny delay after ignite

    public void LightFire()
    {
        // ignite sound
        startSfxSource.PlayOneShot(igniteClip);

        // start loop silent, then fade in
        loopSource.volume = 0f;
        loopSource.Play();
        StartCoroutine(FadeIn(loopSource, loopTargetVolume, fadeInTime, startDelay));
    }

    private IEnumerator FadeIn(AudioSource src, float target, float time, float delay)
    {
        if (delay > 0) yield return new WaitForSeconds(delay);

        float t = 0f;
        while (t < time)
        {
            t += Time.deltaTime;
            src.volume = Mathf.Lerp(0f, target, t / time);
            yield return null;
        }
        src.volume = target;
    }
    private void Update()
    {
        if (turnOffVolume && loopSource.volume > 0f)
        {
            loopSource.volume -= Time.deltaTime * (loopTargetVolume / fadeInTime);
            if (loopSource.volume <= 0f)
            {
                loopSource.volume = 0f;
                loopSource.Stop();
                transform.gameObject.SetActive(false); // Disable the fireplace object after the sound has faded out
            }
        }
    }
}
