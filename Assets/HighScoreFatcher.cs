using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HighScoreFatcher : MonoBehaviour
{
    public TMP_Text HighScoreText;

    private void Start()
    {
        int currentHighScore = PlayerPrefs.GetInt("HightScore", 0);
        if (currentHighScore >= 1000)
        {
            HighScoreText.text = (currentHighScore / 1000) + "k";
        }
        else
        {
            HighScoreText.text = currentHighScore.ToString();
        }
    }

}
