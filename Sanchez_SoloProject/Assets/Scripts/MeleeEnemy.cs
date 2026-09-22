using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class MeleeEnemy : MonoBehaviour
{
    public bool isFollowing = false;
    public bool attacking = false;

    public float attackCooldown = 1f;

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
        if (isFollowing && !attacking)
        {
            agent.destination = player.transform.position;
        }
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            isFollowing = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            isFollowing = false;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            attacking = true;

            StartCoroutine("attackingCooldown");
            //Stop guy
            //Hurt Player
            // Run Coroutine for attack cooldown
        }
    }

    IEnumerator attackingCooldown()
    {
        yield return new WaitForSeconds(attackCooldown);

        attacking = false;
    }
}
