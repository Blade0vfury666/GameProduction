using UnityEngine;

public class PremiumStoreScript : MonoBehaviour
{
    // --- CASH BUTTONS ---

    public void GiveCashOne()
    {
        PlayerManager.instance.playerCash = PlayerManager.instance.playerCash + 38000f;
    }

    public void GiveCashTwo()
    {
        PlayerManager.instance.playerCash = PlayerManager.instance.playerCash + 90000f;
    }

    public void GiveCashThree()
    {
        PlayerManager.instance.playerCash = PlayerManager.instance.playerCash + 300000f;
    }

    public void GiveCashFour()
    {
        PlayerManager.instance.playerCash = PlayerManager.instance.playerCash + 500000f;
    }

    public void GiveCashFive()
    {
        PlayerManager.instance.playerCash = PlayerManager.instance.playerCash + 1200000f;
    }

    // --- GOLD BUTTONS ---

    public void GiveGoldOne()
    {
        PlayerManager.instance.playerGold = PlayerManager.instance.playerGold + 100f;
    }

    public void GiveGoldTwo()
    {
        PlayerManager.instance.playerGold = PlayerManager.instance.playerGold + 220f;
    }

    public void GiveGoldThree()
    {
        PlayerManager.instance.playerGold = PlayerManager.instance.playerGold + 660f;
    }

    public void GiveGoldFour()
    {
        PlayerManager.instance.playerGold = PlayerManager.instance.playerGold + 1500f;
    }

    public void GiveGoldFive()
    {
        PlayerManager.instance.playerGold = PlayerManager.instance.playerGold + 2500f;
    }
}