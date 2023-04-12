using UnityEngine;
using TMPro;


    public class ScoreboardEntryUI : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI entryNameText;
        [SerializeField] TextMeshProUGUI entryScoreText;
        [SerializeField] TextMeshProUGUI entryRankText;

        // Set the UI Text elements to match the ScoreboardEntryData.
        public void InitializeScoreboard(ScoreboardEntryData scoreboardEntryData, int rank)
        {
            entryNameText.text = scoreboardEntryData.playerName;
            entryScoreText.text = scoreboardEntryData.score.ToString();
            entryRankText.text = (rank + 1).ToString();
        }
    }

