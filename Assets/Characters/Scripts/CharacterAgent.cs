using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class CharacterAgent : CharacterBase
{
    [SerializeField] bool CanMove = false;
    [SerializeField] float NearestPointSearchRange = 5f;

    [SerializeField] float AttackInterval = 1f;
    [SerializeField] int AttackDamage = 20;
    float TimeUntilNextAttack = 0;

    [SerializeField] float RepairInterval = 1f;
    [SerializeField] int RepairAmount = 20;
    float TimeUntilNextRepair = 0;

    [SerializeField] float MeleeRange = 1.5f;

    [SerializeField] float ReachedDestinationThreshold = 0.1f;

    NavMeshAgent Agent;
    CharacterController Character;

    BuildingBase TargetBuilding = null;
    CharacterBase TargetCharacter = null;
    bool AtDestination = false;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        Agent = GetComponent<NavMeshAgent>();
        Character = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        if (!CanMove)
            return;

        // update our timers
        if (TimeUntilNextAttack > 0)
            TimeUntilNextAttack -= Time.deltaTime;
        if (TimeUntilNextRepair > 0)
            TimeUntilNextRepair -= Time.deltaTime;

        // do we have a target character
        if (TargetCharacter != null)
            Agent.SetDestination(TargetCharacter.transform.position);

        // detect if we have reached the destination?
        if (Agent.hasPath && Agent.remainingDistance <= Agent.stoppingDistance && Character.velocity.magnitude < 0.1f)
            AtDestination = true;

        // at destination?
        if (AtDestination)
        {
            // do we have a target building
            if (TargetBuilding != null)
            {
                if (TargetBuilding.Faction != Faction)
                    PerformAttack();
                else
                    PerformRepair();
            } // do we have a target character
            else if (TargetCharacter != null)
            {
                if (TargetCharacter.Faction != Faction)
                    PerformAttack();
                else
                    PerformRepair();                
            }
        }
    }

    void PerformAttack()
    {
        // allowed to attack?
        if(TimeUntilNextAttack <= 0)
        {
            // attack the current target
            if (TargetBuilding != null)
                TargetBuilding.TakeDamage(AttackDamage);
            else if (TargetCharacter != null)
                TargetCharacter.TakeDamage(AttackDamage);

            TimeUntilNextAttack = AttackInterval;
        }
    }

    protected virtual void PerformRepair()
    {
        // allowed to repair?
        if (TimeUntilNextRepair <= 0)
        {
            // repair/heal depending on the target
            if (TargetBuilding != null)
                TargetBuilding.RepairDamage(RepairAmount);
            else if (TargetCharacter != null)
                TargetCharacter.RepairDamage(RepairAmount);

            TimeUntilNextRepair = RepairInterval;
        }
    }

    protected virtual void CancelCurrentCommand()
    {
        TargetBuilding = null;
        TargetCharacter = null;
    }

    public virtual void MoveTo(Vector3 destination)
    {
        CancelCurrentCommand();

        // snap the destination to the navmesh
        NavMeshHit hitResult;
        if (NavMesh.SamplePosition(destination, out hitResult, Character.height, NavMesh.AllAreas))
            SetDestination(hitResult.position);
    }

    public virtual void RepairBuilding(BuildingBase building)
    {
        CancelCurrentCommand();

        TargetBuilding = building;

        MoveToBuilding();
    }

    public virtual void AttackBuilding(BuildingBase building)
    {
        CancelCurrentCommand();

        TargetBuilding = building;

        MoveToBuilding();
    }

    protected virtual void MoveToBuilding()
    {
        NavMeshHit hitResult;

        // find the nearest navmesh location to the building
        if (NavMesh.SamplePosition(TargetBuilding.transform.position, out hitResult, NearestPointSearchRange, NavMesh.AllAreas))
            SetDestination(hitResult.position);
    }

    public virtual void HealCharacter(CharacterBase targetCharacter)
    {
        CancelCurrentCommand();

        TargetCharacter = targetCharacter;

        SetDestination(targetCharacter.transform.position);
    }

    public virtual void AttackCharacter(CharacterBase targetCharacter)
    {
        CancelCurrentCommand();

        TargetCharacter = targetCharacter;

        SetDestination(targetCharacter.transform.position);
    }

    public virtual void SetDestination(Vector3 destination)
    {
        Agent.SetDestination(destination);
        Agent.stoppingDistance = MeleeRange;
        AtDestination = false;        
    }
}
