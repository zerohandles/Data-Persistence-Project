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

    private void Start()
    {
        ScoreboardSavedData savedScores =  GetSavedScores();

        SaveScores(savedScores);

        UpdateUI(savedScores);
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

        return JsonUtility.FromJson<ScoreboardSavedData>(json);
    }

    private void UpdateUI(ScoreboardSavedData savedScores)
    {
        int scoreOffset = -50;
        foreach(Transform child in entryContainer)
        {
            Destroy(child.gameObject);
        }
        if (savedScores != null)
        {
            for (int i = 0; i < savedScores.highscores.Count; i++)
            {
                GameObject entry = Instantiate(entryTemplate, entryContainer);
                entry.transform.Find("RankText").GetComponent<TMP_Text>().text = (i + 1).ToString();
                entry.transform.localPosition = new Vector3(0, (scoreOffset * i));
                entry.GetComponent<ScoreboardEntryUI>().InitializeScoreboard(savedScores.highscores[i]);
            }
        }
    }

    public void ResetLeaderboard()
    {
        if (File.Exists(SavePath))
        {
            File.Delete(SavePath);
        }

        ScoreboardSavedData savedScores = GetSavedScores();

        UpdateUI(savedScores);
    }

    private void SaveScores(ScoreboardSavedData scoreboardSavedData)
    {
        using StreamWriter stream = new StreamWriter(SavePath);
        string json = JsonUtility.ToJson(scoreboardSavedData, true);

        stream.Write(json);
    }

    public void GoToMenu()
    {
        SceneManager.LoadScene(1);
    }
    }
