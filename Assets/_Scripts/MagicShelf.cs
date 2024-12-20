using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BlackHole : MonoBehaviour
{
    [Header("UI Panels")]
    public RectTransform tabsPanel;       // Panel contenant les onglets
    public RectTransform itemsPanel;     // Panel contenant les items d'un onglet

    [Header("Prefabs")]
    public GameObject tabPrefab;         // Prefab d'un onglet
    public GameObject itemPrefab;        // Prefab d'un item

    [Header("Configuration")]
    public int minTabs = 1;              // Minimum d'onglets
    public int maxTabs = 4;              // Maximum d'onglets
    public int minItemsPerTab = 3;       // Minimum d'items par onglet
    public int maxItemsPerTab = 12;      // Maximum d'items par onglet

    private int numberOfTabs;            // Nombre d'onglets généré aléatoirement
    private int[] itemsPerTab;           // Tableau pour stocker le nombre d'items par onglet
    private GameObject[] tabs;           // Références aux onglets
    private GameObject[][] items;        // Références aux items par onglet

    private void OnEnable()
    {
        // Appelé à chaque ouverture du Canvas
        GenerateRandomTabsAndItems();
    }

    private void GenerateRandomTabsAndItems()
    {
        // Détermine aléatoirement le nombre d'onglets
        numberOfTabs = Random.Range(minTabs, maxTabs + 1);
        itemsPerTab = new int[numberOfTabs];

        // Détermine aléatoirement le nombre d'items pour chaque onglet
        for (int i = 0; i < numberOfTabs; i++)
        {
            itemsPerTab[i] = Random.Range(minItemsPerTab, maxItemsPerTab + 1);
        }

        // Regénère les onglets et les items
        GenerateTabs();
    }

    private void GenerateTabs()
    {
        // Supprime les anciens onglets et items si nécessaire
        ClearOldTabsAndItems();

        tabs = new GameObject[numberOfTabs];
        items = new GameObject[numberOfTabs][];

        // Le panel est divisé en 4 parties égales
        float totalPanelWidth = tabsPanel.rect.width;
        float tabPanelWidthPerQuarter = totalPanelWidth / 4; // Chaque quart du panel
        float tabHeight = tabsPanel.rect.height;

        for (int i = 0; i < numberOfTabs; i++)
        {
            // Créer un nouvel onglet
            GameObject tab = Instantiate(tabPrefab, tabsPanel);
            RectTransform tabRect = tab.GetComponent<RectTransform>();

            // Ajuster la taille pour occuper 1/4 de l'espace total
            tabRect.sizeDelta = new Vector2(tabPanelWidthPerQuarter, tabHeight);

            // Positionner chaque onglet dans sa portion
            tabRect.anchoredPosition = new Vector2(i * tabPanelWidthPerQuarter, 0);
            tabRect.anchorMin = new Vector2(0, 0);
            tabRect.anchorMax = new Vector2(0, 1);
            tabRect.pivot = new Vector2(0, 0.5f);

            // Configurer le texte
            TextMeshProUGUI text = tab.GetComponentInChildren<TextMeshProUGUI>();
            if (text != null)
            {
                text.text = $"Tab {i + 1}";
            }

            // Ajouter un événement pour afficher les items
            int index = i;
            tab.GetComponent<Button>().onClick.AddListener(() => ShowItems(index));
            tabs[i] = tab;

            // Générer les items pour cet onglet
            GenerateItemsForTab(i);
        }
    }

    private void GenerateItemsForTab(int tabIndex)
    {
        int itemCount = itemsPerTab[tabIndex]; // Récupère le nombre aléatoire d'items pour cet onglet
        items[tabIndex] = new GameObject[itemCount];

        // Calcul dynamique de la taille des items
        int itemsPerRow = Mathf.CeilToInt(Mathf.Sqrt(itemCount));
        float itemWidth = itemsPanel.rect.width / itemsPerRow;
        float itemHeight = itemsPanel.rect.height / itemsPerRow;

        for (int i = 0; i < itemCount; i++)
        {
            // Créer un nouvel item
            GameObject item = Instantiate(itemPrefab, itemsPanel);
            RectTransform itemRect = item.GetComponent<RectTransform>();

            // Calculer la position de l'item
            int row = i / itemsPerRow;
            int col = i % itemsPerRow;
            itemRect.sizeDelta = new Vector2(itemWidth, itemHeight);
            itemRect.anchoredPosition = new Vector2(col * itemWidth, -row * itemHeight);
            itemRect.anchorMin = new Vector2(0, 1);
            itemRect.anchorMax = new Vector2(0, 1);
            itemRect.pivot = new Vector2(0, 1);

            // Activer aléatoirement un enfant
            ActivateRandomChild(item);

            items[tabIndex][i] = item;
            item.SetActive(false); // Cache tous les items au début
        }
    }

    private void ActivateRandomChild(GameObject item)
    {
        int childCount = item.transform.childCount;
        if (childCount > 0)
        {
            // Désactiver tous les enfants
            foreach (Transform child in item.transform)
            {
                child.gameObject.SetActive(false);
            }

            // Activer un enfant aléatoirement
            int randomIndex = Random.Range(0, childCount);
            item.transform.GetChild(randomIndex).gameObject.SetActive(true);
        }
    }

    private void ShowItems(int tabIndex)
    {
        // Masquer les items des autres onglets
        for (int i = 0; i < numberOfTabs; i++)
        {
            if (items[i] == null) continue;

            foreach (var item in items[i])
            {
                item.SetActive(false);
            }
        }

        // Afficher les items de l'onglet sélectionné
        if (items[tabIndex] != null)
        {
            foreach (var item in items[tabIndex])
            {
                item.SetActive(true);
            }
        }
    }

    private void ClearOldTabsAndItems()
    {
        // Supprime tous les anciens onglets
        if (tabs != null)
        {
            foreach (var tab in tabs)
            {
                Destroy(tab);
            }
        }

        // Supprime tous les anciens items
        if (items != null)
        {
            foreach (var itemArray in items)
            {
                if (itemArray == null) continue;
                foreach (var item in itemArray)
                {
                    Destroy(item);
                }
            }
        }
    }
}
