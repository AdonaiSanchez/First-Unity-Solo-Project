using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject enemyType;
    public GameObject gameManager;
    public Transform spawnpoint;

    public float spawnCool = 1f;

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
        if(canSpawn && !spawnerBlocked && gameManager.GetComponent<GameManager>().enemiesSpawned < gameManager.GetComponent<GameManager>().enemyCap)
        {
            GameObject p = Instantiate(enemyType, spawnpoint.position, spawnpoint.rotation);
            spawnerBlocked = true;
            gameManager.GetComponent<GameManager>().enemiesAlive++;
            gameManager.GetComponent<GameManager>().enemiesSpawned++;

            StartCoroutine("spawnCooldown");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other);
        if (other.gameObject.tag == "Player")
        {
            spawnerBlocked = true;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        Debug.Log(other);
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
