using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class CharacterAgent : MonoBehaviour
{
    [SerializeField] Transform TargetPoint;
    [SerializeField] bool CanMove = false;

    NavMeshAgent Agent;
    CharacterController Character;

    // Start is called before the first frame update
    void Start()
    {
        Agent = GetComponent<NavMeshAgent>();
        Character = GetComponent<CharacterController>();

        if (CanMove)
            Agent.SetDestination(TargetPoint.position);
    }

    // Update is called once per frame
    void Update()
    {
        if (!CanMove)
            return;

        // target point changed?
        if (Agent.destination != TargetPoint.transform.position)
            Agent.SetDestination(TargetPoint.position);

        // reached destination?
        if (Agent.remainingDistance < 0.1f && Character.velocity.magnitude < 0.1f)
        {
            // At destination
        }
    }
}
