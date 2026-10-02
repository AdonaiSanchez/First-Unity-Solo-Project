using System.Collections;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class BasicEnemy : MonoBehaviour
{
    public bool canLunge = false;
    public bool isFollowing = false;
    public bool canAttack = false;
    public bool attacking = false;
    public bool lunging;

    public float attackCooldown = 1f;
    public float detectionRange = 5f;
    public float attackRange = 1f;
    public float attackSpeed = 1f;
    public float lungeRange = 5f;
    public float lungeCool = 2f;

    public int attackDmg = 20;
    public int health = 50;
    public int maxHealth = 50;

    public NavMeshAgent agent;
    public PlayerController player;
    public GameObject warning;
    public GameObject gameManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameManager = GameObject.Find("GameManager");
        warning = transform.GetChild(0).gameObject;
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        float targetDistance = Vector3.Distance(player.transform.position, transform.position);

        canAttack = targetDistance <= attackRange;
        canLunge = targetDistance <= lungeRange;

        if (!attacking)
        {
            agent.destination = player.transform.position;
        }

        if (canAttack && !attacking)
        {
            attacking = true;
            agent.destination = gameObject.transform.position;

            StartCoroutine("attackCharge");
        }

        if (health <= 0)
        {
            Destroy(gameObject);

            gameManager.GetComponent<GameManager>().enemiesAlive--;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Projectile")
        {
            health -= collision.gameObject.GetComponent<BulletDmg>().damage;
            Destroy(collision.gameObject);
        }
    }

    virtual public void Attack()
    {
        if (canAttack)
        {
            player.health -= attackDmg;
        }
    }

    IEnumerator lungeDuration()
    {
        agent.speed = 8f;
        lunging = false;

        yield return new WaitForSeconds(1.5f);

        agent.speed = 3.5f;
        StartCoroutine("lungeCooldown");
    }

    IEnumerator lungeCooldown()
    {
        yield return new WaitForSeconds(lungeCool);

        lunging = false;
    }

    IEnumerator attackCharge()
    {
        warning.SetActive(true);

        yield return new WaitForSeconds(attackSpeed);

        warning.SetActive(false);

        Attack();

        StartCoroutine("attackingCooldown");
    }

    IEnumerator attackingCooldown()
    {
        yield return new WaitForSeconds(attackCooldown);

        attacking = false;
    }
}
