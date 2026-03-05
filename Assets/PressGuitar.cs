using UnityEngine;
using System.Collections;

public class PressGuitar : MonoBehaviour, IInteractable
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public AudioClip[] guitarRiffs;
    public AudioSource AudioSource;
    public SpecialEffects specialEffects;
    private Coroutine resetVolumeCoroutine;
    void Start()
    {
        AudioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void Interact(Inventory inventory)
    {
        //if coroutine is not null, stop it
        if (resetVolumeCoroutine != null) StopCoroutine(resetVolumeCoroutine);
        specialEffects.musicVolume = 0;
        int randomIndex = Random.Range(0, guitarRiffs.Length);
        AudioClip randomRiff = guitarRiffs[randomIndex];
        AudioSource.Stop();
        AudioSource.PlayOneShot(randomRiff);
        resetVolumeCoroutine = StartCoroutine(ResetMusicVolume(randomRiff.length));
    }
    // coroutine so when audio stops we can reset the music volume

    private IEnumerator ResetMusicVolume(float delay)
    {
        yield return new WaitForSeconds(delay);
        specialEffects.musicVolume = 1;
    }
}
