using UnityEngine;

public class Mixer : MonoBehaviour
{
    public Transform Pos1, Pos2, Pos3; // Emplacements disponibles
    public GameObject Pos1UI, Pos2UI, Pos3UI;
    private Transform[] availablePositions;

    void Start()
    {
        // Initialise les positions
        availablePositions = new Transform[] { Pos1, Pos2, Pos3 };
        Debug.Log("Mixer initialized with positions: " + Pos1.name + ", " + Pos2.name + ", " + Pos3.name);
    }

    // Trouve un emplacement libre
    public Transform GetAvailablePosition()
    {
        foreach (Transform pos in availablePositions)
        {
            //Debug.Log($"Checking position {pos.name} - Child count: {pos.childCount}");
            if (pos.childCount == 0) // Si l'emplacement est vide
            {
                //Debug.Log($"Position trouvée : {pos.name}");
                return pos;
            }
        }
        //Debug.LogWarning("Aucune position disponible !");
        return null;
    }

    // Place un objet sur une position donnée
    public void PlaceObject(GameObject obj)
    {

        if (IsFull()) // Vérifie si le conteneur est plein
        {
            Debug.LogWarning("Le conteneur est déjà plein. Placement annulé.");
            return; // Sort de la fonction sans rien faire
        }

        Transform targetPos = GetAvailablePosition();

        if (targetPos != null)
        {
            //Debug.Log($"Placement de l'objet {obj.name} sur {targetPos.name}");
            obj.transform.SetParent(targetPos);
            obj.transform.position = targetPos.position; // Place précisément l'objet
            Rigidbody rb = obj.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = true; // Désactive la physique
            }
            Debug.Log($"{obj.name} placé avec succès sur {targetPos.name}");
        }
        else
        {
            Debug.LogWarning("Échec du placement : aucune position disponible.");
        }
    }

    public bool IsFull()
    {
        foreach (Transform pos in availablePositions)
        {
            if (pos.childCount == 0) // Si une position est encore vide
                return false;
        }
        return true; // Toutes les positions sont occupées
    }
}
