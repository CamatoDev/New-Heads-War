using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviour
{
    public float startHealth = 100f;
    public float currentHealth;
    public float points;

    [Header("UI")]
    public Text score;
    //public Image health;

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = startHealth;
        points = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        score.text = "Score : " + points;

        //health.fillAmount = currentHealth / startHealth;
    }
}
