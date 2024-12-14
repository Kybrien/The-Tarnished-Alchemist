using UnityEngine;

public class PotionExplosion : MonoBehaviour
{
    public GameObject explosionEffect; // Effet visuel de l'explosion
    public float explosionRadius = 5f; // Rayon d'effet de l'explosion
    public float explosionForce = 500f; // Force appliquée aux objets à proximité
    public LayerMask explosionLayers; // Couches affectées par l'explosion

    private void OnCollisionEnter(Collision collision)
    {
        // Créer l'effet d'explosion
        if (explosionEffect != null)
        {
            Instantiate(explosionEffect, transform.position, Quaternion.identity);
        }

        // Appliquer une force aux objets dans le rayon
        Collider[] objects = Physics.OverlapSphere(transform.position, explosionRadius, explosionLayers);
        foreach (Collider obj in objects)
        {
            Rigidbody rb = obj.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddExplosionForce(explosionForce, transform.position, explosionRadius);
            }
        }

        // Détruire la potion après l'explosion
        Destroy(gameObject);
    }
}
