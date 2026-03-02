using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;

public class SlidingPuzzle : MonoBehaviour, IInteractable
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public StoryManager2 storyManager;
    public GameObject slidingPuzzleUI;
    private bool gameComplete = false;
    public GameObject Player;

    public Image[] images;
    public Sprite[] sprites;
    public AudioClip movePiece;
    public AudioClip winSound;
    private int[][] wantedGrid = new int[][]
    {
        new int[] {1, 2, 3},
        new int[] {4, 5, 6},
        new int[] {7, 8, 9} 
    };
    // actual grid with numbers assigned random in start
    private int[][] currentGrid = new int[3][];
    void Start()
    {
        currentGrid[0] = new int[3];
        currentGrid[1] = new int[3];
        currentGrid[2] = new int[3];
        ShuffleUI();

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void ComputeGrid()
    {
        int cur = 0;
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                images[cur].sprite = sprites[currentGrid[i][j] - 1];
                if (currentGrid[i][j] == 9)
                {
                    images[cur].color = new Color(1, 1, 1, 0);
                }
                else
                {
                    images[cur].color = new Color(1, 1, 1, 1);
                }
                cur ++;
            }

        }
    }
    public void PressTile(int tileNum)
    {

        int row = tileNum / 3;
        int col = tileNum % 3;
        if (currentGrid[row][col] == 9) return; // empty tile, do nothing
        // check if adjacent to empty tile
        int r = -1;
        int c = -1;
        if (row > 0 && currentGrid[row - 1][col] == 9) // up
        {
            r = row - 1;
            c = col;
        }
        else if (row < 2 && currentGrid[row + 1][col] == 9) // down
        {
            r = row + 1;
            c = col;
        }
        else if (col > 0 && currentGrid[row][col - 1] == 9) // left
        {
            r = row;
            c = col - 1;
        }
        else if (col < 2 && currentGrid[row][col + 1] == 9) // right
        {
            r = row;
            c = col + 1;
        }
        if (r == -1) return; // not adjacent to empty tile, do nothing
        Debug.Log("swapping " + currentGrid[row][col] + " with 9");
        int temp = currentGrid[row][col];
        currentGrid[row][col] = 9;
        currentGrid[r][c] = temp;
        ComputeGrid();
        CheckWin();
        transform.GetComponent<AudioSource>().PlayOneShot(movePiece);
        
    }
    public void CheckWin()
    {
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                if (currentGrid[i][j] != wantedGrid[i][j]) return;
            }
        }
        completePuzzle();
    }
    public void Interact(Inventory inventory)
    {
        if (gameComplete || storyManager.currentStoryState != StoryManager2.StoryState.FixPicturePuzzle) return;
        slidingPuzzleUI.SetActive(true);
        Cursor.visible = true;
        Player.GetComponent<Player_Controller>().enabled = false;

    }
    public void closeUI()
    {
        slidingPuzzleUI.SetActive(false);
        Cursor.visible = false;
        Player.GetComponent<Player_Controller>().enabled = true;
    }
    public void completePuzzle()
    {
        
        gameComplete = true;
        storyManager.OnPicturePuzzleFix();
        StartCoroutine(winCoroutine());
        
        
    }
    public void ShuffleUI()
    {
        List<int> listOfNums = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
        for (int i = 0; i < 3; i++ )
        {
            for (int j = 0; j < 3; j++)
            {
                int randIndex = Random.Range(0, listOfNums.Count);
                int num = listOfNums[randIndex];
                currentGrid[i][j] = num;
                listOfNums.RemoveAt(randIndex);
                Debug.Log(num);
            }

           
        }
        if (!IsSolvable())
        {
            // swap any two non-empty tiles to flip parity
            if (currentGrid[0][0] != 9 && currentGrid[0][1] != 9)
            {
                int temp = currentGrid[0][0];
                currentGrid[0][0] = currentGrid[0][1];
                currentGrid[0][1] = temp;
            }
            else
            {
                int temp = currentGrid[2][1];
                currentGrid[2][1] = currentGrid[2][2];
                currentGrid[2][2] = temp;
            }
        }
        
        ComputeGrid();
    }
    bool IsSolvable()
    {
        int[] flat = new int[9];
        int k = 0;
        for (int i = 0; i < 3; i++)
            for (int j = 0; j < 3; j++)
                flat[k++] = currentGrid[i][j];

        int inversions = 0;
        for (int i = 0; i < 9; i++)
            for (int j = i + 1; j < 9; j++)
                if (flat[i] != 9 && flat[j] != 9 && flat[i] > flat[j])
                    inversions++;

        return inversions % 2 == 0;
    }

    public IEnumerator winCoroutine()
    {
        //slowly fade in the picture puzzle sprite over 2 seconds
        transform.GetComponent<AudioSource>().PlayOneShot(winSound); // play sound on win
        Image finalTile = images[8]; // the empty tile becomes the final image
        //currently alpha is 0, we want to fade it in to 1
        float duration = 1f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Clamp01(elapsed / duration);
            finalTile.color = new Color(1, 1, 1, alpha);
            yield return new WaitForEndOfFrame();
        }
        yield return new WaitForSeconds(1f);
        closeUI();
    }

    
}
