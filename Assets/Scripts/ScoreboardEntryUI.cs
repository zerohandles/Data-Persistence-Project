using UnityEngine;
using TMPro;


    public class ScoreboardEntryUI : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI entryNameText;
        [SerializeField] TextMeshProUGUI entryScoreText;
        [SerializeField] TextMeshProUGUI entryRankText;

        public void InitializeScoreboard(ScoreboardEntryData scoreboardEntryData)
        {
            entryNameText.text = scoreboardEntryData.playerName;
            entryScoreText.text = scoreboardEntryData.score.ToString();
        }
    }

