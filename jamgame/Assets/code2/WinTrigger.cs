using UnityEngine;

public class WinTrigger : MonoBehaviour
{
    public GameObject winPanel;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("ชนแล้วกับ: " + other.name);

        winPanel.SetActive(true); // เปิดเลย ไม่ต้องเช็คก่อน
    }
}