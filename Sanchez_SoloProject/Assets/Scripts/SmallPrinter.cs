using TMPro;
using UnityEngine;

public class SmallPrinter : MonoBehaviour
{

    public GameObject infoPanel;

    public GameObject[] tabs;
    public GameObject[] purchasables;

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

    public int lastPressed;

    public bool infoActive;

    // Update is called once per frame
    void Update()
    {
        infoPanel.SetActive(infoActive);
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
        if(item == lastPressed || !infoActive)
        {
            infoActive = !infoActive;
            infoPanel.SetActive(infoActive);
        }
        lastPressed = item;

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


        firerateText.text = "Firerate: " + (1f / purchasables[item].GetComponent<WeaponScript>().rof) * 60f + " rnds/min";

        if (purchasables[item].GetComponent<WeaponScript>().burst)
            burstAmtText.text = "Burst Amount: " + purchasables[item].GetComponent<WeaponScript>().burstAmt;
        else
            burstAmtText.text = "Burst Amount: N/A";

        magText.text = "Mag Cap: " + purchasables[item].GetComponent<WeaponScript>().magSize + " rounds";
    }
}
