using UnityEngine;

public class TongueSystem : MonoBehaviour
{
    public GameObject tonguePrefab;
    public Transform mouthPoint;

    GameObject currentTongue;
    FixedJoint joint;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            if (currentTongue == null)
            {
                ShootTongue();
            }
            else
            {
                ReleaseTongue();
            }
        }
    }

    void ShootTongue()
    {
        currentTongue = Instantiate(tonguePrefab, mouthPoint.position, mouthPoint.rotation);

        Rigidbody rb = currentTongue.GetComponent<Rigidbody>();
        rb.AddForce(mouthPoint.forward * 500f);

        joint = currentTongue.AddComponent<FixedJoint>();
        joint.connectedBody = GetComponent<Rigidbody>();
    }

    void ReleaseTongue()
    {
        Destroy(currentTongue);
        currentTongue = null;
    }
}