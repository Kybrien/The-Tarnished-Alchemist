using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PotionCombination
{
    public List<string> ingredients; // Liste des ingr�dients (2 ou 3 noms de potions)
    public string resultName;        // Nom du r�sultat
    public GameObject resultPrefab;  // Prefab du r�sultat
}

public class PotionDatabase : MonoBehaviour
{
    [Header("Liste des combinaisons de potions")]
    public List<PotionCombination> combinations; // Liste des combinaisons d�finies dans l'inspecteur

    // Trouve le r�sultat pour une combinaison donn�e
    public bool GetCombinationResult(List<string> inputPotions, out string resultName, out GameObject resultPrefab)
    {
        resultName = null;
        resultPrefab = null;

        // Trie les ingr�dients pour �viter de d�pendre de l'ordre
        inputPotions.Sort();

        foreach (var combination in combinations)
        {
            // V�rifie si la combinaison correspond (en ignorant l'ordre)
            List<string> sortedIngredients = new List<string>(combination.ingredients);
            sortedIngredients.Sort();

            if (sortedIngredients.Count == inputPotions.Count && sortedIngredients.TrueForAll(inputPotions.Contains))
            {
                resultName = combination.resultName;
                resultPrefab = combination.resultPrefab;
                return true; // Combinaison trouv�e
            }
        }

        Debug.LogWarning("Aucune combinaison trouv�e pour les ingr�dients : " + string.Join(", ", inputPotions));
        return false; // Pas de combinaison trouv�e
    }
}
