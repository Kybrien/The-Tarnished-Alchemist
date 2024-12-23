using UnityEngine;

public class HoverWorldSpace : MonoBehaviour
{
    public GameObject hoverCanvas; // R�f�rence au Canvas � afficher
    public Camera mainCamera; // R�f�rence � la cam�ra principale
    public float maxRange = 8f; // Port�e maximale du raycast

    private void Start()
    {
        if (hoverCanvas != null)
        {
            hoverCanvas.SetActive(false); // Assure que le Canvas est d�sactiv� au d�part
        }

    }

    private void Update()
    {
        HandleHoverInteraction();
    }

    void HandleHoverInteraction()
    {
        // V�rifie que la cam�ra est assign�e
        if (mainCamera == null)
        {
            Debug.LogError("HoverWorldSpace: La cam�ra principale n'est pas d�finie.");
            return;
        }

        // Cr�e un raycast depuis la position de la souris
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, maxRange))
        {

            // V�rifie si le raycast touche le collider de l'objet actuel
            if (hit.collider != null && hit.collider.gameObject == gameObject)
            {
                if (hoverCanvas != null && !hoverCanvas.activeSelf)
                {
                    hoverCanvas.SetActive(true); // Active le Canvas
                }
                return; // Emp�che de d�sactiver tant que le raycast est actif
            }
        }

        // D�sactive le Canvas si le raycast ne touche plus
        if (hoverCanvas != null && hoverCanvas.activeSelf)
        {
            hoverCanvas.SetActive(false);
        }
    }
}
