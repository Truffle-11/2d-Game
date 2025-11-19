using TMPro;
using UnityEngine;

public class UsernameSelector : MonoBehaviour
{
    public TMP_InputField usernameInputField;

    public string[] wordType1;
    public string[] wordType2;

    private string[] generatedNames;

    private void Awake()
    {
        GenerateNames();
    }

    private void Start()
    {
        SubmitUsername();
        SubmitColor();
    }

    public void SubmitUsername()
    {
        string name = usernameInputField != null
            ? usernameInputField.text.Trim()
            : "";

        if (string.IsNullOrEmpty(name))
        {
            name = GetRandomName();
        }

        SessionInfo.Username = name;
        Debug.Log("UsernameSelector: saved username as '" + SessionInfo.Username + "'");
    }

    private void GenerateNames()
    {
        int total = wordType1.Length * wordType2.Length;
        generatedNames = new string[total];

        int index = 0;

        foreach (string w1 in wordType1)
        {
            foreach (string w2 in wordType2)
            {
                generatedNames[index] = w1 + w2;
                index++;
            }
        }
    }

    private string GetRandomName()
    {
        if (generatedNames != null && generatedNames.Length > 0)
            return generatedNames[Random.Range(0, generatedNames.Length)];

        return "Player";
    }

    public void SubmitColor()
    {
        if (SessionInfo.PlayerColor == Color.white)
        {
            Color32[] colors = new Color32[]
            {
                new Color32(255, 0, 0, 255),     // Red
                new Color32(0, 0, 255, 255),     // Blue
                new Color32(0, 255, 0, 255),     // Green
                new Color32(255, 255, 0, 255),   // Yellow
                new Color32(255, 127, 0, 255),   // Orange
                new Color32(0, 255, 255, 255),   // Cyan
                new Color32(255, 127, 255, 255), // Pink
                new Color32(127, 0, 255, 255),   // Purple
                new Color32(127, 255, 0, 255)    // Lime green
            };

            SessionInfo.PlayerColor = colors[Random.Range(0, colors.Length)];
            Debug.Log("Random color assigned: " + SessionInfo.PlayerColor);
        }
    }

    public void SetColor(Color32 color)
    {
        SessionInfo.PlayerColor = color;
    }
}
