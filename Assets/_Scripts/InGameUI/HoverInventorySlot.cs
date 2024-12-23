using UnityEngine;
using UnityEngine.EventSystems;

public class HoverInventorySlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject hoverCanvas; // R�f�rence au Canvas � afficher

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
    }

    // Appel� lorsque la souris quitte le slot
    public void OnPointerExit(PointerEventData eventData)
    {
        if (hoverCanvas != null)
        {
            hoverCanvas.SetActive(false);
        }
    }
}
