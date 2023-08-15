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
        public float minPatrolWaitTime;
        public float maxPatrolWaitTime;
        ////////////
        public float minPatrolSpeed = 5;
        public float maxPatrolSpeed = 30;
        ///////////
        public float chaseSpeedMin = 6;
        public float chaseSpeedMax = 12;
        ////
        [SerializeField] private AudioSource noticeSoundEffect;
        [SerializeField] private AudioSource hummingLoop;
        [SerializeField] private AudioSource chaseMusic;
        private bool canPlayNoticeSound = true;
        ////
        [Space(10), Header("Chase Settings"), Space(10)]
        public GameObject playerTarget; // The Player object which the monster chases
        public float detectionRoadius = 10;
        public float killingRadius = 5;
        private NavMeshAgent _agent;
        public LayerMask detectableLayers;
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
            hummingLoop.Play();
            canPlayNoticeSound = true;
            chaseMusic.Stop();
            while (!PlayerInDetectionZone() && currentState == EnemyState.Patrolling)
            {
                //change speed here
                //print(“New Point”);
                _agent.speed = Random.Range(minPatrolSpeed, maxPatrolSpeed);
                //print(_agent.speed);
                //generate a random index to patrol to
                int patrol = Random.Range(0, patrolPoints.Count);
                patrolIndex = patrol;
                //Patrol to the next point in our list:
                _agent.SetDestination(patrolPoints[patrolIndex].position);
                //Wait for the enemy to reach the patrol point
                while (Vector3.Distance(transform.position, patrolPoints[patrolIndex].position) > 1)
                    yield return null;
                //wait for some time
                yield return new WaitForSeconds(Random.Range(minPatrolWaitTime, maxPatrolWaitTime));
            }
            // If the player is detected or the state has changed, switch to chasing player
            currentState = EnemyState.ChasingPlayer;
            StartCoroutine(ChasePlayer());
        }
        void Update()
        {
            // Switch states if needed
            if (PlayerInDetectionZone() && currentState == EnemyState.Patrolling)
            {
                currentState = EnemyState.ChasingPlayer;
                StopCoroutine(Wandering());
                StartCoroutine(ChasePlayer());
            }
            else if (!PlayerInDetectionZone() && currentState == EnemyState.ChasingPlayer)
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
            _agent.speed = Random.Range(chaseSpeedMin, chaseSpeedMax);
                // Checking if the private bool variable is true
            if (canPlayNoticeSound)
            {
                // Do something when isActivated is true
                canPlayNoticeSound = false;
                noticeSoundEffect.Play();
                chaseMusic.Play();
            }
            while (PlayerInDetectionZone() && currentState == EnemyState.ChasingPlayer)
            {
                _agent.SetDestination(playerTarget.transform.position);
                if (Physics.CheckSphere(transform.position, killingRadius, detectableLayers)) //Check if the player is in killing range or not
                {
                    //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                    FindObjectOfType<GameManager>().EndGame();//Find GameManafer and reset the scene
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