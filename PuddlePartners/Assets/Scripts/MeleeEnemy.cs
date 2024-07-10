using UnityEngine;

public class MeleeEnemy : MonoBehaviour
{
    [Header("Attack Parameters")]
    [SerializeField] private float attackCooldown;
    [SerializeField] private float range;
    [SerializeField] private float pushForce; // Stärke des Stoßes

    [Header("Collider Parameters")]
    [SerializeField] private float colliderDistance;
    [SerializeField] private BoxCollider2D boxCollider;

    [Header("Player Layer")]
    [SerializeField] private LayerMask playerLayer;
    private float cooldownTimer = Mathf.Infinity;

    // References
    private Animator anim;
    private Rigidbody2D playerRigidbody; // Rigidbody des Spielers
    private EnemyPatrol enemyPatrol;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        enemyPatrol = GetComponentInParent<EnemyPatrol>();
    }

    private void Update()
    {
        cooldownTimer += Time.deltaTime;

        if (PlayerInSight())
        {
            if (cooldownTimer >= attackCooldown)
            {
                cooldownTimer = 0;
                anim.SetTrigger("meleeAttack");
                PushPlayer();
            }
        }

        if (enemyPatrol != null)
            enemyPatrol.enabled = !PlayerInSight();
    }

    private bool PlayerInSight()
    {
        RaycastHit2D hit = Physics2D.BoxCast(
            boxCollider.bounds.center + transform.right * range * transform.localScale.x * colliderDistance,
            new Vector3(boxCollider.bounds.size.x * range, boxCollider.bounds.size.y, boxCollider.bounds.size.z),
            0, Vector2.left, 0, playerLayer);

        if (hit.collider != null)
            playerRigidbody = hit.transform.GetComponent<Rigidbody2D>();

        return hit.collider != null;
    }


    private void OnDrawGizmos()
    {
        if (boxCollider == null)
        {
            return; // Verhindere den Zugriff auf ein null BoxCollider
        }

        Gizmos.color = Color.red;
        Vector3 center = boxCollider.bounds.center + transform.right * range * transform.localScale.x * colliderDistance;
        Vector3 size = new Vector3(boxCollider.bounds.size.x * range, boxCollider.bounds.size.y, boxCollider.bounds.size.z);

        // Zeichne den Erkennungsbereich
        Gizmos.DrawWireCube(center, size);

        // Visualisiere die Stoßrichtung, sicherstellen, dass das Spiel läuft und playerRigidbody nicht null ist
        if (Application.isPlaying && playerRigidbody != null)
        {
            Vector3 pushDirection = transform.right * (transform.localScale.x > 0 ? 1 : -1);
            pushDirection.y = 0; // Nur horizontale Komponente

            Gizmos.color = Color.blue;
            Gizmos.DrawLine(center, center + pushDirection * pushForce);
            Gizmos.DrawSphere(center + pushDirection * pushForce, 0.1f); // Zeichnet eine kleine Kugel am Ende der Stoßrichtung
        }
    }



    private void PushPlayer()
    {
        if (playerRigidbody != null)
        {
            // Bestimme die Richtung, in die gestoßen wird (rechts oder links, abhängig von der Richtung des Feindes)
            Vector2 pushDirection = transform.right * (transform.localScale.x > 0 ? 1 : -1);
            pushDirection.y = 0; // Stelle sicher, dass keine vertikale Komponente vorhanden ist

            playerRigidbody.AddForce(pushDirection * pushForce, ForceMode2D.Impulse);
        }
    }

}
