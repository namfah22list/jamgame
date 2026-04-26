using UnityEngine;

public class CatWalk : MonoBehaviour
{
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

    [Header("ทดสอบ")]
    public bool alwaysAnimate = true; // เปิดไว้ก่อน เพื่อทดสอบ

    [HideInInspector] public float moveSpeed = 0f;

    private Quaternion initFR, initFL, initBR, initBL;
    private Quaternion initTail, initHead;
    private Vector3 initBodyPos;

    void Start()
    {
        if (legFrontRight) initFR = legFrontRight.localRotation;
        if (legFrontLeft) initFL = legFrontLeft.localRotation;
        if (legBackRight) initBR = legBackRight.localRotation;
        if (legBackLeft) initBL = legBackLeft.localRotation;
        if (tail) initTail = tail.localRotation;
        if (head) initHead = head.localRotation;
        initBodyPos = transform.localPosition;
    }

    void Update()
    {
        // ถ้า alwaysAnimate = true จะขยับตลอด ไม่ต้องรอ moveSpeed
        if (!alwaysAnimate && moveSpeed <= 0.01f) return;

        float t = Time.time;
        float wave = Mathf.Sin(t * walkSpeed);
        float waveBack = Mathf.Sin(t * walkSpeed + Mathf.PI);

        if (legFrontRight)
            legFrontRight.localRotation = initFR *
                Quaternion.Euler(wave * legSwingAngle, 0f, 0f);

        if (legBackLeft)
            legBackLeft.localRotation = initBL *
                Quaternion.Euler(wave * legSwingAngle, 0f, 0f);

        if (legFrontLeft)
            legFrontLeft.localRotation = initFL *
                Quaternion.Euler(waveBack * legSwingAngle, 0f, 0f);

        if (legBackRight)
            legBackRight.localRotation = initBR *
                Quaternion.Euler(waveBack * legSwingAngle, 0f, 0f);

        float bob = Mathf.Abs(Mathf.Sin(t * bodyBobSpeed)) * bodyBobAmount;
        transform.localPosition = initBodyPos + new Vector3(0f, -bob, 0f);

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
}