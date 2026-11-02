using System.Collections;
using TMPro;
using UnityEngine;

public class SimpleCountdown : MonoBehaviour
{
    public TextMeshProUGUI timerText;
    
    public float timerLength;
    public float count;

    public bool timerActive;
    public bool incTimer;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (count > 0 && incTimer)
        {
            StartCoroutine("Timer");
        }

        if (timerActive)
        {
            timerText.text = "" + count;
        }
        
    }
    IEnumerator Timer()
    {
        incTimer = false;

        yield return new WaitForSeconds(1);

        count--;
        incTimer = true;
    }
}
