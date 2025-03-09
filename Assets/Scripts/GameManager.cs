using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public Text timerText;
    float timer;

    [Header("UI")]
    public SceneFader sceneFader;
    public AudioSource pauseAudioSource;
    bool pause;
    public Button shoot;
    public GameObject pauseMenuUI;
    public GameObject gameOverUI;
    bool gameEnded;

    // Start is called before the first frame update
    void Start()
    {
        timer = 180f;
        gameEnded = false;
        pause = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (gameEnded)
        {
            return;
        }

        // Gestion du temps 
        timer -= Time.deltaTime;
        timer = Mathf.Clamp(timer, 0f, Mathf.Infinity);
        timerText.text = string.Format("{0:00.00}", timer);

        if(timer <= 0f)
        {
            GameOver();
        }
    }

    public void Toggle()
    {
        pauseAudioSource.Play();
        pause = !pause;

        //mettre le temps sur pause si on est en pause et le faire continuer sinon 
        if (pause)
        {
            shoot.interactable = false;
            pauseMenuUI.SetActive(true);
            Time.timeScale = 0f; //temps en pause 
        }
        else
        {
            shoot.interactable = true;
            pauseMenuUI.SetActive(false);
            Time.timeScale = 1f; //temps sur play 
        }
    }

    public void Menu()
    {
        pauseAudioSource.Play();
        Time.timeScale = 1f;
        sceneFader.FadeTo("MainMenu");
    }

    public void GameOver()
    {
        gameEnded = true;
        Debug.Log("Fin de la partie !");
        gameOverUI.SetActive(true);
    }
}
