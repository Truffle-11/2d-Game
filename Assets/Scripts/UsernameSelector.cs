using TMPro;
using UnityEngine;

public class UsernameSelector : MonoBehaviour
{
    public TMP_InputField usernameInputField;

    public string[] wordType1;
    public string[] wordType2;

    public string[] rareWordType1;
    public string[] rareWordType2;

    [Range(0f, 100f)]
    public float rareChancePercentage = 1f;

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

    private string GetRandomName()
    {
        bool rare = Random.value * 100f < rareChancePercentage;

        if (rare)
        {
            return GetRareCombo();
        }

        return GetNormalCombo();
    }

    private string GetNormalCombo()
    {
        if (wordType1.Length == 0 || wordType2.Length == 0)
            return "Player";

        int i1 = Random.Range(0, wordType1.Length);
        int i2 = Random.Range(0, wordType2.Length);
        return wordType1[i1] + wordType2[i2];
    }

    private string GetRareCombo()
    {
        bool rare1Available = rareWordType1.Length > 0;
        bool rare2Available = rareWordType2.Length > 0;

        bool normal1Available = wordType1.Length > 0;
        bool normal2Available = wordType2.Length > 0;

        bool useRare1 = rare1Available && Random.value < 0.5f;
        bool useRare2 = rare2Available && Random.value < 0.5f;

        string part1 = useRare1 && rare1Available
            ? rareWordType1[Random.Range(0, rareWordType1.Length)]
            : wordType1[Random.Range(0, wordType1.Length)];

        string part2 = useRare2 && rare2Available
            ? rareWordType2[Random.Range(0, rareWordType2.Length)]
            : wordType2[Random.Range(0, wordType2.Length)];

        return part1 + part2;
    }

    public void SubmitColor()
    {
        if (SessionInfo.PlayerColor == Color.white)
        {
            Color32[] colors = new Color32[]
            {
                new Color32(255, 0, 0, 255),
                new Color32(0, 0, 255, 255),
                new Color32(0, 255, 0, 255),
                new Color32(255, 255, 0, 255),
                new Color32(255, 127, 0, 255),
                new Color32(0, 255, 255, 255),
                new Color32(255, 127, 255, 255),
                new Color32(127, 0, 255, 255),
                new Color32(127, 255, 0, 255)
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
