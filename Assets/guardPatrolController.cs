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
        startGuardPatrol();
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
            patrolIndex++;
            if (patrolIndex >= navMeshDestinations.Length)
            {
                patrolIndex = 0;
            }
            startGuardPatrol();

        }
        if (other.CompareTag("oil"))
        {
            guardAgent.isStopped = true;
            guardAnimator.SetTrigger("slip");
            StartCoroutine("waitToGetUp");
            Destroy(other.gameObject);
        }
    }
    
    IEnumerator waitToGetUp()
    {
        yield return new WaitForSeconds(2);
        guardAnimator.SetTrigger("get up");

        guardAgent.isStopped = false;
        startGuardPatrol();

        yield return null;
    }


    void startGuardPatrol()
    {
        Debug.Log("rerouting");
        guardAgent.SetDestination(navMeshDestinations[patrolIndex].position);
    }
}
