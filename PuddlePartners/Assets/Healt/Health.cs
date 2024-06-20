using UnityEngine;
using System.Collections;

public class Health : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private float startingHealth; // Anfangsgesundheit des Charakters
    public float currentHealth { get; private set; } // Aktuelle Gesundheit des Charakters, nur lesbar
    private Animator anim; // Animator-Komponente des Charakters
    private bool dead; // Gibt an, ob der Charakter tot ist

    [Header("iFrames")]
    [SerializeField] private float iFramesDuration; // Dauer der Unverwundbarkeit nach Schaden
    [SerializeField] private int numberOfFlashes; // Anzahl der Blinkvorgänge während der Unverwundbarkeit
    private SpriteRenderer spriteRend; // SpriteRenderer des Charakters

    [Header("Components")]
    [SerializeField] private Behaviour[] components; // Liste der zu deaktivierenden Komponenten bei Tod
    private bool invulnerable; // Gibt an, ob der Charakter derzeit unverwundbar ist

    private void Awake()
    {
        // Initialisiert die aktuelle Gesundheit, den Animator und den SpriteRenderer
        currentHealth = startingHealth;
        anim = GetComponent<Animator>();
        spriteRend = GetComponent<SpriteRenderer>();
    }

    public void TakeDamage(float _damage)
    {
        // Verarbeitet den eingehenden Schaden, wenn der Charakter nicht unverwundbar ist
        if (invulnerable) return;
        currentHealth = Mathf.Clamp(currentHealth - _damage, 0, startingHealth);

        if (currentHealth > 0)
        {
            // Löst den "hurt"-Trigger im Animator aus und startet die Unverwundbarkeitsroutine
            anim.SetTrigger("hurt");
            StartCoroutine(Invunerability());
        }
        else
        {
            if (!dead)
            {
                // Löst den "die"-Trigger im Animator aus und deaktiviert alle angegebenen Komponenten
                anim.SetTrigger("die");

                // Deaktiviert alle angehängten Komponentenklassen
                foreach (Behaviour component in components)
                    component.enabled = false;

                dead = true;
            }
        }
    }

    public void AddHealth(float _value)
    {
        // Fügt der aktuellen Gesundheit einen Wert hinzu, bis zum Maximum der Anfangsgesundheit
        currentHealth = Mathf.Clamp(currentHealth + _value, 0, startingHealth);
    }

    private IEnumerator Invunerability()
    {
        // Macht den Charakter für eine gewisse Zeit unverwundbar und blinkt dabei
        invulnerable = true;
        Physics2D.IgnoreLayerCollision(10, 11, true);
        for (int i = 0; i < numberOfFlashes; i++)
        {
            spriteRend.color = new Color(1, 0, 0, 0.5f); // Setzt die Farbe auf halbtransparent rot
            yield return new WaitForSeconds(iFramesDuration / (numberOfFlashes * 2));
            spriteRend.color = Color.white; // Setzt die Farbe zurück auf weiß
            yield return new WaitForSeconds(iFramesDuration / (numberOfFlashes * 2));
        }
        Physics2D.IgnoreLayerCollision(10, 11, false);
        invulnerable = false;
    }
}
