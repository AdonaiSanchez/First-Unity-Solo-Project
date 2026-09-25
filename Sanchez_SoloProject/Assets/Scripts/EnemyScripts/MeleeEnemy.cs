using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.AI;

public class BasicEnemy : MonoBehaviour
{
    public bool isFollowing = false;
    public bool canAttack = false;
    public bool attacking = false;

    public float attackCooldown = 1f;
    public float detectionRange = 5f;
    public float attackRange = 1f;
    public float attackSpeed = 1f;

    public int attackDmg = 20;
    public int health = 50;
    public int maxHealth = 50;

    public NavMeshAgent agent;
    public PlayerController player;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        float targetDistance = Vector3.Distance(player.transform.position, transform.position);

        isFollowing = targetDistance <= detectionRange;
        canAttack = targetDistance <= attackRange;

        if (isFollowing && !attacking)
        {
            agent.destination = player.transform.position;
        }

        if (canAttack && !attacking)
        {
            attacking = true;
            agent.destination = gameObject.transform.position;
            
            if (canAttack)
            {
               player.health -= attackDmg;
            }

            StartCoroutine("attackCharge");
        }

        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Projectile")
        {
            health -= collision.gameObject.GetComponent<BulletDmg>().damage;
            Destroy(collision.gameObject);
        }
    }

    IEnumerator attackCharge()
    {
        yield return new WaitForSeconds(attackSpeed);

        StartCoroutine("attackingCooldown");
    }

    IEnumerator attackingCooldown()
    {
        yield return new WaitForSeconds(attackCooldown);

        attacking = false;
    }
}
