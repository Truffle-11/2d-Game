using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoseLife : MonoBehaviour
{
    public Movement parent;
    public GameObject contain;
    public float currentHearts;

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
                heart.gameObject.SetActive(true);   // show heart
            else
                heart.gameObject.SetActive(false);  // hide heart
        }
    }
}