using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using Unity.Mathematics;
using System.Collections;

public class SmallPrinter : MonoBehaviour
{
    public PlayerController player;
    
    public GameObject infoPanel;
    public GameObject itemSpawn;
    public GameObject itemOpen;

    public GameObject[] tabs;
    public GameObject[] purchasables;

    public TextMeshProUGUI playerSheets;
    public TextMeshProUGUI playerCartridges;
    public TextMeshProUGUI itemName;
    public TextMeshProUGUI firemodeText;
    public TextMeshProUGUI wieldAkimboText;
    public TextMeshProUGUI reloadTypeText;
    public TextMeshProUGUI dmgText;
    public TextMeshProUGUI reloadText;
    public TextMeshProUGUI firerateText;
    public TextMeshProUGUI burstAmtText;
    public TextMeshProUGUI magText;
    public TextMeshProUGUI sheetCost;
    public TextMeshProUGUI cartridgeCost;
    public TextMeshProUGUI printTime;
    public TextMeshProUGUI printButtonText;

    public bool infoActive;
    public bool printing;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();

        for (int i = 0; i < tabs.Length; i++)
        {
            tabs[i].gameObject.SetActive(false);
        }

        infoPanel.SetActive(infoActive);
    }

    // Update is called once per frame
    void Update()
    {
        playerSheets.text = "Your Sheets: " + player.paperSheets;
        playerCartridges.text = "Your Cartridges: " + player.inkCartridges;

        if (printing)
        {
            for (int i = 0; i < tabs.Length; i++)
            {
                tabs[i].gameObject.SetActive(false);
            }

            infoPanel.SetActive(infoActive);
        }
    }

    public void TabClick(int tab)
    {
        for(int i = 0; i < tabs.Length; i++)
        {
            tabs[i].gameObject.SetActive(false);
        }
        
        tabs[tab].gameObject.SetActive(true);
    }

    public void ShowInfo(int item)
    {
        if(!itemOpen || purchasables[item] != itemOpen)
        {
            itemOpen = purchasables[item];
            infoActive = true;
        }
        else
        {
            infoActive = false;
            itemOpen = null;
        }

        infoPanel.SetActive(infoActive);

        itemName.text = purchasables[item].GetComponent<WeaponScript>().weaponName;
        firemodeText.text = "Firemode: " + purchasables[item].GetComponent<WeaponScript>().firemode;

        if (purchasables[item].GetComponent<WeaponScript>().canAkimbo)
            wieldAkimboText.text = "Wield Akimbo: Yes";
        else
            wieldAkimboText.text = "Wield Akimbo: No";

        if (purchasables[item].GetComponent<WeaponScript>().singleLoad)
            reloadTypeText.text = "Reload Type: Single";
        else
            reloadTypeText.text = "Reload Type: Mag";

        dmgText.text = "Damage: " + purchasables[item].GetComponent<WeaponScript>().weaponDmg;
        reloadText.text = "Reload: " + purchasables[item].GetComponent<WeaponScript>().reloadCooldown + " seconds";

        if (!purchasables[item].GetComponent<WeaponScript>().holdToAttack && !purchasables[item].GetComponent<WeaponScript>().burst)
            firerateText.text = "Firerate: Click Speed";
        else if (purchasables[item].GetComponent<WeaponScript>().burst)
        {
            firerateText.text = "Firerate: " + math.round((1f / (purchasables[item].GetComponent<WeaponScript>().burstAmt * purchasables[item].GetComponent<WeaponScript>().rof + purchasables[item].GetComponent<WeaponScript>().burstCool)) * 60f) + " rnds/min";
        }
        else
            firerateText.text = "Firerate: " + (1f / purchasables[item].GetComponent<WeaponScript>().rof) * 60f + " rnds/min";

        if (purchasables[item].GetComponent<WeaponScript>().burst)
            burstAmtText.text = "Burst Amount: " + purchasables[item].GetComponent<WeaponScript>().burstAmt;
        else
            burstAmtText.text = "Burst Amount: N/A";

        magText.text = "Mag Cap: " + purchasables[item].GetComponent<WeaponScript>().magSize + " rounds";

        sheetCost.text = "Paper sheets: " + purchasables[item].GetComponent<WeaponScript>().sheetCost;

        cartridgeCost.text = "Ink Cartridges: " + purchasables[item].GetComponent<WeaponScript>().cartridgeCost;

        printTime.text = "Print Time: " + purchasables[item].GetComponent<WeaponScript>().printTime + " seconds";
    }

    public void Print()
    {
        if (!printing && player.inkCartridges >= itemOpen.GetComponent<WeaponScript>().cartridgeCost && player.paperSheets >= itemOpen.GetComponent<WeaponScript>().sheetCost)
        {
            player.inkCartridges -= itemOpen.GetComponent<WeaponScript>().cartridgeCost;
            player.paperSheets -= itemOpen.GetComponent<WeaponScript>().sheetCost;
            StartCoroutine("Printing");
        }
    }

    IEnumerator Printing()
    {
        printing = true;

        yield return new WaitForSeconds(itemOpen.GetComponent<WeaponScript>().printTime);

        GameObject p = Instantiate(itemOpen, itemSpawn.transform.position, itemSpawn.transform.rotation);
        printing = false;
    }

    /*IEnumerator PrintingText()
    {
        yield return new WaitForSeconds(0.5f);

        print
    }*/
}
