using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(Rigidbody))]
public class PhysicsDamage : MonoBehaviour
{
    public float damageAmount = 50f;
    [Tooltip("Velocidad m�nima requerida para hacer da�o")]
    public float minVelocityToDamage = 2f;

    private Rigidbody rb;
    private XRGrabInteractable interactable;
    private bool isGrabbed = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        interactable = GetComponent<XRGrabInteractable>();

        if (interactable != null)
        {
            interactable.selectEntered.AddListener(OnGrabbed);
            interactable.selectExited.AddListener(OnReleased);
        }
    }

    private void OnGrabbed(SelectEnterEventArgs args) { isGrabbed = true; }
    private void OnReleased(SelectExitEventArgs args) { isGrabbed = false; }

    void OnCollisionEnter(Collision collision)
    {
        // Verificamos si el arma va lo suficientemente r�pido para hacer da�o
        if (rb.linearVelocity.magnitude >= minVelocityToDamage)
        {
            // Aqu� puedes buscar el script de salud del enemigo
            // EnemyHealth enemy = collision.gameObject.GetComponent<EnemyHealth>();
            // if (enemy != null) enemy.TakeDamage(damageAmount);

            Debug.Log($"�Golpeaste a {collision.gameObject.name} con una fuerza de {rb.linearVelocity.magnitude}!");
        }
    }
}