using UnityEngine;
using UnityEngine.EventSystems;

public class HoverInventorySlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject hoverCanvas; // Référence au Canvas à afficher

    private void Start()
    {
        if (hoverCanvas != null)
        {
            hoverCanvas.SetActive(false); // Assure que le Canvas est désactivé au départ
        }
    }

    // Appelé lorsque la souris entre dans le slot
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (hoverCanvas != null)
        {
            hoverCanvas.SetActive(true);
        }
    }

    // Appelé lorsque la souris quitte le slot
    public void OnPointerExit(PointerEventData eventData)
    {
        if (hoverCanvas != null)
        {
            hoverCanvas.SetActive(false);
        }
    }
}
