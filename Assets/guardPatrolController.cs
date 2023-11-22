using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class guardPatrolController : MonoBehaviour
{


    [SerializeField] private Transform[] navMeshDestinations;
    int patrolIndex = 0;
    NavMeshAgent guardAgent;
    [SerializeField] private Animator guardAnimator;
    public bool dead = false;
    // Start is called before the first frame update
    void Start()
    {
        guardAgent = GetComponent<NavMeshAgent>();
        guardAgent.SetDestination(navMeshDestinations[patrolIndex].position);
    }


    public void getStrangled()
    {
        if (!dead)
        {
            guardAgent.isStopped = true;
            guardAnimator.SetTrigger("strangle");
            dead = true;
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("patrolPoint"))
        {
            Debug.Log("rerouting");
            patrolIndex++;
            if (patrolIndex >= navMeshDestinations.Length)
            {
                patrolIndex = 0;
            }
            guardAgent.SetDestination(navMeshDestinations[patrolIndex].position);

        }
    }
}
