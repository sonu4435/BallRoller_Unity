using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GoldInfoFatcher : MonoBehaviour
{
    public TMP_Text CoinsText;

    private void Start()
    {
        int currentHighScore = PlayerPrefs.GetInt("Coin", 0);
        if (currentHighScore >= 1000)
        {
            CoinsText.text = (currentHighScore / 1000) + "k";
        }
        else
        {
            CoinsText.text = currentHighScore.ToString();
        }
    }
}
