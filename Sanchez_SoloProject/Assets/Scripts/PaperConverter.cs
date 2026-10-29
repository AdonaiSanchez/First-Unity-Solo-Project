using System.Collections;
using TMPro;
using UnityEngine;

public class PaperConverter : MonoBehaviour
{
    public bool menuActive = false;
    public bool converting = false;

    public int insertAmt;
    public int paperScraps;
    public int paperSheets;

    public float conversionCooldown;

    public PlayerController player;
    public GameObject gameManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        gameManager = GameObject.Find("GameManager");
    }

    // Update is called once per frame
    void Update()
    {
        gameManager.GetComponent<GameManager>().scrapsCounter.text = "" + paperScraps;
        gameManager.GetComponent<GameManager>().sheetsCounter.text = "" + paperSheets;

        if (paperScraps >= 10 && !converting)
        {
            StartCoroutine("ConversionCool");
        }
    }

    public void InsertScraps()
    {
        if(player.paperScraps >= 10)
        {
            player.paperScraps -= 10;
            paperScraps += 10;
        }
    }

    public void CollectPaper()
    {
        player.paperSheets += paperSheets;
        paperSheets = 0;
    }

    IEnumerator ConversionCool()
    {
        converting = true;

        yield return new WaitForSeconds(conversionCooldown);

        paperScraps -= 10;
        paperSheets++;
        converting = false;
    }
}
