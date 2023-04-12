using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;
using TMPro;

public class MainManager : MonoBehaviour
{
    public Brick BrickPrefab;
    public int LineCount = 6;
    public Rigidbody Ball;

    public Text ScoreText;
    public Text highScoreText;
    public TextMeshProUGUI newHighScoreText;
    public GameObject GameOverText;
    public GameObject showHighScoreButton;

   [SerializeField] int maxHighScoresEntries = 10;
    private bool m_Started = false;
    private int m_Points;
    private int highScore;

    private bool m_GameOver = false;
    private string SavePath => $"{Application.persistentDataPath}/highscores.json";


    void Start()
    {
        const float step = 0.6f;
        int perLine = Mathf.FloorToInt(4.0f / step);
        
        int[] pointCountArray = new [] {1,1,2,2,5,5};
        for (int i = 0; i < LineCount; ++i)
        {
            for (int x = 0; x < perLine; ++x)
            {
                Vector3 position = new Vector3(-1.5f + step * x, 2.5f + i * 0.3f, 0);
                var brick = Instantiate(BrickPrefab, position, Quaternion.identity);
                brick.PointValue = pointCountArray[i];
                brick.onDestroyed.AddListener(AddPoint);
            }
        }

        LoadHighScore();
    }

    private void Update()
    {
        if (!m_Started)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                m_Started = true;
                float randomDirection = Random.Range(-1.0f, 1.0f);
                Vector3 forceDir = new Vector3(randomDirection, 1, 0);
                forceDir.Normalize();

                Ball.transform.SetParent(null);
                Ball.AddForce(forceDir * 2.0f, ForceMode.VelocityChange);
            }
        }
        else if (m_GameOver)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }
    }

    void AddPoint(int point)
    {
        m_Points += point;
        ScoreText.text = $"Score : {m_Points}";
    }

    public void GameOver()
    {
        m_GameOver = true;
        GameOverText.SetActive(true);
        showHighScoreButton.SetActive(true);

        ScoreboardEntryData newScore = new ScoreboardEntryData
        {
            playerName = PlayerData.playerName,
            score = m_Points
        };
        AddHighscore(newScore);
        LoadHighScore();
    }

    void AddHighscore(ScoreboardEntryData newScore)
    {
        ScoreboardSavedData savedScores = GetSavedScores();

        bool scoreAdded = false;

        for (int i = 0; i < savedScores.highscores.Count; i++)
        {
            if (newScore.score > savedScores.highscores[i].score)
            {
                savedScores.highscores.Insert(i, newScore);
                scoreAdded = true;
                ShowNewHighScoreMessage();
                break;
            }
        }
        if (!scoreAdded && savedScores.highscores.Count < maxHighScoresEntries)
        {
            savedScores.highscores.Add(newScore);
            ShowNewHighScoreMessage();
        }

        if (savedScores.highscores.Count > maxHighScoresEntries)
        {
            savedScores.highscores.RemoveRange(maxHighScoresEntries, savedScores.highscores.Count - maxHighScoresEntries);
        }

        SaveScores(savedScores);
    }


    void LoadHighScore()
    {
        highScoreText.text = "No High Score yet!";

        if (File.Exists(SavePath))
        {
            using StreamReader stream = new StreamReader(SavePath);
            string json = stream.ReadToEnd();

            ScoreboardSavedData data = JsonUtility.FromJson<ScoreboardSavedData>(json);

            if (data != null)
            {
                highScore = data.highscores[0].score;
                string name = data.highscores[0].playerName;

                highScoreText.text = $"High Score: {name} - {highScore} points!";
            }
        }
    }

    void ShowNewHighScoreMessage()
    {
        newHighScoreText.gameObject.SetActive(true);
    }

    public ScoreboardSavedData GetSavedScores()
    {
        if (!File.Exists(SavePath))
        {
            File.Create(SavePath).Dispose();
            return new ScoreboardSavedData();
        }

        using StreamReader stream = new StreamReader(SavePath);
        string json = stream.ReadToEnd();

        return JsonUtility.FromJson<ScoreboardSavedData>(json) != null ? JsonUtility.FromJson<ScoreboardSavedData>(json) : new ScoreboardSavedData();
    }
    private void SaveScores(ScoreboardSavedData scoreboardSavedData)
    {
        using (StreamWriter stream = new StreamWriter(SavePath))
        {
            string json = JsonUtility.ToJson(scoreboardSavedData, true);
            stream.Write(json);
        }
    }

    public void LoadLeaderboard()
    {
        SceneManager.LoadScene(2);
    }
}
