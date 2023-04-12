using UnityEngine;
using TMPro;

public class PlayerData : MonoBehaviour
{
    public static PlayerData Instance;
    public TextMeshProUGUI nameField;
    public static string playerName;

    // Mark the first instance of PlayerData as DontDestoyOnLoad, otherwise destroy it.
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

}
