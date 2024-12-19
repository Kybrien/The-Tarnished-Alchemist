using UnityEngine;

public class PageButtonSimple : MonoBehaviour
{
    public int targetPageIndex; // Index de la page cible
    public Camera mainCamera; // Caméra pour le raycast

    private void Update()
    {
        HandleButtonClick();
    }

    void HandleButtonClick()
    {
        if (mainCamera == null)
        {
            Debug.LogError("PageButtonSimple: Caméra principale non définie.");
            return;
        }

        // Crée un raycast depuis la position de la souris
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (Input.GetMouseButtonDown(0)) // Clic gauche
            {
                if (hit.collider != null && hit.collider.gameObject == gameObject)
                {
                    NavigateToPage();
                }
            }
        }
    }

    void NavigateToPage()
    {
        // Trouve le PageManager dans la scène
        PageManager pageManager = FindObjectOfType<PageManager>();

        if (pageManager != null)
        {
            // Définit l'index cible et met à jour la page
            pageManager.currentIndex = targetPageIndex;
            pageManager.UpdatePage();
        }
        else
        {
            Debug.LogError("PageButtonSimple: Aucun PageManager trouvé dans la scène.");
        }
    }
}
