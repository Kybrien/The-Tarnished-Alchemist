using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public GameObject inventoryUI; // Canvas contenant l'inventaire
    public List<InventorySlot> inventorySlots; // Les 3 slots centraux
    public InventorySlot leftHandSlot; // Slot de la main gauche
    public InventorySlot rightHandSlot; // Slot de la main droite
    public PickUpScript pickUpScript; // Référence au script pour les mains

    private bool isInventoryOpen = false;

    void Update()
    {
        // Ouvrir/fermer l'inventaire avec Tab
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ToggleInventory();
        }
    }

    public void ToggleInventory()
    {
        isInventoryOpen = !isInventoryOpen;

        // Activer ou désactiver l'inventaire
        inventoryUI.SetActive(isInventoryOpen);

        // Bloquer ou débloquer la souris
        Cursor.lockState = isInventoryOpen ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = isInventoryOpen;

        // Désactiver les interactions normales si l'inventaire est ouvert
        pickUpScript.enabled = !isInventoryOpen;
    }

    public void SwapWithSlot(InventorySlot clickedSlot, int handIndex)
    {
        // Main gauche (0) ou main droite (1)
        InventorySlot handSlot = (handIndex == 0) ? leftHandSlot : rightHandSlot;

        // Échanger les objets
        GameObject temp = handSlot.storedObject;
        handSlot.storedObject = clickedSlot.storedObject;
        clickedSlot.storedObject = temp;

        // Mettre à jour les UI des slots
        handSlot.UpdateUI();
        clickedSlot.UpdateUI();
    }
}
