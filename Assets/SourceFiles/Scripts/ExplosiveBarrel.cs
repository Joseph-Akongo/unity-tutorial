using UnityEngine;
using UnityEngine.VFX; // Required for VFX Graph

public class ExplosiveBarrel : MonoBehaviour
{
    [Header("Particle Effects")]
    [Tooltip("Assign your Built-in Particle System prefab here")]
    public GameObject builtInExplosionPrefab;

    [Tooltip("Assign your VFX Graph prefab here")]
    public VisualEffect vfxExplosion;

    [Header("Explosion Settings")]
    [Tooltip("Force applied to the player on explosion")]
    public float explosionForce = 500f;

    [Tooltip("Radius of the explosion force")]
    public float explosionRadius = 5f;

    [Tooltip("Upward force modifier for explosion")]
    public float upwardModifier = 1f;

    [Header("Audio")]
    public AudioClip explosionSound;

    [Header("Settings")]
    [Tooltip("Delay before destroying the barrel GameObject after explosion (seconds)")]
    public float destroyDelay = 2f;

    private bool hasExploded = false;

    private void OnCollisionEnter(Collision collision)
    {
        // Only trigger if it hasn't exploded yet and the colliding object is the Player
        if (!hasExploded && collision.gameObject.CompareTag("Player"))
        {
            Explode(collision.gameObject);
        }
    }

    private void Explode(GameObject player)
    {
        hasExploded = true;

        if (builtInExplosionPrefab != null)
        {
            GameObject ps = Instantiate(builtInExplosionPrefab, transform.position, Quaternion.identity);
            // Auto-destroy the particle effect after it finishes
            ParticleSystem particleSystem = ps.GetComponent<ParticleSystem>();
            if (particleSystem != null)
            {
                Destroy(ps, particleSystem.main.duration + particleSystem.main.startLifetime.constantMax);
            }
            else
            {
                Destroy(ps, 3f); // Fallback destroy time
            }
        }

        if (vfxExplosion != null)
        {
            vfxExplosion.transform.SetParent(null); // Detach so it persists after barrel is destroyed
            vfxExplosion.Play();
            Destroy(vfxExplosion.gameObject, destroyDelay + 1f);
        }

        if (explosionSound != null)
        {
            AudioSource.PlayClipAtPoint(explosionSound, transform.position);
        }

        Rigidbody playerRb = player.GetComponent<Rigidbody>();
        if (playerRb != null)
        {
            playerRb.AddExplosionForce(explosionForce, transform.position, explosionRadius, upwardModifier, ForceMode.Impulse);
        }

        foreach (Renderer r in GetComponentsInChildren<Renderer>())
        {
            r.enabled = false;
        }

        Destroy(gameObject, destroyDelay);
    }

    // Visualise explosion radius in the Scene view (editor only)
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 0.3f, 0f, 0.3f);
        Gizmos.DrawSphere(transform.position, explosionRadius);
    }
}
