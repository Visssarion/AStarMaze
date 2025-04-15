using UnityEngine;
using System.Collections;
using Behaviour;

public class NPCBehaviour : BehaviourTree
{
    public float detectionRadius = 10f;
    public float alertRadius = 15f;
    public float walkSpeed = 3f;
    public float restDuration = 5f;
    public float sleepDuration = 10f;
    public Transform player;
    public Transform[] patrolPoints;

    private Unit unit;
    private int currentPatrolIndex;
    private float stateTimer;
    private Vector3 walkTarget;

    protected override void Setup()
    {
        unit = GetComponent<Unit>();
        unit.speed = walkSpeed;

        var patrolState = new AtomicState(Patrol, () => currentPatrolIndex = 0, null);
        var walkState = new AtomicState(WalkRandomly, SetRandomWalkTarget, null);
        var restState = new AtomicState(Rest, () => stateTimer = restDuration, null);
        var sleepState = new AtomicState(Sleep, () => stateTimer = sleepDuration, null);
        var fleeState = new AtomicState(Flee, AlertNearbyNPCs, null);

        var mainBehaviour = new CompoundState(0, CheckPlayerDetection);
        mainBehaviour.AddChild(patrolState);
        mainBehaviour.AddChild(walkState);
        mainBehaviour.AddChild(restState);
        mainBehaviour.AddChild(sleepState);

        var rootBinary = new BinaryState(false, null);
        rootBinary.trueState = fleeState;
        rootBinary.falseState = mainBehaviour;

        rootState = rootBinary;

        StartCoroutine(StateTimer());
    }

    void Patrol()
    {
        if (patrolPoints.Length == 0) return;

        if (Vector3.Distance(transform.position, patrolPoints[currentPatrolIndex].position) < 1f)
        {
            currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
            unit.target = patrolPoints[currentPatrolIndex];
        }
    }

    void WalkRandomly()
    {
        if (Vector3.Distance(transform.position, walkTarget) < 1f)
        {
            SetRandomWalkTarget();
        }
    }

    void Rest()
    {
        stateTimer -= Time.deltaTime;
    }

    void Sleep()
    {
        stateTimer -= Time.deltaTime;
    }

    void Flee()
    {
        Vector3 fleeDirection = (transform.position - player.position).normalized;
        unit.target.position = transform.position + fleeDirection * 20f;
    }

    void CheckPlayerDetection()
    {
        if (Vector3.Distance(transform.position, player.position) < detectionRadius)
        {
            ((BinaryState)rootState).GetChangeStateAction()(true);
        }
        else
        {
            ((BinaryState)rootState).GetChangeStateAction()(false);
        }
    }

    void AlertNearbyNPCs()
    {
        Collider[] npcs = Physics.OverlapSphere(transform.position, alertRadius);
        foreach (var npc in npcs)
        {
            var behaviour = npc.GetComponent<NPCBehaviour>();
            if (behaviour != null)
            {
                ((BinaryState)behaviour.rootState).GetChangeStateAction()(true);
            }
        }
    }

    void SetRandomWalkTarget()
    {
        walkTarget = transform.position + new Vector3(Random.Range(-10f, 10f), 0, Random.Range(-10f, 10f));
        unit.target.position = walkTarget;
    }

    IEnumerator StateTimer()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(5f, 15f));
            if (!((BinaryState)rootState).GetCurrentCondition())
            {
                ((CompoundState)((BinaryState)rootState).falseState).GetChangeStateAction()(Random.Range(0, 4));
            }
        }
    }
}