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
        {
            return generatedNames[Random.Range(0, generatedNames.Length)];
        }

        return "Player";
    }
}
