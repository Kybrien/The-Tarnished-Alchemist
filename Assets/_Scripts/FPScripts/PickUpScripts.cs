using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PickUpScript : MonoBehaviour
{
    public GameObject player;
    public Transform holdPosLeft; // Position pour la main gauche
    public Transform holdPosRight; // Position pour la main droite

    public float throwForce = 500f;
    public float pickUpRange = 5f;

    public GameObject inventoryCanvas; // Canvas d'inventaire

    private GameObject heldObjLeft; // Objet tenu par la main gauche
    private GameObject heldObjRight; // Objet tenu par la main droite
    private Rigidbody heldObjRbLeft;
    private Rigidbody heldObjRbRight;

    private bool canDropLeft = true;
    private bool canDropRight = true;

    private int LayerNumber;

    private GameObject currentHoveredObject = null; // Objet actuellement "hovered"

    // UI Elements for hands
    public GameObject leftHandElements;
    public GameObject rightHandElements;

    #region Animation Holding
    public Animator animatorLeftHand;
    public Animator animatorRightHand;
    #endregion

    private bool isInventoryOpen = false; // État d'ouverture de l'inventaire

    void Start()
    {
        LayerNumber = LayerMask.NameToLayer("CanBeHold");
        inventoryCanvas.SetActive(false); // Cache l'inventaire au départ
        Cursor.visible = false;
    }

    void Update()
    {
        HandleHover();
        HandleInput(KeyCode.Mouse0, ref heldObjLeft, ref heldObjRbLeft, holdPosLeft, ref canDropLeft, animatorLeftHand, leftHandElements);
        HandleInput(KeyCode.Mouse1, ref heldObjRight, ref heldObjRbRight, holdPosRight, ref canDropRight, animatorRightHand, rightHandElements);

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ToggleInventory();
        }
    }

    void ToggleInventory()
    {
        isInventoryOpen = !isInventoryOpen;
        inventoryCanvas.SetActive(isInventoryOpen);
        Cursor.lockState = isInventoryOpen ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = isInventoryOpen;

        // Active/désactive les contrôles en fonction de l'état de l'inventaire
        SetPlayerControls(!isInventoryOpen);
    }

    void HandleHover()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, pickUpRange))
        {
            GameObject target = hit.collider.gameObject;

            if (target != currentHoveredObject)
            {
                if (currentHoveredObject != null)
                {
                    DisableOutline(currentHoveredObject);
                }

                if ((target.CompareTag("HoldableObject") || target.CompareTag("PrimaryElement")) &&
                    target != heldObjLeft && target != heldObjRight)
                {
                    EnableOutline(target);
                    currentHoveredObject = target;
                }
                else
                {
                    currentHoveredObject = null;
                }
            }
        }
        else if (currentHoveredObject != null)
        {
            DisableOutline(currentHoveredObject);
            currentHoveredObject = null;
        }
    }

    void EnableOutline(GameObject obj)
    {
        Outline outline = obj.GetComponent<Outline>();
        if (outline != null) outline.enabled = true;

        HoverCanvas hoverCanvas = obj.GetComponent<HoverCanvas>();
        if (hoverCanvas != null) hoverCanvas.OnHoverEnter();
    }

    void DisableOutline(GameObject obj)
    {
        Outline outline = obj.GetComponent<Outline>();
        if (outline != null) outline.enabled = false;

        HoverCanvas hoverCanvas = obj.GetComponent<HoverCanvas>();
        if (hoverCanvas != null) hoverCanvas.OnHoverExit();
    }

    void HandleInput(KeyCode key, ref GameObject heldObj, ref Rigidbody heldObjRb, Transform holdPos, ref bool canDrop, Animator animator, GameObject handElements)
    {
        if (Input.GetKeyDown(key))
        {
            animator.SetTrigger("OnClick");

            if (heldObj == null)
            {
                RaycastHit hit;
                if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, pickUpRange))
                {
                    GameObject target = hit.transform.gameObject;

                    if (target.CompareTag("PrimaryElement"))
                    {
                        if (target.transform.parent == null)
                        {
                            PickUpObject(target, ref heldObj, ref heldObjRb, holdPos, animator);
                        }
                        else
                        {
                            heldObj = Instantiate(target, holdPos.position, Quaternion.identity);
                            heldObj.name = CleanName(target.name);
                            heldObj.transform.localScale = target.transform.lossyScale;
                            heldObj.transform.SetParent(holdPos, true);

                            heldObjRb = heldObj.GetComponent<Rigidbody>();
                            if (heldObjRb != null) heldObjRb.isKinematic = true;

                            heldObj.layer = LayerNumber;
                            animator.SetBool("isHolding", true);
                            UpdateUI(holdPos == holdPosLeft ? leftHandElements : rightHandElements, heldObj.name);
                        }
                    }
                    else if (target.CompareTag("HoldableObject"))
                    {
                        PickUpObject(target, ref heldObj, ref heldObjRb, holdPos, animator);
                        UpdateUI(handElements, "");
                    }

                    DisableOutline(target);
                    currentHoveredObject = null;
                }
            }
            else
            {
                if (canDrop)
                {
                    DropObject(ref heldObj, ref heldObjRb, animator);
                    UpdateUI(handElements, "");
                }
            }
        }

        if (heldObj != null)
        {
            MoveObject(heldObj, holdPos);
            if (Input.GetKeyDown(KeyCode.Q) && canDrop)
            {
                ThrowObject(ref heldObj, ref heldObjRb, animator);
                UpdateUI(handElements, "");
            }
        }
    }

    void PickUpObject(GameObject pickUpObj, ref GameObject heldObj, ref Rigidbody heldObjRb, Transform holdPos, Animator animator)
    {
        if (pickUpObj.GetComponent<Rigidbody>())
        {
            heldObj = pickUpObj;
            heldObj.name = CleanName(pickUpObj.name);
            heldObjRb = pickUpObj.GetComponent<Rigidbody>();
            heldObjRb.isKinematic = true;
            heldObjRb.transform.parent = holdPos;
            heldObj.layer = LayerNumber;
            Physics.IgnoreCollision(heldObj.GetComponent<Collider>(), player.GetComponent<Collider>(), true);

            animator.SetBool("isHolding", true);
            UpdateUI(holdPos == holdPosLeft ? leftHandElements : rightHandElements, heldObj.name);
        }
    }

    void DropObject(ref GameObject heldObj, ref Rigidbody heldObjRb, Animator animator)
    {
        if (heldObj != null && heldObjRb != null)
        {
            Physics.IgnoreCollision(heldObj.GetComponent<Collider>(), player.GetComponent<Collider>(), false);
            heldObj.layer = 0;
            heldObjRb.isKinematic = false;
            heldObj.transform.parent = null;
            heldObj = null;
            heldObjRb = null;
            animator.SetBool("isHolding", false);
        }
    }

    void MoveObject(GameObject heldObj, Transform holdPos)
    {
        heldObj.transform.position = holdPos.transform.position;
    }

    void ThrowObject(ref GameObject heldObj, ref Rigidbody heldObjRb, Animator animator)
    {
        if (heldObj != null && heldObjRb != null)
        {
            Physics.IgnoreCollision(heldObj.GetComponent<Collider>(), player.GetComponent<Collider>(), false);
            heldObj.layer = 0;
            heldObjRb.isKinematic = false;
            heldObj.transform.parent = null;
            heldObjRb.AddForce(transform.forward * throwForce);
            heldObj = null;
            heldObjRb = null;
            animator.SetBool("isHolding", false);
        }
    }

    void UpdateUI(GameObject handElements, string elementName)
    {
        foreach (Transform child in handElements.transform)
        {
            child.gameObject.SetActive(false);
        }

        if (!string.IsNullOrEmpty(elementName))
        {
            Transform uiElement = handElements.transform.Find(elementName);
            if (uiElement != null)
            {
                uiElement.gameObject.SetActive(true);
            }
            else
            {
                Debug.LogWarning($"UI Element '{elementName}' not found in {handElements.name}");
            }
        }
    }

    public void OnClickHandSlot(GameObject handSlotButton)
    {
        string elementName = handSlotButton.name;
        Debug.Log($"HandSlot clicked: {elementName}");

        bool isLeftHand = handSlotButton.transform.IsChildOf(leftHandElements.transform);
        Debug.Log($"Is Left Hand: {isLeftHand}");

        GameObject targetSlot = FindEmptyInventorySlot();
        if (targetSlot == null)
        {
            Debug.LogWarning("No empty inventory slot found.");
            return;
        }

        // Activer l'élément dans l'inventaire
        GameObject elementInInventory = targetSlot.transform.Find(elementName)?.gameObject;
        if (elementInInventory != null)
        {
            elementInInventory.SetActive(true);
            Debug.Log($"Activated {elementName} in inventory slot.");
        }
        else
        {
            Debug.LogWarning($"Element {elementName} not found in inventory slot.");
        }

        // Désactiver l'élément dans la main
        GameObject handElements = isLeftHand ? leftHandElements : rightHandElements;
        GameObject elementInHand = handElements.transform.Find(elementName)?.gameObject;
        if (elementInHand != null)
        {
            elementInHand.SetActive(false);
            Debug.Log($"Deactivated {elementName} in hand.");
        }
        else
        {
            Debug.LogWarning($"Element {elementName} not found in hand.");
        }

        // Supprimer l'objet en main
        if (isLeftHand)
        {
            Destroy(heldObjLeft);
            heldObjLeft = null;
        }
        else
        {
            Destroy(heldObjRight);
            heldObjRight = null;
        }
    }


    public void OnClickInventorySlot(GameObject inventorySlotButton)
    {
        Debug.Log("Slot clicked: " + inventorySlotButton.name);

        string elementName = inventorySlotButton.name;
        GameObject objPrefab = Resources.Load<GameObject>(elementName);

        if (objPrefab == null)
        {
            Debug.LogWarning($"Prefab {elementName} not found in Resources.");
            return;
        }

        // Vérifie si la main gauche est disponible
        if (heldObjLeft == null)
        {
            InstantiateToHand(objPrefab, ref heldObjLeft, holdPosLeft, leftHandElements, elementName, inventorySlotButton);
        }
        // Sinon utilise la main droite
        else if (heldObjRight == null)
        {
            InstantiateToHand(objPrefab, ref heldObjRight, holdPosRight, rightHandElements, elementName, inventorySlotButton);
        }
        else
        {
            Debug.LogWarning("Both hands are occupied. Cannot pick up a new item.");
        }
    }

    private void InstantiateToHand(GameObject prefab, ref GameObject heldObj, Transform holdPos, GameObject handElements, string elementName, GameObject inventorySlotButton)
    {
        // Instancie l'objet dans la main
        heldObj = Instantiate(prefab, holdPos.position, Quaternion.identity);
        heldObj.transform.SetParent(holdPos);

        // Met à jour l'UI
        UpdateUI(handElements, elementName);

        // Désactive l'objet dans l'inventaire
        Transform slotElement = inventorySlotButton.transform.Find(elementName);
        if (slotElement != null)
        {
            slotElement.gameObject.SetActive(false);
        }
    }


    GameObject FindEmptyInventorySlot()
    {
        foreach (Transform slot in inventoryCanvas.transform)
        {
            bool hasActiveChild = false;
            foreach (Transform child in slot)
            {
                if (child.gameObject.activeSelf)
                {
                    hasActiveChild = true;
                    break;
                }
            }

            if (!hasActiveChild) // Si aucun enfant actif
                return slot.gameObject;
        }

        return null;
    }


    private string CleanName(string name)
    {
        if (name.EndsWith(" (Clone)"))
        {
            return name.Substring(0, name.Length - 7);
        }
        return name;
    }

    void SetPlayerControls(bool isEnabled)
    {
        // Désactiver les clics et mouvements
        FirstPersonController controller = player.GetComponent<FirstPersonController>();
        if (controller != null)
        {
            controller.cameraCanMove = isEnabled; // Contrôle de la caméra
            controller.playerCanMove = isEnabled; // Mouvements du joueur
        }

        // Désactiver les actions de PickUp
        if (!isEnabled)
        {
            canDropLeft = false;
            canDropRight = false;
        }
        else
        {
            canDropLeft = true;
            canDropRight = true;
        }
    }
}
