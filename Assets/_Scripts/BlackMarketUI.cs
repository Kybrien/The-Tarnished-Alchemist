using UnityEngine;
using UnityEngine.UI;

public class BlackMarketUIManager : MonoBehaviour
{
    [Header("Onglets et Pages")]
    public Button[] tabs;         // Boutons des onglets (à assigner dans l'inspecteur)
    public GameObject[] pages;    // Pages correspondantes (à assigner dans l'inspecteur)

    private void Start()
    {
        // Vérifie la validité des onglets et pages
        if (tabs.Length != 3 || pages.Length != 3)
        {
            Debug.LogError("Assurez-vous d'avoir exactement 3 onglets et 3 pages assignés !");
            return;
        }

        // Ajoute les événements pour chaque onglet
        for (int i = 0; i < tabs.Length; i++)
        {
            int pageIndex = i; // Capture l'index pour l'événement
            tabs[i].onClick.AddListener(() => ShowPage(pageIndex));
        }

        // Active la première page au démarrage
        ShowPage(0);
    }

    private void ShowPage(int pageIndex)
    {
        if (pageIndex < 0 || pageIndex >= pages.Length)
        {
            Debug.LogWarning($"Index de page invalide : {pageIndex}");
            return;
        }

        // Désactiver toutes les pages
        foreach (GameObject page in pages)
        {
            page.SetActive(false);
        }

        // Activer la page sélectionnée
        pages[pageIndex].SetActive(true);
        Debug.Log($"Page {pageIndex + 1} activée.");
    }
}
