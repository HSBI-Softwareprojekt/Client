using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    [SerializeField] private float speed; // Geschwindigkeit des Projektils
    [SerializeField] private float resetTime; // Zeit bis zur Deaktivierung des Projektils
    [SerializeField] private float pushForce = 10f; // Stärke des Stoßes

    private Animator anim; // Animator-Komponente des Projektils
    private Rigidbody2D rb; // Rigidbody des Projektils für physikalische Interaktion

    private void Awake()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    public void ActivateProjectile()
    {
        gameObject.SetActive(true);
        rb.isKinematic = false; // Erlaubt physikalische Einwirkungen auf das Projektil
    }

    private void Update()
    {
        if (!gameObject.activeInHierarchy) return;

        // Bewegt das Projektil
        transform.Translate(Vector2.right * speed * Time.deltaTime);

        // Deaktiviert das Projektil nach der eingestellten Zeit
        resetTime -= Time.deltaTime;
        if (resetTime <= 0)
            DeactivateProjectile();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            // Stoßt den Spieler weg, anstatt Schaden zuzufügen
            Rigidbody2D playerRb = collision.GetComponent<Rigidbody2D>();
            if (playerRb != null)
            {
                Vector2 pushDirection = (collision.transform.position - transform.position).normalized;
                playerRb.AddForce(pushDirection * pushForce, ForceMode2D.Impulse);
            }

            if (anim != null)
                anim.SetTrigger("explode");

            DeactivateProjectile(); // Deaktiviert das Projektil nach der Interaktion
        }
    }

    private void DeactivateProjectile()
    {
        gameObject.SetActive(false);
    }
}
