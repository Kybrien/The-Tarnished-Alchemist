using UnityEngine;

public class Mixer : MonoBehaviour
{
    public Transform Pos1, Pos2, Pos3; // Emplacements disponibles
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
                //Debug.Log($"Position trouv�e : {pos.name}");
                return pos;
            }
        }
        //Debug.LogWarning("Aucune position disponible !");
        return null;
    }

    // Place un objet sur une position donn�e
    public void PlaceObject(GameObject obj)
    {
        Transform targetPos = GetAvailablePosition();

        if (targetPos != null)
        {
            //Debug.Log($"Placement de l'objet {obj.name} sur {targetPos.name}");
            obj.transform.SetParent(targetPos);
            obj.transform.position = targetPos.position; // Place pr�cis�ment l'objet
            Rigidbody rb = obj.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = true; // D�sactive la physique
            }
            Debug.Log($"{obj.name} plac� avec succ�s sur {targetPos.name}");
        }
        else
        {
            Debug.LogWarning("�chec du placement : aucune position disponible.");
        }
    }
}
