using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private Transform player;

    public float speed = 30f;

    public int damage = 100;

    public float bulletRadius = 3f;

    //public GameObject impactEffect;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
        float Distance = Vector3.Distance(transform.position, player.position);

        if (Distance > 100f)
        {
            Destroy(gameObject);
            return;
        }

        //GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        //foreach (GameObject enemy in enemies)
        //{
        //    float DistanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position);

        //    if(DistanceToEnemy <= bulletRadius)
        //    {
        //        Damage(enemy.transform);
        //    }
        //}

        Collider[] collides = Physics.OverlapSphere(transform.position, bulletRadius);
        foreach (Collider collider in collides)
        {
            if (collider.tag == "Enemy")
            {
                Damage(collider.transform);
            }
        }

        // Mouvement de la balle suivant l'orientation de l'écran
        transform.position += transform.forward * Time.deltaTime * speed;
    }
    //void HitTarget()
    //{
    //    GameObject effectIns = (GameObject)Instantiate(impactEffect, transform.position, transform.rotation);
    //    Destroy(effectIns, 5f);
    //}
    void Damage(Transform enemy)
    {
        EnemyAI e = enemy.GetComponent<EnemyAI>();
        if (e != null)
        {
            e.Damage(damage);
        }
        else
        {
            Debug.Log("Pas de script Enemy sur l'enemi.");
        }

        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, bulletRadius);
    }
}
// Diriger la balle tout droit et la detruire lorsqu'elle touche le drone (ou si elle vas trop loin)