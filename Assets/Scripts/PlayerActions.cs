using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerActions : MonoBehaviour
{
    [Header("Variables")]
    private PlayerStats playerStats;
    public GameManager gameManager;
    public Button shootButton;
    public AudioSource audioSource;


    [Header("Shoot Action")]
    public float shootRate = 1f;
    private float spawnProtection;
    private float shootCountdown = 0f;
    public GameObject bulletPrefab;
    public Transform shootPoint;

    private void Start()
    {
        playerStats = gameObject.GetComponent<PlayerStats>();
        spawnProtection = 10f;
    }
    private void Update()
    {
        spawnProtection -= Time.deltaTime;

        if(spawnProtection > 0f)
        {
            shootButton.interactable = false;
        }
        else
        {
            shootButton.interactable = true;
        }
    }
    public void Shoot()
    {
        if(shootCountdown <= 0)
        {
            GameObject bulletGO = (GameObject)Instantiate(bulletPrefab, shootPoint.position, shootPoint.rotation);

            shootCountdown = 1 / shootRate;
        }

        shootCountdown -= 10;
        audioSource.Play();

        StartCoroutine(ShootColdDown());
    }

    public void TakeDamage(float damage)
    {
        playerStats.currentHealth -= damage;

        if (playerStats.currentHealth <= 0)
        {
            gameManager.GameOver();
        }
    }

    //création de la couroutin pour désactivé le spiritual Shot 
    IEnumerator ShootColdDown()
    {
        shootButton.interactable = false;
        yield return new WaitForSeconds(2f);
        shootButton.interactable = true;
    }
}
