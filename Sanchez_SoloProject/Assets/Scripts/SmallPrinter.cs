using UnityEngine;

public class SmallPrinter : MonoBehaviour
{
    public GameObject[] tabs;
    public GameObject[] purchasables;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TabClick(int tab)
    {
        for(int i = 0; i < tabs.Length; i++)
        {
            tabs[i].gameObject.SetActive(false);
        }
        
        tabs[tab].gameObject.SetActive(true);
    }
}
