using UnityEngine;
using UnityEngine.UI;

public class BlackMarketUIManager : MonoBehaviour
{
    [Header("Onglets et Pages")]
    public Button[] tabs;         // Boutons des onglets (� assigner dans l'inspecteur)
    public GameObject[] pages;    // Pages correspondantes (� assigner dans l'inspecteur)

    private void Start()
    {
        // V�rifie la validit� des onglets et pages
        if (tabs.Length != 3 || pages.Length != 3)
        {
            Debug.LogError("Assurez-vous d'avoir exactement 3 onglets et 3 pages assign�s !");
            return;
        }

        // Ajoute les �v�nements pour chaque onglet
        for (int i = 0; i < tabs.Length; i++)
        {
            int pageIndex = i; // Capture l'index pour l'�v�nement
            tabs[i].onClick.AddListener(() => ShowPage(pageIndex));
        }

        // Active la premi�re page au d�marrage
        ShowPage(0);
    }

    private void ShowPage(int pageIndex)
    {
        if (pageIndex < 0 || pageIndex >= pages.Length)
        {
            Debug.LogWarning($"Index de page invalide : {pageIndex}");
            return;
        }

        // D�sactiver toutes les pages
        foreach (GameObject page in pages)
        {
            page.SetActive(false);
        }

        // Activer la page s�lectionn�e
        pages[pageIndex].SetActive(true);
        Debug.Log($"Page {pageIndex + 1} activ�e.");
    }
}
