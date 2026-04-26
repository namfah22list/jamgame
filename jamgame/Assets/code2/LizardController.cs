using UnityEngine;

public class LizardController : MonoBehaviour
{
    public Rigidbody body;
    public Rigidbody frontL, frontR, backL, backR, tail;
    public Camera cam;

    [Header("Movement")]
    public float moveForce = 20f;
    public float turnForce = 5f;
    public float maxSpeed = 6f;

    [Header("Balance")]
    public float uprightForce = 150f;

    [Header("Assist")]
    public float groundCheckDistance = 1.5f;
    public float stickToGroundForce = 25f;
    public float moveAssist = 20f;

    [Header("Front Leg")]
    public float frontLegForce = 8f;
    public float frontLegLift = 5f;

    void Update()
    {
        // 🎥 ทิศจากกล้อง
        Vector3 forward = cam.transform.forward;
        Vector3 right = cam.transform.right;

        forward.y = 0;
        right.y = 0;

        forward.Normalize();
        right.Normalize();

        // 🟢 เดินหน้า
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
        {
            body.AddForce(forward * moveForce, ForceMode.Acceleration);

            backL.AddForce(forward * 3f, ForceMode.Acceleration);
            backR.AddForce(forward * 3f, ForceMode.Acceleration);
        }

        // 🟢 Q = ขาหน้าซ้าย + เลี้ยว
        if (Input.GetKey(KeyCode.Q))
        {
            body.AddTorque(Vector3.up * -turnForce, ForceMode.Acceleration);

            Vector3 lift = Vector3.up * frontLegLift;
            Vector3 push = -right * frontLegForce;

            frontL.AddForce(lift + push, ForceMode.Acceleration);
        }

        // 🔵 E = ขาหน้าขวา + เลี้ยว
        if (Input.GetKey(KeyCode.E))
        {
            body.AddTorque(Vector3.up * turnForce, ForceMode.Acceleration);

            Vector3 lift = Vector3.up * frontLegLift;
            Vector3 push = right * frontLegForce;

            frontR.AddForce(lift + push, ForceMode.Acceleration);
        }

        // 🟣 หางช่วยดัน
        if (Input.GetKey(KeyCode.X))
        {
            tail.AddForce(forward * 6f, ForceMode.Acceleration);
        }
    }

    void FixedUpdate()
    {
        // 🟫 เช็คพื้น + assist
        if (Physics.Raycast(body.position, Vector3.down, out RaycastHit hit, groundCheckDistance))
        {
            body.AddForce(-hit.normal * stickToGroundForce, ForceMode.Acceleration);

            Vector3 forward = cam.transform.forward;
            forward.y = 0;
            forward.Normalize();

            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
            {
                body.AddForce(forward * moveAssist, ForceMode.Acceleration);
            }
        }

        // 🔥 เด้งขาหน้าเล็กน้อย (ให้ดูมีชีวิต)
        float bounce = Mathf.Sin(Time.time * 10f) * 0.5f;
        frontL.AddForce(Vector3.up * bounce, ForceMode.Acceleration);
        frontR.AddForce(Vector3.up * bounce, ForceMode.Acceleration);

        // 🔒 จำกัดความเร็ว
        Vector3 flatVel = new Vector3(body.velocity.x, 0, body.velocity.z);

        if (flatVel.magnitude > maxSpeed)
        {
            flatVel = flatVel.normalized * maxSpeed;
            body.velocity = new Vector3(flatVel.x, body.velocity.y, flatVel.z);
        }

        // 🧍 พยุงตัว
        Vector3 torque = Vector3.Cross(transform.up, Vector3.up);
        body.AddTorque(torque * uprightForce);

        // 🧊 หน่วง
        body.velocity *= 0.97f;

        // 🔥 กันลื่น
        Vector3 lateral = Vector3.Project(body.velocity, transform.right);
        body.velocity -= lateral * 0.2f;
    }
}