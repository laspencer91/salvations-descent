using TMPro;
using UnityEngine;

public class HealthCounter : MonoBehaviour
{
    public FPSPlayer player;

    TextMeshProUGUI healthCounterText;

    private void Awake() 
    {
        healthCounterText = GetComponent<TextMeshProUGUI>();
    }

    private void Update() 
    {
        healthCounterText.text = (player.CurrentHealth).ToString();
    }
}
