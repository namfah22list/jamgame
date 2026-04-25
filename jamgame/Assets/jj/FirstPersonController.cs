using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CharacterController))]
public class FirstPersonController : MonoBehaviour
{
    [Header("References")]
    public Camera playerCamera;

    [Header("Movement Settings")]
    public float walkSpeed = 5f;
    public float sprintSpeed = 8f;
    public float jumpHeight = 2f;
    public float gravity = -9.81f;

    [Header("Mouse Look Settings")]
    public float mouseSensitivity = 2f;
    public float verticalLookLimit = 85f;

    [Header("Head Bob Settings")]
    public float walkBobFrequency = 1.8f;
    public float walkBobAmplitude = 0.05f;
    public float sprintBobFrequency = 2.5f;
    public float sprintBobAmplitude = 0.1f;

    [Header("Landing & Jump Settings")]
    public float landingImpact = 0.3f;
    public float landingSpeedThreshold = -10f;
    public float jumpTiltAngle = 8f;

    [Header("Dynamic FOV Settings")]
    public float baseFOV = 60f;
    public float sprintFOV = 75f;
    public float fovChangeSpeed = 8f;

    [Header("Camera Sway Settings")]
    public float swayAmount = 0.02f;
    public float swaySmoothness = 8f;

    [Header("Other Settings")]
    public bool lockCursor = true;

    [Header("Footstep Settings")]
    public AudioSource footstepSource;
    public AudioClip footstepClip;

    [Header("Landing Sound Settings")]
    public AudioSource landingSource;
    public AudioClip landingClip;
    private bool landingPlayed = false;

    [Header("Sprint Stamina Settings")]
    public float maxStamina = 5f;        // stamina เต็ม (วินาที)
    public float staminaRegenRate = 1f;  // ฟื้นต่อวินาที
    public float staminaDrainRate = 1f;  // ลดต่อวินาที

    // ✅ เอาเฉพาะอนิเมชั่นจากโค้ดที่ 2 มาใส่
    [Header("Animation")]
    public Animator animator;

    private float currentStamina;
    private bool isSprinting;

    private bool footstepPlayed = false;

    private CharacterController controller;
    private Vector3 moveDirection;
    private float yVelocity;
    private float pitch;
    private float yaw;

    private Vector3 originalCamLocalPos;
    private float bobTimer;
    private float currentBobAmplitude;
    private float currentBobFrequency;

    private float targetFOV;

    private float landingOffsetY;
    private float landingOffsetVelocity; // สำหรับ SmoothDamp

    private float tiltAngle;
    private float tiltVelocity; // สำหรับ SmoothDamp

    private float previousYVelocity;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    void Start()
    {
        if (lockCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        yaw = transform.eulerAngles.y;
        pitch = playerCamera.transform.localEulerAngles.x;
        originalCamLocalPos = playerCamera.transform.localPosition;

        playerCamera.fieldOfView = baseFOV;
        targetFOV = baseFOV;

        currentStamina = maxStamina;
    }

    void Update()
    {
        HandleMouseLook();
        HandleMovement();
        HandleHeadBobAndLanding();
        HandleDynamicFOV();
        HandleCameraSway();

        // ✅ เรียกอนิเมชั่น (จากโค้ดที่ 2)
        HandleAnimation();
    }

    void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        yaw += mouseX;
        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, -verticalLookLimit, verticalLookLimit);

        transform.rotation = Quaternion.Euler(0f, yaw, 0f);
    }

    void HandleMovement()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        bool sprintKey = Input.GetKey(KeyCode.LeftShift);
        bool isMovingForward = moveZ > 0.1f;

        // ฟื้น stamina ตอนที่ไม่ได้วิ่ง
        if (!sprintKey || !isMovingForward || !controller.isGrounded)
        {
            currentStamina += staminaRegenRate * Time.deltaTime;
            currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);
        }

        // เช็คการวิ่ง
        if (sprintKey && isMovingForward && controller.isGrounded && currentStamina > 0.1f * maxStamina)
        {
            isSprinting = true;
            currentStamina -= staminaDrainRate * Time.deltaTime;
            currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);
        }
        else
        {
            isSprinting = false;
        }

        float speed = isSprinting ? sprintSpeed : walkSpeed;

        Vector3 move = transform.right * moveX + transform.forward * moveZ;
        move *= speed;

        if (controller.isGrounded)
        {
            if (previousYVelocity < landingSpeedThreshold)
            {
                landingOffsetY = landingImpact;

                if (!landingPlayed && landingSource != null && landingClip != null)
                {
                    landingSource.PlayOneShot(landingClip);
                    landingPlayed = true;
                }
            }

            yVelocity = -1f;

            if (Input.GetKeyDown(KeyCode.Space))
            {
                yVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
                landingPlayed = false;
            }
        }
        else
        {
            yVelocity += gravity * Time.deltaTime;
        }

        moveDirection = new Vector3(move.x, yVelocity, move.z);
        controller.Move(moveDirection * Time.deltaTime);

        previousYVelocity = yVelocity;

        // Dynamic FOV
        targetFOV = isSprinting ? sprintFOV : baseFOV;
    }

    void HandleHeadBobAndLanding()
    {
        bool isMoving = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).magnitude > 0.1f
                        && controller.isGrounded;

        bool sprintKey = Input.GetKey(KeyCode.LeftShift);

        if (isMoving)
        {
            currentBobAmplitude = sprintKey ? sprintBobAmplitude : walkBobAmplitude;
            currentBobFrequency = sprintKey ? sprintBobFrequency : walkBobFrequency;

            bobTimer += Time.deltaTime * currentBobFrequency;

            if (Mathf.Sin(bobTimer) < -0.99f && !footstepPlayed)
            {
                if (footstepSource != null && footstepClip != null)
                    footstepSource.PlayOneShot(footstepClip);

                footstepPlayed = true;
            }
            else if (Mathf.Sin(bobTimer) > -0.5f)
            {
                footstepPlayed = false;
            }
        }
        else
        {
            bobTimer = 0;
        }

        float bobOffset = isMoving ? Mathf.Sin(bobTimer) * currentBobAmplitude : 0f;

        // Smooth landing offset
        landingOffsetY = Mathf.SmoothDamp(landingOffsetY, 0, ref landingOffsetVelocity, 0.2f);

        // Smooth tilt สำหรับ Jump
        float targetTilt = controller.isGrounded ? 0 : jumpTiltAngle * Mathf.Sign(yVelocity);
        tiltAngle = Mathf.SmoothDamp(tiltAngle, targetTilt, ref tiltVelocity, 0.2f);

        Vector3 targetPos = originalCamLocalPos + new Vector3(0, bobOffset - landingOffsetY, 0);
        playerCamera.transform.localPosition =
            Vector3.Lerp(playerCamera.transform.localPosition, targetPos, Time.deltaTime * 10f);

        playerCamera.transform.localRotation = Quaternion.Euler(pitch - tiltAngle, 0, 0);
    }

    void HandleDynamicFOV()
    {
        playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, targetFOV, Time.deltaTime * fovChangeSpeed);
    }

    void HandleCameraSway()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        Vector3 sway = new Vector3(-mouseX * swayAmount, -mouseY * swayAmount, 0);

        // เก็บค่า pos ปัจจุบันไว้ก่อน
        Vector3 current = playerCamera.transform.localPosition;
        Vector3 target = current + sway;

        // ลื่นไหลด้วย Lerp ไปหา target (อันเดิมของคุณ Lerp ตัวเองกับตัวเองเลยไม่เกิดผล)
        playerCamera.transform.localPosition = Vector3.Lerp(current, target, Time.deltaTime * swaySmoothness);
    }

    // ✅ เอามาจากโค้ดที่ 2 แบบ “อนิเมชั่นล้วนๆ”
    void HandleAnimation()
    {
        if (animator == null) return;

        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        // ใช้กติกาวิ่งเดียวกับระบบ stamina ของโค้ดอันแรก
        bool isRunning = isSprinting && controller.isGrounded;

        animator.SetFloat("horizontal", horizontal);
        animator.SetFloat("vertical", vertical);
        animator.SetBool("isRunning", isRunning);
    }

    // (ถ้าคุณมี UI stamina อยู่แล้ว จะเรียกใช้ได้เหมือนเดิม)
    public float GetCurrentStamina() => currentStamina;
    public float GetMaxStamina() => maxStamina;
    public float GetStaminaPercent() => (maxStamina <= 0f) ? 0f : currentStamina / maxStamina;
}