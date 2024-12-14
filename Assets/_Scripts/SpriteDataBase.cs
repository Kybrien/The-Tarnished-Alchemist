using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameObjectDatabase", menuName = "Inventory/GameObject Database")]
public class GameObjectDatabase : ScriptableObject
{
    [System.Serializable]
    public class GameObjectEntry
    {
        public string objectName; // Nom de l'objet
        public GameObject gameObjectPrefab; // GameObject associé (préfab avec anims, etc.)
    }

    public List<GameObjectEntry> entries = new List<GameObjectEntry>();

    // Méthode pour récupérer un GameObject par nom
    public GameObject GetGameObjectByName(string name)
    {
        foreach (var entry in entries)
        {
            if (entry.objectName == name)
            {
                return entry.gameObjectPrefab;
            }
        }
        return null; // Retourne null si aucun GameObject n'est trouvé
    }
}
