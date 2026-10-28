using System.Collections;
using UnityEngine;

public class InkSynthesizer : MonoBehaviour
{
    public GameObject gameManager;

    public PlayerController player;

    public bool canCreate;

    public float cartridgeCooldown = 1f;

    public int createAmount = 1;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameManager = GameObject.Find("GameManager");
        player = GameObject.Find("Player").GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (gameManager.GetComponent<GameManager>().intermission && canCreate)
        {
            player.inkCartridges += createAmount;
            canCreate = false;
        }
        else if(!gameManager.GetComponent <GameManager>().intermission)
        {
            canCreate = true;
        }
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
