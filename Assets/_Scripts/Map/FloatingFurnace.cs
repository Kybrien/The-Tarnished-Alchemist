using UnityEngine;

public class FloatingObject : MonoBehaviour
{
    public float floatAmplitude = 0.5f; // Distance maximale entre les intervalles de Y
    public float floatSpeed = 1f; // Vitesse du mouvement flottant

    private Vector3 startPosition;

    void Start()
    {
        // Sauvegarde de la position initiale de l'objet
        startPosition = transform.position;
    }

    void Update()
    {
        // Calcul du décalage en Y en utilisant une fonction sinus
        float newY = startPosition.y + Mathf.Sin(Time.time * floatSpeed) * floatAmplitude;
        transform.position = new Vector3(startPosition.x, newY, startPosition.z);
    }
}
