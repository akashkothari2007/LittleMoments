using UnityEngine;
using System.Collections;

public class TypeHelper : MonoBehaviour
{
    public float timeToStart = 1f;
    public string textToType = "Hello, World!";
    public float delayBetweenChars = 0.1f;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        transform.GetComponent<TMPro.TextMeshProUGUI>().text = "";
        StartCoroutine(TypeCoroutine());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private IEnumerator TypeCoroutine()
    {
        yield return new WaitForSeconds(timeToStart);
         // clear text before typing
        transform.GetComponent<AudioSource>().Play();
        for (int i = 0; i < textToType.Length; i++)
        {
            //get TMP component
            transform.GetComponent<TMPro.TextMeshProUGUI>().text = textToType.Substring(0, i + 1);
            yield return new WaitForSeconds(delayBetweenChars); 
        }
        transform.GetComponent<AudioSource>().Stop();
    }
}
