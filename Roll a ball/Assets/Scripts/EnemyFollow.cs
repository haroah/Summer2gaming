using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyState
{
    Patrolling,     
    Chasing,
}


public class EnemyFollow : MonoBehaviour
{

    public EnemyState currentState;
    private NavMeshAgent _Agent;
    public Transform target;
    public float detectionRadius = 5.0f; // radius will detect player
    public LayerMask detectibleLayers;

    [Header("Patrol Settings"), Space(10)]

    [Tooltip("List of transforms the spider randomly patrolls to")]
    public List<Transform> patrolPoints;

    [Tooltip("Randomly generated index for selecting current patrol point")]
    public int patrolIndex;

    [Tooltip("Min amount of time this waits after reaching a patrol point before moving to the next")]
    public float minWaittime = 1f;

    [Tooltip("Max amount of time this waits after reaching a patrol point before moving to the next")]
    public float maxWaittime = 3f;
    
    // Start is called before the first frame update
    void Start()
    {
        currentState = EnemyState.Patrolling;
        _Agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        if(PlayerInDetection() && currentState != EnemyState.Chasing)
        {
            // enemy follow player
            currentState = EnemyState.Chasing;
            StartCoroutine(Patrol());
            StartCoroutine(ChasePlayer());
        } else if (!PlayerInDetection() && currentState != EnemyState.Patrolling)
        {
            // enemy patolls
            currentState = EnemyState.Patrolling;
            StopCoroutine(ChasePlayer());
            StartCoroutine(ChasePlayer());
        }
    }

    private IEnumerator ChasePlayer()
    {
        while(PlayerInDetection() && currentState == EnemyState.Chasing) // only chases player if within detection
        {
            _Agent.SetDestination(target.position);

            yield return null;
        } 
    }
    
    private IEnumerator Patrol()
    {
       while (!PlayerInDetection() && currentState == EnemyState.Patrolling)
       {
            // Pick random Location
            int index = Random.RandomRange(0, patrolPoints.Count);
            patrolIndex = index;
            // Move to location
            _Agent.SetDestination(patrolPoints[patrolIndex].position);
            // Wait for Spider to reach patrol point
            while (Vector3.Distance(transform.position, patrolPoints[patrolIndex].position) > 1)
                yield return null;
            // Has reached its point, now it needs to wait
            yield return new WaitForSeconds(Random.Range(minWaittime, maxWaittime));


            yield return null;
       }
    }
    
    private bool PlayerInDetection()
    {
       return Physics.CheckSphere(transform.position, detectionRadius, detectibleLayers);
       
    }


    void onDrawGizmoSelcted()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
