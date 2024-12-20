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
    public int numberOfTabs = 3;         // Nombre d'onglets
    public int itemsPerTab = 10;         // Nombre d'items par onglet

    private GameObject[] tabs;           // Références aux onglets
    private GameObject[][] items;        // Références aux items par onglet

    private void Start()
    {
        GenerateTabs();
    }

    private void GenerateTabs()
    {
        // Initialisation des structures
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

                // Désactiver l'auto-sizing pour une taille fixe
                text.enableAutoSizing = false;
                text.fontSize = Mathf.Min(tabHeight / 2, 24); // Ajuste la taille du texte à la hauteur du tab
                text.alignment = TextAlignmentOptions.Center; // Centre le texte horizontalement et verticalement

                // Configuration du RectTransform du texte
                RectTransform textRect = text.GetComponent<RectTransform>();
                textRect.anchorMin = new Vector2(0, 0);
                textRect.anchorMax = new Vector2(1, 1);
                textRect.pivot = new Vector2(0.5f, 0.5f); // Centre le pivot
                textRect.offsetMin = new Vector2(5, 5);  // Padding intérieur
                textRect.offsetMax = new Vector2(-5, -5); // Padding intérieur
            }

            // Ajoute un bouton pour afficher les items
            int index = i;
            tab.GetComponent<Button>().onClick.AddListener(() => ShowItems(index));
            tabs[i] = tab;

            // Générer les items pour cet onglet
            GenerateItemsForTab(i);
        }
    }




    private void GenerateItemsForTab(int tabIndex)
    {
        items[tabIndex] = new GameObject[itemsPerTab];

        // Calcul dynamique de la taille des items
        int itemsPerRow = Mathf.CeilToInt(Mathf.Sqrt(itemsPerTab));
        float itemWidth = itemsPanel.rect.width / itemsPerRow;
        float itemHeight = itemsPanel.rect.height / itemsPerRow;

        for (int i = 0; i < itemsPerTab; i++)
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

            // Configurer le texte
            item.GetComponentInChildren<TextMeshProUGUI>().text = $"Item {i + 1 + tabIndex * itemsPerTab}";

            items[tabIndex][i] = item;
            item.SetActive(false); // Cache tous les items au début
        }
    }

    private void ShowItems(int tabIndex)
    {
        // Masquer les items des autres onglets
        for (int i = 0; i < numberOfTabs; i++)
        {
            foreach (var item in items[i])
            {
                item.SetActive(false);
            }
        }

        // Afficher les items de l'onglet sélectionné
        foreach (var item in items[tabIndex])
        {
            item.SetActive(true);
        }
    }
}
