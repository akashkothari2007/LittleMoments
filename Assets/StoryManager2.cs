using UnityEngine;
using UnityEngine.UI;
using System.Collections;
public class StoryManager2 : MonoBehaviour
{

    public enum StoryState
    {
        TurnOnLights,
        FixPicturePuzzle,
        StokeFire,
        TurnOnTV,
        None,
    }
    [Header("Story State")]
    public StoryState currentStoryState = StoryState.None;


    public Canvas dialogueBox;
    public TMPro.TextMeshProUGUI dialogueText;
    public AudioClip toggleDialogueAudio;
    public AudioSource audioSource;
    public AudioClip typeDialogueAudio;
    public AudioSource typeAudioSource;
    public Camera mainCamera;
    public TMPro.TextMeshProUGUI continuePrompt;
    private string wantedCurrentDialogue;
    private string currentDialogue;
    public string[] currentDialogues;
    public float typeDelay = 0.02f; // speed
    private Coroutine typingRoutine;
    private bool isTyping = false;
    private int currentIndex = -1;
    public SpecialEffects specialEffects;


    [Header("Specific Story Objects")]
    [Header("Turn On Lights")]
    public GameObject lights;
    public string[] turnOnLightsDialogues;
    public GameObject fuseBoxGlow;

    [Header("Fix Picture Puzzle")]

    public GameObject picturePuzzle;
    public string[] fixPicturePuzzleDialogues;
    public GameObject picturePuzzleGlow;

    [Header("Stoke Fire")]
    public GameObject fire;
    public string[] stokeFireDialogues;
    public GameObject fireGlow;

    [Header("Turn on TV")]
    public GameObject tv;
    public string[] turnOnTVDialogues;
    public GameObject tvGlow;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentStoryState = StoryState.TurnOnLights;
        currentDialogues = turnOnLightsDialogues;
        specialEffects.blackScreen = false;
        DialogueNext();
        ToggleDialogue(true);

        
    }



    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            HandleDialogueClick();
        }
 
    }

    public void OnFuseBoxFix()
    {
        if (currentStoryState == StoryState.TurnOnLights)
        {
            lights.SetActive(true);
            currentStoryState = StoryState.FixPicturePuzzle;
            currentDialogues = fixPicturePuzzleDialogues;
            currentIndex = -1;
            DialogueNext();
            ToggleDialogue(true);
            specialEffects.night = false;
            Debug.Log("Fuse box fixed, going to next story state!");

            fuseBoxGlow.SetActive(false);
            picturePuzzleGlow.SetActive(true);
        }
    }
    public void OnPicturePuzzleFix()
    {
        if (currentStoryState == StoryState.FixPicturePuzzle)
        {
            picturePuzzle.GetComponent<SpriteRenderer>().color = Color.white;
            
            currentStoryState = StoryState.StokeFire;
            Debug.Log("Picture puzzle fixed, going to next story state!");
            currentDialogues = stokeFireDialogues;
            currentIndex = -1;
            DialogueNext();
            ToggleDialogue(true);

            picturePuzzleGlow.SetActive(false);
            fireGlow.SetActive(true);
        }
    }
    public void OnFireStoked()
    {
        if (currentStoryState == StoryState.StokeFire)
        {
            fire.SetActive(true);
            currentStoryState = StoryState.TurnOnTV;
            Debug.Log("Fire stoked, going to next story state!");
            currentDialogues = turnOnTVDialogues;
            currentIndex = -1;
            DialogueNext();
            ToggleDialogue(true);

            fireGlow.SetActive(false);
            tvGlow.SetActive(true);
        }
    }
    public void OnTVTurnedOn()
    {
        if (currentStoryState == StoryState.TurnOnTV)
        {
            tv.SetActive(true);
            currentStoryState = StoryState.None;
            ToggleDialogue(false);
            Debug.Log("TV turned on, byebye");
            // set dialogues for next state here when we have them

            tvGlow.SetActive(false);
        }
    }



    public void ToggleDialogue(bool show)
    {
        audioSource.PlayOneShot(toggleDialogueAudio);
        dialogueBox.gameObject.SetActive(show);

    }
    public void DialogueNext()
    {
        typeAudioSource.Stop();
        currentIndex += 1;
        if (currentIndex == currentDialogues.Length - 1)
        {
            continuePrompt.text = "Restart";
        } else
        {
            continuePrompt.text = "Next";
        }
        if (currentIndex >= currentDialogues.Length)
        {
            currentIndex = 0;
        }
        wantedCurrentDialogue = currentDialogues[currentIndex];
        audioSource.PlayOneShot(toggleDialogueAudio);
        StartTyping(wantedCurrentDialogue);
    }

    void HandleDialogueClick()
    {

        Vector2 mouseWorldPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mouseWorldPos, Vector2.zero);

        if (!hit.collider) return;

        if (hit.collider.CompareTag("Knight"))
        {
            if (!string.IsNullOrEmpty(wantedCurrentDialogue))
            {
                bool isActive = dialogueBox.gameObject.activeSelf;
                ToggleDialogue(!isActive);
            }
        }
    }

    private void StartTyping(string fullText)
    {
        // stop previous typing
        if (typingRoutine != null)
            StopCoroutine(typingRoutine);

        typingRoutine = StartCoroutine(TypeLine(fullText));
    }

    private System.Collections.IEnumerator TypeLine(string fullText)
    {
        isTyping = true;
        currentDialogue = "";
        typeAudioSource.PlayOneShot(typeDialogueAudio);
        // If you want: hide the "Next" prompt while typing
        continuePrompt.gameObject.SetActive(false);

        for (int i = 0; i < fullText.Length; i++)
        {
            currentDialogue += fullText[i];
            dialogueText.text = currentDialogue; // update here (not in Update)
            yield return new WaitForSeconds(typeDelay);
        }
        typeAudioSource.Stop();
        isTyping = false;

        continuePrompt.gameObject.SetActive(true);
    }
   
}