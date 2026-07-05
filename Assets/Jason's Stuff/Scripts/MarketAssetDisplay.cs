using UnityEngine;
using TMPro;

public class MarketAssetDisplay : MonoBehaviour
{
    public TMP_Text serverLevelText;
    public TMP_Text chairLevelText;
    
    void Update()
    {
        if (PlayerManager.instance == null) return;
        
        if (serverLevelText != null)
            serverLevelText.text = "Server Level: " + PlayerManager.instance.serverLevel;
        
        if (chairLevelText != null)
            chairLevelText.text = "Chair Level: " + PlayerManager.instance.chairLevel;
    }
}