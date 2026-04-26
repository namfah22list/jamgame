using UnityEngine;
using UnityEngine.AI;

public class CatEnemy : MonoBehaviour
{
    public enum CatState { Patrol, Chase, Attack }
    public CatState currentState = CatState.Patrol;

    [Header("Detection")]
    public float detectionRange = 10f;
    public float attackRange = 2f;
    public float loseRange = 15f;

    [Header("Movement")]
    public float patrolSpeed = 2f;
    public float chaseSpeed = 5f;

    [Header("Attack")]
    public float attackDamage = 10f;
    public float attackCooldown = 1.5f;

    [Header("Patrol Waypoints")]
    public Transform[] waypoints;

    private NavMeshAgent agent;
    private Transform player;
    private CatWalk catWalk;
    private int currentWaypoint = 0;
    private float attackTimer = 0f;
    private bool isReady = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        catWalk = GetComponent<CatWalk>();

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;
        else
            Debug.LogError("หา Player ไม่เจอ! ตรวจสอบว่า Player มี Tag = Player");

        if (waypoints.Length > 0)
            agent.SetDestination(waypoints[0].position);

        isReady = true;
    }

    void Update()
    {
        if (!isReady || player == null) return;

        CatEnemyHealth health = GetComponent<CatEnemyHealth>();
        if (health != null && health.currentHP <= 0)
        {
            agent.isStopped = true;
            return;
        }

        agent.isStopped = false;
        attackTimer -= Time.deltaTime;

        float distToPlayer = Vector3.Distance(transform.position, player.position);

        Debug.Log($"State: {currentState} | dist: {distToPlayer}");

        switch (currentState)
        {
            case CatState.Patrol:
                DoPatrol();
                if (distToPlayer < detectionRange)
                    ChangeState(CatState.Chase);
                break;

            case CatState.Chase:
                Debug.Log("อยู่ใน Chase case!");
                DoChase();
                if (distToPlayer < attackRange)
                    ChangeState(CatState.Attack);
                else if (distToPlayer > loseRange)
                    ChangeState(CatState.Patrol);
                break;

            case CatState.Attack:
                DoAttack();
                if (distToPlayer > attackRange)
                    ChangeState(CatState.Chase);
                break;
        }

        if (catWalk != null)
            catWalk.moveSpeed = agent.velocity.magnitude;
    }

    void DoPatrol()
    {
        if (waypoints.Length == 0) return;
        agent.speed = patrolSpeed;

        if (!agent.pathPending && agent.hasPath && agent.remainingDistance < 0.5f)
        {
            currentWaypoint = (currentWaypoint + 1) % waypoints.Length;
            agent.SetDestination(waypoints[currentWaypoint].position);
        }
    }

    void DoChase()
    {
        agent.speed = chaseSpeed;
        Vector3 targetPos = new Vector3(player.position.x, transform.position.y, player.position.z);
        NavMeshHit hit;
        bool canReach = NavMesh.SamplePosition(targetPos, out hit, 5f, NavMesh.AllAreas);
        Debug.Log($"Chase canReach: {canReach}");
        agent.SetDestination(hit.position);
    }

    void DoAttack()
    {
        agent.SetDestination(transform.position);
        transform.LookAt(new Vector3(player.position.x, transform.position.y, player.position.z));

        if (attackTimer <= 0f)
        {
            attackTimer = attackCooldown;
            player.GetComponent<PlayerHealth>()?.TakeDamage(attackDamage);
            Debug.Log($"แมวโจมตี! -{attackDamage} HP");
        }
    }

    void ChangeState(CatState newState)
    {
        currentState = newState;
        if (newState == CatState.Patrol && waypoints.Length > 0)
            agent.SetDestination(waypoints[currentWaypoint].position);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.gray;
        Gizmos.DrawWireSphere(transform.position, loseRange);
    }
}