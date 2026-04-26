using UnityEngine;

public class LizardController : MonoBehaviour
{
    public Rigidbody body;
    public Rigidbody head;
    public Rigidbody frontL, frontR, backL, backR, tail;
    public Camera cam;
    public LineRenderer tongueLine;
    private SpringJoint tongueJoint;
    public ConfigurableJoint tailJoint;

    [Header("Force Settings")]
    public float sideForce = 15f;
    public float forwardForce = 30f;
    public float liftForce = 15f;
    public float torqueForce = 10f;
    public float maxSpeed = 6f;

    [Header("Wall Climb")]
    public float clingForce = 80f;
    private int wallContacts = 0;
    private bool isClinging = false;

    void Start()
    {
        Debug.Log("LizardController ทำงานแล้ว");
    }

    void Update()
    {
        Vector3 forward = transform.forward;
        Vector3 right = transform.right;

        // 🟢 ขาหน้าซ้าย → ไปซ้าย
        if (Input.GetKey(KeyCode.Q))
        {
            frontL.AddForce(-right * sideForce, ForceMode.VelocityChange);
        }

        // 🔵 ขาหน้าขวา → ไปขวา
        if (Input.GetKey(KeyCode.E))
        {
            frontR.AddForce(right * sideForce, ForceMode.VelocityChange);
        }

        // 🟡 ขาหลังซ้าย → ซ้ายนิด + ไปหน้า
        if (Input.GetKey(KeyCode.A))
        {
            Vector3 dir = (-right * 0.5f + forward).normalized;
            backL.AddForce(dir * forwardForce, ForceMode.VelocityChange);
        }

        // 🟠 ขาหลังขวา → ขวานิด + ไปหน้า
        if (Input.GetKey(KeyCode.D))
        {
            Vector3 dir = (right * 0.5f + forward).normalized;
            backR.AddForce(dir * forwardForce, ForceMode.VelocityChange);
        }

        // 🔴 ตัว → ลอยนิด
        if (Input.GetKey(KeyCode.S))
        {
            body.AddForce(Vector3.up * liftForce, ForceMode.VelocityChange);
        }

        // 🟣 หาง → ช่วยดันไปหน้า
        if (Input.GetKey(KeyCode.X))
        {
            tail.AddForce(forward * forwardForce, ForceMode.VelocityChange);
        }

        // 🖱️ หัวหันตามเมาส์
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Vector3 dir = hit.point - head.position;
            head.AddTorque(dir.normalized * torqueForce);
        }

        // 👅 ลิ้น
        if (Input.GetKeyDown(KeyCode.W))
        {
            ShootTongue();
        }

        if (Input.GetKeyUp(KeyCode.W))
        {
            ReleaseTongue();
        }

        // 🐍 สลัดหาง
        if (Input.GetKeyDown(KeyCode.G))
        {
            DetachTail();
        }
    }

    void FixedUpdate()
    {
        // 🧗 ตรวจว่ากำลังเกาะกำแพงไหม
        isClinging = wallContacts > 0;

        // รีเซ็ต counter ทุกเฟรม
        wallContacts = 0;

        // 🧲 ถ้าเกาะ → ปิดแรงโน้มถ่วง + ดูดเข้ากำแพง
        if (isClinging)
        {
            body.useGravity = false;

            // ดูดเข้าหากำแพง (ใช้ velocity ลดการหลุด)
            body.velocity *= 0.98f; // กันเด้งออก
        }
        else
        {
            body.useGravity = true;
        }

        // 🔒 จำกัดความเร็ว
        if (body.velocity.magnitude > maxSpeed)
        {
            body.velocity = body.velocity.normalized * maxSpeed;
        }

        // 🧍 พยุงตัว
        Vector3 torque = Vector3.Cross(transform.up, Vector3.up);
        body.AddTorque(torque * 150f);
    }

    void OnCollisionStay(Collision col)
    {
        if (col.gameObject.CompareTag("Wall"))
        {
            wallContacts++;

            foreach (ContactPoint contact in col.contacts)
            {
                // 🧲 แรงดูดเข้ากำแพง
                body.AddForce(-contact.normal * clingForce, ForceMode.Force);
            }
        }
    }

    void ShootTongue()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, 20f))
        {
            if (hit.rigidbody != null)
            {
                tongueJoint = head.gameObject.AddComponent<SpringJoint>();
                tongueJoint.connectedBody = hit.rigidbody;
                tongueJoint.spring = 200f;
                tongueJoint.damper = 10f;
            }
        }
    }

    void ReleaseTongue()
    {
        if (tongueJoint != null)
            Destroy(tongueJoint);
    }

    void DetachTail()
    {
        if (tailJoint != null)
            Destroy(tailJoint);
    }
}