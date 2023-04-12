using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.IO;
using UnityEngine.SceneManagement;


public class HighScoreTable : MonoBehaviour
{
    [SerializeField] Transform entryContainer;
    [SerializeField] GameObject entryTemplate;
        

    private string SavePath => $"{Application.persistentDataPath}/highscores.json";


    // Get existing highscore data and update the UI on when scene starts
    private void Start()
    {
        ScoreboardSavedData savedScores =  GetSavedScores();
        UpdateUI(savedScores);
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

    // Create a highscore entry UI element for each saved highscore
    private void UpdateUI(ScoreboardSavedData savedScores)
    {
        // Amount of space between each entry
        int scoreOffset = -50;

        // Destroy all existing highscore entry objects
        foreach(Transform child in entryContainer)
        {
            Destroy(child.gameObject);
        }

        // Create a new entry gameobject for each saved highscore entry
        // Moves new gameobject by offset to avoid all score being stacked up
        // Initialze each new entry with corresponding savedScores entry data
        for (int i = 0; i < savedScores.highscores.Count; i++)
        {
            GameObject entry = Instantiate(entryTemplate, entryContainer);
            entry.transform.localPosition = new Vector3(0, (scoreOffset * i));
            entry.GetComponent<ScoreboardEntryUI>().InitializeScoreboard(savedScores.highscores[i], i);
        }
    }

    // Deletes the existing highscores json file and run UpdateUI
    public void ResetLeaderboard()
    {
        if (File.Exists(SavePath))
        {
            File.Delete(SavePath);
        }

        ScoreboardSavedData savedScores = GetSavedScores();

        UpdateUI(savedScores);
    }

    // Go to the the main game scene
    public void GoToGame()
    {
        SceneManager.LoadScene(1);
    }
    }
