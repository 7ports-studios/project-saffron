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
    public Transform player;

    public bool playerIsBehind = false;
    Collider[] inSight;
    // Start is called before the first frame update
    void Start()
    {
        guardAgent = GetComponent<NavMeshAgent>();
        startGuardPatrol();
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }


    private void Update()
    {
        var heading = player.position - transform.position;
        float dot = Vector3.Dot(heading, transform.forward);

        if(dot < 0 )
        {
            playerIsBehind = true;
        }
        else
        {
            playerIsBehind = false;
        }
    }


    private void FixedUpdate()
    {
        inSight = Physics.OverlapSphere(transform.position + (transform.forward*5), 5f);
        foreach(Collider collider in inSight)
        {
            //only compute line of sight if the object is relevant (i.e. player or a dead body)
            if (collider.CompareTag("Player"))
            {
                RaycastHit hit = new RaycastHit();
                Physics.Raycast(new Ray(transform.position + transform.forward, collider.transform.position - transform.position), out hit, 20);
                if (hit.collider != null)
                {
                    Debug.Log(hit.collider.name);
                    if (hit.collider.Equals(collider) && !actionPointsController.instance.caught)
                    {
                        guardAnimator.SetTrigger("point");
                        guardAgent.isStopped = true;
                        actionPointsController.instance.caught = true;
                        actionPointsController.instance.StartCoroutine("beingCaught");
                    }

                }
            }
        }
    }



    IEnumerator waitForTime()
    {
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        yield return new WaitUntil(() => !actionPointsController.instance.caught);
        guardAnimator.SetTrigger("run");
        guardAgent.isStopped = false;
        guardAgent.SetDestination(player.transform.position);

    }

    public void getStrangled()
    {
        if (!dead && playerIsBehind)
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


    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawSphere(transform.position + transform.forward * 5, 5f);
    }
}
