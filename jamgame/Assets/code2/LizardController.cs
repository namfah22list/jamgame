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
        // 🔒 จำกัดความเร็ว (คุมง่ายขึ้นมาก)
        if (body.velocity.magnitude > maxSpeed)
        {
            body.velocity = body.velocity.normalized * maxSpeed;
        }

        // 🧍 พยุงตัวไม่ให้ล้มง่าย
        Vector3 torque = Vector3.Cross(transform.up, Vector3.up);
        body.AddTorque(torque * 150f);
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

    void OnCollisionStay(Collision col)
    {
        if (col.gameObject.CompareTag("Wall"))
        {
            body.AddForce(-col.contacts[0].normal * 50f);
        }
    }

    void DetachTail()
    {
        if (tailJoint != null)
            Destroy(tailJoint);
    }
}