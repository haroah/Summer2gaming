using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyFollow : MonoBehaviour
{
    private NavMeshAgent _Agent;

    public Transform _target;
    
    // Start is called before the first frame update
    void Start()
    {
        _Agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        _Agent.SetDestination(_target.position);
    }
}
