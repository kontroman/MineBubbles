using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ScoreController : MonoBehaviour
{
    public static ScoreController Instance { get; private set; }

    private int maxScoreNormal;
    private int maxScoreHard;

    private int currentGameScore;

    private string defaultString;

    [SerializeField]
    private TextMeshProUGUI scoreText;

    void Awake()
    {
        if (Instance != null) return;
        else Instance = this;

        Init();
    }

    public int GetCurrentScore()
    {
        return currentGameScore;
    }

    public void AddScore(int _score)
    {
        currentGameScore += _score;

        UpdateScoreText();
    }

    private void Update()
    {
        UpdateScoreText();

        if (currentGameScore <= 0)
            Debug.LogError("GameOver by score");
    }

    private void UpdateScoreText()
    {
        scoreText.text = "" + currentGameScore;
    }

    private void Init()
    {
        currentGameScore = PlayerPrefs.GetInt("CurrentScore", 10000); ;

        maxScoreNormal = PlayerPrefs.GetInt("MaxScoreNormal", 10000);
        maxScoreHard = PlayerPrefs.GetInt("MaxScoreHard", 10000);

        scoreText = GameObject.Find("ScoreText").GetComponent<TextMeshProUGUI>();

        UpdateScoreText();
    }

    internal void CheckAndSetNewMaxScore()
    {
        if(currentGameScore >= GetSavedScore())
            SaveCurrentScore();
    }

    public ScoreState SaveState()
    {
        return new ScoreState(currentGameScore);
    }

    public void RestoreFrom(ScoreState _ss)
    {
        currentGameScore = _ss.score;

        UpdateScoreText();
    }

    private int GetSavedScore()
    {
        return PlayerPrefs.GetInt("CurrentScore", 10000);
    }

    public void SaveCurrentScore()
    {
        PlayerPrefs.SetInt("CurrentScore", currentGameScore);
    }

    public void ResetScore()
    {
        PlayerPrefs.DeleteKey("CurrentScore");
    }
}
