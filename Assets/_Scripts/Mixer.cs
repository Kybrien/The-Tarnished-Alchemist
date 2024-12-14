using UnityEngine;

public class Mixer : MonoBehaviour
{
    public Transform mixPoint; // Point o� les objets seront positionn�s pour �tre m�lang�s
    public GameObject concoctionPrefab; // Pr�fab de la concoction r�sultante

    public void Mix(GameObject elementPrimary, GameObject elementSecondary, Transform handPos)
    {
        // V�rifier que les deux �l�ments sont valides
        if (elementPrimary == null || elementSecondary == null)
        {
            Debug.LogWarning("Vous devez avoir un �l�ment primaire et un �l�ment secondaire pour m�langer.");
            return;
        }

        // Supprimer les ingr�dients
        Destroy(elementPrimary);
        Destroy(elementSecondary);

        // Cr�er la concoction
        GameObject concoction = Instantiate(concoctionPrefab, mixPoint.position, Quaternion.identity);

        // Placer la concoction dans la main gauche
        concoction.transform.SetParent(handPos);
        concoction.transform.localPosition = Vector3.zero;
        concoction.transform.localRotation = Quaternion.identity;

        // Ajout de logique suppl�mentaire si n�cessaire
        Debug.Log("Les ingr�dients ont �t� m�lang�s avec succ�s !");
    }
}
