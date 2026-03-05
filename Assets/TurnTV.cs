using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using System.Collections;

public class TurnTV : MonoBehaviour, IInteractable
{
    public StoryManager2 storyManager;
    public SpecialEffects sp;
    public Player_Controller pc;
    public VideoPlayer videoPlayer;
    public RawImage videoScreen; // the RawImage showing the render texture
    public AudioSource sillyAudio;
    public AudioSource fireworkAudio;
    private bool fadeAudio = false;
    float elapsed = 0f;
    public float fadeAudioDuration = 1f;
    public GameObject happyText;

    [Header("Antenna")]
    public Transform antenna; // the antenna gameobject (not the sprite child)
    public float targetAngle = 45f; // sweet spot, change to whatever feels good
    public float tolerance = 15f; // how close she needs to be
    public float holdTime = 1f; // seconds to hold in sweet spot

    [Header("Audio")]
    public AudioSource staticSound; // plays while tuning
    public AudioSource successSound; // plays on win

    private bool isTuning = false;
    private bool gameComplete = false;
    private float holdTimer = 0f;
    private float mouseDragSensitivity = 1.5f;
    private Coroutine currentCoroutine;

    public void Interact(Inventory inventory)
    {
        if (storyManager.currentStoryState == StoryManager2.StoryState.End)
        {
            if (currentCoroutine != null) StopCoroutine(currentCoroutine);
            // if we interact with the TV after the cutscene, just replay the cutscene
            currentCoroutine = StartCoroutine(VideoCoroutine());
            return;
        }
        if (gameComplete) return;
        if (storyManager.currentStoryState != StoryManager2.StoryState.TurnOnTV) return;
        if (isTuning) return; // prevent re-interacting while tuning
        isTuning = true;
        if (staticSound != null) staticSound.Play();
        
    }

    void Update()
    {
        if (!isTuning || gameComplete) return;

        // rotate antenna with mouse drag
        if (Input.GetMouseButton(0))
        {
            float mouseDelta = Input.GetAxis("Mouse X");
            antenna.Rotate(0f, 0f, -mouseDelta * mouseDragSensitivity);
        }

        // clamp rotation so it doesnt spin forever
        float currentAngle = antenna.eulerAngles.z;
        // convert to -180 to 180 range
        if (currentAngle > 180f) currentAngle -= 360f;
        currentAngle = Mathf.Clamp(currentAngle, -90f, 90f);
        antenna.eulerAngles = new Vector3(0f, 0f, currentAngle);

        // check if in sweet spot
        float diff = Mathf.Abs(currentAngle - targetAngle);
        if (diff <= tolerance)
        {
            holdTimer += Time.deltaTime;
            if (holdTimer >= holdTime)
            {
                Win();
            }
        }
        else
        {
            holdTimer = 0f; // reset if she moves out
        }

        if (fadeAudio) {
            elapsed += Time.deltaTime;
            sillyAudio.volume = Mathf.Lerp(sillyAudio.volume, 0f, elapsed / fadeAudioDuration );
            if (sillyAudio.volume <= 0.01f) {
                sillyAudio.Stop();
                sillyAudio.volume = 0f; 
                fadeAudio = false;
            }

        }
    }

    void Win()
    {
        gameComplete = true;
        StartCoroutine(VideoCoroutine());
        isTuning = false;
        if (staticSound != null) staticSound.Stop();
        if (successSound != null) successSound.Play();
        storyManager.OnTVTurnedOn();
    }

    IEnumerator VideoCoroutine()
    {
        // disable player
        pc.enabled = false;

        // fade to black
        sp.blackScreen = true;
        yield return new WaitForSeconds(1.5f); // wait for black screen to fully appear

        // prepare video
        videoScreen.gameObject.SetActive(true);
        videoScreen.color = new Color(1, 1, 1, 0);
        videoPlayer.Prepare();
        yield return new WaitUntil(() => videoPlayer.isPrepared);
        videoPlayer.Play();

        // fade black screen away to reveal video
        sp.blackScreen = false;
        float elapsed = 0f;
        float fadeIn = 1f;
        while (elapsed < fadeIn)
        {
            elapsed += Time.deltaTime;
            videoScreen.color = new Color(1, 1, 1, Mathf.Clamp01(elapsed / fadeIn));
            yield return null;
        }
        videoScreen.color = new Color(1, 1, 1, 1);

        // wait for video to finish

        yield return new WaitUntil(() => !videoPlayer.isPlaying);

        // fade to black
        sp.blackScreen = true;
        yield return new WaitForSeconds(1.5f);

        // hide video
        videoScreen.gameObject.SetActive(false);

        happyText.SetActive(true);
        sillyAudio.Play();
        yield return new WaitForSeconds(1f); // let the audio start before fading
        fadeAudio = true;
        fireworkAudio.Play();
        yield return new WaitForSeconds(4f); // let the player see the happy text
        happyText.SetActive(false);
        // fade back in
        sp.blackScreen = false;
        yield return new WaitForSeconds(1f); // give it a moment to fade in

        // re-enable player
        pc.enabled = true;
        storyManager.OnCutSceneEnd();
    }
}