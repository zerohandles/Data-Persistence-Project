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
    [SerializeField] PlayerData playerData;


    // Provided code, Set initial brick layout and assign points to each. 
    void Start()
    {
        const float step = 0.6f;
        int perLine = Mathf.FloorToInt(4.0f / step);

        int[] pointCountArray = new[] { 1, 1, 2, 2, 5, 5 };
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
        DisplayHighScoreText();
    }


    // Provided code, Start the game on Spacebar press.
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


    // Provided code, undate the onscreen ScoreText UI element
    void AddPoint(int point)
    {
        m_Points += point;
        ScoreText.text = $"Score : {m_Points}";
    }


    // Display gameover UI elements.  
    // Attempt to add the the current score as a new highscore
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
        DisplayHighScoreText();
    }


    // Add a new highscore to the highscore save file 
    void AddHighscore(ScoreboardEntryData newScore)
    {
        ScoreboardSavedData savedScores = GetSavedScores();

        bool scoreAdded = false;

        // Add the new highscore to the savedScores ScoreboardSavedData
        // only if it is greater than an existing score and break the loop.
        // Creates a sorted list of scores.
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
        // If the score wasn't added to savedScores and the highscore limit hasn't been reached
        // add the newScore to the savedScores
        if (!scoreAdded && savedScores.highscores.Count < maxHighScoresEntries)
        {
            savedScores.highscores.Add(newScore);
            ShowNewHighScoreMessage();
        }

        // Remove the lowest entries beyond the maxHighScoreEntries limit from the savedScores data.
        if (savedScores.highscores.Count > maxHighScoresEntries)
        {
            savedScores.highscores.RemoveRange(maxHighScoresEntries, savedScores.highscores.Count - maxHighScoresEntries);
        }
        // Save the new savedScores ScoreboardSavedData object.
        SaveScores(savedScores);
    }


    // Retrieves the existing highscore data 
    public ScoreboardSavedData GetSavedScores()
    {
        // If there is no highscores file present, return a new ScoreboardSavedData object
        if (!File.Exists(SavePath))
        {
            File.Create(SavePath).Dispose();
            return new ScoreboardSavedData();
        }

        // Read through the existing highscores json file and return them as a ScoreboardSavedData object.
        using StreamReader stream = new StreamReader(SavePath);
        string json = stream.ReadToEnd();

        // If there are no recorded highscores in the file, return a new ScoreboardSavedData to prevent null object reference errors.
        return JsonUtility.FromJson<ScoreboardSavedData>(json) != null ? JsonUtility.FromJson<ScoreboardSavedData>(json) : new ScoreboardSavedData();
    }


    // Write the ScoreboardSavedData to a json format and save it to locally
    private void SaveScores(ScoreboardSavedData scoreboardSavedData)
    {
        using (StreamWriter stream = new StreamWriter(SavePath))
        {
            string json = JsonUtility.ToJson(scoreboardSavedData, true);
            stream.Write(json);
        }
    }


    // Display the highest recorded score
    void DisplayHighScoreText()
    {
        // Default value of highScoreText
        highScoreText.text = "No High Score yet!";

        // If a highscores save file exists load it from json into a ScoreboardSavedData object
        if (File.Exists(SavePath))
        {
            using StreamReader stream = new StreamReader(SavePath);
            string json = stream.ReadToEnd();

            ScoreboardSavedData data = JsonUtility.FromJson<ScoreboardSavedData>(json);

            // If the newly loaded ScoreboardSavedData is not empty, set the highscoreText to the first entry. 
            if (data != null)
            {
                highScore = data.highscores[0].score;
                string name = data.highscores[0].playerName;

                highScoreText.text = $"High Score: {name} - {highScore} points!";
            }
        }
    }


    // Displays the new high score message in the UI.
    void ShowNewHighScoreMessage()
    {
        newHighScoreText.gameObject.SetActive(true);
    }


    // Load the leaderboard scene
    public void LoadLeaderboard()
    {
        SceneManager.LoadScene(2);
    }
}
