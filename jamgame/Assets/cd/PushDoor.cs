using UnityEngine;

public class PushDoor : MonoBehaviour
{
    public float openForce = 5f;
    public float returnForce = 2f;
    public float maxAngle = 90f;

    private Rigidbody rb;
    private Quaternion startRot;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        startRot = transform.rotation;
    }

    void FixedUpdate()
    {
        // ดึงกลับให้ปิด
        Quaternion delta = startRot * Quaternion.Inverse(transform.rotation);
        delta.ToAngleAxis(out float angle, out Vector3 axis);

        if (angle > 180) angle -= 360;

        rb.AddTorque(axis * angle * returnForce);
    }

    void OnCollisionStay(Collision collision)
    {
        // ถ้า player ชน → ผลัก
        if (collision.gameObject.CompareTag("Player"))
        {
            Vector3 pushDir = collision.relativeVelocity;
            rb.AddTorque(pushDir * openForce);
        }
    }
}