using UnityEngine;

public class EnemyDamage : MonoBehaviour
{
    [SerializeField] protected float damage; // Schaden, den der Feind verursacht

    protected void OnTriggerEnter2D(Collider2D collision)
    {
        // Überprüft, ob das kollidierende Objekt den Tag "Player" hat
        if (collision.tag == "Player")
        {
            // Ruft die TakeDamage-Methode auf der Health-Komponente des Spielers auf und fügt Schaden zu
            collision.GetComponent<Health>().TakeDamage(damage);
        }
    }
}
