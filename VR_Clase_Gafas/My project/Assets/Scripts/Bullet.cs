using UnityEngine;

/// <summary>
/// Bullet - Va en el Prefab de la bala.
/// Se mueve usando Time.deltaTime → automáticamente se ve afectada por el SuperHotTimeManager.
/// Cuando el jugador no se mueve: bala lenta. Cuando se mueve: bala rápida.
/// </summary>
public class Bullet : MonoBehaviour
{
    // Estos valores los setea GunShooter al instanciar
    private Vector3 direction;
    private float speed;
    private float lifetime;
    private bool initialized = false;

    // Para efectos visuales opcionales
    private TrailRenderer trailRenderer;

    void Awake()
    {
        trailRenderer = GetComponent<TrailRenderer>();
    }

    /// <summary>
    /// Llamado por GunShooter inmediatamente después de Instantiate.
    /// </summary>
    public void Initialize(Vector3 dir, float spd, float life)
    {
        direction = dir.normalized;
        speed = spd;
        lifetime = life;
        initialized = true;

        // Orientar la bala hacia donde va
        transform.forward = direction;

        // Destruirse sola después del lifetime
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        if (!initialized) return;

        // Time.deltaTime ya está escalado por SuperHotTimeManager
        // → automáticamente va lento o rápido según el movimiento del jugador
        transform.Translate(Vector3.forward * speed * Time.deltaTime, Space.Self);
    }

    void OnCollisionEnter(Collision collision)
    {
        // Ignorar colisión con el arma que la disparó (por si el collider se superpone)
        if (collision.gameObject.CompareTag("Weapon")) return;

        // Acá podés agregar:
        // - Partículas de impacto
        // - Daño al enemigo: collision.gameObject.GetComponent<Enemy>()?.TakeDamage();
        // - Sonido de impacto

        Destroy(gameObject);
    }

    void OnTriggerEnter(Collider other)
    {
        // Por si usás Is Trigger en vez de colisión física
        if (other.CompareTag("Weapon")) return;

        Destroy(gameObject);
    }
}