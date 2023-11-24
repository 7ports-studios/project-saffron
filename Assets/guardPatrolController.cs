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
    bool playerDetected = false;
    //number of frames the player needs to stay out of sight to avoid continually being chased
    int framesToLose = 5000;
    int framesOutOfSight = 0;
    int framesSinceReset = 0;
    public float runSpeed = 3.5f;
    public float walkSpeed = 2.5f;
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

        if(playerDetected)
        {
            Debug.Log("searching for Player");
            RaycastHit hit = new RaycastHit();
            Physics.Raycast(new Ray(transform.position + transform.forward, player.transform.position - transform.position), out hit, 20);
            if(hit.collider != null )
            {
                if (!hit.collider.CompareTag("Player"))
                {
                    framesOutOfSight++;
                    framesSinceReset++;
                }
                else
                {
                    Debug.Log("player Spotted");
                    framesOutOfSight = 0;
                    framesSinceReset = 0;
                    guardAgent.SetDestination(player.transform.position);
                    //guardAnimator.SetTrigger("run");
                    guardAgent.speed = runSpeed;
                }
                if (framesSinceReset >= 1000)
                {
                    Debug.Log("fuck they're not here");
                    guardAgent.SetDestination(player.transform.position);
                    framesSinceReset = 0;
                }
                if (framesOutOfSight >= framesToLose)
                {
                    Debug.Log("Player lost");
                    playerDetected = false;
                    guardAnimator.SetTrigger("walk");
                    guardAgent.isStopped = false;
                    framesOutOfSight = 0;
                    framesSinceReset= 0;
                    guardAgent.speed = walkSpeed;
                    guardAnimator.SetBool("chasing", false);
                    startGuardPatrol();
                }
            }
        }
    }


    private void FixedUpdate()
    {
        inSight = Physics.OverlapSphere(transform.position + (transform.forward*6), 6f);
        foreach(Collider collider in inSight)
        {
            //only compute line of sight if the object is relevant (i.e. player or a dead body)
            if (collider.CompareTag("Player"))
            {
                RaycastHit hit = new RaycastHit();
                Physics.Raycast(new Ray(transform.position + transform.forward, collider.transform.position - transform.position), out hit, 20);
                if (hit.collider != null)
                {
                    if (hit.collider.Equals(collider) && !actionPointsController.instance.caught && !playerDetected)
                    {
                        guardAnimator.SetTrigger("point");
                        guardAgent.isStopped = true;
                        actionPointsController.instance.caught = true;
                        actionPointsController.instance.StartCoroutine("beingCaught");
                        StartCoroutine("waitForTime");
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
        playerDetected = true;
        guardAnimator.SetTrigger("run");
        guardAgent.isStopped = false;
        guardAgent.SetDestination(player.transform.position);
        guardAgent.speed = runSpeed;
        guardAnimator.SetBool("chasing", true);

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
        guardAgent.SetDestination(navMeshDestinations[patrolIndex].position);
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawSphere(transform.position + transform.forward * 6, 6f);
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            Destroy(collision.gameObject);
        }
    }
}
