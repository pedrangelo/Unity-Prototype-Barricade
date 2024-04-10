using UnityEngine;

public class DestructibleObject : MonoBehaviour
{
    public float maxHealth = 2000f;
    private float currentHealth;
    private Renderer objRenderer;

    void Start()
    {
        currentHealth = maxHealth;
        objRenderer = GetComponent<Renderer>();
        UpdateColor();
    }

    public void ApplyDamage(float force)
    {
        currentHealth -= force;
        if (currentHealth <= 0)
        {
            Destroy(gameObject); // Optionally, replace with your own method for handling object destruction.
        }
        UpdateColor();
    }

    void UpdateColor()
    {
        float healthRatio = currentHealth / maxHealth;
        objRenderer.material.color = Color.Lerp(Color.red, Color.green, healthRatio);
    }

    void OnCollisionEnter(Collision collision)
    {
        float force = collision.impulse.magnitude / Time.fixedDeltaTime;
        ApplyDamage(force);
    }

}
