using UnityEngine;
using TMPro;

public class PageManager : MonoBehaviour
{
    [Header("Pages et Navigation")]
    public GameObject[] pages; // Liste des pages (Canvas)
    public int currentIndex = 0; // Index de la page actuelle
    public Camera mainCamera; // Cam�ra pour le raycast

    public GameObject buttonPrevious;
    public GameObject buttonNext;
    public GameObject buttonMainMenu;

    public GameObject eraseObject; // GameObject pour l'animation d'effacement
    public float animationDuration = 1f; // Dur�e de l'animation avant le changement de page

    private int targetIndex = -1; // L'index de la page � atteindre apr�s l'animation
    private bool isTransitioning = false; // Indique si une transition est en cours

    [Header("Gestion des Recettes")]
    public GameObject addRecipeButton;         // Bouton Ajouter Une Recette
    public GameObject validationButton;       // Bouton Valider
    public TMP_InputField inputTitle;         // Champ pour entrer le titre
    public GameObject recipeInputPanel;       // Panel contenant les champs d'entr�e
    public TextMeshProUGUI[] recipeSlots;     // Liste des slots pour afficher les titres des recettes

    private void Start()
    {
        UpdatePage();
        //recipeInputPanel.SetActive(true); // D�sactiver le panel d'entr�e au d�part
    }

    private void Update()
    {
        HandleButtonInteraction();
    }

    void HandleButtonInteraction()
    {
        if (mainCamera == null)
        {
            Debug.LogError("PageManager: Cam�ra non assign�e.");
            return;
        }

        if (isTransitioning) return; // Ignore les clics pendant la transition

        // Cr�e un raycast depuis la position de la souris
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (Input.GetMouseButtonDown(0)) // Clic gauche
            {
                if (hit.collider.gameObject == buttonPrevious && currentIndex > 0)
                {
                    StartPageTransition(currentIndex - 1);
                }
                else if (hit.collider.gameObject == buttonNext && currentIndex < pages.Length - 1)
                {
                    StartPageTransition(currentIndex + 1);
                }
                else if (hit.collider.gameObject == buttonMainMenu)
                {
                    StartPageTransition(0);
                }
            }
        }
    }

    void StartPageTransition(int newIndex)
    {
        if (eraseObject != null)
        {
            isTransitioning = true; // Une transition est en cours
            targetIndex = newIndex; // D�finit la page cible
            eraseObject.SetActive(true); // Active le GameObject pour lancer l'animation
            Invoke(nameof(CompleteTransition), animationDuration); // Attend la fin de l'animation
        }
        else
        {
            Debug.LogError("PageManager: EraseObject non assign�.");
        }
    }

    void CompleteTransition()
    {
        eraseObject.SetActive(false); // D�sactive le GameObject "Erase"
        currentIndex = targetIndex; // Change l'index de la page
        UpdatePage(); // Met � jour la page
        isTransitioning = false; // Fin de la transition
    }

    public void UpdatePage()
    {
        // D�sactive toutes les pages
        foreach (GameObject page in pages)
        {
            page.SetActive(false);
        }

        // Active la page actuelle
        if (pages.Length > 0 && currentIndex >= 0 && currentIndex < pages.Length)
        {
            pages[currentIndex].SetActive(true);
        }

        // G�re l'�tat des boutons
        buttonPrevious.SetActive(currentIndex > 0);
        buttonNext.SetActive(currentIndex < pages.Length - 1);
    }

    // --- Ajout de la gestion des recettes ---
    public void OnAddRecipeButtonClicked()
    {
        Debug.Log("Bouton Ajouter Une Recette cliqu�.");

        // V�rifie si le panel et le bouton sont correctement assign�s
        if (addRecipeButton == null || recipeInputPanel == null)
        {
            Debug.LogError("addRecipeButton ou recipeInputPanel n'est pas assign� !");
            return;
        }

        // D�sactiver le bouton Ajouter Une Recette et activer les champs d'entr�e
        addRecipeButton.SetActive(false);
        recipeInputPanel.SetActive(true);

        Debug.Log("Panel d'entr�e activ�.");
    }


    public void OnValidationButtonClicked()
    {
        // R�cup�rer les valeurs des champs d'entr�e
        string title = inputTitle.text;

        if (string.IsNullOrEmpty(title))
        {
            Debug.LogWarning("Le titre est vide.");
            return;
        }

        // Remplacer la premi�re slot vide disponible
        bool slotFound = false;
        for (int i = 0; i < recipeSlots.Length; i++)
        {
            if (string.IsNullOrEmpty(recipeSlots[i].text))
            {
                recipeSlots[i].text = title; // Met � jour le titre
                slotFound = true;
                break;
            }
        }

        if (!slotFound)
        {
            Debug.LogWarning("Toutes les slots sont d�j� remplies.");
        }

        // R�initialiser les champs d'entr�e
        inputTitle.text = "";

        // D�sactiver le panel d'entr�e et r�activer le bouton Ajouter Une Recette
        recipeInputPanel.SetActive(false);
        addRecipeButton.SetActive(true);
    }
}
