using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject enemyType;
    public GameObject gameManager;
    public Transform spawnpoint;

    public int waveLock;

    public float spawnCool = 1f;

    public bool singleSpawn = false;
    public bool spawnerBlocked;
    public bool canSpawn = true;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        spawnpoint = GetComponent<Transform>();
        gameManager = GameObject.Find("GameManager");
    }

    // Update is called once per frame
    void Update()
    {
        if(canSpawn && !spawnerBlocked && gameManager.GetComponent<GameManager>().enemiesSpawned < gameManager.GetComponent<GameManager>().enemyCap && gameManager.GetComponent<GameManager>().waveCount >= waveLock && gameManager.GetComponent<GameManager>().wavesActive == true)
        {
            GameObject p = Instantiate(enemyType, spawnpoint.position, spawnpoint.rotation);
            gameManager.GetComponent<GameManager>().enemiesAlive++;
            gameManager.GetComponent<GameManager>().enemiesSpawned++;

            StartCoroutine("spawnCooldown");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            spawnerBlocked = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            spawnerBlocked = false;
        }
    }

    IEnumerator spawnCooldown()
    {
        canSpawn = false;

        yield return new WaitForSeconds(spawnCool);

        canSpawn = true;
    }
}
