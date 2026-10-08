using UnityEngine;

public class PaperSynthisizer : MonoBehaviour
{

    public GameObject gameManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameManager = GameObject.Find("GameManager");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void startWave()
    {
        if (!gameManager.GetComponent<GameManager>().wavesActive)
        {
            gameManager.GetComponent<GameManager>().wavesActive = true;
            gameManager.GetComponent<GameManager>().waveCount++;
        }
    }
}
