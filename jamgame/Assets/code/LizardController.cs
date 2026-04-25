using UnityEngine;

public class LizardController : MonoBehaviour
{
    public Rigidbody frontLeft;
    public Rigidbody frontRight;
    public Rigidbody backLeft;
    public Rigidbody backRight;
    public Rigidbody body;
    public Rigidbody tail;

    public float force = 50f;

    void Update()
    {
        // ขาหน้า
        if (Input.GetKey(KeyCode.Q))
            frontLeft.AddForce(Vector3.forward * force);

        if (Input.GetKey(KeyCode.E))
            frontRight.AddForce(Vector3.forward * force);

        // ขาหลัง
        if (Input.GetKey(KeyCode.A))
            backLeft.AddForce(Vector3.forward * force);

        if (Input.GetKey(KeyCode.D))
            backRight.AddForce(Vector3.forward * force);

        // ลำตัว
        if (Input.GetKey(KeyCode.S))
            body.AddForce(Vector3.forward * force);

        // หาง
        if (Input.GetKey(KeyCode.X))
            tail.AddForce(Vector3.left * force);
    }
}