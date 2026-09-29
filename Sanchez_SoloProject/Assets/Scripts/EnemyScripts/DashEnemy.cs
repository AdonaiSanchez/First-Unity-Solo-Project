using UnityEngine;
using UnityEngine.AI;

public class DashEnemy : BasicEnemy
{
   void Update()
    {
        float targetDistance = Vector3.Distance(player.transform.position, transform.position);

        isFollowing = targetDistance <= detectionRange;
        canAttack = targetDistance <= attackRange;
        canLunge = targetDistance <= lungeRange;

        if (isFollowing && !attacking)
        {
            agent.destination = player.transform.position;
        }

        if (canLunge && !attacking && !canAttack)
        {
            StartCoroutine("lungeDuration");
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
}
