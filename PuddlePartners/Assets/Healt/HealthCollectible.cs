using UnityEngine;

public class HealthCollectible : MonoBehaviour
{
    [SerializeField] private float healthValue; // Wert der Gesundheitsauffüllung durch das Sammelobjekt

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Überprüft, ob das kollidierende Objekt den Tag "Player" hat
        if (collision.tag == "Player")
        {
            // Fügt dem Spieler Gesundheit hinzu und deaktiviert das Sammelobjekt
            collision.GetComponent<Health>().AddHealth(healthValue);
            gameObject.SetActive(false);
        }
    }
}
