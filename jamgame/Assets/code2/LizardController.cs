using UnityEngine;

public class LizardController : MonoBehaviour
{
    public Rigidbody body;
    public Rigidbody head;
    public Rigidbody frontL, frontR, backL, backR, tail;
    public Camera cam;

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
    }
}