using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Bullet : MonoBehaviour
{
    [Header("Configuración de la Bala")]
    public float speed = 20f;
    public float damage = 50f;
    public float lifeTime = 5f;

    [Header("Efectos")]
    [Tooltip("Arrastra aquí el componente Trail Renderer de la bala")]
    public TrailRenderer trail; // <--- Referencia al rastro

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.linearVelocity = transform.forward * speed;

        Destroy(gameObject, lifeTime);
    }

    void OnCollisionEnter(Collision collision)
    {
        TargetHealth target = collision.gameObject.GetComponent<TargetHealth>();

        if (target != null)
        {
            target.TakeDamage(damage);
        }

        // --- MAGIA DEL RASTRO ---
        if (trail != null)
        {
            // Desvinculamos el rastro de la bala para que no se destruya con ella
            trail.transform.parent = null;
            // Le decimos al rastro que se destruya solo cuando termine su tiempo de vida
            trail.autodestruct = true;
        }

        // Destruimos la bala
        Destroy(gameObject);
    }
}