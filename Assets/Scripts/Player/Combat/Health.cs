using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthSystem : MonoBehaviour
{
    public Slider healthSlider;
    public TextMeshProUGUI healthBarValueText;

    public int maxHealth;
    public int currentHealth;
    void Start()
    {
        currentHealth = maxHealth;
        healthSlider.maxValue = maxHealth;
        healthSlider.value = currentHealth;
        if (healthBarValueText == null)
        {
            Debug.LogError("Health Bar Value Text НЕ назначен в инспекторе!");
        }
    }
    void Update()
    {
        healthBarValueText.text = currentHealth.ToString()+ "/" + maxHealth.ToString();
        healthSlider.value=currentHealth;
        healthSlider.maxValue = maxHealth; 
    }
}
