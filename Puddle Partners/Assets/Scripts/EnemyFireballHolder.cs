using UnityEngine;

public class EnemyFireballHolder : MonoBehaviour
{
    [SerializeField] private Transform enemy; // Transform des Feindes, dessen Skalierung übernommen wird

    private void Update()
    {
        // Setzt die lokale Skalierung dieses Objekts (FireballHolder) auf die Skalierung des Feindes
        transform.localScale = enemy.localScale;
    }
}
