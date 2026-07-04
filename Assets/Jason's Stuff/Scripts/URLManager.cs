using UnityEngine;
using TMPro;

public class URLBarManager : MonoBehaviour
{
    public static URLBarManager instance;

    [Header("URL Bar")]
    public TMP_Text urlBarText;
    public string baseURL = "PCMARKET.COM/";

    [Header("Tab URL Suffixes")]
    public string computersURL = "COMPUTERS";
    public string chairsURL = "CHAIRS";
    public string serversURL = "SERVERS";
    public string inventoryURL = "INVENTORY";

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        // Default to Computers
        UpdateURL(computersURL);
    }

    public void UpdateURL(string page)
    {
        if (urlBarText != null)
        {
            urlBarText.text = baseURL + page;
        }
    }

    // Convenience methods for each tab
    public void GoToComputers()
    {
        UpdateURL(computersURL);
    }

    public void GoToChairs()
    {
        UpdateURL(chairsURL);
    }

    public void GoToServers()
    {
        UpdateURL(serversURL);
    }

    public void GoToInventory()
    {
        UpdateURL(inventoryURL);
    }
}