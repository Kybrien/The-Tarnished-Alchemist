using UnityEngine;

public class HoverWorldSpace : MonoBehaviour
{
    public GameObject hoverCanvas; // Référence au Canvas à afficher
    public Camera mainCamera; // Référence à la caméra principale
    public float maxRange = 8f; // Portée maximale du raycast

    private void Start()
    {
        if (hoverCanvas != null)
        {
            hoverCanvas.SetActive(false); // Assure que le Canvas est désactivé au départ
        }

    }

    private void Update()
    {
        HandleHoverInteraction();
    }

    void HandleHoverInteraction()
    {
        // Vérifie que la caméra est assignée
        if (mainCamera == null)
        {
            Debug.LogError("HoverWorldSpace: La caméra principale n'est pas définie.");
            return;
        }

        // Crée un raycast depuis la position de la souris
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, maxRange))
        {

            // Vérifie si le raycast touche le collider de l'objet actuel
            if (hit.collider != null && hit.collider.gameObject == gameObject)
            {
                if (hoverCanvas != null && !hoverCanvas.activeSelf)
                {
                    hoverCanvas.SetActive(true); // Active le Canvas
                }
                return; // Empêche de désactiver tant que le raycast est actif
            }
        }

        // Désactive le Canvas si le raycast ne touche plus
        if (hoverCanvas != null && hoverCanvas.activeSelf)
        {
            hoverCanvas.SetActive(false);
        }
    }
}
