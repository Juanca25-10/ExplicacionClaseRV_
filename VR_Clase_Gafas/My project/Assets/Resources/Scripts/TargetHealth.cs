using UnityEngine;

public class TargetHealth : MonoBehaviour
{
    [Header("Atributos")]
    public float maxHealth = 100f;
    private float currentHealth;

    void Start()
    {
        // Al iniciar, el objeto tiene la vida al máximo
        currentHealth = maxHealth;
    }

    // Método público para que las armas o las balas puedan llamarlo
    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        Debug.Log($"[{gameObject.name}] recibió {amount} de daño. Vida: {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        // Más adelante, aquí puedes instanciar un prefab de partículas 
        // de cristales rojos rompiéndose para el efecto SUPERHOT real.
        Debug.Log($"[{gameObject.name}] ha sido destruido.");
        Destroy(gameObject);
    }
}