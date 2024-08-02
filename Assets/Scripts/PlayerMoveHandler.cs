using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerSetting : MonoBehaviour
{
    public int totalCoins = 0;
    public GameRoadManager gameRoadManager;
    public Rigidbody rb;
    public float PlayerCameraSpeed;
    public float PlayerMoveSpeed;
    public float InitialForwardSpeed = 5f; // Initial slow forward speed
    public GameObject GameOverPanel;
    public GameObject Player;
    public Transform HeartsParent;
    private int heartsCount = 3;
    private bool IsGameOver = false;
    private bool isFrozen = false;
    private float currentForwardSpeed; // To store the current forward speed

    [Header("Stat Text")]
    public TMP_Text HighScoreText;
    public TMP_Text ScoreText;
    public TMP_Text CoinText;
    public TMP_Text CountdownText;
    public List<Color> colors;
    public Material _mat;

    private void Start()
    {
        int currentHighScore = PlayerPrefs.GetInt("HightScore", 0);
        HighScoreText.text = currentHighScore.ToString();
        _mat.EnableKeyword("_Emission");
        int activeColorIndex = PlayerPrefs.GetInt("ActiveColorIndex");
        Color color = colors[activeColorIndex];
        color *= 2;
        _mat.SetColor("_EmissionColor", color);

        // Start with the saved forward speed or the initial speed if no speed is saved
        currentForwardSpeed = PlayerPrefs.GetFloat("PlayerForwardSpeed", InitialForwardSpeed);

    }

    private void Update()
    {
        if (!IsGameOver && !isFrozen)
        {
            MovementHandler();
            HandleScore();
        }
    }

    public void HandleScore()
    {
        ScoreText.text = ((int)transform.position.z + 147).ToString();
    }

    public void MovementHandler()
    {
        if (Player.transform.position.x >= 5 || Player.transform.position.x <= -5)
        {
            LoseHeart();
        }
        else
        {
            // Moves the player forward
            rb.AddForce(Vector3.forward * currentForwardSpeed);

            // Left/Right Movement
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                MoveLeft();
            }
            if (Input.GetKey(KeyCode.RightArrow))
            {
                MoveRight();
            }
        }
    }

    public void MoveLeft()
    {
        rb.AddForce(Vector3.left * PlayerMoveSpeed);
    }
    public void MoveRight()
    {
        rb.AddForce(Vector3.right * PlayerMoveSpeed);
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Obstacle")
        {
            LoseHeart();
            audioManager.instance.PlayObstacleHit();
        }
    }

    private void OnTriggerCoin()
    {
        totalCoins++;
        int previousCoins = PlayerPrefs.GetInt("Coin", 0);
        previousCoins += 1;
        PlayerPrefs.SetInt("Coin", previousCoins);
        CoinText.text = totalCoins.ToString();
        audioManager.instance.PlayCoinCollect();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "RoadEndObstacle")
        {
            gameRoadManager.SpawnRoad();
        }
        if (other.gameObject.tag == "Coin")
        {
            Destroy(other.gameObject);
            OnTriggerCoin();
        }
    }

    private void LoseHeart()
    {
        heartsCount--;
        UpdateHeartsUI();
        if (heartsCount > 0)
        {
            if (transform.position.z < -115)
            {
                Player.transform.position = new Vector3(0, Player.transform.position.y, -147);
            }
            else
            {
                Player.transform.position = new Vector3(0, Player.transform.position.y, Player.transform.position.z - 20);
            }

            StartCoroutine(CountdownCoroutine());
        }
        else
        {
            OnGameOver();
        }
    }

    private void UpdateHeartsUI()
    {
        for (int i = 0; i < HeartsParent.childCount; i++)
        {
            GameObject heart = HeartsParent.GetChild(i).gameObject;
            heart.SetActive(i < heartsCount);
        }
    }

    private IEnumerator CountdownCoroutine()
    {
        isFrozen = true;
        rb.velocity = Vector3.zero;
        rb.isKinematic = true;

        CountdownText.gameObject.SetActive(true);
        CountdownText.text = "3";
        yield return new WaitForSeconds(1f);

        CountdownText.text = "2";
        yield return new WaitForSeconds(1f);

        CountdownText.text = "1";
        yield return new WaitForSeconds(1f);

        CountdownText.text = "Go!";
        yield return new WaitForSeconds(1f);

        CountdownText.gameObject.SetActive(false);
        rb.isKinematic = false;
        isFrozen = false;

        // Resume the player's movement with the stored forward speed
        rb.AddForce(Vector3.forward * currentForwardSpeed);
    }

    public void OnGameOver()
    {
        IsGameOver = true;
        GameOverPanel.SetActive(true);
        calculateAndSaveHightscore();
        rb.velocity = Vector3.zero;
    }

    void calculateAndSaveHightscore()
    {
        int currentScore = (int)(transform.position.z + 147);
        int currentHighScore = PlayerPrefs.GetInt("HightScore", 0);
        if (currentScore > currentHighScore)
        {
            LeaderBoardHandler.instance.SendHighScore(currentScore);
            PlayerPrefs.SetInt("HightScore", currentScore);
        }
    }

    public void GameOverScreen()
    {
        SceneManager.LoadScene(0);
    }

    public void OnClickPlay()
    {
        // Reset the forward speed to the saved forward speed or the initial speed when restarting the game
        currentForwardSpeed = PlayerPrefs.GetFloat("PlayerForwardSpeed", InitialForwardSpeed);
        SceneManager.LoadScene(1);
    }
}
