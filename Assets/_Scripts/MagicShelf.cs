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

    private int numberOfTabs;            // Nombre d'onglets g�n�r� al�atoirement
    private int[] itemsPerTab;           // Tableau pour stocker le nombre d'items par onglet
    private GameObject[] tabs;           // R�f�rences aux onglets
    private GameObject[][] items;        // R�f�rences aux items par onglet

    private void OnEnable()
    {
        // Appel� � chaque ouverture du Canvas
        GenerateRandomTabsAndItems();
    }

    private void GenerateRandomTabsAndItems()
    {
        // D�termine al�atoirement le nombre d'onglets
        numberOfTabs = Random.Range(minTabs, maxTabs + 1);
        itemsPerTab = new int[numberOfTabs];

        // D�termine al�atoirement le nombre d'items pour chaque onglet
        for (int i = 0; i < numberOfTabs; i++)
        {
            itemsPerTab[i] = Random.Range(minItemsPerTab, maxItemsPerTab + 1);
        }

        // Reg�n�re les onglets et les items
        GenerateTabs();
    }

    private void GenerateTabs()
    {
        // Supprime les anciens onglets et items si n�cessaire
        ClearOldTabsAndItems();

        tabs = new GameObject[numberOfTabs];
        items = new GameObject[numberOfTabs][];

        // Le panel est divis� en 4 parties �gales
        float totalPanelWidth = tabsPanel.rect.width;
        float tabPanelWidthPerQuarter = totalPanelWidth / 4; // Chaque quart du panel
        float tabHeight = tabsPanel.rect.height;

        for (int i = 0; i < numberOfTabs; i++)
        {
            // Cr�er un nouvel onglet
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

            // Ajouter un �v�nement pour afficher les items
            int index = i;
            tab.GetComponent<Button>().onClick.AddListener(() => ShowItems(index));
            tabs[i] = tab;

            // G�n�rer les items pour cet onglet
            GenerateItemsForTab(i);
        }
    }

    private void GenerateItemsForTab(int tabIndex)
    {
        int itemCount = itemsPerTab[tabIndex]; // R�cup�re le nombre al�atoire d'items pour cet onglet
        items[tabIndex] = new GameObject[itemCount];

        // Calcul dynamique de la taille des items
        int itemsPerRow = Mathf.CeilToInt(Mathf.Sqrt(itemCount));
        float itemWidth = itemsPanel.rect.width / itemsPerRow;
        float itemHeight = itemsPanel.rect.height / itemsPerRow;

        for (int i = 0; i < itemCount; i++)
        {
            // Cr�er un nouvel item
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

            // Activer al�atoirement un enfant
            ActivateRandomChild(item);

            items[tabIndex][i] = item;
            item.SetActive(false); // Cache tous les items au d�but
        }
    }

    private void ActivateRandomChild(GameObject item)
    {
        int childCount = item.transform.childCount;
        if (childCount > 0)
        {
            // D�sactiver tous les enfants
            foreach (Transform child in item.transform)
            {
                child.gameObject.SetActive(false);
            }

            // Activer un enfant al�atoirement
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

        // Afficher les items de l'onglet s�lectionn�
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
