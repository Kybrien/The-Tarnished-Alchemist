using UnityEngine;
using UnityEngine.EventSystems;

public class HoverBlackMarket : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject hoverCanvas;       // R�f�rence au Canvas � afficher
    public GameObject descriptionParent; // Le GameObject parent contenant les descriptions

    private void Start()
    {
        if (hoverCanvas != null)
        {
            hoverCanvas.SetActive(false); // Assure que le Canvas est d�sactiv� au d�part
        }
    }

    // Appel� lorsque la souris entre dans le slot
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (hoverCanvas != null)
        {
            hoverCanvas.SetActive(true);
        }

        // Active la description correspondante
        if (descriptionParent != null)
        {
            ActivateDescription();
        }
    }

    // Appel� lorsque la souris quitte le slot
    public void OnPointerExit(PointerEventData eventData)
    {
        if (hoverCanvas != null)
        {
            hoverCanvas.SetActive(false);
        }

        // D�sactive toutes les descriptions
        if (descriptionParent != null)
        {
            DeactivateAllDescriptions();
        }
    }

    private void ActivateDescription()
    {
        // D�sactive toutes les descriptions pour �viter les conflits
        DeactivateAllDescriptions();

        // Trouve et active le GameObject correspondant
        Transform description = descriptionParent.transform.Find(gameObject.name);
        if (description != null)
        {
            description.gameObject.SetActive(true);
        }
        else
        {
            Debug.LogWarning($"Description '{gameObject.name}' non trouv�e sous '{descriptionParent.name}'.");
        }
    }

    private void DeactivateAllDescriptions()
    {
        foreach (Transform child in descriptionParent.transform)
        {
            child.gameObject.SetActive(false);
        }
    }
}
