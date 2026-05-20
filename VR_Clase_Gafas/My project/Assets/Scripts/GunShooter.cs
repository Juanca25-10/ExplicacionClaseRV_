using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

/// <summary>
/// GunShooter - Va en el GameObject "Pistola"
/// Se conecta automáticamente al evento Activated del XR Grab Interactable.
/// El trigger del controller = disparar.
/// </summary>
public class GunShooter : MonoBehaviour
{
    [Header("Referencias")]
    [Tooltip("El GameObject vacío en la punta del cañón (hijo de la Pistola)")]
    public Transform muzzlePoint;

    [Tooltip("Prefab de la bala (lo vas a crear como una esfera con Trail Renderer)")]
    public GameObject bulletPrefab;

    [Header("Configuración de Disparo")]
    [Tooltip("Velocidad de la bala en unidades/segundo (a timeScale=1)")]
    public float bulletSpeed = 20f;

    [Tooltip("Tiempo entre disparos en segundos (usa unscaledTime para no verse afectado por slow-mo)")]
    public float fireRate = 0.3f;

    [Tooltip("Cuántos segundos vive la bala antes de destruirse")]
    public float bulletLifetime = 5f;

    [Header("Efecto Visual")]
    [Tooltip("Prefab del efecto BANG (opcional)")]
    public GameObject bangEffectPrefab;

    // Privados
    private XRGrabInteractable grabInteractable;
    private float lastFireTime = -999f;
    private bool isGrabbed = false;

    void Awake()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();

        if (grabInteractable == null)
        {
            Debug.LogError("[GunShooter] No se encontró XRGrabInteractable en la Pistola.");
            return;
        }

        // Conectar eventos por código — no necesitás tocar el Inspector
        grabInteractable.activated.AddListener(OnTriggerPulled);
        grabInteractable.selectEntered.AddListener(OnGrabbed);
        grabInteractable.selectExited.AddListener(OnReleased);
    }

    void OnDestroy()
    {
        // Buena práctica: desuscribirse para evitar memory leaks
        if (grabInteractable != null)
        {
            grabInteractable.activated.RemoveListener(OnTriggerPulled);
            grabInteractable.selectEntered.RemoveListener(OnGrabbed);
            grabInteractable.selectExited.RemoveListener(OnReleased);
        }
    }

    private void OnGrabbed(SelectEnterEventArgs args)
    {
        isGrabbed = true;
    }

    private void OnReleased(SelectExitEventArgs args)
    {
        isGrabbed = false;
    }

    private void OnTriggerPulled(ActivateEventArgs args)
    {
        // Verificar que el arma está siendo sostenida
        if (!isGrabbed) return;

        // Verificar que tenemos todo configurado
        if (bulletPrefab == null)
        {
            Debug.LogWarning("[GunShooter] ¡Falta asignar el Bullet Prefab en el Inspector!");
            return;
        }

        if (muzzlePoint == null)
        {
            Debug.LogWarning("[GunShooter] ¡Falta asignar el MuzzlePoint en el Inspector!");
            return;
        }

        // Fire rate usando tiempo real (no escalado) para que el cooldown no se vea afectado por slow-mo
        // Así podés disparar incluso en cámara lenta sin esperar 6 segundos reales
        if (Time.unscaledTime - lastFireTime < fireRate) return;

        lastFireTime = Time.unscaledTime;
        Fire();
    }

    private void Fire()
    {
        // Instanciar la bala en la punta del cañón, apuntando hacia adelante
        GameObject bullet = Instantiate(bulletPrefab, muzzlePoint.position, muzzlePoint.rotation);

        // Pasarle los parámetros al script de la bala
        Bullet bulletScript = bullet.GetComponent<Bullet>();
        if (bulletScript != null)
        {
            bulletScript.Initialize(muzzlePoint.forward, bulletSpeed, bulletLifetime);
        }
        else
        {
            Debug.LogWarning("[GunShooter] El Bullet Prefab no tiene el script 'Bullet'.");
        }

        // Destruir la bala después del tiempo de vida (seguro de respaldo)
        Destroy(bullet, bulletLifetime);

        // Efecto BANG - aparece adelante del muzzle, siempre a velocidad real
        if (bangEffectPrefab != null)
        {
            Vector3 bangPos = muzzlePoint.position
                            + muzzlePoint.forward * 0.3f
                            + muzzlePoint.up * 0.05f
                            + muzzlePoint.right * 0.15f; // offset derecha — ajustá al gusto
            Instantiate(bangEffectPrefab, bangPos, Quaternion.identity);
        }
    }
}