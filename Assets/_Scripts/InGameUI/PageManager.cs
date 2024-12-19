using UnityEngine;

public class PageManager : MonoBehaviour
{
    public GameObject[] pages; // Liste des pages (Canvas)
    public int currentIndex = 0; // Index de la page actuelle
    public Camera mainCamera; // Caméra pour le raycast

    public GameObject buttonPrevious;
    public GameObject buttonNext;
    public GameObject buttonMainMenu;

    private void Start()
    {
        UpdatePage();
    }

    private void Update()
    {
        HandleButtonInteraction();
    }

    void HandleButtonInteraction()
    {
        if (mainCamera == null)
        {
            Debug.LogError("PageManager: Caméra non assignée.");
            return;
        }

        // Crée un raycast depuis la position de la souris
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (Input.GetMouseButtonDown(0)) // Clic gauche
            {
                if (hit.collider.gameObject == buttonPrevious && currentIndex > 0)
                {
                    currentIndex--;
                    UpdatePage();
                }
                else if (hit.collider.gameObject == buttonNext && currentIndex < pages.Length - 1)
                {
                    currentIndex++;
                    UpdatePage();
                }
                else if (hit.collider.gameObject == buttonMainMenu)
                {
                    currentIndex = 0;
                    UpdatePage();
                }
            }
        }
    }

    public void UpdatePage()
    {
        // Désactive toutes les pages
        foreach (GameObject page in pages)
        {
            page.SetActive(false);
        }

        // Active la page actuelle
        if (pages.Length > 0 && currentIndex >= 0 && currentIndex < pages.Length)
        {
            pages[currentIndex].SetActive(true);
        }

        // Gère l'état des boutons
        buttonPrevious.SetActive(currentIndex > 0);
        buttonNext.SetActive(currentIndex < pages.Length - 1);
    }
}
