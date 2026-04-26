using UnityEngine;
using UnityEngine.UI;

public class HungerSystem : MonoBehaviour
{
    public float maxHunger = 100f;
    public float hunger;

    public float timeToZero = 120f; // 2 นาที = 120 วิ
    private float drainRate;

    public Slider hungerBar;

    void Start()
    {
        hunger = maxHunger;

        // คำนวณว่าต้องลดต่อวินาทีเท่าไหร่
        drainRate = maxHunger / timeToZero;
    }

    void Update()
    {
        // ลดความหิวตามเวลา
        hunger -= drainRate * Time.deltaTime;
        hunger = Mathf.Clamp(hunger, 0, maxHunger);

        // อัพเดตหลอด
        if (hungerBar != null)
        {
            hungerBar.value = hunger / maxHunger;
        }
    }

    // ฟังก์ชันกิน
    public void Eat(float amount)
    {
        hunger += amount;
        hunger = Mathf.Clamp(hunger, 0, maxHunger);
    }
}