using System.Collections;
using UnityEngine;
using Behaviour;
using UnityEngine.Events;

public class EnemyBehaviour : BehaviourTree
{
    public float detectionRadius = 10f;
    public float alertRadius = 15f;
    public float fleeDistance = 20f;
    public Transform player;
    public Transform[] patrolPoints;

    private Enemy enemy;
    private int currentPatrolIndex;
    private Vector3 walkTarget;
    private bool isPlayerDetected = false;

    public UnityAction<bool> SetAnxietyCondition;
    private UnityAction<int> SetCalmState;
    private UnityAction<int> SetScaredState;

    private int _currentStrollIndex = 0;

    private Vector3 fleeDirection = Vector3.zero;
    protected override void Setup()
    {
        enemy = GetComponent<Enemy>();
        enemy.speed = 12f;

        BinaryState anxietyState = new BinaryState(false);
        SetAnxietyCondition = anxietyState.GetChangeStateAction();
        rootState = anxietyState;

        CompoundState calm = new CompoundState(0, null);
        SetCalmState = calm.GetChangeStateAction();
        anxietyState.falseState = calm;

        AtomicState patrol = new AtomicState(Patrol, () => currentPatrolIndex = 0, null);
        AtomicState stroll = new AtomicState(Stroll, () => { SetRandomWalkTarget(); _currentStrollIndex = 0; }, null);
        AtomicState sleep = new AtomicState(null, () => StartCoroutine(SleepRoutine()), null);

        calm.AddChild(patrol);
        calm.AddChild(stroll);
        calm.AddChild(sleep);

        CompoundState scared = new CompoundState(0, null);
        SetScaredState = scared.GetChangeStateAction();
        anxietyState.trueState = scared;

        AtomicState flee = new AtomicState(Flee, () => { SetFleeTarget(); AlertNearbyNPCs(); } , null);; //AlertNearbyNPCs
        scared.AddChild(flee);


        //StartCoroutine(RandomStateSwitcher());
    }

    public void PlayerDetected()
    {
        isPlayerDetected = true;
        SetAnxietyCondition(true);
    }

    public void PlayerLeftScareTrigger()
    {
        isPlayerDetected = false;
    }

    void Patrol()
    {
        if (patrolPoints.Length == 0) return;

        if (Vector3.Distance(transform.position, patrolPoints[currentPatrolIndex].position) < 2f)
        {
            if ((currentPatrolIndex + 1) % patrolPoints.Length == 0)
            {
                if (Random.value < 0.5f)
                {
                    SetCalmState(Random.Range(1, 3));
                }
            }
            currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
        }
        enemy.target = patrolPoints[currentPatrolIndex];
    }

    void Stroll()
    {
        if (Vector3.Distance(transform.position, walkTarget) < 2f)
        {
            _currentStrollIndex++;
            if (_currentStrollIndex >= 3)
            {
                SetCalmState(2);
            }
            else
            {
                SetRandomWalkTarget();
            }
        }
    }


    void Flee()
    {
        if (Vector3.Distance(transform.position, enemy.target.position) < 3f)
        {
            if (isPlayerDetected)
            {
                SetFleeTarget();
            }
            else
            {
                SetAnxietyCondition(false);
                SetCalmState(2);
            }
        }
    }
    void SetFleeTarget()
    {
        Vector3 fleeDirection = (transform.position - player.position).normalized;
        enemy.target.position = transform.position + fleeDirection * fleeDistance;
    }

    void SetRandomWalkTarget()
    {
        walkTarget = transform.position + new Vector3(Random.Range(-20f, 20f), 0, Random.Range(-20f, 20f));
        enemy.target.position = walkTarget;
    }

    void AlertNearbyNPCs()
    {
        Collider[] npcs = Physics.OverlapSphere(transform.position, alertRadius);
        foreach (var npc in npcs)
        {
            var behaviour = npc.GetComponent<EnemyBehaviour>();
            if (behaviour != null && !behaviour.isPlayerDetected)
            {
                behaviour.isPlayerDetected = true;
                behaviour.SetAnxietyCondition(true);
            }
        }
    }

    IEnumerator RestRoutine()
    {
        yield return new WaitForSeconds(Random.Range(2f, 4f));
        SetCalmState(Random.Range(0, 2));
    }

    IEnumerator SleepRoutine()
    {
        float originalDetection = detectionRadius;
        detectionRadius = 0; // Не видит игрока пока спит

        yield return new WaitForSeconds(Random.Range(3f, 5f));

        detectionRadius = originalDetection;
        SetCalmState(Random.Range(0, 3));
    }

    /*IEnumerator RandomStateSwitcher()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(1f, 3f));
            if (!isPlayerDetected)
            {
                SetCalmState(Random.Range(0, 4));
            }
        }
    }*/
}