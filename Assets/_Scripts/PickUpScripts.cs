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
    //private float rotationSensitivity = 1f;

    private GameObject heldObjLeft; // Objet tenu par la main gauche
    private GameObject heldObjRight; // Objet tenu par la main droite
    private Rigidbody heldObjRbLeft;
    private Rigidbody heldObjRbRight;

    private bool canDropLeft = true;
    private bool canDropRight = true;

    private int LayerNumber;

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
        HandleInput(KeyCode.Mouse0, ref heldObjLeft, ref heldObjRbLeft, holdPosLeft, ref canDropLeft, animatorLeftHand);
        HandleInput(KeyCode.Mouse1, ref heldObjRight, ref heldObjRbRight, holdPosRight, ref canDropRight, animatorRightHand);
    }

    private void HandleInput(KeyCode key, ref GameObject heldObj, ref Rigidbody heldObjRb, Transform holdPos, ref bool canDrop, Animator animator)
    {
        if (Input.GetKeyDown(key))
        {
            animator.SetTrigger("OnClick");

            if (heldObj == null)
            {
                RaycastHit hit;
                if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, pickUpRange))
                {
                    if (hit.transform.gameObject.tag == "HoldableObject")
                    {
                        PickUpObject(hit.transform.gameObject, ref heldObj, ref heldObjRb, holdPos, animator);
                    }
                }
            }
            else
            {
                if (canDrop)
                {
                    StopClipping(heldObj);
                    DropObject(ref heldObj, ref heldObjRb, animator);
                }
            }
        }

        if (heldObj != null)
        {
            MoveObject(heldObj, holdPos);
            if (Input.GetKeyDown(KeyCode.Q) && canDrop)
            {
                StopClipping(heldObj);
                ThrowObject(ref heldObj, ref heldObjRb, animator);
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
        //Debug.Log("Main gauche isHolding: " + animatorLeftHand.GetBool("isHolding"));
        //Debug.Log("Main droite isHolding: " + animatorRightHand.GetBool("isHolding"));

    }

    void DropObject(ref GameObject heldObj, ref Rigidbody heldObjRb, Animator animator)
    {
        Physics.IgnoreCollision(heldObj.GetComponent<Collider>(), player.GetComponent<Collider>(), false);
        heldObj.layer = 0;
        heldObjRb.isKinematic = false;
        heldObj.transform.parent = null;
        heldObj = null;
        animator.SetBool("isHolding", false); // Désactive l'animation de prise en main
    }

    void MoveObject(GameObject heldObj, Transform holdPos)
    {
        heldObj.transform.position = holdPos.transform.position;
    }

    void ThrowObject(ref GameObject heldObj, ref Rigidbody heldObjRb, Animator animator)
    {
        Physics.IgnoreCollision(heldObj.GetComponent<Collider>(), player.GetComponent<Collider>(), false);
        heldObj.layer = 0;
        heldObjRb.isKinematic = false;
        heldObj.transform.parent = null;
        heldObjRb.AddForce(transform.forward * throwForce);
        heldObj = null;
        animator.SetBool("isHolding", false); // Désactive l'animation de prise en main
    }

    void StopClipping(GameObject heldObj)
    {
        var clipRange = Vector3.Distance(heldObj.transform.position, transform.position);
        RaycastHit[] hits;
        hits = Physics.RaycastAll(transform.position, transform.TransformDirection(Vector3.forward), clipRange);
        if (hits.Length > 1)
        {
            heldObj.transform.position = transform.position + new Vector3(0f, -0.5f, 0f);
        }
    }
}