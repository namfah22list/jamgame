using UnityEngine;
using UnityEngine.AI;

public class CatWalk : MonoBehaviour
{
    [Header("ตัวแมว")]
    public Transform body;
    [Header("ขาหน้า")]
    public Transform legFrontRight;
    public Transform legFrontLeft;
    [Header("ขาหลัง")]
    public Transform legBackRight;
    public Transform legBackLeft;
    [Header("หาง")]
    public Transform tail;
    [Header("หัว")]
    public Transform head;
    [Header("การตั้งค่า")]
    public float walkSpeed = 3f;
    public float legSwingAngle = 30f;
    public float bodyBobAmount = 0.08f;
    public float bodyBobSpeed = 6f;
    public float tailSwingAngle = 40f;
    public float tailSwingSpeed = 2f;
    public float headBobAmount = 5f;

    [Header("Attack Animation")]
    public float attackLegAngle = 60f;    // มุมขาหน้าตอนตี
    public float attackBodyLift = 0.15f;  // ตัวยกขึ้นเท่าไหร่
    public float attackAnimSpeed = 8f;    // ความเร็ว animation โจมตี

    [HideInInspector] public float moveSpeed = 0f;

    // ควบคุมจาก CatEnemy
    [HideInInspector] public bool isAttacking = false;

    private NavMeshAgent agent;
    private Quaternion initFR, initFL, initBR, initBL;
    private Quaternion initTail, initHead;
    private Vector3 initBodyPos;

    private float attackAnimTime = 0f;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        if (legFrontRight) initFR = legFrontRight.localRotation;
        if (legFrontLeft) initFL = legFrontLeft.localRotation;
        if (legBackRight) initBR = legBackRight.localRotation;
        if (legBackLeft) initBL = legBackLeft.localRotation;
        if (tail) initTail = tail.localRotation;
        if (head) initHead = head.localRotation;
        if (body) initBodyPos = body.localPosition;
    }

    void Update()
    {
        if (agent != null)
            moveSpeed = agent.velocity.magnitude;

        if (isAttacking)
        {
            DoAttackAnim();
            return;
        }

        if (moveSpeed <= 0.01f)
        {
            ResetPose();
            return;
        }

        DoWalkAnim();
    }

    void DoAttackAnim()
    {
        attackAnimTime += Time.deltaTime * attackAnimSpeed;

        // ขาหน้ายกขึ้นแกว่งแรงๆ ด้วย sin
        float swing = Mathf.Sin(attackAnimTime) * attackLegAngle;

        if (legFrontRight) legFrontRight.localRotation = initFR * Quaternion.Euler(-Mathf.Abs(swing), 0f, 0f);
        if (legFrontLeft) legFrontLeft.localRotation = initFL * Quaternion.Euler(-Mathf.Abs(swing) * 0.8f, 0f, 0f);

        // ขาหลังอยู่นิ่ง
        if (legBackRight) legBackRight.localRotation = initBR;
        if (legBackLeft) legBackLeft.localRotation = initBL;

        // ตัวยกขึ้นเล็กน้อย
        if (body)
        {
            float lift = Mathf.Abs(Mathf.Sin(attackAnimTime)) * attackBodyLift;
            body.localPosition = initBodyPos + new Vector3(0f, lift, 0f);
        }

        // หัวก้มลงหา player
        if (head) head.localRotation = initHead * Quaternion.Euler(20f, 0f, 0f);

        // หางตั้ง
        if (tail) tail.localRotation = initTail * Quaternion.Euler(-30f, 0f, 0f);
    }

    // เรียกจาก CatEnemy ตอนโจมตี
    public void TriggerAttackAnim()
    {
        isAttacking = true;
        attackAnimTime = 0f;
    }

    // เรียกจาก CatEnemy ตอนหยุดโจมตี
    public void StopAttackAnim()
    {
        isAttacking = false;
        attackAnimTime = 0f;
        ResetPose();
    }

    void DoWalkAnim()
    {
        float t = Time.time;
        float wave = Mathf.Sin(t * walkSpeed);
        float waveBack = Mathf.Sin(t * walkSpeed + Mathf.PI);

        if (legFrontRight) legFrontRight.localRotation = initFR * Quaternion.Euler(wave * legSwingAngle, 0f, 0f);
        if (legBackLeft) legBackLeft.localRotation = initBL * Quaternion.Euler(wave * legSwingAngle, 0f, 0f);
        if (legFrontLeft) legFrontLeft.localRotation = initFL * Quaternion.Euler(waveBack * legSwingAngle, 0f, 0f);
        if (legBackRight) legBackRight.localRotation = initBR * Quaternion.Euler(waveBack * legSwingAngle, 0f, 0f);

        if (body)
        {
            float bob = Mathf.Abs(Mathf.Sin(t * bodyBobSpeed)) * bodyBobAmount;
            body.localPosition = initBodyPos + new Vector3(0f, -bob, 0f);
        }
        if (tail)
        {
            float tailWave = Mathf.Sin(t * tailSwingSpeed) * tailSwingAngle;
            tail.localRotation = initTail * Quaternion.Euler(0f, tailWave, 0f);
        }
        if (head)
        {
            float headNod = Mathf.Sin(t * walkSpeed) * headBobAmount;
            head.localRotation = initHead * Quaternion.Euler(headNod, 0f, 0f);
        }
    }

    void ResetPose()
    {
        if (legFrontRight) legFrontRight.localRotation = initFR;
        if (legFrontLeft) legFrontLeft.localRotation = initFL;
        if (legBackRight) legBackRight.localRotation = initBR;
        if (legBackLeft) legBackLeft.localRotation = initBL;
        if (body) body.localPosition = initBodyPos;
        if (head) head.localRotation = initHead;
        if (tail) tail.localRotation = initTail;
    }
}
