using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float speed; // Geschwindigkeit des Projektils
    private float direction; // Bewegungsrichtung des Projektils
    private bool hit; // Gibt an, ob das Projektil etwas getroffen hat
    private float lifetime; // Lebensdauer des Projektils

    private Animator anim; // Animator-Komponente des Projektils
    private BoxCollider2D boxCollider; // BoxCollider2D-Komponente des Projektils

    private void Awake()
    {
        // Initialisiert die Animator- und BoxCollider2D-Komponenten
        anim = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();
    }

    private void Update()
    {
        // Bewegt das Projektil, wenn es nichts getroffen hat
        if (hit) return;
        float movementSpeed = speed * Time.deltaTime * direction;
        transform.Translate(movementSpeed, 0, 0);

        // Erhöht die Lebensdauer des Projektils und deaktiviert es nach 5 Sekunden
        lifetime += Time.deltaTime;
        if (lifetime > 5) gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Setzt den Trefferstatus, deaktiviert den BoxCollider und startet die Explosionsanimation
        hit = true;
        boxCollider.enabled = false;
        anim.SetTrigger("explode");

        // Verursacht Schaden bei Kollision mit einem "Enemy"-Tag-Objekt
        if (collision.tag == "Enemy")
            collision.GetComponent<Health>().TakeDamage(1);
    }

    public void SetDirection(float _direction)
    {
        // Setzt die Richtung und initialisiert das Projektil für die Bewegung
        lifetime = 0;
        direction = _direction;
        gameObject.SetActive(true);
        hit = false;
        boxCollider.enabled = true;

        // Passt die Skalierung des Projektils basierend auf der Bewegungsrichtung an
        float localScaleX = transform.localScale.x;
        if (Mathf.Sign(localScaleX) != _direction)
            localScaleX = -localScaleX;

        transform.localScale = new Vector3(localScaleX, transform.localScale.y, transform.localScale.z);
    }

    private void Deactivate()
    {
        // Deaktiviert das Projektil
        gameObject.SetActive(false);
    }
}
