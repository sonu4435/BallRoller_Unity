using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ShopManager : MonoBehaviour
{
    public TMP_Text totalBalance;
    public TMP_Text logMessage;
    public GameObject logMessagePopup;
    public GameObject confirmMessage;
    public GameObject confirmMessageButton;
    public TMP_Text priceText;
    public List<GameObject> playerStyles;
    public List<int> playersPrice;
    public List<float> playersSpeed; // Speed for each item

    private int activePurchasePrice;
    private int activePurchaseIndex;

    public const int CurrentVersion = 2; // Increment this whenever you make changes

    public float InitialForwardSpeed = 5f; // Set your initial speed here




    private void Start()
    {
        int savedVersion = PlayerPrefs.GetInt("DataVersion", 0);

        if (savedVersion < CurrentVersion)
        {
            ResetPlayerPrefs();
            PlayerPrefs.SetInt("DataVersion", CurrentVersion);
        }

        totalBalance.text = PlayerPrefs.GetInt("Coin").ToString();
        for (int i = 1; i < playerStyles.Count; i++)
        {
            string purchasedString = "Item" + i;
            if (PlayerPrefs.GetString(purchasedString) == "1")
            {
                playerStyles[i].transform.GetChild(0).gameObject.SetActive(false);
            }
            else
            {
                playerStyles[i].transform.GetChild(0).gameObject.SetActive(true);
            }
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            GoHome();
        }
    }

    private void GoHome()
    {
        SceneManager.LoadScene(0);
    }

    private void ResetPlayerPrefs()
    {
        // Loop through your items and reset their states
        for (int i = 1; i < playerStyles.Count; i++)
        {
            string purchasedString = "Item" + i;
            PlayerPrefs.SetString(purchasedString, "0");
        }

        // Optionally, you can reset other preferences if needed
        PlayerPrefs.SetInt("Coin", 0);
        PlayerPrefs.SetInt("ActiveColorIndex", 0);
        PlayerPrefs.SetFloat("PlayerForwardSpeed", InitialForwardSpeed);

        Debug.Log("PlayerPrefs have been reset due to version update.");
    }

    public void PurchaseItem(int index)
    {
        string purchasedString = "Item" + index;
        if (PlayerPrefs.GetString(purchasedString) == "1")
        {
            playerStyles[index].transform.GetChild(0).gameObject.SetActive(false);
            PlayerPrefs.SetInt("ActiveColorIndex", index);
            PlayerPrefs.SetFloat("PlayerForwardSpeed", playersSpeed[index]);
        }
        else
        {
            activePurchasePrice = playersPrice[index];
            activePurchaseIndex = index;
            priceText.text = activePurchasePrice.ToString();
            confirmMessage.SetActive(true);
        }
    }

    public void ConfirmPurchase()
    {
        int totalCoins = PlayerPrefs.GetInt("Coin");
        if (totalCoins >= activePurchasePrice)
        {
            totalCoins -= activePurchasePrice;
            PlayerPrefs.SetInt("Coin", totalCoins);
            totalBalance.text = totalCoins.ToString();
            logMessage.text = "Purchase Successful!";
            StartCoroutine(ShowLogMessageForDuration(2f));
            confirmMessage.SetActive(false);
            playerStyles[activePurchaseIndex].transform.GetChild(0).gameObject.SetActive(false);
            string purchasedString = "Item" + activePurchaseIndex;
            PlayerPrefs.SetString(purchasedString, "1");
            PlayerPrefs.SetInt("ActiveColorIndex", activePurchaseIndex);
            PlayerPrefs.SetFloat("PlayerForwardSpeed", playersSpeed[activePurchaseIndex]);
            Debug.Log("Item " + activePurchaseIndex + " purchased with forward speed: " + playersSpeed[activePurchaseIndex]);
        }
        else
        {
            logMessage.text = "Not enough coins!";
            StartCoroutine(ShowLogMessageForDuration(2f));
        }
    }

    public void OnClickPlay()
    {
        SceneManager.LoadScene(1);
    }

    private IEnumerator ShowLogMessageForDuration(float duration)
    {
        logMessagePopup.SetActive(true);
        yield return new WaitForSeconds(duration);
        logMessagePopup.SetActive(false);
    }
}
