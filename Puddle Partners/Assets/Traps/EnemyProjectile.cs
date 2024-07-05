using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class EnemyProjectile : EnemyDamage
{
    [SerializeField] private float speed; // Geschwindigkeit des Projektils
    [SerializeField] private float resetTime; // Zeit bis zur Deaktivierung des Projektils
    private float lifetime; // Lebensdauer des Projektils
    private Animator anim; // Animator-Komponente des Projektils
    private BoxCollider2D coll; // BoxCollider2D-Komponente des Projektils

    private bool hit; // Gibt an, ob das Projektil etwas getroffen hat

    private void Awake()
    {
        // Initialisiert die Animator- und BoxCollider2D-Komponenten
        anim = GetComponent<Animator>();
        coll = GetComponent<BoxCollider2D>();
    }

    public void ActivateProjectile()
    {
        // Aktiviert das Projektil und setzt den Trefferstatus und die Lebensdauer zurück
        hit = false;
        lifetime = 0;
        gameObject.SetActive(true);
        coll.enabled = true;
    }

    private void Update()
    {
        // Bewegt das Projektil, wenn es nichts getroffen hat
        if (hit) return;
        float movementSpeed = speed * Time.deltaTime;
        transform.Translate(movementSpeed, 0, 0);

        // Erhöht die Lebensdauer des Projektils und deaktiviert es nach Ablauf der Reset-Zeit
        lifetime += Time.deltaTime;
        if (lifetime > resetTime)
            gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Setzt den Trefferstatus und führt die Logik des Elternskripts aus
        hit = true;
        base.OnTriggerEnter2D(collision); // Führt zuerst die Logik aus dem Elternskript aus
        coll.enabled = false;

        // Löst die Explosionsanimation aus, wenn vorhanden, oder deaktiviert das Projektil
        if (anim != null)
            anim.SetTrigger("explode"); // Wenn das Objekt ein Feuerball ist, explodiert es
        else
            gameObject.SetActive(false); // Deaktiviert das Projektil, wenn es ein Objekt trifft
    }

    private void Deactivate()
    {
        // Deaktiviert das Projektil
        gameObject.SetActive(false);
    }
}
