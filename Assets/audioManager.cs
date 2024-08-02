using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class audioManager : MonoBehaviour
{
    public AudioClip ButtomClick_A;
    public AudioClip coinCollect_A;
    public AudioClip Obstacle_Hit_A;
    public AudioSource audioSource;

    public static audioManager instance;

    public AudioSource backgroundMusic; // Assign your background music AudioSource here

    private void Awake()
    {
        // Ensure only one instance of AudioManager exists
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Keep this object across scenes
        }
        else
        {
            Destroy(gameObject); // Destroy duplicate instance
        }
    }

    private void Start()
    {
        if (backgroundMusic != null)
        {
            backgroundMusic.Play(); // Play background music
        }
    }
    public void PlayButtonClick()
    {
        audioSource.clip = ButtomClick_A;
        audioSource.loop = false;
        audioSource.playOnAwake = false;
        audioSource.Play();
    }

    public void PlayCoinCollect()
    {
        audioSource.clip = coinCollect_A;
        audioSource.loop = false;
        audioSource.playOnAwake = false;
        audioSource.Play();
    }

    public void PlayObstacleHit()
    {
        audioSource.clip = Obstacle_Hit_A;
        audioSource.loop = false;
        audioSource.playOnAwake = false;
        audioSource.Play();
    }
}
