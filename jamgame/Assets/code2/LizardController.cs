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


    public float force = 50f;
    public float torqueForce = 10f;

    void Update()
    {
        // 🐾 ควบคุมขา
        if (Input.GetKey(KeyCode.Q))
            frontL.AddForce(transform.forward * force);

        if (Input.GetKey(KeyCode.E))
            frontR.AddForce(transform.forward * force);

        if (Input.GetKey(KeyCode.A))
            backL.AddForce(transform.forward * force);

        if (Input.GetKey(KeyCode.D))
            backR.AddForce(transform.forward * force);

        if (Input.GetKey(KeyCode.S))
            body.AddForce(transform.forward * force);

        if (Input.GetKey(KeyCode.X))
            tail.AddForce(transform.forward * force);

        // 🖱️ หัวหันตามเมาส์
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Vector3 dir = hit.point - head.position;
            head.AddTorque(dir.normalized * torqueForce);
        }
        
        if (Input.GetKeyDown(KeyCode.W))
        {
        ShootTongue();
        }

        if (Input.GetKeyUp(KeyCode.W))
        {
        ReleaseTongue();
        }
        if (Input.GetKeyDown(KeyCode.G))
    {
        DetachTail();
    }
    }
    void ShootTongue()
{
    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

    if (Physics.Raycast(ray, out RaycastHit hit, 20f))
    {
        tongueJoint = head.gameObject.AddComponent<SpringJoint>();
        tongueJoint.connectedBody = hit.rigidbody;
        tongueJoint.spring = 200f;
        tongueJoint.damper = 10f;
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
    Destroy(tailJoint);
}
}