using UnityEngine;
using UnityEngine.EventSystems;

public class InventorySlotHover: MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Animator animator; // Référence à l'Animator
    public string hoverParameter = "IsHovered"; // Nom du paramètre dans l'Animator

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (animator != null)
        {
            animator.SetBool(hoverParameter, true); // Activer le paramètre
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (animator != null)
        {
            animator.SetBool(hoverParameter, false); // Désactiver le paramètre
        }
    }
}
