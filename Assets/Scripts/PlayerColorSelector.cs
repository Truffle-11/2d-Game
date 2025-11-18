using UnityEngine;

public class PlayerCustomization : MonoBehaviour
{
    public void SetRed()
    {
        SessionInfo.PlayerColor = new Color32(255, 0, 0, 255);
        Debug.Log("Color set to RED: " + SessionInfo.PlayerColor);
    }

    public void SetBlue()
    {
        SessionInfo.PlayerColor = new Color32(0, 0, 255, 255);
        Debug.Log("Color set to BLUE: " + SessionInfo.PlayerColor);
    }

    public void SetGreen()
    {
        SessionInfo.PlayerColor = new Color32(0, 255, 0, 255);
        Debug.Log("Color set to GREEN: " + SessionInfo.PlayerColor);
    }

    public void SetYellow()
    {
        SessionInfo.PlayerColor = new Color32(255, 255, 0, 255);
        Debug.Log("Color set to YELLOW: " + SessionInfo.PlayerColor);
    }

    public void SetOrange()
    {
        SessionInfo.PlayerColor = new Color32(255, 127, 0, 255);
        Debug.Log("Color set to ORANGE: " + SessionInfo.PlayerColor);
    }

    public void SetCyan()
    {
        SessionInfo.PlayerColor = new Color32(0, 255, 255, 255);
        Debug.Log("Color set to CYAN: " + SessionInfo.PlayerColor);
    }

    public void SetPink()
    {
        SessionInfo.PlayerColor = new Color32(255, 127, 255, 255);
        Debug.Log("Color set to PINK: " + SessionInfo.PlayerColor);
    }

    public void SetPurple()
    {
        SessionInfo.PlayerColor = new Color32(127, 0, 255, 255);
        Debug.Log("Color set to PURPLE: " + SessionInfo.PlayerColor);
    }

    public void SetLimeGreen()
    {
        SessionInfo.PlayerColor = new Color32(127, 255, 0, 255);
        Debug.Log("Color set to LIME GREEN: " + SessionInfo.PlayerColor);
    }
}
