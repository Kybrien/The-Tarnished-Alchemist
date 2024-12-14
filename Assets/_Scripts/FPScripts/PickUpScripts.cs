using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpScript : MonoBehaviour
{
    public GameObject player;
    public Transform holdPosLeft; // Position pour la main gauche
    public Transform holdPosRight; // Position pour la main droite

    public float throwForce = 500f;
    public float pickUpRange = 5f;

    private GameObject heldObjLeft; // Objet tenu par la main gauche
    private GameObject heldObjRight; // Objet tenu par la main droite
    private Rigidbody heldObjRbLeft;
    private Rigidbody heldObjRbRight;

    private bool canDropLeft = true;
    private bool canDropRight = true;

    private int LayerNumber;

    private GameObject currentHoveredObject = null; // Object actuellement "hovered"

    // UI Elements for hands
    public GameObject leftHandElements;
    public GameObject rightHandElements;

    #region Animation Holding

    public Animator animatorLeftHand;
    public Animator animatorRightHand;

    #endregion

    void Start()
    {
        LayerNumber = LayerMask.NameToLayer("CanBeHold");
    }

    void Update()
    {
        HandleHover();
        HandleInput(KeyCode.Mouse0, ref heldObjLeft, ref heldObjRbLeft, holdPosLeft, ref canDropLeft, animatorLeftHand, leftHandElements);
        HandleInput(KeyCode.Mouse1, ref heldObjRight, ref heldObjRbRight, holdPosRight, ref canDropRight, animatorRightHand, rightHandElements);
    }

    void HandleHover()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, pickUpRange))
        {
            GameObject target = hit.collider.gameObject;

            // Si on regarde un nouvel objet
            if (target != currentHoveredObject)
            {
                // Désactiver l'outline de l'ancien objet
                if (currentHoveredObject != null)
                {
                    DisableOutline(currentHoveredObject);
                }

                // Activer l'outline du nouvel objet
                if ((target.CompareTag("HoldableObject") || target.CompareTag("PrimaryElement")) && target != heldObjLeft && target != heldObjRight)
                {
                    EnableOutline(target);
                    currentHoveredObject = target;
                }
                else
                {
                    currentHoveredObject = null; // Aucun hover actif
                }
            }
        }
        else if (currentHoveredObject != null) // Si on ne regarde plus rien
        {
            DisableOutline(currentHoveredObject);
            currentHoveredObject = null;
        }
    }

    void EnableOutline(GameObject obj)
    {
        Outline outline = obj.GetComponent<Outline>();
        if (outline != null)
        {
            outline.enabled = true;
        }

        HoverCanvas hoverCanvas = obj.GetComponent<HoverCanvas>();
        if (hoverCanvas != null)
        {
            hoverCanvas.OnHoverEnter();
        }
    }

    void DisableOutline(GameObject obj)
    {
        Outline outline = obj.GetComponent<Outline>();
        if (outline != null)
        {
            outline.enabled = false;
        }

        HoverCanvas hoverCanvas = obj.GetComponent<HoverCanvas>();
        if (hoverCanvas != null)
        {
            hoverCanvas.OnHoverExit();
        }
    }

    private void HandleInput(KeyCode key, ref GameObject heldObj, ref Rigidbody heldObjRb, Transform holdPos, ref bool canDrop, Animator animator, GameObject handElements)
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

                    // Gestion des PrimaryElements (duplication)
                    if (target.CompareTag("PrimaryElement"))
                    {
                        // Instancier sans parent pour conserver l'échelle originale
                        heldObj = Instantiate(target, holdPos.position, Quaternion.identity);

                        // Forcer l'échelle globale de l'objet à celle de l'original
                        heldObj.transform.localScale = target.transform.lossyScale;

                        // Parentage après avoir défini l'échelle
                        heldObj.transform.SetParent(holdPos, true);

                        heldObjRb = heldObj.GetComponent<Rigidbody>();
                        if (heldObjRb != null)
                        {
                            heldObjRb.isKinematic = true;
                        }
                        heldObj.layer = LayerNumber; // Assurez-vous que la copie a le bon layer
                        UpdateUI(handElements, target.name);
                    }
                    // Gestion des HoldableObjects (pickup normal)
                    else if (target.CompareTag("HoldableObject"))
                    {
                        PickUpObject(target, ref heldObj, ref heldObjRb, holdPos, animator);
                        UpdateUI(handElements, ""); // Réinitialise l'UI si ce n'est pas un élément primaire
                    }

                    DisableOutline(target); // Désactiver l'outline lorsqu'on prend l'objet
                    currentHoveredObject = null; // Réinitialiser le hover
                }
            }
            else
            {
                if (canDrop)
                {
                    DropObject(ref heldObj, ref heldObjRb, animator);
                    UpdateUI(handElements, ""); // Réinitialiser l'UI après un drop
                }
            }
        }

        if (heldObj != null)
        {
            MoveObject(heldObj, holdPos);
            if (Input.GetKeyDown(KeyCode.Q) && canDrop)
            {
                ThrowObject(ref heldObj, ref heldObjRb, animator);
                UpdateUI(handElements, ""); // Réinitialiser l'UI après un lancer
            }
        }
    }

    void PickUpObject(GameObject pickUpObj, ref GameObject heldObj, ref Rigidbody heldObjRb, Transform holdPos, Animator animator)
    {
        if (pickUpObj.GetComponent<Rigidbody>())
        {
            heldObj = pickUpObj;
            heldObjRb = pickUpObj.GetComponent<Rigidbody>();
            heldObjRb.isKinematic = true;
            heldObjRb.transform.parent = holdPos.transform;
            heldObj.layer = LayerNumber;
            Physics.IgnoreCollision(heldObj.GetComponent<Collider>(), player.GetComponent<Collider>(), true);

            animator.SetBool("isHolding", true); // Active l'animation de prise en main
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
            animator.SetBool("isHolding", false); // Désactive l'animation de prise en main
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
            animator.SetBool("isHolding", false); // Désactive l'animation de prise en main
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
        }
    }
}
