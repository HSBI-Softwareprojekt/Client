using UnityEngine;

public class EnemyPatrol : MonoBehaviour
{
    [Header("Patrol Points")]
    [SerializeField] private Transform leftEdge; // Transform für den linken Patrouillenpunkt
    [SerializeField] private Transform rightEdge; // Transform für den rechten Patrouillenpunkt

    [Header("Enemy")]
    [SerializeField] private Transform enemy; // Transform des Feindes

    [Header("Movement parameters")]
    [SerializeField] private float speed; // Geschwindigkeit der Feindbewegung
    private Vector3 initScale;  // Ursprüngliche Skalierung des Feindes
    private bool movingLeft;  // Bool-Wert, der anzeigt, ob der Feind nach links geht

    [Header("Idle Behaviour")]
    [SerializeField] private float idleDuration; // Dauer des Stillstands
    private float idleTimer;  // Timer für den Stillstand

    [Header("Enemy Animator")]
    [SerializeField] private Animator anim;  // Animator für den Feind

    private void Awake()
    {
        initScale = enemy.localScale;  // Initialisiert die ursprüngliche Skalierung des Feindes
    }
    private void OnDisable()
    {
        anim.SetBool("moving", false);  // Setzt den "Moving"-Parameter im Animator auf false, wenn das Objekt deaktiviert wird
    }

    private void Update()
    {
        if (movingLeft) // Überprüft die Bewegungsrichtung und bewegt den Feind entsprechend
        {
            // Bewegt den Feind nach links, wenn er nicht den linken Rand erreicht hat
            if (enemy.position.x >= leftEdge.position.x)
                MoveInDirection(-1);
            else
                DirectionChange();
        }
        else
        {
            // Bewegt den Feind nach rechts, wenn er nicht den rechten Rand erreicht hat
            if (enemy.position.x <= rightEdge.position.x)
                MoveInDirection(1);
            else
                DirectionChange();
        }
    }

    private void DirectionChange()
    {

        // Setzt den "Moving"-Parameter im Animator auf false und startet den Stillstand-Timer
        anim.SetBool("moving", false);
        idleTimer += Time.deltaTime;


        // Wechselt die Bewegungsrichtung, wenn der Stillstand-Timer die Dauer überschreitet
        if (idleTimer > idleDuration)
            movingLeft = !movingLeft;
    }

    private void MoveInDirection(int _direction)
    {
        idleTimer = 0;  // Setzt den Stillstand-Timer zurück und setzt den "Moving"-Parameter im Animator auf true
        anim.SetBool("moving", true);

        //Dreht den Feind in die Bewegungsrichtung
        enemy.localScale = new Vector3(-Mathf.Abs(initScale.x) * _direction,
            initScale.y, initScale.z);

        // Bewegt den Feind in die angegebene Richtung
        enemy.position = new Vector3(enemy.position.x + Time.deltaTime * _direction * speed,
            enemy.position.y, enemy.position.z);
    }
}