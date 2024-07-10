using UnityEngine;

public class RangedEnemy : MonoBehaviour
{
    [Header("Attack Parameters")]
    [SerializeField] private float attackCooldown; // Time between attacks
    [SerializeField] private float range; // Attack range
    [SerializeField] private int damage; // Attack damage

    [Header("Ranged Attack")]
    [SerializeField] private Transform firepoint; // Projectile spawn point
    [SerializeField] private GameObject[] fireballs; // Array of fireballs

    [Header("Collider Parameters")]
    [SerializeField] private float colliderDistance; // Distance for detecting the player
    [SerializeField] private BoxCollider2D boxCollider; // BoxCollider2D for detection

    [Header("Player Layer")]
    [SerializeField] private LayerMask playerLayer; // LayerMask for the player

    private float cooldownTimer = Mathf.Infinity; // Timer for attack cooldown
    private GameObject player;
    private Rigidbody2D rb;

    // References
    private Animator anim;
    private EnemyPatrol enemyPatrol;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        enemyPatrol = GetComponentInParent<EnemyPatrol>();
        player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            rb = player.GetComponent<Rigidbody2D>();
            if (rb == null)
            {
                Debug.LogError("Player does not have a Rigidbody2D component.");
            }
        }
        else
        {
            Debug.LogError("Player object not found.");
        }
    }

    private void Update()
    {
        cooldownTimer += Time.deltaTime;

        if (PlayerInSight() && cooldownTimer >= attackCooldown)
        {
            cooldownTimer = 0;
            anim.SetTrigger("rangedAttack");
        }

        if (enemyPatrol != null)
        {
            enemyPatrol.enabled = !PlayerInSight();
        }
    }

    private void RangedAttack()
    {
        cooldownTimer = 0;
        int fireballIndex = FindFireball();
        if (fireballIndex != -1)
        {
            fireballs[fireballIndex].transform.position = firepoint.position;
            fireballs[fireballIndex].GetComponent<EnemyProjectile>().ActivateProjectile();
        }
    }

    private int FindFireball()
    {
        for (int i = 0; i < fireballs.Length; i++)
        {
            if (!fireballs[i].activeInHierarchy)
                return i;
        }
        return -1; // Return -1 if no inactive fireball is found
    }

    private bool PlayerInSight()
    {
        RaycastHit2D hit = Physics2D.BoxCast(
            boxCollider.bounds.center + transform.right * range * transform.localScale.x * colliderDistance,
            new Vector3(boxCollider.bounds.size.x * range, boxCollider.bounds.size.y, boxCollider.bounds.size.z),
            0, Vector2.left, 0, playerLayer);

        return hit.collider != null;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (rb != null)
        {
            Vector2 forceDirection = transform.localScale.x > 0 ? new Vector2(400f, 100f) : new Vector2(-400f, 100f);
            rb.AddForce(forceDirection);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(
            boxCollider.bounds.center + transform.right * range * transform.localScale.x * colliderDistance,
            new Vector3(boxCollider.bounds.size.x * range, boxCollider.bounds.size.y, boxCollider.bounds.size.z));
    }
}
