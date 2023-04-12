using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] TMP_InputField nameField;
    [SerializeField] PlayerData playerData;

    public void StartGame()
    {
        // If player entered a name load the main game scene
        if (PlayerData.playerName != null)
        {
            if (PlayerData.playerName.Length > 1)
            {
                SceneManager.LoadScene("main");
            }
        }

        // Highlight input field if no name was entered
        var colors = nameField.colors;
        colors.normalColor = Color.red;
        nameField.colors = colors;
    }

    public void EnterPlayerName()
    {
        PlayerData.playerName = nameField.text;
    }

    // Exit the application
    public void ExitGame()
    {
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
#else
        Application.Quit();
#endif
        
    }
}
