using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraController: MonoBehaviour
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
                RaycastHit hit;
                if (Physics.Raycast(transform.position, transform.forward, out hit, pickUpRange))
                {
                    if (hit.collider.CompareTag("Mixer")) // Vérifie si on regarde un Mixer
                    {
                        Debug.Log("Mixer détecté au clic");
                        Mixer mixer = hit.collider.GetComponent<Mixer>();

                        if (mixer != null)
                        {
                            mixer.PlaceObject(heldObj);
                            heldObj = null;       // Libère la main
                            heldObjRb = null;
                            animator.SetBool("isHolding", false);
                            UpdateUI(handElements, "");
                            Debug.Log("Objet placé dans le conteneur.");
                        }
                    }
                    else if (canDrop)// Sinon, lâche l'objet normalement
                    {
                        DropObject(ref heldObj, ref heldObjRb, animator);
                        UpdateUI(handElements, "");
                    }
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

        bool isLeftHand = handSlotButton.transform.IsChildOf(leftHandElements.transform);

        // Trouve un slot vide dans l'inventaire
        GameObject targetSlot = FindEmptyInventorySlot();
        if (targetSlot != null)
        {
            // Active l'objet dans le slot
            Transform slotElement = targetSlot.transform.Find(elementName);
            if (slotElement != null)
            {
                slotElement.gameObject.SetActive(true);
            }

            // Désactive l'objet dans la main
            GameObject handElements = isLeftHand ? leftHandElements : rightHandElements;
            Transform handElement = handElements.transform.Find(elementName);
            if (handElement != null)
            {
                handElement.gameObject.SetActive(false);
            }

            // Détruit l'objet tenu et réinitialise l'Animator
            if (isLeftHand)
            {
                Destroy(heldObjLeft);
                heldObjLeft = null;
                animatorLeftHand.SetBool("isHolding", false);
            }
            else
            {
                Destroy(heldObjRight);
                heldObjRight = null;
                animatorRightHand.SetBool("isHolding", false);
            }
        }
    }



    public void OnClickInventorySlot(GameObject inventorySlotButton)
    {
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
            InstantiateToHand(objPrefab, ref heldObjLeft, ref heldObjRbLeft, holdPosLeft, leftHandElements, animatorLeftHand, inventorySlotButton);
        }
        // Sinon utilise la main droite
        else if (heldObjRight == null)
        {
            InstantiateToHand(objPrefab, ref heldObjRight, ref heldObjRbRight, holdPosRight, rightHandElements, animatorRightHand, inventorySlotButton);
        }
        else
        {
            Debug.LogWarning("Both hands are occupied. Cannot pick up a new item.");
        }
    }


    private void InstantiateToHand(GameObject prefab, ref GameObject heldObj, ref Rigidbody heldObjRb, Transform holdPos, GameObject handElements, Animator animator, GameObject inventorySlotButton)
    {
        // Instancie l'objet
        heldObj = Instantiate(prefab, holdPos.position, Quaternion.identity);
        heldObj.name = CleanName(prefab.name);
        heldObj.transform.localScale = prefab.transform.lossyScale; // Préserve l'échelle
        heldObj.transform.SetParent(holdPos, true);

        // Configure le Rigidbody
        heldObjRb = heldObj.GetComponent<Rigidbody>();
        if (heldObjRb != null) heldObjRb.isKinematic = true;

        // Configure le Layer
        heldObj.layer = LayerNumber;

        // Met à jour l'UI et l'animation
        animator.SetBool("isHolding", true);
        UpdateUI(handElements, heldObj.name);

        // Désactive le slot dans l'inventaire
        inventorySlotButton.SetActive(false);
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
