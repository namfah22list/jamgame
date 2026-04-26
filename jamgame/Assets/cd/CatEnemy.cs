using UnityEngine;
using UnityEngine.AI;

public class CatEnemy : MonoBehaviour
{
    public enum CatState { Patrol, Chase, Attack }
    public CatState currentState = CatState.Patrol;
    private CatWalk catWalk;
    [Header("Detection")]
    public float detectionRange = 10f;  // รัศมีมองเห็น Player
    public float attackRange = 2f;   // รัศมีโจมตี
    public float loseRange = 15f;  // ไกลเกินนี้ → หยุดไล่

    [Header("Movement")]
    public float patrolSpeed = 2f;
    public float chaseSpeed = 5f;

    [Header("Attack")]
    public float attackDamage = 10f;
    public float attackCooldown = 1.5f;

    [Header("Patrol Waypoints")]
    public Transform[] waypoints;  // ลาก Waypoint มาใส่ตรงนี้

    // References
    private NavMeshAgent agent;
    private Transform player;
    private Animator anim;

    // Private state
    private int currentWaypoint = 0;
    private float attackTimer = 0f;

    void Start()
    {

        catWalk = GetComponent<CatWalk>();
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        if (catWalk != null)
            catWalk.moveSpeed = agent.velocity.magnitude;

        attackTimer -= Time.deltaTime;

        float distToPlayer = Vector3.Distance(transform.position, player.position);

        // ── เลือก State ──────────────────────────────
        switch (currentState)
        {
            case CatState.Patrol:
                DoPatrol();
                if (distToPlayer < detectionRange)
                    ChangeState(CatState.Chase);
                break;

            case CatState.Chase:
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
    }

    // ── Patrol ───────────────────────────────────────
    void DoPatrol()
    {
        if (waypoints.Length == 0) return;

        agent.speed = patrolSpeed;

        // ถึง Waypoint แล้ว → ไป Waypoint ถัดไป
        if (agent.remainingDistance < 0.5f && !agent.pathPending)
        {
            currentWaypoint = (currentWaypoint + 1) % waypoints.Length;
            agent.SetDestination(waypoints[currentWaypoint].position);
        }

        anim?.SetFloat("Speed", agent.velocity.magnitude);
    }

    // ── Chase ────────────────────────────────────────
    void DoChase()
    {
        agent.speed = chaseSpeed;
        agent.SetDestination(player.position);

        anim?.SetFloat("Speed", agent.velocity.magnitude);
    }

    // ── Attack ───────────────────────────────────────
    void DoAttack()
    {
        agent.SetDestination(transform.position); // หยุดเดิน
        transform.LookAt(player);                 // หันหน้าหา Player

        anim?.SetFloat("Speed", 0f);

        if (attackTimer <= 0f)
        {
            attackTimer = attackCooldown;
            anim?.SetTrigger("Attack");

            // ดึง Health Script จาก Player แล้วลดเลือด
            player.GetComponent<PlayerHealth>()?.TakeDamage(attackDamage);

            Debug.Log($"แมวโจมตี! -{attackDamage} HP");
        }
    }

    // ── เปลี่ยน State ────────────────────────────────
    void ChangeState(CatState newState)
    {
        currentState = newState;

        switch (newState)
        {
            case CatState.Patrol:
                agent.SetDestination(waypoints[currentWaypoint].position);
                anim?.SetBool("IsChasing", false);
                break;
            case CatState.Chase:
                anim?.SetBool("IsChasing", true);
                break;
            case CatState.Attack:
                anim?.SetBool("IsChasing", false);
                break;
        }
    }

    // ── Gizmos (แสดง Range ใน Scene view) ───────────
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