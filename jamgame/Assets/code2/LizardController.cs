using UnityEngine;

public class LizardController : MonoBehaviour
{
    public Rigidbody body;
    public Rigidbody head;
    public Rigidbody frontL, frontR, backL, backR, tail;
    public Camera cam;
    public LineRenderer tongueLine;
    private SpringJoint tongueJoint;
    private Rigidbody grabbedObject;
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

        // 🐾 Movement
        if (Input.GetKey(KeyCode.Q))
            frontL.AddForce(-right * sideForce, ForceMode.VelocityChange);

        if (Input.GetKey(KeyCode.E))
            frontR.AddForce(right * sideForce, ForceMode.VelocityChange);

        if (Input.GetKey(KeyCode.A))
        {
            Vector3 dir = (-right * 0.5f + forward).normalized;
            backL.AddForce(dir * forwardForce, ForceMode.VelocityChange);
        }

        if (Input.GetKey(KeyCode.D))
        {
            Vector3 dir = (right * 0.5f + forward).normalized;
            backR.AddForce(dir * forwardForce, ForceMode.VelocityChange);
        }

        if (Input.GetKey(KeyCode.S))
            body.AddForce(Vector3.up * liftForce, ForceMode.VelocityChange);

        if (Input.GetKey(KeyCode.X))
            tail.AddForce(forward * forwardForce, ForceMode.VelocityChange);

        

        // 👅 ลิ้น (กด = จับ / ปล่อย = ขว้าง)
        if (Input.GetKeyDown(KeyCode.W))
        {
            ShootTongue();
        }

        if (Input.GetKeyUp(KeyCode.W))
        {
            ReleaseTongue(true);
        }

        // 🐍 สลัดหาง
        if (Input.GetKeyDown(KeyCode.G))
        {
            DetachTail();
        }
    }

    void FixedUpdate()
    {
        isClinging = wallContacts > 0;
        wallContacts = 0;

        if (isClinging)
        {
            body.useGravity = false;
            body.velocity *= 0.98f;
        }
        else
        {
            body.useGravity = true;
        }

        if (body.velocity.magnitude > maxSpeed)
        {
            body.velocity = body.velocity.normalized * maxSpeed;
        }

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
                body.AddForce(-contact.normal * clingForce, ForceMode.Force);
            }
        }
    }

    

    // 👅 ยิงลิ้น
    void ShootTongue()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, 20f))
        {
            if (hit.rigidbody != null)
            {
                grabbedObject = hit.rigidbody;

                tongueJoint = head.gameObject.AddComponent<SpringJoint>();
                tongueJoint.connectedBody = grabbedObject;

                tongueJoint.spring = 300f;
                tongueJoint.damper = 15f;
                tongueJoint.maxDistance = 2f;
            }
        }
    }

    // 👅 ปล่อย + ขว้าง
    void ReleaseTongue(bool throwObject)
    {
        if (tongueJoint != null)
        {
            Rigidbody obj = tongueJoint.connectedBody;

            Destroy(tongueJoint);

            if (throwObject && obj != null)
            {
                Vector3 throwDir = head.transform.forward;
                obj.AddForce(throwDir * 500f, ForceMode.Impulse);
            }
        }
    }

    void DetachTail()
    {
        if (tailJoint != null)
            Destroy(tailJoint);
    }
}