using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EconomyManager : Singleton<EconomyManager>
{
    private TMP_Text goldText;
    public int currentGold;
    const string COIN_AMOUNT_TEXT = "Gold Amount Text";

    public int CurrentGold
    {
        get { return currentGold; }
        set
        {
            currentGold = value;
            UpdateGoldText();
        }
    }
    private void UpdateGoldText()
    {
        if (goldText == null)
        {
            goldText = GameObject.Find(COIN_AMOUNT_TEXT).GetComponent<TMP_Text>();
        }
        goldText.text = currentGold.ToString("D3");
    }
    public void UpdateCurrentGold(){
        currentGold += 1;

        if (goldText == null)
        {
            goldText = GameObject.Find(COIN_AMOUNT_TEXT).GetComponent<TMP_Text>();
        }
        goldText.text = currentGold.ToString("D3");

    }
    public void SetGold(int gold)
    {
        CurrentGold = gold;
    }
}
