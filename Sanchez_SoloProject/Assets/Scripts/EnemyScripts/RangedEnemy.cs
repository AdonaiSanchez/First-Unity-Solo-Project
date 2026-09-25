using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class RangedEnemy : BasicEnemy
{
    public Transform firepoint;
    public GameObject projectile;

    public float projVelocity;
    public float projLifespan;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        firepoint = transform.GetChild(0);
    }

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
                GameObject p = Instantiate(projectile, firepoint.position, gameObject.transform.rotation);
                p.GetComponent<BulletDmg>().damage = attackDmg;
                p.GetComponent<Rigidbody>().AddForce(gameObject.transform.forward * projVelocity);
                Destroy(p, projLifespan);
            }

            StartCoroutine("attackCharge");
        }

        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }
}
