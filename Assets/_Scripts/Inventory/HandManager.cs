using UnityEngine;

public class HandManager : MonoBehaviour
{
    public Transform leftHand;
    public Transform rightHand;

    private GameObject heldLeft;
    private GameObject heldRight;

    public void PickUp(GameObject element, bool isLeftHand)
    {
        if (isLeftHand)
        {
            if (heldLeft != null) Drop(heldLeft);
            heldLeft = Instantiate(element, leftHand.position, Quaternion.identity, leftHand);
        }
        else
        {
            if (heldRight != null) Drop(heldRight);
            heldRight = Instantiate(element, rightHand.position, Quaternion.identity, rightHand);
        }
    }

    public void Drop(GameObject heldObject)
    {
        if (heldObject != null)
        {
            heldObject.transform.SetParent(null);
            heldObject.layer = 0; // Reset layer
        }
    }

    public GameObject GetHeldItem(bool isLeftHand)
    {
        return isLeftHand ? heldLeft : heldRight;
    }

    public void SwapWithInventory(GameObject inventorySlot, bool isLeftHand)
    {
        GameObject temp = inventorySlot;

        if (isLeftHand)
        {
            inventorySlot = heldLeft;
            PickUp(temp, true);
        }
        else
        {
            inventorySlot = heldRight;
            PickUp(temp, false);
        }
    }
}
