using UnityEngine;
using UnityEngine.UI;

public class TurnOnFire : MonoBehaviour, IInteractable
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public StoryManager2 storyManager;
    public GameObject canvas;
    public RectTransform arrow;
    
    public float minAngle = 125;
    public float maxAngle = 235;
    public float minWinAngle = 156;
    public float maxWinAngle = 208;
    
    public float[] levelSpeed = new float[] {0.5f, 0.75f, 1f, 1.25f, 1.5f};
    public float[] fireSizes = new float[] {0.2f, 0.4f, 0.6f, 0.8f, 1f};
    public Image[] hearts;
    
    public float scoreToWin = 5f;
    
    public float lives = 3f;

    private bool gamePlaying = false;
    private bool gameFinished = false;
    private int currentScore = 0;
    private float curLives;
    private float curAngle;
    private bool goingRight = true;
    private bool scoredThisRound = false;

    public AudioClip winSound;
    public AudioClip loseSound;
    public AudioClip fireSound;
    public GameObject fire;

    private AudioSource audioSource;

    
    // Start is called once 
    public void Interact(Inventory inventory)
    {
        if (gameFinished || storyManager.currentStoryState != StoryManager2.StoryState.StokeFire) return;
        if (!gamePlaying)
        {
            OpenUI();
            fire.SetActive(true);
            fire.transform.localScale = Vector3.zero;
            gamePlaying = true;
        }
        else
        {
            if (curAngle >= minWinAngle && curAngle <= maxWinAngle)
            {
                if (!scoredThisRound) {
                    currentScore += 1;
                    scoredThisRound = true;
                    audioSource.PlayOneShot(fireSound);
                    fire.transform.localScale = new Vector3(fireSizes[currentScore - 1], fireSizes[currentScore - 1], fireSizes[currentScore - 1]);
                    if (currentScore >= scoreToWin)
                    {
                        WinGame();
                        CloseUI();
                    }
                }
            }
            else
            {
                curLives -= 1;
                audioSource.PlayOneShot(loseSound);

                UpdateHearts();
                if (curLives <= 0)
                {
                    fire.SetActive(false);
                    LoseGame();
                    CloseUI();
                }
            }
        }
    }
    public void LoseGame()
    {
        currentScore = 0;
        curLives = lives;
        gamePlaying = false;

    }
    public void WinGame()
    {
        gamePlaying = false;
        gameFinished = true;
        audioSource.PlayOneShot(winSound);
        storyManager.OnFireStoked();
    }
    public void OpenUI()
    {
        canvas.SetActive(true);
        for (int i = 0; i < hearts.Length; i++)
        {
            hearts[i].color = Color.white;
        }
        UpdateHearts();
    }
    public void UpdateHearts()
    {
        
        for (int i = 0; i < hearts.Length; i++)
        {
            if (i < curLives)
            {
                hearts[i].color = Color.white;
            }
            else
            {
                hearts[i].color = Color.black;
            }
        }
    }
    public void CloseUI()
    {
        canvas.SetActive(false);
    }
    void Start()
    {
        curAngle = arrow.localEulerAngles.z;
        curLives = lives;
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!gamePlaying || currentScore >= levelSpeed.Length) return;
        if (!goingRight)
        {
            curAngle += levelSpeed[currentScore] * Time.fixedDeltaTime * 100;
            if (curAngle >= maxAngle)
            {
                curAngle = maxAngle;
                scoredThisRound = false;
                goingRight = true;
            }
        }
        else
        {
            curAngle -= levelSpeed[currentScore] * Time.fixedDeltaTime * 100;
            if (curAngle <= minAngle)
            {
                curAngle = minAngle;
                scoredThisRound = false;
                goingRight = false;
            }
        }
        Debug.Log(curAngle);
        arrow.localEulerAngles = new Vector3(0, 0, curAngle);
    }
}
