using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
namespace MimicSpace
{
    public enum EnemyState
    {
        Patrolling,
        ChasingPlayer
    }
    /// <summary>
    /// This is a very basic movement script, if you want to replace it
    /// Just don’t forget to update the Mimic’s velocity vector with a Vector3(x, 0, z)
    /// </summary>
    public class Movement : MonoBehaviour
    {
        [SerializeField] private GameObject _player;
        [Header("Mimic Settings"), Space(10)]
        public EnemyState currentState;
        [Tooltip("Body Height from ground")]
        [Range(0.5f, 5f)]
        public float height = 0.8f;
        public float speed = 5f;
        public float velocityLerpCoef = 4f;
        Vector3 velocity = Vector3.zero;
        Mimic myMimic;
        [Space(10), Header("Patrol Settings"), Space(10)]
        public List<Transform> patrolPoints;
        [SerializeField] private int patrolIndex; // The current index of the partol point this is following READ ONLY

        [Space(10), Header("Chase Settings"), Space(10)]
        public GameObject playerTarget; // The Player object which the monster chases
        public float detectionRoadius = 10;
        public float killingRadius = 5;
        private NavMeshAgent _agent;
        public LayerMask detectableLayers; // The layer that the palyer is on
        public LayerMask wallayers; // Layer for all walls
        private void Start()
        {
            currentState = EnemyState.Patrolling;
            myMimic = GetComponent<Mimic>();
            _agent = GetComponent<NavMeshAgent>();
            StartCoroutine(Wandering());
        }
        private IEnumerator Wandering()
        {
            Debug.Log("Wandering");

            while (!PlayerInLineOfSight() && currentState == EnemyState.Patrolling)
            {
                //generate a random index to patrol to
                int patrol = Random.Range(0, patrolPoints.Count);
                patrolIndex = patrol;

                //Patrol to the next point in our list:
                _agent.SetDestination(patrolPoints[patrolIndex].position);
                //Wait for the enemy to reach the patrol point
                while (Vector3.Distance(transform.position, patrolPoints[patrolIndex].position) > 2)
                    yield return null;
                //wait for some time
                // yield return new WaitForSeconds(Random.Range(minPatrolWaitTime, maxPatrolWaitTime));
            }
            // If the player is detected or the state has changed, switch to chasing player
            currentState = EnemyState.ChasingPlayer;
            StartCoroutine(ChasePlayer());
        }
        void Update()
        {
            // Switch states if needed
            if (PlayerInLineOfSight() && currentState == EnemyState.Patrolling)
            {
                currentState = EnemyState.ChasingPlayer;
                StopCoroutine(Wandering());
                StartCoroutine(ChasePlayer());
            }

            if (!PlayerInLineOfSight() && currentState == EnemyState.ChasingPlayer)
            {
                currentState = EnemyState.Patrolling;
                StopCoroutine(ChasePlayer());
                StartCoroutine(Wandering());
            }

            //velocity = Vector3.Lerp(velocity, new Vector3(Input.GetAxisRaw(“Horizontal”), 0, Input.GetAxisRaw(“Vertical”)).normalized * speed, velocityLerpCoef * Time.deltaTime);
            velocity = Vector3.Lerp(velocity, _agent.velocity.normalized * speed, velocityLerpCoef * Time.deltaTime);
            // Assigning velocity to the mimic to assure great leg placement
            myMimic.velocity = velocity;
            transform.position = transform.position + velocity * Time.deltaTime;
            RaycastHit hit;
            Vector3 destHeight = transform.position;
            if (Physics.Raycast(transform.position + Vector3.up * 5f, -Vector3.up, out hit))
                destHeight = new Vector3(transform.position.x, hit.point.y + height, transform.position.z);
            transform.position = Vector3.Lerp(transform.position, destHeight, velocityLerpCoef * Time.deltaTime);
        }


        private IEnumerator ChasePlayer()
        {
            Debug.Log("Chasing");
                // Checking if the private bool variable is true
           
            while (PlayerInLineOfSight() && currentState == EnemyState.ChasingPlayer)
            {
                _agent.SetDestination(playerTarget.transform.position);
                if (Physics.CheckSphere(transform.position, killingRadius, detectableLayers)) //Check if the player is in killing range or not
                {
                   gameManager.Instance.EndGame();
                   playerTarget.GetComponent<FirstPersonController>().Die();
                }
                yield return null;
            }
            // If the player is no longer detected or the state has changed, switch back to patrolling
            currentState = EnemyState.Patrolling;
            StartCoroutine(Wandering());
        }
        private bool PlayerInDetectionZone()
        {
            // Returns true if the player is within the detection radius
            return Physics.CheckSphere(transform.position, detectionRoadius, 1 << 6);
        }

        private bool PlayerInLineOfSight()
        {
            // Calculate the direction from the enemy to the player
            Vector3 directionToPlayer = _player.transform.position - transform.position;

            // Create a ray starting from the enemy position in direction of the player
            Ray LineOfSight = new Ray(transform.position, directionToPlayer);

            // Set a max range for the raycast to avoid hitting anything beyond the player
            float maxRaycastDistance = directionToPlayer.magnitude;

            Debug.DrawRay(transform.position, directionToPlayer,  Color.red);
             // if the Ray does not hit a wall, meaning the player is not in sight
            if(!Physics.Raycast(LineOfSight, out RaycastHit rayHit, maxRaycastDistance, wallayers))
            {
                // Return true if player is in sight
                return true;
            } else return false;
        }

        void OnDrawGizmosSelected()
        {
            // Display the explosion radius when selected
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, detectionRoadius);
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, killingRadius);
        }


    }
}