using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    [Header("Player Stats")]
    public int maxPlayerHealth = 3;
    private int score;
    private int health;

    [Header("Audio")]
    public AudioSource backgroundMusic;
    public AudioClip gameOverMusic;
    public AudioClip gameWinMusic;
    public AudioClip mainSceneMusic;
    public AudioClip secondSceneMusic;

    private int totalOrcs;
    private int orcsDefeated;
    [Header("Game Data")]
    [SerializeField] public GameConstants gameConstants;  // Assign in Inspector

    protected override void OnAwake()
    {
        health = gameConstants != null && gameConstants.playerHealth > 0
            ? gameConstants.playerHealth
            : maxPlayerHealth;

        // Setup audio
        backgroundMusic = GetComponent<AudioSource>();
        if (backgroundMusic == null)
        {
            backgroundMusic = gameObject.AddComponent<AudioSource>();
            backgroundMusic.loop = true;
            backgroundMusic.playOnAwake = false;
        }

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "SecondScene")
        {
            health = gameConstants.playerHealth;
            totalOrcs = FindObjectsOfType<Orc>().Length;
            orcsDefeated = 0;
            if (secondSceneMusic != null)
            {
                backgroundMusic.clip = secondSceneMusic;
                backgroundMusic.Play();
            }
        }
        else if (scene.name == "MainScene")
        {
            if (gameConstants.playerHealth <= 0)
            {
                health = maxPlayerHealth;
                gameConstants.playerHealth = health;
            }
            if (mainSceneMusic != null)
            {
                backgroundMusic.clip = mainSceneMusic;
                backgroundMusic.Play();
            }
        }

        UpdateUI();
    }

    private void UpdateUI()
    {
        HUDManager.Instance.SetScore(score);
        HUDManager.Instance.SetHealth(health);
        HUDManager.Instance.SetHighScore(gameConstants.highScore);
    }

    public void SetHealth(int newHealth)
    {
        health = newHealth;
        gameConstants.playerHealth = health;
        HUDManager.Instance.SetHealth(health);
    }

    public void IncreaseScore(int increment)
    {
        score += increment;
        HUDManager.Instance.SetScore(score);
    }

    public void GameOver()
    {
        Time.timeScale = 0.0f;
        HUDManager.Instance.ShowGameOver();
        backgroundMusic.Stop();
        if (gameOverMusic != null)
        {
            AudioSource.PlayClipAtPoint(gameOverMusic, Camera.main.transform.position);
        }
    }

    public void GameRestart()
    {
        if (score > gameConstants.highScore)
        {
            gameConstants.highScore = score;
            HUDManager.Instance.SetHighScore(gameConstants.highScore);
        }
        score = 0;
        health = maxPlayerHealth;
        gameConstants.playerHealth = health;
        UpdateUI();
        HUDManager.Instance.HideGameOver();
        Time.timeScale = 1.0f;
        SceneManager.LoadScene("MainScene");
    }

    public void GameWin()
    {
        Time.timeScale = 0.0f;
        HUDManager.Instance.GameWin();
        backgroundMusic.Stop();
        if (gameWinMusic != null)
        {
            AudioSource.PlayClipAtPoint(gameWinMusic, Camera.main.transform.position);
        }
        if (score > gameConstants.highScore)
        {
            gameConstants.highScore = score;
            HUDManager.Instance.SetHighScore(score);
        }
    }

    public void OrcDefeated()
    {
        orcsDefeated++;
        if (SceneManager.GetActiveScene().name == "SecondScene" && orcsDefeated >= totalOrcs)
        {
            GameWin();
        }
    }
}
