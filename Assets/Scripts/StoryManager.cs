using UnityEngine;
using UnityEngine.UI;
using System.Collections;
public class StoryManager : MonoBehaviour
{

    public enum StoryState
    {
        Intro,
        FlowerPlanting,
        PathBuilding,
        LightCampfire,
        FinishLightCampfire,
        CampfireCutscene,
        EnterHouse,
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
     

    [Header("Specific State Variables")]
    
    [Header("Intro")]
    public string[] introDialogues;

    [Header("Flower Planting")]
    public GameObject[] gardenPlots;
    public string[] plantFlowerDialogues;
    public int flowersNeeded = 8;
    public int flowersPlanted = 0;


    [Header("Path Building")]
    public GameObject[] pathSpots;
    public string[] placeStoneDialogues;
    public int stonesNeeded = 7;
    public int stonesPlaced = 0;

    [Header("Campfire Lighting")]
    public GameObject fireSpot;
    public string[] lightFireDialogues;
    public bool fireLit = false;

    [Header("Finish Campfire Lighting")]
    public string[] finishLightFireDialogues;
    public SpecialEffects specialEffects;
    
    [Header("Campfire Cutscene")]

    private bool cutscenePlayed = false;
    public GameObject cutsceneObjects;
    public GameObject player;
    public GameObject knight;
    public AudioClip guitarClip;
    public FireplaceScript campfire;

    [Header("Enter House")]
    public string[] enterHouseDialogues;
    public GameObject houseLight;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentStoryState = StoryState.Intro;
        currentDialogues = introDialogues;
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
        switch (currentStoryState)
        {
            case StoryState.Intro: 
                if (currentIndex >= currentDialogues.Length - 1) //IF YOU FINISH DIALOGUE GO ONTO FLOWER PLANTING
                {
                    currentStoryState = StoryState.FlowerPlanting;
                    for (int i = 0; i < gardenPlots.Length; i++)
                    {
                        //get the child of each and set it active
                        gardenPlots[i].transform.GetChild(0).gameObject.SetActive(true);
                    }
                    currentDialogues = plantFlowerDialogues;
                    currentIndex = -1;
                    DialogueNext();
                    ToggleDialogue(true);
                    Debug.Log("Intro complete! Moving to next story state.");
                }
                break;

            case StoryState.FlowerPlanting: 
                if (flowersPlanted >= flowersNeeded) //IF YOU PLANT ALL FLOWERS GO ONTO NEXT DIALOGUE AND PATH BUILDING
                {
   
                    for (int i = 0; i < gardenPlots.Length; i++)
                    {
                        gardenPlots[i].transform.GetChild(0).gameObject.SetActive(false);
                    }
                    for (int i = 0; i < pathSpots.Length; i++)
                    {
                        pathSpots[i].SetActive(true);
                    }
                    currentStoryState = StoryState.PathBuilding;
                    currentDialogues = placeStoneDialogues;
                    currentIndex = -1;
                    DialogueNext();
                    ToggleDialogue(true);
                    Debug.Log("All flowers planted! Moving to next story state.");
                }
                break;
            case StoryState.PathBuilding:
                if (stonesPlaced >= stonesNeeded) //IF U BUILD PATH GO ONTO NEXT DIALOGUE AND CAMPFIRE LIGHTING
                {
                    for (int i = 0; i < pathSpots.Length; i++)
                    {
                        pathSpots[i].SetActive(false);
                    }
                    fireSpot.SetActive(true);
                    currentStoryState = StoryState.LightCampfire;
                    currentDialogues = lightFireDialogues;
                    currentIndex = -1;
                    DialogueNext();
                    ToggleDialogue(true); 
                    Debug.Log("All stones placed! Moving to next story state.");
                }
                break;
            case StoryState.LightCampfire:
                if (fireLit) //IF YOU LIGHT CAMPFIRE GO ONTO NEXT DIALOGUE 
                {
                    fireSpot.SetActive(false);
                    currentStoryState = StoryState.FinishLightCampfire;
                    currentDialogues = finishLightFireDialogues;
                    currentIndex = -1;
                    DialogueNext();
                    ToggleDialogue(true);
                    Debug.Log("Fire lit!, temporary dialogue");
                    specialEffects.night = true;
                    specialEffects.musicVolume = 0f; 
                }
                break;
            case StoryState.FinishLightCampfire:
                if (currentIndex >= currentDialogues.Length - 1) //ONCE U FINISH DIALOGUE START COROUTINE!
                {
                    currentStoryState = StoryState.CampfireCutscene;
                    StartCoroutine(CampfireCutScene());
                    ToggleDialogue(false);
                    Debug.Log("Finish light campfire! Moving to next story state.");
                }
                break;
            case StoryState.CampfireCutscene:
                if (cutscenePlayed)
                {
                    
                    currentStoryState = StoryState.EnterHouse; //ONCE CUTSCENE PLAYS (coroutine ends) WAIT TO GO IN HOUSE
                    currentDialogues = enterHouseDialogues;
                    houseLight.SetActive(true);
                    currentIndex = -1;
                    DialogueNext();
                    ToggleDialogue(true);
                    Debug.Log("Cutscene played! Moving to next story state. Waiting to go into house");
                }
                break;
            case StoryState.EnterHouse:
                if ((player.transform.position - houseLight.transform.position).magnitude < 0.5f)
                {
                    currentStoryState = StoryState.None;
                    StartCoroutine(EnterHouseCutScene());
                    ToggleDialogue(false);
                    Debug.Log("Entered house.... tbd");
                    
                }
                break;
            
        }
    }

    public void OnFlowerPlanted()
    {
        flowersPlanted++;
    }
    public void OnStonePlaced()
    {
        stonesPlaced++;
    }
    public void OnFireLit()
    {
        fireLit = true;
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
    private IEnumerator CampfireCutScene()
    {
        
        specialEffects.blackScreen = true;

        yield return new WaitForSeconds(3f);
        player.SetActive(false);
        knight.SetActive(false);
        cutsceneObjects.SetActive(true);

        specialEffects.blackScreen = false;

        yield return new WaitForSeconds(2f);

        audioSource.PlayOneShot(guitarClip);
        yield return new WaitForSeconds(guitarClip.length);

        specialEffects.blackScreen = true;
        yield return new WaitForSeconds(4f);
        specialEffects.night = false;
        campfire.turnOffVolume = true; 
        yield return new WaitForSeconds(3f);
        player.SetActive(true);
        knight.SetActive(true);
        cutsceneObjects.SetActive(false);

        specialEffects.blackScreen = false;
        
        specialEffects.musicVolume = 1f; 
        cutscenePlayed = true;

    }

    private IEnumerator EnterHouseCutScene()
    {
        specialEffects.blackScreen = true;
        specialEffects.musicVolume = 0f;
        yield return new WaitForSeconds(3f);
        // tbd

    }


}