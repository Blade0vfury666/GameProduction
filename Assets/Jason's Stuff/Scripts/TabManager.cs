using UnityEngine;

public class TabManager : MonoBehaviour
{
    public GameObject computersPage;
    public GameObject chairsPage;
    public GameObject serversPage;
    public GameObject inventoryPage;
    
    public void ShowComputers()
    {
        computersPage.SetActive(true);
        chairsPage.SetActive(false);
        serversPage.SetActive(false);
        inventoryPage.SetActive(false);
    }
    
    public void ShowChairs()
    {
        computersPage.SetActive(false);
        chairsPage.SetActive(true);
        serversPage.SetActive(false);
         inventoryPage.SetActive(false);
    }
    
    public void ShowServers()
    {
        computersPage.SetActive(false);
        chairsPage.SetActive(false);
        serversPage.SetActive(true);
        inventoryPage.SetActive(false);
    }

     public void ShowInventory() 
    {
        computersPage.SetActive(false);
        chairsPage.SetActive(false);
        serversPage.SetActive(false);
        inventoryPage.SetActive(true);
    }
   
}