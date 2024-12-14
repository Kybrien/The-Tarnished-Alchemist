using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    public GameObject storedObject; // L'objet stocké dans ce slot
    public Image slotImage; // Image représentant l'objet
    public Sprite emptySlotSprite; // Sprite pour un slot vide
    public InventoryManager inventoryManager; // Référence au gestionnaire d'inventaire

    public void OnSlotClick(int handIndex)
    {
        // Appelé lorsqu'on clique sur ce slot
        inventoryManager.SwapWithSlot(this, handIndex);
    }

    public void UpdateUI()
    {
        if (storedObject != null)
        {
            // Mettre à jour l'image avec celle de l'objet
            slotImage.sprite = storedObject.GetComponent<SpriteRenderer>().sprite;
            slotImage.color = Color.white;
        }
        else
        {
            // Remettre un slot vide
            slotImage.sprite = emptySlotSprite;
            slotImage.color = Color.clear;
        }
    }
}
