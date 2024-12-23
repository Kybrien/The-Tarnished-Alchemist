using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    public GameObject inventoryCanvas;
    public Button[] inventorySlots;
    public HandManager handManager;

    private bool inventoryOpen = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            inventoryOpen = !inventoryOpen;
            inventoryCanvas.SetActive(inventoryOpen);
            Cursor.lockState = inventoryOpen ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = inventoryOpen;
        }
    }

    public void OnSlotClick(int slotIndex, bool isLeftHand)
    {
        GameObject heldItem = handManager.GetHeldItem(isLeftHand);
        GameObject slotItem = inventorySlots[slotIndex].GetComponentInChildren<Image>().gameObject;

        if (heldItem != null)
        {
            handManager.SwapWithInventory(slotItem, isLeftHand);
            UpdateSlot(slotIndex, heldItem);
        }
        else
        {
            handManager.PickUp(slotItem, isLeftHand);
            UpdateSlot(slotIndex, null);
        }
    }

    void UpdateSlot(int slotIndex, GameObject newItem)
    {
        inventorySlots[slotIndex].GetComponentInChildren<Image>().sprite = newItem?.GetComponent<Image>().sprite;
        inventorySlots[slotIndex].GetComponentInChildren<Image>().enabled = newItem != null;
    }
}
