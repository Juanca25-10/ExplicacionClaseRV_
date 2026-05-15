using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Bullet : MonoBehaviour
{
    [Header("Configuraci�n de la Bala")]
    public float speed = 20f;
    public float damage = 50f;
    public float lifeTime = 5f; // Tiempo antes de desaparecer si no choca con nada

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // En SUPERHOT las balas viajan en l�nea recta, no caen por la gravedad
        rb.useGravity = false;

        // Le damos el impulso constante hacia el frente desde el momento en que nace
        rb.linearVelocity = transform.forward * speed;

        // Medida de seguridad: destruimos la bala tras unos segundos para no llenar la memoria
        Destroy(gameObject, lifeTime);
    }

    void OnCollisionEnter(Collision collision)
    {
        // Buscamos si el objeto con el que chocamos tiene el script TargetHealth
        TargetHealth target = collision.gameObject.GetComponent<TargetHealth>();

        if (target != null)
        {
            // Si lo tiene, le aplicamos el da�o
            target.TakeDamage(damage);
        }

        // Independientemente de si chocamos con un enemigo o una pared, la bala se destruye
        Destroy(gameObject);
    }
}