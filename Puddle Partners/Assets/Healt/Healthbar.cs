using UnityEngine;
using UnityEngine.UI;

public class Healthbar : MonoBehaviour
{
    [SerializeField] private Health playerHealth; // Referenz auf die Health-Komponente des Spielers
    [SerializeField] private Image totalhealthBar; // Image-Komponente der gesamten Gesundheitsleiste
    [SerializeField] private Image currenthealthBar; // Image-Komponente der aktuellen Gesundheitsleiste

    private void Start()
    {
        // Initialisiert die gesamte Gesundheitsleiste basierend auf der aktuellen Gesundheit des Spielers
        totalhealthBar.fillAmount = playerHealth.currentHealth / 10;
    }

    private void Update()
    {
        // Aktualisiert die aktuelle Gesundheitsleiste basierend auf der aktuellen Gesundheit des Spielers
        currenthealthBar.fillAmount = playerHealth.currentHealth / 10;
    }
}
