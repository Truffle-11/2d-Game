using TMPro;
using UnityEngine;

public class UsernameSelector : MonoBehaviour
{
    public TMP_InputField usernameInputField;

    public void SubmitUsername()
    {
        if (usernameInputField == null)
        {
            Debug.LogWarning("UsernameSelector: usernameInputField is not assigned.");
            SessionInfo.Username = "Player";
            return;
        }

        string name = usernameInputField.text.Trim();

        if (string.IsNullOrEmpty(name))
        {
            name = "Player";
        }

        SessionInfo.Username = name;
        Debug.Log("UsernameSelector: saved username as '" + SessionInfo.Username + "'");
    }
}
