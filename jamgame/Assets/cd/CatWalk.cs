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

    [HideInInspector] public float moveSpeed = 0f;

    private NavMeshAgent agent;
    private Quaternion initFR, initFL, initBR, initBL;
    private Quaternion initTail, initHead;
    private Vector3 initBodyPos;

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
        // ดึง speed จาก NavMesh Agent โดยตรง
        if (agent != null)
            moveSpeed = agent.velocity.magnitude;

        // ถ้าไม่ขยับให้หยุด animate
        if (moveSpeed <= 0.01f)
        {
            ResetPose();
            return;
        }

        float t = Time.time;
        float wave = Mathf.Sin(t * walkSpeed);
        float waveBack = Mathf.Sin(t * walkSpeed + Mathf.PI);

        if (legFrontRight)
            legFrontRight.localRotation = initFR * Quaternion.Euler(wave * legSwingAngle, 0f, 0f);
        if (legBackLeft)
            legBackLeft.localRotation = initBL * Quaternion.Euler(wave * legSwingAngle, 0f, 0f);
        if (legFrontLeft)
            legFrontLeft.localRotation = initFL * Quaternion.Euler(waveBack * legSwingAngle, 0f, 0f);
        if (legBackRight)
            legBackRight.localRotation = initBR * Quaternion.Euler(waveBack * legSwingAngle, 0f, 0f);

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
    }
}