using UnityEngine;

public class RangedEnemy : MonoBehaviour
{
    [Header("Attack Parameters")]
    [SerializeField] private float attackCooldown; // Abklingzeit zwischen Angriffen
    [SerializeField] private float range; // Reichweite des Angriffs
    [SerializeField] private int damage; // Schaden des Angriffs

    [Header("Ranged Attack")]
    [SerializeField] private Transform firepoint; // Ausgangspunkt für die Projektile
    [SerializeField] private GameObject[] fireballs; // Array von Feuerbällen für den Angriff

    [Header("Collider Parameters")]
    [SerializeField] private float colliderDistance; // Abstand des Kolliders zur Erkennung des Spielers
    [SerializeField] private BoxCollider2D boxCollider; // BoxCollider2D-Komponente zur Erkennung des Spielers

    [Header("Player Layer")]
    [SerializeField] private LayerMask playerLayer; // LayerMask für den Spieler
    private float cooldownTimer = Mathf.Infinity; // Timer zur Überprüfung der Abklingzeit
    private GameObject player;
    private Rigidbody2D rb;

    // Referenzen
    private Animator anim; // Animator-Komponente
    private EnemyPatrol enemyPatrol; // Referenz zur EnemyPatrol-Komponente

    private void Awake()
    {
        // Initialisiert die Animator- und EnemyPatrol-Komponenten
        anim = GetComponent<Animator>();
        enemyPatrol = GetComponentInParent<EnemyPatrol>();
        player = GameObject.FindGameObjectWithTag("Player");
        rb = player.GetComponent<Rigidbody2D>();

    }

    private void Update()
    {
        // Erhöht den Abklingzeit-Timer
        cooldownTimer += Time.deltaTime;

        // Greift nur an, wenn der Spieler in Sichtweite ist
        if (PlayerInSight())
        {
            if (cooldownTimer >= attackCooldown)
            {
                cooldownTimer = 0;
                anim.SetTrigger("RangeAttack");
            }
        }

        // Deaktiviert das Patrouillieren, wenn der Spieler in Sichtweite ist
        if (enemyPatrol != null)
            enemyPatrol.enabled = !PlayerInSight();
    }

    private void RangedAttack()
    {
        // Setzt den Abklingzeit-Timer zurück und aktiviert das Projektil
        cooldownTimer = 0;
        fireballs[FindFireball()].transform.position = firepoint.position;
        fireballs[FindFireball()].GetComponent<EnemyProjectile>().ActivateProjectile();
    }

    private int FindFireball()
    {
        // Findet das erste inaktive Projektil im Array
        for (int i = 0; i < fireballs.Length; i++)
        {
            if (!fireballs[i].activeInHierarchy)
                return i;
        }
        return 0;
    }

    private bool PlayerInSight()
    {
        // Überprüft, ob der Spieler in Sichtweite ist, indem ein BoxCast verwendet wird
        RaycastHit2D hit = Physics2D.BoxCast(
            boxCollider.bounds.center + transform.right * range * transform.localScale.x * colliderDistance,
            new Vector3(boxCollider.bounds.size.x * range, boxCollider.bounds.size.y, boxCollider.bounds.size.z),
            0, Vector2.left, 0, playerLayer);

        return hit.collider != null;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Vector2 forceDirection = transform.localScale.x > 0 ? new Vector2(400f, 100f) : new Vector2(-400f, 100f);
        rb.AddForce(forceDirection);
    }

    private void OnDrawGizmos()
    {
        // Zeichnet eine rote Box zur Visualisierung der Erkennungsreichweite
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(
            boxCollider.bounds.center + transform.right * range * transform.localScale.x * colliderDistance,
            new Vector3(boxCollider.bounds.size.x * range, boxCollider.bounds.size.y, boxCollider.bounds.size.z));
    }
}
