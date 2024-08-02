using System;
using System.Collections;
using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LeaderBoardHandler : MonoBehaviour
{
    bool isPlayerLoggedIn;
    string playerID;
    public GameObject namePanel;
    string leaderboardName = "UnityPlayerData";
    [SerializeField] GameObject playerPrefab;
    [SerializeField] GameObject playerParent;
    public static LeaderBoardHandler instance;
    private int countBackButton = 0;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(instance.gameObject);
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }

    void Start()
    {
        Login();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            countBackButton++;
            if (countBackButton > 1)
            {
                ExitGame();
                return;
            }
        }
    }

    public void ExitGame()
    {
        // Save any necessary data before quitting
        Application.Quit();
    }
    void Login()
    {
        var request = new LoginWithCustomIDRequest { CustomId = SystemInfo.deviceUniqueIdentifier, CreateAccount = true };
        PlayFabClientAPI.LoginWithCustomID(request, OnLoginSuccess, OnLoginFailure);
    }

    private void OnLoginFailure(PlayFabError error)
    {
        Debug.Log("Login failed: " + error.GenerateErrorReport());
        isPlayerLoggedIn = false;
    }

    private void OnLoginSuccess(LoginResult result)
    {
        Debug.Log("Login success: " + result.PlayFabId);
        isPlayerLoggedIn = true;
        playerID = result.PlayFabId;

        GetPlayerProfile();
    }

    private void GetPlayerProfile()
    {
        var request = new GetPlayerProfileRequest { PlayFabId = playerID };
        PlayFabClientAPI.GetPlayerProfile(request, OnGetPlayerProfileSuccess, OnGetPlayerProfileFailure);
    }

    private void OnGetPlayerProfileSuccess(GetPlayerProfileResult result)
    {
        var displayName = result.PlayerProfile.DisplayName;
        if (string.IsNullOrEmpty(displayName))
        {
            if (namePanel != null)
            {
                namePanel.SetActive(true);
            }
        }
        else
        {
            if (namePanel != null)
            {
                namePanel.SetActive(false);  // Hide the name panel for existing accounts with a display name
            }
        }
    }

    private void OnGetPlayerProfileFailure(PlayFabError error)
    {
        Debug.Log("Get player profile failed: " + error.GenerateErrorReport());
    }

    public void ChangePlayerName(TMP_InputField nameInput)
    {
        if (string.IsNullOrEmpty(nameInput.text))
        {
            Debug.Log("Name field is empty");
            return;
        }
        var request = new UpdateUserTitleDisplayNameRequest
        {
            DisplayName = nameInput.text,
        };
        PlayFabClientAPI.UpdateUserTitleDisplayName(request, OnNameSuccess, OnNameFailure);
    }

    private void OnNameFailure(PlayFabError error)
    {
        Debug.Log("Name change failed: " + error.GenerateErrorReport());
    }

    private void OnNameSuccess(UpdateUserTitleDisplayNameResult result)
    {
        Debug.Log("Name change success: " + result.DisplayName);
        if (namePanel != null)
        {
            namePanel.SetActive(false); // Hide the name panel after name change
        }
    }

    public void SendHighScore(int highScore)
    {
        var request = new UpdatePlayerStatisticsRequest
        {
            Statistics = new List<StatisticUpdate>
            {
                new StatisticUpdate { StatisticName = leaderboardName, Value = highScore }
            }
        };
        PlayFabClientAPI.UpdatePlayerStatistics(request, result =>
        {
            Debug.Log("User statistics updated with score: " + highScore);
        },
        error =>
        {
            Debug.LogError("Failed to update statistics: " + error.GenerateErrorReport());
        });
    }

    public void GetLeaderBoard()
    {
        var request = new GetLeaderboardRequest
        {
            StatisticName = leaderboardName,
            StartPosition = 0,
            MaxResultsCount = 100,
        };
        PlayFabClientAPI.GetLeaderboard(request, OnGetLeaderboardSuccess, OnGetLeaderboardFailure);
    }

    private void OnGetLeaderboardFailure(PlayFabError error)
    {
        Debug.Log("Leaderboard fetch failed: " + error.GenerateErrorReport());
    }

    private void OnGetLeaderboardSuccess(GetLeaderboardResult result)
    {
        // Check if playerParent is not null
        if (playerParent == null)
        {
            Debug.LogWarning("PlayerParent GameObject is not assigned.");
            return;
        }

        // Destroy previous leaderboard entries safely
        for (int i = playerParent.transform.childCount - 1; i >= 0; i--)
        {
            Destroy(playerParent.transform.GetChild(i).gameObject);
        }

        if (result.Leaderboard.Count == 0)
        {
            Debug.Log("Leaderboard is empty");
            return;
        }

        foreach (var item in result.Leaderboard)
        {
            if (playerPrefab != null && playerParent != null)
            {
                GameObject player = Instantiate(playerPrefab, playerParent.transform, false);
                player.transform.GetChild(0).GetComponent<TMP_Text>().text = (item.Position + 1).ToString();
                player.transform.GetChild(1).GetComponent<TMP_Text>().text = item.DisplayName ?? "Unknown Player";
                player.transform.GetChild(2).GetComponent<TMP_Text>().text = item.StatValue.ToString();
            }
            else
            {
                Debug.LogWarning("PlayerPrefab or PlayerParent is not assigned.");
            }
        }
    }
}
