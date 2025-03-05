using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class HUDManager : Singleton<HUDManager>
{
    public GameObject gameOverPanel;
    public GameObject scoreText;
    public GameObject healthText;
    public GameObject gameWinPanel;
    public GameObject highScoreText;

    protected override void OnAwake()
    {
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
        if (gameWinPanel != null)
            gameWinPanel.SetActive(false);
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void GameStart()
    {
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
    }

    public void SetScore(int score)
    {
        if (scoreText != null)
            scoreText.GetComponent<TextMeshProUGUI>().text = "Score: " + score.ToString();
    }

    public void ShowGameOver()
    {
        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);
    }

    public void GameWin()
    {
        if (gameWinPanel != null)
            gameWinPanel.SetActive(true);
    }

    public void HideGameOver()
    {
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
    }

    public void SetHealth(int health)
    {
        if (healthText != null)
            healthText.GetComponent<TextMeshProUGUI>().text = "Health: " + health.ToString();
    }

    public void OnRestartButtonClick()
    {
        GameManager.Instance.GameRestart();
    }

    public void SetHighScore(int highScore)
    {
        if (highScoreText != null)
            highScoreText.GetComponent<TextMeshProUGUI>().text = "High Score: " + highScore.ToString();
    }
}

