using TMPro;
using UnityEngine;

public class JoinCode : MonoBehaviour
{
    public TMP_Text joinCodeText;

    private void Start()
    {
        Debug.Log("JoinCode.Start, SessionJoinCode = " + SessionInfo.JoinCode);

        if (joinCodeText != null)
        {
            joinCodeText.text = SessionInfo.JoinCode;
        }
        else
        {
            Debug.LogError("Join code text refrence is missing on " + gameObject.name);
        }
    }
}
