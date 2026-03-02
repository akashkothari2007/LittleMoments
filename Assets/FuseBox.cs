using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class FuseBox : MonoBehaviour, IInteractable
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public StoryManager2 storyManager;
    public GameObject fuseBoxUI;
    private bool gameComplete = false;
    public GameObject Player;
    public Color flashColor;
    public Image finalImage;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void Interact(Inventory inventory)
    {
        if (gameComplete || storyManager.currentStoryState != StoryManager2.StoryState.TurnOnLights) return;
        fuseBoxUI.SetActive(true);
        Cursor.visible = true;
        Player.GetComponent<Player_Controller>().enabled = false;

    }
    public void closeUI()
    {
        fuseBoxUI.SetActive(false);
        Cursor.visible = false;
        Player.GetComponent<Player_Controller>().enabled = true;
    }
    public void completePuzzle()
    {
        
        
        storyManager.OnFuseBoxFix();
        gameComplete = true;
        StartCoroutine(winGameCoroutine());
        
    }

    private IEnumerator winGameCoroutine()
    {
        transform.GetComponent<AudioSource>().Play(); // play sound on win
        finalImage.color = flashColor;
        //flash the ui with the specified color and back to white repeadetly a few times
         for (int i = 0; i < 4; i++)
        {
            fuseBoxUI.GetComponent<Image>().color = flashColor;
            yield return new WaitForSeconds(0.3f);
            fuseBoxUI.GetComponent<Image>().color = Color.white;
            yield return new WaitForSeconds(0.3f);
        }
        closeUI();

    }
}
