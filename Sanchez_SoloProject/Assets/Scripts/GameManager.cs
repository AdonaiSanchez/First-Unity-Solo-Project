using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public PlayerController player;

    public Image healthBar;

    public TextMeshProUGUI ammoText;
    public TextMeshProUGUI magText;
    public TextMeshProUGUI akimboText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();

        healthBar = GameObject.Find("Health").GetComponent<Image>();

        ammoText = GameObject.Find("AmmoCounter").GetComponent<TextMeshProUGUI>();
        magText = GameObject.Find("MagCounter").GetComponent<TextMeshProUGUI>();
        akimboText = GameObject.Find("AkimboCounter").GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        healthBar.fillAmount = (float)player.health / (float)player.maxHealth;

        if(player.currentWeapon)
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
    }
}
