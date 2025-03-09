using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    // Variables de la cible 
    public Transform target;
    //Rayon de l'explosion
    public float explosionRadius = 2.0f;

    // Variables pour le déplacement
    [Header("Enemy mouvement")]
    public float moveSpeed = 5.0f;
    public float distanceMax = 40.0f;
    public float chaseRange = 10.0f;
    public float attackRange = 1.0f;
    public float orbitDistance = 5.0f; // Distance à laquelle le drone orbite autour du joueur
    public float orbitSpeed = 2.0f; // Vitesse à laquelle le drone orbite autour du joueur
    public float heightVariationSpeed = 1.0f; // Vitesse de variation de la hauteur
    public float heightVariationRange = 2.0f; // Amplitude de la variation de hauteur
    //Distance entre le joueur et l'ennemi
    private float distance;

    [Header("Enemy Stats")]
    public float coin = 100f;
    //Pour la vie de l'ennemi 
    public float startHealth = 100;
    private float health;

    [Header("Clone Option")]
    // Variables pour le dédoublement
    public float cloneCooldown = 5.0f; // Temps entre chaque dédoublement
    private float cloneTimer = 0.0f;
    public GameObject dronePrefab; // Préfabriqué du drone à cloner
    public float cloneSpawnDistance = 3.0f; // Distance minimale entre le clone et l'objet de référence
    public int maxDrones = 12; // Nombre maximum de drones autorisés

    // Variables pour la variation de hauteur
    private float baseHeight;
    private float targetHeight;
    private float heightTimer;

    // Liste statique pour garder une trace de tous les drones
    private static List<EnemyAI> allDrones = new List<EnemyAI>();

    // Start is called before the first frame update
    void Start()
    {
        // Assignation des variables de lancement
        target = GameObject.FindGameObjectWithTag("Player").transform;
        health = startHealth;
        cloneTimer = cloneCooldown; // Initialisation du timer de dédoublement

        // Initialisation de la hauteur de base
        baseHeight = transform.position.y;
        targetHeight = baseHeight;
        heightTimer = 0.0f;

        // Ajouter ce drone à la liste des drones
        allDrones.Add(this);
    }

    // Update is called once per frame
    void Update()
    {
        // Teste si la cible existe 
        if (target == null) return;

        distance = Vector3.Distance(transform.position, target.position);

        // Si le drone est trop loin, il revient vers le joueur
        if (distance > distanceMax)
        {
            ReturnToPlayer();
        }
        else
        {
            // On orbite autour du joueur
            OrbitPlayer();
            VaryHeight(); // Variation de la hauteur
        }

        // Si le drone est à la bonne distance, il tourne autour du joueur 
        if (distance <= distanceMax)
        {
            RotateAroundPlayer();
            VaryHeight(); // Variation de la hauteur
        }

        // Gestion du dédoublement
        cloneTimer -= Time.deltaTime;
        if (cloneTimer <= 0)
        {
            if (allDrones.Count < maxDrones)
            {
                CloneDrone();
            }
            // Ajuster dynamiquement le cooldown en fonction du nombre de drones
            cloneTimer = cloneCooldown * (1 + (allDrones.Count / (float)maxDrones));
        }
    }

    // Fonction pour orbiter autour du joueur
    public void OrbitPlayer()
    {
        Vector3 direction = (target.position - transform.position).normalized;
        Vector3 orbitPosition = target.position + (transform.position - target.position).normalized * orbitDistance;

        // Maintenir la distance orbitale
        transform.position = Vector3.MoveTowards(transform.position, orbitPosition, moveSpeed * Time.deltaTime);
    }

    // Fonction pour la rotation ciculaire autour du joueur 
    public void RotateAroundPlayer()
    {
        // Mouvement circulaire autour du joueur
        transform.RotateAround(target.position, Vector3.up, orbitSpeed * Time.deltaTime);
    }

    // Fonction pour revenir vers le joueur si trop loin
    public void ReturnToPlayer()
    {
        Vector3 direction = (target.position - transform.position).normalized;
        transform.position = Vector3.MoveTowards(transform.position, target.position + direction * orbitDistance, moveSpeed * Time.deltaTime);
    }

    // Fonction pour varier la hauteur
    public void VaryHeight()
    {
        heightTimer += Time.deltaTime * heightVariationSpeed;

        // Changer la hauteur cible de manière aléatoire
        if (heightTimer >= 1.0f)
        {
            targetHeight = baseHeight + Random.Range(-heightVariationRange, heightVariationRange);
            heightTimer = 0.0f;
        }

        // Interpolation vers la nouvelle hauteur
        float newY = Mathf.Lerp(transform.position.y, targetHeight, Time.deltaTime * heightVariationSpeed);
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }

    // Fonction pour l'attaque explosive
    public void Explosion()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Player"))
            {
                // Infliger des dégâts au joueur
                hitCollider.GetComponent<PlayerActions>().TakeDamage(50); // Exemple de dégâts
            }
        }
        // Détruire le drone après l'explosion
        Dead();
    }

    public void Damage(float damage)
    {
        health -= damage;

        if (health <= 0)
        {
            Dead();
        }
    }

    public void Dead()
    {
        target.gameObject.GetComponent<PlayerStats>().points += coin;
        // Retirer ce drone de la liste des drones
        allDrones.Remove(this);
        // Rajouter une animation d'explosion 
        Destroy(gameObject, 1.5f);
    }

    // Fonction pour cloner le drone
    public void CloneDrone()
    {
        if (dronePrefab != null)
        {
            // Calculer une position aléatoire à une distance suffisante
            Vector3 spawnPosition;
            do
            {
                spawnPosition = transform.position + new Vector3(Random.Range(-cloneSpawnDistance, cloneSpawnDistance), 0, Random.Range(-cloneSpawnDistance, cloneSpawnDistance));
            } while (Vector3.Distance(spawnPosition, transform.position) < cloneSpawnDistance);

            // Instancier le clone
            Instantiate(dronePrefab, spawnPosition, Quaternion.identity);
        }
    }

    // Fonction pour nettoyer la liste des drones (au cas où)
    private void OnDestroy()
    {
        allDrones.Remove(this);
    }
}