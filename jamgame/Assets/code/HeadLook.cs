using UnityEngine;

public class HeadLook : MonoBehaviour
{
    public float sensitivity = 3f;

    float rotX;
    float rotY;

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * sensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity;

        rotY += mouseX;
        rotX -= mouseY;

        transform.rotation = Quaternion.Euler(rotX, rotY, 0);
    }
}