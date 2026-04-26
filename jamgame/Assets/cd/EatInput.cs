using UnityEngine;
using TMPro;

public class EatInput : MonoBehaviour
{
    public HungerSystem hungerSystem;
    public TMP_InputField inputField;

    public void OnClickEat()
    {
        float amount;

        if (float.TryParse(inputField.text, out amount))
        {
            hungerSystem.Eat(amount);
        }
        else
        {
            Debug.Log("ใส่ตัวเลขด้วย");
        }
    }
}