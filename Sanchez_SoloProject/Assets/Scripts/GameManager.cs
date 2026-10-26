using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;
using Unity.Cinemachine;

public class GameManager : MonoBehaviour
{
    public PlayerController player;

    public GameObject cinemachineObj;

    public Image healthBar;
    public Image armorBar;

    public TextMeshProUGUI interactText;
    public TextMeshProUGUI ammoText;
    public TextMeshProUGUI magText;
    public TextMeshProUGUI akimboMagText;
    public TextMeshProUGUI weaponText;
    public TextMeshProUGUI scrapsCounter;
    public TextMeshProUGUI sheetsCounter;
    public TextMeshProUGUI infoPlayerScraps;
    public TextMeshProUGUI infoPlayerSheets;
    public TextMeshProUGUI converterPlayerScraps;
    public TextMeshProUGUI converterPlayerSheets;

    public TextMeshProUGUI waveText;

    public GameObject pauseMenu;
    public GameObject[] menus;

    public int enemiesAlive;
    public int enemiesSpawned;
    public int enemyCap = 10;
    public int waveCount;

    public float intermissionTime;
    public float waveCountdown = 5f;

    public bool paused = false;
    public bool menuOpen = false;
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

            cinemachineObj = GameObject.Find("CinemachineCamera");

            pauseMenu = GameObject.FindGameObjectWithTag("Pause");
            pauseMenu.SetActive(false);

            infoPlayerScraps = GameObject.Find("PlayerScraps").GetComponent<TextMeshProUGUI>();
            infoPlayerSheets = GameObject.Find("PlayerSheets").GetComponent<TextMeshProUGUI>();

            scrapsCounter = GameObject.Find("ScrapCounter").GetComponent<TextMeshProUGUI>();
            sheetsCounter = GameObject.Find("SheetCounter").GetComponent<TextMeshProUGUI>();

            healthBar = GameObject.Find("Health").GetComponent<Image>();
            armorBar = GameObject.Find("Armor").GetComponent<Image>();

            interactText = GameObject.Find("InteractText").GetComponent<TextMeshProUGUI>();
            ammoText = GameObject.Find("AmmoCounter").GetComponent<TextMeshProUGUI>();
            magText = GameObject.Find("MagCounter").GetComponent<TextMeshProUGUI>();
            akimboMagText = GameObject.Find("AkimboCounter").GetComponent<TextMeshProUGUI>();
            weaponText = GameObject.Find("WeaponUsing").GetComponent<TextMeshProUGUI>();

            waveText = GameObject.Find("WaveCounter").GetComponent<TextMeshProUGUI>();

            for (var i = 0; i < menus.Length; i++)
            {
                menus[i].SetActive(false);
            }
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (SceneManager.GetActiveScene().buildIndex != 0)
        {
            healthBar.fillAmount = (float)player.health / (float)player.maxHealth;
            armorBar.fillAmount = (float)player.armor / (float)player.maxArmor;

            infoPlayerScraps.text = "Paper Scraps: " + player.paperScraps;
            infoPlayerSheets.text = "Paper Sheets: " + player.paperSheets;

            converterPlayerScraps.text = "Scraps: " + player.paperScraps;
            converterPlayerSheets.text = "Sheets: " + player.paperSheets;

            if (intermission == true)
            {
                waveText.text = "Intermission: " + intermissionTime;
                
            }
            else if (wavesActive == true)
            {
                waveText.text = "Wave: " + waveCount + " | Enemies Spawned: " + enemiesSpawned + "/" + enemyCap;
            }
            
            if (player.currentWeapon)
            {
                if (player.akimboWeapon)
                {
                    if (player.akimboWeapon.reloading)
                        akimboMagText.text = "Reloading...";
                    else
                        akimboMagText.text = "Mag: " + player.akimboWeapon.mag + "/" + player.akimboWeapon.magSize;
                }
                else
                    akimboMagText.text = "";

                if (player.currentWeapon.reloading)
                    magText.text = "Reloading...";
                else
                    magText.text = player.currentWeapon.mag + "/" + player.currentWeapon.magSize + " :Mag";

                ammoText.text = player.currentWeapon.ammo + "/" + player.currentWeapon.maxAmmo + " :Ammo";
                weaponText.text = player.currentWeapon.firemode + "  |  " + player.currentWeapon.weaponName;
            }
            else
            {
                ammoText.text = "";
                magText.text = "";
                akimboMagText.text = "";
                weaponText.text = "";
            }

            if (menuOpen)
            {
                menus[1].SetActive(false);
            }

            if (player.pickupObj)
            {
                if(player.pickupObj.tag == "Teleporter")
                {
                    if (!wavesActive)
                    {
                        interactText.text = "Teleport";
                    }
                    else
                        interactText.text = "Teleporter Inactive";
                    
                }
                else if(player.pickupObj.tag == "Ammo")
                {
                    interactText.text = "Ammo";
                }
                else if(player.pickupObj.tag == "Armor")
                {
                    interactText.text = "Armor";
                }
                else if(player.pickupObj.tag == "StartWave")
                {
                    interactText.text = "Start Wave";
                }
                else if(player.pickupObj.tag == "Weapon")
                {
                    interactText.text = player.pickupObj.GetComponent<WeaponScript>().weaponName;
                }
                else if (player.pickupObj.tag == "Converter")
                {
                    interactText.text = "Convert Scraps";
                }
            }
            else
                interactText.text = "";

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

    public void OpenInfo()
    {
        if (!menuOpen)
        {
            menus[1].SetActive(!menus[1].activeInHierarchy);
        }
        
    }

    public void OpenMenu(int menuOpening)
    {
        menuOpen = !menuOpen;

        menus[menuOpening].SetActive(menuOpen);
        cinemachineObj.GetComponent<CinemachineInputAxisController>().enabled = !menuOpen;
        Cursor.visible = menuOpen;

        if (menuOpen)
        {
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
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

        Time.timeScale = 1f;
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
        
        if (waveCount % 5 == 0)
        {
            wavesActive = false;
            waveText.text = "";
        }
        
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
