using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoseLife : MonoBehaviour
{
    public Movement parent;
    public GameObject contain;
    public float currentHearts;

    public Sprite alive;
    public Sprite dead;

    // Start is called before the first frame update
    void Start()
    {
        currentHearts = parent.health;
    }

    // Update is called once per frame
    void Update()
    {
       currentHearts = parent.health;
       UpdateHearts();
    }

     void UpdateHearts()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform heart = transform.GetChild(i);

            if (i < currentHearts)
                heart.GetComponent<SpriteRenderer>().sprite = alive;   // show heart
            else
                heart.GetComponent<SpriteRenderer>().sprite = dead; // hide heart
            
            heart.gameObject.SetActive(true);
        }
    }
}