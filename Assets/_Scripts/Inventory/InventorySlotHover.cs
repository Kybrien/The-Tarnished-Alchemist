using UnityEngine;
using UnityEngine.EventSystems;

public class InventorySlotHover: MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Animator animator; // R�f�rence � l'Animator
    public string hoverParameter = "IsHovered"; // Nom du param�tre dans l'Animator

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (animator != null)
        {
            animator.SetBool(hoverParameter, true); // Activer le param�tre
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (animator != null)
        {
            animator.SetBool(hoverParameter, false); // D�sactiver le param�tre
        }
    }
}
