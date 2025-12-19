using TMPro;
using UnityEngine;

public class NameTag : MonoBehaviour
{
    public TMP_Text nameText;

    private PlayerName playerName;

    void Awake()
    {
        playerName = GetComponentInParent<PlayerName>();
    }

    void LateUpdate()
    {
        if (nameText == null) return;

        if (playerName == null)
            playerName = GetComponentInParent<PlayerName>();

        if (playerName == null) return;

        nameText.text = playerName.GetDisplayName();
    }
}
