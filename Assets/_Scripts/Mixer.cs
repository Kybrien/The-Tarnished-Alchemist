using UnityEngine;

public class Mixer : MonoBehaviour
{
    public Transform mixPoint; // Point où les objets seront positionnés pour être mélangés
    public GameObject concoctionPrefab; // Préfab de la concoction résultante

    public void Mix(GameObject elementPrimary, GameObject elementSecondary, Transform handPos)
    {
        // Vérifier que les deux éléments sont valides
        if (elementPrimary == null || elementSecondary == null)
        {
            Debug.LogWarning("Vous devez avoir un élément primaire et un élément secondaire pour mélanger.");
            return;
        }

        // Supprimer les ingrédients
        Destroy(elementPrimary);
        Destroy(elementSecondary);

        // Créer la concoction
        GameObject concoction = Instantiate(concoctionPrefab, mixPoint.position, Quaternion.identity);

        // Placer la concoction dans la main gauche
        concoction.transform.SetParent(handPos);
        concoction.transform.localPosition = Vector3.zero;
        concoction.transform.localRotation = Quaternion.identity;

        // Ajout de logique supplémentaire si nécessaire
        Debug.Log("Les ingrédients ont été mélangés avec succès !");
    }
}
