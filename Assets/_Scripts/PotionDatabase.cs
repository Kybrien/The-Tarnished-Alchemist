using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PotionCombination
{
    public List<string> ingredients; // Liste des ingrédients (2 ou 3 noms de potions)
    public string resultName;        // Nom du résultat
    public GameObject resultPrefab;  // Prefab du résultat
}

public class PotionDatabase : MonoBehaviour
{
    [Header("Liste des combinaisons de potions")]
    public List<PotionCombination> combinations; // Liste des combinaisons définies dans l'inspecteur

    // Trouve le résultat pour une combinaison donnée
    public bool GetCombinationResult(List<string> inputPotions, out string resultName, out GameObject resultPrefab)
    {
        resultName = null;
        resultPrefab = null;

        // Trie les ingrédients pour éviter de dépendre de l'ordre
        inputPotions.Sort();

        foreach (var combination in combinations)
        {
            // Vérifie si la combinaison correspond (en ignorant l'ordre)
            List<string> sortedIngredients = new List<string>(combination.ingredients);
            sortedIngredients.Sort();

            if (sortedIngredients.Count == inputPotions.Count && sortedIngredients.TrueForAll(inputPotions.Contains))
            {
                resultName = combination.resultName;
                resultPrefab = combination.resultPrefab;
                return true; // Combinaison trouvée
            }
        }

        Debug.LogWarning("Aucune combinaison trouvée pour les ingrédients : " + string.Join(", ", inputPotions));
        return false; // Pas de combinaison trouvée
    }
}
