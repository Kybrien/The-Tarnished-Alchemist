using UnityEngine;
using TMPro;

public class PageManager : MonoBehaviour
{
    [Header("Pages et Navigation")]
    public GameObject[] pages; // Liste des pages (Canvas)
    public int currentIndex = 0; // Index de la page actuelle
    public Camera mainCamera; // Caméra pour le raycast

    public GameObject buttonPrevious;
    public GameObject buttonNext;
    public GameObject buttonMainMenu;

    public GameObject eraseObject; // GameObject pour l'animation d'effacement
    public float animationDuration = 1f; // Durée de l'animation avant le changement de page

    private int targetIndex = -1; // L'index de la page à atteindre après l'animation
    private bool isTransitioning = false; // Indique si une transition est en cours

    [Header("Gestion des Recettes")]
    public GameObject addRecipeButton;         // Bouton Ajouter Une Recette
    public GameObject validationButton;       // Bouton Valider
    public TMP_InputField inputTitle;         // Champ pour entrer le titre
    public GameObject recipeInputPanel;       // Panel contenant les champs d'entrée
    public TextMeshProUGUI[] recipeSlots;     // Liste des slots pour afficher les titres des recettes

    private void Start()
    {
        UpdatePage();
        //recipeInputPanel.SetActive(true); // Désactiver le panel d'entrée au départ
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

        if (isTransitioning) return; // Ignore les clics pendant la transition

        // Crée un raycast depuis la position de la souris
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
            targetIndex = newIndex; // Définit la page cible
            eraseObject.SetActive(true); // Active le GameObject pour lancer l'animation
            Invoke(nameof(CompleteTransition), animationDuration); // Attend la fin de l'animation
        }
        else
        {
            Debug.LogError("PageManager: EraseObject non assigné.");
        }
    }

    void CompleteTransition()
    {
        eraseObject.SetActive(false); // Désactive le GameObject "Erase"
        currentIndex = targetIndex; // Change l'index de la page
        UpdatePage(); // Met à jour la page
        isTransitioning = false; // Fin de la transition
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

    // --- Ajout de la gestion des recettes ---
    public void OnAddRecipeButtonClicked()
    {
        Debug.Log("Bouton Ajouter Une Recette cliqué.");

        // Vérifie si le panel et le bouton sont correctement assignés
        if (addRecipeButton == null || recipeInputPanel == null)
        {
            Debug.LogError("addRecipeButton ou recipeInputPanel n'est pas assigné !");
            return;
        }

        // Désactiver le bouton Ajouter Une Recette et activer les champs d'entrée
        addRecipeButton.SetActive(false);
        recipeInputPanel.SetActive(true);

        Debug.Log("Panel d'entrée activé.");
    }


    public void OnValidationButtonClicked()
    {
        // Récupérer les valeurs des champs d'entrée
        string title = inputTitle.text;

        if (string.IsNullOrEmpty(title))
        {
            Debug.LogWarning("Le titre est vide.");
            return;
        }

        // Remplacer la première slot vide disponible
        bool slotFound = false;
        for (int i = 0; i < recipeSlots.Length; i++)
        {
            if (string.IsNullOrEmpty(recipeSlots[i].text))
            {
                recipeSlots[i].text = title; // Met à jour le titre
                slotFound = true;
                break;
            }
        }

        if (!slotFound)
        {
            Debug.LogWarning("Toutes les slots sont déjà remplies.");
        }

        // Réinitialiser les champs d'entrée
        inputTitle.text = "";

        // Désactiver le panel d'entrée et réactiver le bouton Ajouter Une Recette
        recipeInputPanel.SetActive(false);
        addRecipeButton.SetActive(true);
    }
}
