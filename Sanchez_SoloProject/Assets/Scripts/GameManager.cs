using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.InputSystem.Android;

public class GameManager : MonoBehaviour
{
    public PlayerController player;

    public Image healthBar;

    public TextMeshProUGUI ammoText;
    public TextMeshProUGUI magText;
    public TextMeshProUGUI akimboText;

    public TextMeshProUGUI waveText;

    public GameObject pauseMenu;

    public int enemiesAlive;
    public int enemiesSpawned;
    public int enemyCap = 10;
    public int waveCount;

    public float intermissionTime;
    public float waveCountdown = 5f;

    public bool paused = false;
    public bool incTimer = true;
    public bool intermission = false;
    public bool wavesActive = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(SceneManager.GetActiveScene().buildIndex != 0)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();

            pauseMenu = GameObject.FindGameObjectWithTag("Pause");
            pauseMenu.SetActive(false);

            healthBar = GameObject.Find("Health").GetComponent<Image>();

            ammoText = GameObject.Find("AmmoCounter").GetComponent<TextMeshProUGUI>();
            magText = GameObject.Find("MagCounter").GetComponent<TextMeshProUGUI>();
            akimboText = GameObject.Find("AkimboCounter").GetComponent<TextMeshProUGUI>();

            waveText = GameObject.Find("WaveCounter").GetComponent<TextMeshProUGUI>();
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (SceneManager.GetActiveScene().buildIndex != 0)
        {
            healthBar.fillAmount = (float)player.health / (float)player.maxHealth;
            
            if (intermission == true)
            {
                waveText.text = "Intermission: " + intermissionTime;
                
            }
            else if (wavesActive == true)
            {
                waveText.text = "Wave: " + waveCount + " Enemies Left: " + enemiesSpawned + "/" + enemyCap;
            } 
            
            if (player.currentWeapon)
            {
                if (player.akimboWeapon)
                {
                    akimboText.text = "Mag: " + player.akimboWeapon.mag + "/" + player.akimboWeapon.magSize;
                }
                else
                    akimboText.text = "";

                ammoText.text = player.currentWeapon.ammo + "/" + player.currentWeapon.maxAmmo + " :Ammo";
                magText.text = player.currentWeapon.mag + "/" + player.currentWeapon.magSize + " :Mag";
            }
            else
            {
                ammoText.text = "";
                magText.text = "";
                akimboText.text = "";
            }
            
            if(waveCount % 5 == 0)
            {
                wavesActive = false;
            }

            if (intermissionTime >= 0 && incTimer)
            {
                StartCoroutine("intermissionTimer");
            }

            if(enemiesAlive == 0 && wavesActive == true && !intermission && enemiesSpawned == enemyCap)
            {
                intermissionTime = waveCountdown;
                StartCoroutine("resetWave");
            }
            
        }
            
    }

    public void Pause()
    {
        paused = !paused;

        pauseMenu.SetActive(paused);
        Cursor.visible = paused;

        if (paused)
        {
            Time.timeScale = 0;

            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            Time.timeScale = 1f;

            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    public void LoadLevel(int levelID)
    {
        if (levelID >= SceneManager.sceneCountInBuildSettings)
        {
            Debug.Log("Level ID is too high: " + levelID);
        }
        else
            SceneManager.LoadScene(levelID);
    }

    public void LoadNextLevel()
    {
        LoadLevel(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void Quit()
    {
        Application.Quit();
    }

    IEnumerator resetWave()
    {
        intermission = true;

        yield return new WaitForSeconds(waveCountdown);

        intermission = false;
        enemiesSpawned = 0;
        enemyCap += 5;
        waveCount++;

        StopCoroutine("resetWave");
    }

    IEnumerator intermissionTimer()
    {
        incTimer = false;

        yield return new WaitForSeconds(1);
        
        intermissionTime--;
        incTimer = true;
    }
}
