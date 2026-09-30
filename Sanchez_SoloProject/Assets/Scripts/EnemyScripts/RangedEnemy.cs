using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class RangedEnemy : BasicEnemy
{
    public bool stationary = false;
    public bool friendlyBlocking = false;

    public Transform firepoint;
    public GameObject projectile;

    public float projVelocity;
    public float projLifespan;

    Ray friendlyCheck;

    void Start()
    {
        warning = transform.GetChild(0).gameObject;
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        firepoint = transform.GetChild(1);

        friendlyCheck = new Ray();
    }

    void Update()
    {
        float targetDistance = Vector3.Distance(player.transform.position, transform.position);

        isFollowing = targetDistance <= detectionRange;

        friendlyCheck.origin = firepoint.transform.position;
        friendlyCheck.direction = firepoint.transform.forward;

        if (targetDistance <= detectionRange && !friendlyBlocking)
            canAttack = true;
        else
            canAttack = false;

        if (isFollowing && !attacking && !stationary)
        {
            agent.destination = player.transform.position;
        }

        if (canAttack)
        {
            transform.LookAt(player.transform.position);
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
        }
    }

    override public void Attack()
    {
        if (canAttack)
        {
            GameObject p = Instantiate(projectile, firepoint.position, gameObject.transform.rotation);
            p.GetComponent<BulletDmg>().damage = attackDmg;
            p.GetComponent<Rigidbody>().AddForce(gameObject.transform.forward * projVelocity);
            Destroy(p, projLifespan);
        }
    }
}
