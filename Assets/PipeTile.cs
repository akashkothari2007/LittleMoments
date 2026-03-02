using UnityEngine;
using UnityEngine.UI;

public class PipeTile : MonoBehaviour
{
    public enum PipeType { Straight, Elbow, T, Cross, Source, Target }

    public PipeType pipeType;
    public bool powered;

    [Header("Visuals")]
    public Color offColor = Color.gray;
    public Color onColor = Color.yellow;

    // Openings order: Up, Right, Down, Left
    public bool[] GetOpenings()
    {
        int cwSteps = GetRotationStepsCW(); // 0..3

        // Base openings at rotation = 0 (match your art rules)
        bool up = false, right = false, down = false, left = false;

        switch (pipeType)
        {
            case PipeType.Straight:
                up = true; down = true;                 // vertical
                break;

            case PipeType.Elbow:
                right = true; down = true;              // RIGHT + DOWN
                break;

            case PipeType.T:
                left = true; right = true; down = true; // LEFT + RIGHT + DOWN
                break;

            case PipeType.Cross:
                up = right = down = left = true;
                break;

            case PipeType.Source:
                up = true; // shoots RIGHT at base orientation
                break;

            case PipeType.Target:
                up = true;  // expects FROM LEFT at base orientation
                break;
        }

        // Rotate openings CLOCKWISE cwSteps times:
        // CW: Up->Right->Down->Left->Up
        for (int i = 0; i < cwSteps; i++)
        {
            bool oldUp = up;
            up = left;
            left = down;
            down = right;
            right = oldUp;
            // ^^^ THIS was your bug: your mapping was backwards relative to your step definition
        }

        return new bool[] { up, right, down, left };
    }

    int GetRotationStepsCW()
    {
        // Unity angles increase CCW. We convert to CW steps.
        float z = transform.eulerAngles.z % 360f;
        if (z < 0) z += 360f;

        int ccwSteps = Mathf.RoundToInt(z / 90f) % 4;
        int cwSteps = (4 - ccwSteps) % 4; // convert CCW to CW
        return cwSteps;
    }

    public void RotateCW()
    {
        // Visual clockwise for UI
        transform.Rotate(0f, 0f, -90f);
    }

    public void SetPoweredVisual(bool isPowered)
    {
        powered = isPowered;

        var img = GetComponent<Image>();
        if (img != null)
            img.color = isPowered ? onColor : offColor;
    }
}