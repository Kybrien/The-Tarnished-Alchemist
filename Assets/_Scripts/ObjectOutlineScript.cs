using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectOutlineScript : MonoBehaviour
{
    public Camera playerCamera;
    public float lookRange = 5f; // Distance maximale pour d�tecter les objets
    public LayerMask interactableLayer; // Layer pour les objets interactables

    private GameObject currentTarget; // Objet actuellement vis�
    private Material originalMaterial; // Mat�riau d'origine
    public Material outlineMaterial; // Mat�riau pour l'effet Outline

    void Update()
    {
        CheckForInteractable();
    }

    void CheckForInteractable()
    {
        // Raycast � partir de la cam�ra du joueur
        RaycastHit hit;
        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, lookRange, interactableLayer))
        {
            GameObject target = hit.collider.gameObject;

            // Si on change de cible
            if (target != currentTarget)
            {
                ResetOutline(); // R�initialiser l'outline de l'ancien objet
                ApplyOutline(target); // Appliquer l'outline sur le nouvel objet
                currentTarget = target;
            }
        }
        else
        {
            ResetOutline(); // R�initialiser l'outline si aucun objet interactable n'est d�tect�
            currentTarget = null;
        }
    }

    void ApplyOutline(GameObject target)
    {
        Renderer renderer = target.GetComponent<Renderer>();
        if (renderer != null)
        {
            originalMaterial = renderer.material; // Sauvegarder le mat�riau actuel
            renderer.material = outlineMaterial; // Appliquer le mat�riau Outline
        }
    }

    void ResetOutline()
    {
        if (currentTarget != null)
        {
            Renderer renderer = currentTarget.GetComponent<Renderer>();
            if (renderer != null && originalMaterial != null)
            {
                renderer.material = originalMaterial; // Restaurer le mat�riau d'origine
            }
        }
    }
}

