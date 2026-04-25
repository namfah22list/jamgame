using UnityEngine;

public class SimpleMove : MonoBehaviour
{
    public float speed = 5f;

    void Update()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 move = new Vector3(h, 0, v);
        transform.position += move * speed * Time.deltaTime;

        if (move != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(move);
        }
    }
}